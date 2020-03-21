import * as React from 'react';
import { useEffect, useState } from 'react';
import { Paged } from '../../store/Paged';
import { User } from '../../store/User';
import Grid from '@material-ui/core/Grid';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import Paper from '@material-ui/core/Paper';
import TextField from '@material-ui/core/TextField';
import TablePagination from '@material-ui/core/TablePagination';
import IconButton from '@material-ui/core/IconButton';
import Tooltip from '@material-ui/core/Tooltip';
import SearchIcon from '@material-ui/icons/Search';
import RefreshIcon from '@material-ui/icons/Refresh';
import { UserCards } from './UserCards';
import { useSelector } from '../../store/useSelector';
import { AccountState } from '../../store/Account';
import { makeStyles } from '@material-ui/core/styles';
import { postResponse } from '../../api'
import Alert from '@material-ui/lab/Alert';

type UsersProps = {
  url: string;
  searchPlaceholder: string;
};

const useStyles = makeStyles(theme => ({
    searchBar: {
      borderBottom: '1px solid rgba(0, 0, 0, 0.12)',
      marginBottom: '16px'
    },
    searchInput: {
      fontSize: theme.typography.fontSize,
    },
    block: {
      display: 'block',
    }
  }));

export const Users = (props: UsersProps) => {
  const { url, searchPlaceholder } = props;
  const classes = useStyles();
  const [error, setError] = useState<string>("");
  const [search, setSearch] = useState('');
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const { password, userName } = useSelector<AccountState>(state => state.account);
  const [users, setUsers] = useState<Paged<User>>();

  const searchRequest = () => {
    setError('');  
    postResponse<Paged<User>>(url, {userName, password, search, pageNumber, pageSize})
      .then(data => {
        if (data) {
          if (data.isError)
            setError(data.message!);
          else {
            setUsers(data.response);
          }
        }
      });
    };

  useEffect(() => {
    searchRequest();
  }, [pageNumber, pageSize, search]);

  return (
    <React.Fragment>
        { error && <Alert severity="error">{error}</Alert> }
        <AppBar className={classes.searchBar} position="static" color="default" elevation={0}>
            <Toolbar>
                <Grid container spacing={2} alignItems="center">
                    <Grid item>
                        <SearchIcon className={classes.block} color="inherit" />
                    </Grid>
                    <Grid item xs>
                        <TextField fullWidth placeholder={searchPlaceholder} InputProps={{
                            disableUnderline: true,
                            className: classes.searchInput,
                            }} onChange={(event: any) => setSearch(event.target.value)}
                        />
                    </Grid>
                    <Grid item>
                        <Tooltip title="Reload">
                            <IconButton>
                                <RefreshIcon className={classes.block} color="inherit" />
                            </IconButton>
                        </Tooltip>
                    </Grid>
                </Grid>
            </Toolbar>
        </AppBar>
        { users && (
          <>
            <UserCards users={users.rows} />
            <TablePagination 
                count={users.totalRows}
                rowsPerPage={users.pageSize}
                rowsPerPageOptions={[20, 50, 100]}
                component={Paper}
                onChangePage={(_: any, page: number) => setPageNumber(page)}
                onChangeRowsPerPage={((_: any, select: any) => setPageSize(select.key)) as any}
                page={users.page}
            />
          </>
        )}
    </React.Fragment>);
};
