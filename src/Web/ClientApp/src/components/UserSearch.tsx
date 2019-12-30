import * as React from 'react';
import { useEffect, useState } from 'react';
import { Paged } from '../store/Paged';
import { User } from '../store/User';
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
import { makeStyles } from '@material-ui/core/styles';

type UserSearchProps = {
  searchPlaceholder: string;
  searchRequest: (search: string, pageNumber: number, pageSize: number) => any;
  users: Paged<User>;
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

export const UserSearch = (props: UserSearchProps) => {
  const { searchPlaceholder, searchRequest, users, } = props;
  const classes = useStyles();
  
  const [search, setSearch] = useState('');
  const [pageNumber, setPageNumber] = useState(users.page);
  const [pageSize, setPageSize] = useState(users.pageSize);

  useEffect(() => {
    searchRequest(search, pageNumber, pageSize);
  }, [pageNumber, pageSize, search]);

  return (
    <React.Fragment>
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
    </React.Fragment>);
};