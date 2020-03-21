import React, {useEffect, useState} from "react";
import {User} from '../../store/User';
import Avatar from '@material-ui/core/Avatar';
import { makeStyles } from '@material-ui/core/styles';
import { Typography } from "@material-ui/core";
import { postResponse } from '../../api';
import { useSelector } from '../../store/useSelector';
import { AccountState } from '../../store/Account';

type Props = {
    id: string;
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
    const [user, setUser] = useState<User>();
    const classes = useStyles();
    const { password, userName } = useSelector<AccountState>(state => state.account);

    useEffect(() => {
        postResponse<User>(`api/yammer/user`, {userName, password, id})
        .then(data => {
          if (data && data.response) {
            setUser(data.response);
          }
        });
    }, [id])

    if (user && user.name) {
      return (
        <React.Fragment>
          {user.avatarUrl ? (
            <Avatar alt={user.name} src={user.avatarUrl.startsWith('data') ? undefined : user.avatarUrl} className={classes.large}>
                {user.avatarUrl.startsWith('data') ? user.name.split(' ').map(i => i.charAt(0).toUpperCase()) : undefined }
            </Avatar>
          ) : <Avatar></Avatar> }
          <Typography>
            {user.name}
          </Typography>
        </React.Fragment>
      );
    }
    return <Avatar>?</Avatar>
}
