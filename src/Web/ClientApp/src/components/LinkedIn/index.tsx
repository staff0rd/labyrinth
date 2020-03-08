import React, { useState, useEffect } from 'react';
import { Route, useLocation } from 'react-router-dom'
import { useHeader } from '../../store/useHeader';
import { Backfill } from './Backfill';
import { Queue } from '../Queue';
import { postResponse } from '../../api';
import { useSelector } from '../../store/useSelector';
import { AccountState } from '../../store/Account';
import { Users } from '../Users';
import { Messages } from '../Messages';
import Alert from '@material-ui/lab/Alert';
import { Overview, OverviewProps } from '../Overview'

const Yammer = () => {
  const [overview, setOverview] = useState<OverviewProps>();
  const { password, userName } = useSelector<AccountState>(state => state.account);
  const [error, setError] = useState<string>("");
  const location = useLocation();

  useEffect(() => {
    postResponse<OverviewProps>(`api/events/overview?network=LinkedIn`, {userName, password})
    .then(data => {
      if (data.isError)
        setError(data.message!);
      else
        data && setOverview(data.response);
    });
  }, [])

  useHeader({
      title: 'LinkedIn',
      route: '/linkedin',
      items: [
        { title: 'Overview', to: ''},
        // { title: 'Users', badge: overview ? overview.users : undefined, to: '/users'},
        // { title: 'Messages', badge: overview ? overview.messages : undefined, to: '/messages'},
        { title: 'Hydrate', to: '/hydrate'},
        { title: 'Backfill', to: '/backfill'},
        { title: 'Process', to: '/process'},
      ],
    }, [overview]);

  return (
    <>
      <Route path='/linkedin/backfill' component={Backfill} />
      <Route path='/linkedin/hydrate' component={() => <Queue url={'api/linkedin/hydrate'} />} />
      <Route path='/linkedin/process' component={() => <Queue url={'api/linkedin/process'} />} />
      <Route path='/linkedin/users' component={() => (
        <Users
          url={`api/linkedin/users`} 
          searchPlaceholder="Search by name or job title"
        />
      )} />
      <Route path='/linkedin/messages' component={() => (
        <Messages
          url={`api/linkedin/messages`} 
          searchPlaceholder="Search by sender or message content"
        />
      )} />

      { error && <Alert severity="error">{error}</Alert> }
      { location.pathname === "/linkedin" && overview && (<Overview {...overview} />) }
    </>
  );
};

export default Yammer;
