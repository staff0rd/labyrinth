import * as React from 'react';
import { useEffect} from 'react';
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
import { makeStyles } from '@material-ui/core/styles';

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
    title: {
      fontSize: 14,
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
  }));

const UserTable = (props: LinkedInProps) => {
  const classes = useStyles();
  return (
    <Grid container alignItems="stretch" spacing={1}>
        {props.users.map((user: LinkedInStore.User) => 
          <Grid xs={6} item key={user.profileUrl} style={{display: 'flex'}}>
            <Card className={classes.card}>
            <CardContent>
              <Typography className={classes.title} color="textSecondary" gutterBottom>
                LinkedIn
              </Typography>
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
                  {/* <Typography variant="body2" component="p">
                    well meaning and kindly.
                    <br />
                    {'"a benevolent smile"'}
                  </Typography> */}
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
  );
}

const LinkedIn = (props: LinkedInProps) => {
  useEffect(() => {
    props.requestUsers();
  }, []);

  return (
    <React.Fragment>
      <h1 id="tabelLabel">LinkedIn</h1>
      <p>This component demonstrates fetching data from the server and working with URL parameters.</p>
      <UserTable {...props} />
      {/* {this.renderPagination()} */}
    </React.Fragment>
  );

  // private renderPagination() {
  //   const prevStartDateIndex = (this.props.startDateIndex || 0) - 5;
  //   const nextStartDateIndex = (this.props.startDateIndex || 0) + 5;

  //   return (
  //     <div className="d-flex justify-content-between">
  //       <Link className='btn btn-outline-secondary btn-sm' to={`/fetch-data/${prevStartDateIndex}`}>Previous</Link>
  //       {this.props.isLoading && <span>Loading...</span>}
  //       <Link className='btn btn-outline-secondary btn-sm' to={`/fetch-data/${nextStartDateIndex}`}>Next</Link>
  //     </div>
  //   );
  // }
}

export default connect(
  (state: ApplicationState) => state.linkedIn, // Selects which state properties are merged into the component's props
  LinkedInStore.actionCreators // Selects which action creators are merged into the component's props
)(LinkedIn as any);
