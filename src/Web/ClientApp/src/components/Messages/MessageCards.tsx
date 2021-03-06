import * as React from 'react'
import { Message } from '../../store/Message';
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
import ReactHtmlParser from 'react-html-parser'; 

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

    const parse = (message: string, network: string) => {
        if (!message)
            return message;

        // fix for yammer
        // let replacedText = reactStringReplace(message, /\[\[user:(\d+)\]\]/g, (match, i) => (
        //     <UserBullet id={`yammer/user/${match}`} network={network} />
        //   )); // users
        
        //   replacedText = reactStringReplace(replacedText, /(https?:\/\/.+.(?:gif|jpg))/g, (match, i) => (
        //     <img src={match} />
        //   )); // images
          
        //   replacedText = reactStringReplace(replacedText, /((?:http|ftp|https):\/\/(?:[\w_-]+(?:(?:\.[\w_-]+)+))(?:[\w.,@?^=%&:\/~+#-]*[\w@?^=%&\/~+#-])?)/g, (match, i) => {
        //       return ( // images
        //     <a href={match}>{match}</a>
        //   )}); // urls
        // return (
        // <React.Fragment>
        //     {replacedText}
        // </React.Fragment>
        // );
            
        return (
            <>
                { ReactHtmlParser(message, {
                    transform: (node, index) => {
                        if (node.type === 'tag' && node.name === 'img' && node.attribs){
                            delete node.attribs.style;
                        }
                    }
                })}
            </>
        )
    }

    return (
        <Grid xs={12} item key={message.id} style={{display: 'flex'}}>
            
            <Card className={classes.card}>
                <CardContent>
                    <Grid container>
                        <Grid item xs={3}>
                            <UserSquare id={message.senderId} network={message.network} />
                        </Grid>
                        <Grid item xs={9}>
                            <Typography variant="overline">
                                
                                    <Moment format="dddd, MMMM Do YYYY, h:mm:ss a">{message.createdAt}</Moment>
                                    
                            </Typography>
                            <Typography>
                                {parse(message.bodyParsed, message.network) || parse(message.bodyPlain, message.network)}
                            </Typography>
                        </Grid>
                    </Grid>
                </CardContent>
                <CardActions>
                    <Button variant='contained' href={message.permalink} target="_blank">Source</Button>
                </CardActions>
            </Card>
        </Grid>
    );
}