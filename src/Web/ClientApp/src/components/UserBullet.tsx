import React, {useEffect, useState} from "react";
import { Link } from 'react-router-dom';
import {User} from '../store/User';
import Avatar from '@material-ui/core/Avatar';
import Chip from '@material-ui/core/Chip';
import { makeStyles } from '@material-ui/core/styles';
import { Typography } from "@material-ui/core";

type Props = {
    id: string;
}

const useStyles = makeStyles(theme => ({
    avatar: {
      width: theme.spacing(2),
      height: theme.spacing(2),
    },
}));

export const UserBullet = ({ id }: Props) => {
    const [user, setUser] = useState<User|undefined>(undefined);
    const classes = useStyles();

    useEffect(() => {
        fetch(`api/yammer/users/${id}`)
        .then(response => response.json() as Promise<User>)
        .then(data => {
            setUser(data);
        });
    }, [id])

    const handleClick = () => {};

    if (user) 
    return (
      <Chip avatar={(
        <Avatar alt={user.name} src={user.avatarUrl.startsWith('data') ? undefined : user.avatarUrl} className={classes.avatar}>
          {user.avatarUrl.startsWith('data') ? user.name.split(' ').map(i => i.charAt(0).toUpperCase()) : undefined }
        </Avatar>
      )} label={user.name} onClick={handleClick} />
    );
    return <Chip avatar={<Avatar>?</Avatar>}> /></Chip>
}
