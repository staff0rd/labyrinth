import React, {useEffect, useState} from "react";
import { Link } from 'react-router-dom';
import {User} from '../../store/User';
import Avatar from '@material-ui/core/Avatar';
import Chip from '@material-ui/core/Chip';
import { makeStyles } from '@material-ui/core/styles';
import { Typography } from "@material-ui/core";
import { postResponse } from '../../api';
import { useSelector } from '../../store/useSelector';
import { AccountState } from '../../store/Account';

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

    const { password, userName } = useSelector<AccountState>(state => state.account);

    useEffect(() => {
        postResponse<User>(`api/yammer/user`, {userName, password, id})
        .then(data => {
          if (data && data.response)
            setUser(data.response);
        });
    }, [id])

    const handleClick = () => {};

    if (user) 
    return (
      <Chip avatar={user.avatarUrl ? (
        <Avatar alt={user.name} src={user.avatarUrl.startsWith('data') ? undefined : user.avatarUrl} className={classes.avatar}>
          {user.avatarUrl.startsWith('data') ? user.name.split(' ').map(i => i.charAt(0).toUpperCase()) : undefined }
      </Avatar> ) : <Avatar></Avatar>}
      label={user.name} onClick={handleClick} />
    );
    return <Chip avatar={<Avatar>?</Avatar>}> /></Chip>
}
