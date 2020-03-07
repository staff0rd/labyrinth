import React, { useState, useEffect } from 'react';
import { useLocation, Route } from 'react-router-dom'
import { useHeader } from '../../store/useHeader';
import { useDispatch } from 'react-redux';
import { Backfill } from './Backfill';
import { Queue } from './Queue';
import { postResponse } from '../../api';
import { useSelector } from '../../store/useSelector';
import { AccountState } from '../../store/Account';
import { Users } from '../Users';
import Alert from '@material-ui/lab/Alert';

type Overview = {
  groups: number;
  messages: number;
  threads: number;
  users: number;
}

const Yammer = () => {
  const [overview, setOverview] = useState<Overview|undefined>(undefined);
  const dispatch = useDispatch();
  const { password, userName } = useSelector<AccountState>(state => state.account);
  const [error, setError] = useState<string>("");

  // const requestUsers = (search: string, pageNumber: number, pageSize: number) => 
  //   dispatch(YammerStore.actionCreators.requestUsers(pageNumber, pageSize, search));
  // const requestMessages = (search: string, pageNumber: number, pageSize: number) => 
  //   dispatch(YammerStore.actionCreators.requestMessages(pageNumber, pageSize, search));
  //   const { users, messages } = useSelector<YammerStore.YammerState>(state => state.yammer);

  useEffect(() => {
    postResponse<Overview>(`api/yammer/overview`, {userName, password})
    .then(data => {
      if (data.isError)
        setError(data.message!);
      else
        data && setOverview(data.response);
    });
  }, [])

  useHeader({
      title: 'Yammer',
      route: '/yammer',
      items: [
        { title: 'Overview', to: ''},
        { title: 'Users', badge: overview ? overview.users : undefined, to: '/users'},
        { title: 'Messages', badge: overview ? overview.messages : undefined, to: '/messages'},
        // { title: 'Notifications', to: '/notifications'},
        { title: 'Hydrate', to: '/hydrate'},
        { title: 'Backfill', to: '/backfill'},
        { title: 'Process', to: '/process'},
      ],
    }, [overview]);

  // switch (location.pathname) {
  //   case '/yammer/users': return (
  //     <Users users={users} searchPlaceholder="Search by name or job title" searchRequest={requestUsers} />
  //   );
  //   case '/yammer/messages': return (
  //     <Messages messages={messages} searchPlaceholder="Search by sender or message content" searchRequest={requestMessages} />
  //   );
  //   default: return (<div>{location.pathname}</div>)
  // }

  return (
    <>
      <Route path='/yammer/backfill' component={Backfill} />
      <Route path='/yammer/hydrate' component={() => <Queue url={'api/yammer/hydrate'} />} />
      <Route path='/yammer/process' component={() => <Queue url={'api/yammer/process'} />} />
      <Route path='/yammer/users' component={() => (<Users
        url={`api/yammer/users`} 
        searchPlaceholder="Search by name or job title"
      />)}
      />

      { error && <Alert severity="error">{error}</Alert> }
    </>
  );
};

export default Yammer;
