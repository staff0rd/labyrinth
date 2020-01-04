import React, {useEffect, useState} from "react";
import {User} from '../store/User';
import Avatar from '@material-ui/core/Avatar';
import { makeStyles } from '@material-ui/core/styles';
import { Typography } from "@material-ui/core";

type Props = {
    id: number;
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

export const UserSquare = ({ id }: Props) => {
    const [user, setUser] = useState<User|undefined>(undefined);
    const classes = useStyles();

    useEffect(() => {
        fetch(`api/yammer/users/${id}`)
        .then(response => response.json() as Promise<User>)
        .then(data => {
            setUser(data);
        });
    }, [id])

    if (user) 
    return (
      <React.Fragment>
        <Avatar alt={user.name} src={user.avatarUrl.startsWith('data') ? undefined : user.avatarUrl} className={classes.large}>
            {user.avatarUrl.startsWith('data') ? user.name.split(' ').map(i => i.charAt(0).toUpperCase()) : undefined }
        </Avatar>
        <Typography>
          {user.name}
        </Typography>
      </React.Fragment>
    );
    return <Avatar>?</Avatar>
}
