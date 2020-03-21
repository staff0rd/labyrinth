import * as React from 'react'
import { User } from '../../store/User';
import Card from '@material-ui/core/Card';
import CardActions from '@material-ui/core/CardActions';
import CardContent from '@material-ui/core/CardContent';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import Avatar from '@material-ui/core/Avatar';
import { makeStyles } from '@material-ui/core/styles';
import Moment from 'react-moment';
import moment from 'moment';


type UserCardsProps = {
    users: User[];
}

const useStyles = makeStyles(theme => ({
    card: {
      width: '100%',
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

export const UserCards = (props: UserCardsProps) => {
    
    return (
        <Grid container alignItems="stretch" spacing={1} style={{marginBottom: 8}}>
            {props.users.map((user: User) => <UserCard {...user} />)}
        </Grid>
    );
}

type ConnectedSinceProps = {
    knownSince?: Date;
}

export const ConnectedSince = (user: ConnectedSinceProps) => {
    if (user.knownSince) {
        return (
            <React.Fragment>
                Connected <Moment fromNow>{user.knownSince}</Moment>
            </React.Fragment>
        );
    }
    return <React.Fragment></React.Fragment>;
}

const UserCard = (user: User) => {
    const classes = useStyles();
    return (
        <Grid xs={12} sm={6} item key={user.id} style={{display: 'flex'}}>
            <Card className={classes.card}>
                <CardContent>
                    <Grid container justify="space-between">
                        <Grid item>
                            <Typography color="textSecondary">
                                {user.network}
                            </Typography>
                        </Grid>
                        <Grid item>
                            <Typography color="textSecondary">
                                <ConnectedSince {...user} />
                            </Typography>
                        </Grid>
                    </Grid>
                    <div className={classes.root}>
                        <Avatar alt={user.name} src={user.avatarUrl.startsWith('data') ? undefined : user.avatarUrl} className={classes.large}>
                            {user.avatarUrl.startsWith('data') ? user.name.split(' ').map(i => i.charAt(0).toUpperCase()) : undefined }
                        </Avatar>
                        <div>
                            <Typography variant="h5" component="h2">
                                {user.name}
                            </Typography>
                            <Typography className={classes.pos} color="textSecondary">
                                {user.description}
                            </Typography>
                        </div>
                    </div>
                </CardContent>
                {/* <CardActions>
                <Button size="small">Learn More</Button>
                </CardActions> */}
            </Card>
        </Grid>
    );
}