import * as React from 'react';
import { useEffect, useState } from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as LinkedInStore from '../store/LinkedIn';
import Card from '@material-ui/core/Card';
import CardActions from '@material-ui/core/CardActions';
import CardContent from '@material-ui/core/CardContent';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import Avatar from '@material-ui/core/Avatar';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import Paper from '@material-ui/core/Paper';
import TextField from '@material-ui/core/TextField';
import Button from '@material-ui/core/Button';
import TablePagination from '@material-ui/core/TablePagination';
import IconButton from '@material-ui/core/IconButton';
import Tooltip from '@material-ui/core/Tooltip';
import SearchIcon from '@material-ui/icons/Search';
import RefreshIcon from '@material-ui/icons/Refresh';
import { makeStyles } from '@material-ui/core/styles';
import Moment from 'react-moment';

// At runtime, Redux will merge together...
type LinkedInProps =
  LinkedInStore.LinkedInState // ... state we've requested from the Redux store
  & typeof LinkedInStore.actionCreators // ... plus action creators we've requested
  & RouteComponentProps<{ startDateIndex: string }>; // ... plus incoming routing parameters

  const useStyles = makeStyles(theme => ({
    card: {
      minWidth: 275,
      width: '100%',
    },
    bullet: {
      display: 'inline-block',
      margin: '0 2px',
      transform: 'scale(0.8)',
    },
    pos: {
      marginBottom: 12,
    },
    large: {
      width: theme.spacing(7),
      height: theme.spacing(7),
    },
    root: {
      display: 'flex',
      '& > *': {
        margin: theme.spacing(1),
      },
    },
    searchBar: {
      borderBottom: '1px solid rgba(0, 0, 0, 0.12)',
      marginBottom: '16px'
    },
    searchInput: {
      fontSize: theme.typography.fontSize,
    },
    block: {
      display: 'block',
    },
    addUser: {
      marginRight: theme.spacing(1),
    },
  }));

const LinkedIn = (props: LinkedInProps) => {
  const {
    users
  } = props

  const classes = useStyles();

  const [search, setSearch] = useState('');
  const [pageNumber, setPageNumber] = useState(props.users.page);
  const [pageSize, setPageSize] = useState(props.users.pageSize);

  useEffect(() => {
    props.requestUsers(pageNumber, pageSize, search);
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
              <TextField
                fullWidth
                placeholder="Search by name or occupation"
                InputProps={{
                  disableUnderline: true,
                  className: classes.searchInput,
                }}
                onChange={(event: any) => setSearch(event.target.value)}
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
      <Grid container alignItems="stretch" spacing={1}>
        {props.users.rows.map((user: LinkedInStore.User) => 
          <Grid xs={6} item key={user.profileUrl} style={{display: 'flex'}}>
            <Card className={classes.card}>
            <CardContent>
              <Grid container justify="space-between">
                <Grid item>
                  <Typography color="textSecondary">
                    LinkedIn
                  </Typography>
                </Grid>
                <Grid item>
                  <Typography color="textSecondary">
                    Connected <Moment fromNow>{user.connected}</Moment>
                  </Typography>
                </Grid>
              </Grid>
              <div className={classes.root}>
                <Avatar alt={user.name} src={user.mugshotUrl.startsWith('data') ? undefined : user.mugshotUrl} className={classes.large}>
                  {user.mugshotUrl.startsWith('data') ? user.name.split(' ').map(i => i.charAt(0).toUpperCase()) : undefined }
                </Avatar>
                <div>
                  <Typography variant="h5" component="h2">
                    {user.name}
                  </Typography>
                  <Typography className={classes.pos} color="textSecondary">
                    {user.occupation}
                  </Typography>
                </div>
              </div>
            </CardContent>
            {/* <CardActions>
              <Button size="small">Learn More</Button>
            </CardActions> */}
            </Card>
        </Grid>
        )}
    </Grid>
    <TablePagination 
      count={users.totalRows}
      rowsPerPage={users.pageSize}
      rowsPerPageOptions={[20, 50, 100]}
      component={Paper}
      onChangePage={(_: any, page: number) => setPageNumber(page)}
      onChangeRowsPerPage={((_:any, select:any) => setPageSize(select.key)) as any}
      page={users.page}
      />
    </React.Fragment>
  );
}

export default connect(
  (state: ApplicationState) => state.linkedIn, // Selects which state properties are merged into the component's props
  LinkedInStore.actionCreators // Selects which action creators are merged into the component's props
)(LinkedIn as any);
