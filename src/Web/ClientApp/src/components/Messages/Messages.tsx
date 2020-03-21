import * as React from 'react';
import { useEffect, useState } from 'react';
import { Paged } from '../../store/Paged';
import { Message } from '../../store/Message';
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
import { makeStyles } from '@material-ui/core/styles';
import { MessageCards } from './MessageCards';
import { useSelector } from '../../store/useSelector'
import { AccountState } from '../../store/Account';
import { postResponse } from '../../api'
import Alert from '@material-ui/lab/Alert';

type MessagesProps = {
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

export const Messages = (props: MessagesProps) => {
  const { url, searchPlaceholder } = props;
  const classes = useStyles();
  const [error, setError] = useState<string>("");
  const [search, setSearch] = useState('');
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const { password, userName } = useSelector<AccountState>(state => state.account);
  const [messages, setMessages] = useState<Paged<Message>>();

  const searchRequest = (search: string, pageNumber: number, pageSize: number) => {
    setError('');  
    postResponse<Paged<Message>>(url, {userName, password, search, pageNumber, pageSize})
      .then(data => {
        if (data) {
          if (data.isError)
            setError(data.message!);
          else {
            setMessages(data.response);
          }
        }
    });
  };

  useEffect(() => {
    searchRequest(search, pageNumber, pageSize);
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
        { messages && (
          <>
            <MessageCards messages={messages.rows} />
            <TablePagination 
                count={messages.totalRows}
                rowsPerPage={messages.pageSize}
                rowsPerPageOptions={[20, 50, 100]}
                component={Paper}
                onChangePage={(_: any, page: number) => setPageNumber(page)}
                onChangeRowsPerPage={((_: any, select: any) => setPageSize(select.key)) as any}
                page={messages.page}
            />
          </>
        )}
    </React.Fragment>);
};
