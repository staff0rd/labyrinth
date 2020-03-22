import * as React from 'react'
import { Event } from '../../store/Event';
import Card from '@material-ui/core/Card';
import CardActions from '@material-ui/core/CardActions';
import CardContent from '@material-ui/core/CardContent';
import Link from '@material-ui/core/Link';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import Avatar from '@material-ui/core/Avatar';
import { makeStyles } from '@material-ui/core/styles';
import Moment from 'react-moment';
import moment from 'moment';
import {UserSquare} from '../Users/UserSquare';
import {UserBullet} from '../Users/UserBullet';
import reactStringReplace from 'react-string-replace';
import { Button } from '@material-ui/core';
import ReactJson from 'react-json-view'

type EventCardsProps = {
    events: Event[];
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

export const EventCards = (props: EventCardsProps) => {
    
    return (
        <Grid container alignItems="stretch" spacing={1} style={{marginBottom: 8}}>
            {props.events.map((event: Event) => <EventCard {...event} />)}
        </Grid>
    );
}

const EventCard = (event: Event) => {
    const classes = useStyles();

    return (
        <Grid xs={12} item key={event.id} style={{display: 'flex'}}>
            
            <Card className={classes.card}>
                <CardContent>
                    <Grid container>
                        <Grid item xs={12}>
                            <Typography variant="overline">
                                    <Moment format="dddd, MMMM Do YYYY, h:mm:ss a">{event.timestamp}</Moment>
                            </Typography>
                            <ReactJson src={JSON.parse(event.body)} />
                        </Grid>
                    </Grid>
                </CardContent>
                {/* <CardActions>
                    
                </CardActions> */}
            </Card>
        </Grid>
    );
}