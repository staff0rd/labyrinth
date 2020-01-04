import * as React from 'react'
import { Message } from '../store/Message';
import Card from '@material-ui/core/Card';
import CardActions from '@material-ui/core/CardActions';
import CardContent from '@material-ui/core/CardContent';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import Avatar from '@material-ui/core/Avatar';
import { makeStyles } from '@material-ui/core/styles';
import Moment from 'react-moment';
import moment from 'moment';
import {UserSquare} from './UserSquare';
import {UserBullet} from './UserBullet';
import reactStringReplace from 'react-string-replace';

type MessageCardsProps = {
    messages: Message[];
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

export const MessageCards = (props: MessageCardsProps) => {
    
    return (
        <Grid container alignItems="stretch" spacing={1} style={{marginBottom: 8}}>
            {props.messages.map((message: Message) => <MessageCard {...message} />)}
        </Grid>
    );
}

const MessageCard = (message: Message) => {
    const classes = useStyles();

    const parse = (message: string) => {
        if (!message)
            return message;

        let replacedText = reactStringReplace(message, /\[\[user:(\d+)\]\]/g, (match, i) => (
            <UserBullet id={match} />
          ));
        
          replacedText = reactStringReplace(replacedText, /(https?:\/\/.+.(?:gif|jpg))/g, (match, i) => (
            <img src={match} />
          ));

        return (
        <React.Fragment>
            {replacedText}
        </React.Fragment>
        );
            
    }

    return (
        <Grid xs={12} item key={message.id} style={{display: 'flex'}}>
            
            <Card className={classes.card}>
                <CardContent>
                    <Grid container>
                        <Grid item xs={3}>
                            <UserSquare id={message.sender_id} />
                        </Grid>
                        <Grid item xs={9}>
                            <Typography variant="overline">
                                <Moment format="dddd, MMMM Do YYYY, h:mm:ss a">{message.created_at}</Moment>
                            </Typography>
                            <Typography>
                                {parse(message.body.parsed) || parse(message.body.plain)}
                            </Typography>
                        </Grid>
                    </Grid>
                </CardContent>
                {/* <CardActions>
                <Button size="small">Learn More</Button>
                </CardActions> */}
            </Card>
        </Grid>
    );
}