import React, { useState, useEffect } from 'react';
import { useLocation, Route } from 'react-router-dom'
import { useHeader } from '../../store/useHeader';
import { Backfill } from './Backfill';
import { Queue } from '../Queue';
import { postResponse } from '../../api';
import { useSelector } from '../../store/useSelector';
import { AccountState } from '../../store/Account';
import { Users } from '../Users/Users';
import { Messages } from '../Messages/Messages';
import Alert from '@material-ui/lab/Alert';
import { OverviewProps, Overview } from '../Overview';

const Teams = () => {
  const [overview, setOverview] = useState<OverviewProps>();
  const { password, userName } = useSelector<AccountState>(state => state.account);
  const [error, setError] = useState<string>("");
  const location = useLocation();

  useEffect(() => {
    postResponse<OverviewProps>(`api/events/overview?network=Teams`, {userName, password})
    .then(data => {
      if (data.isError)
        setError(data.message!);
      else
        data && setOverview(data.response);
    });
  }, [])

  useHeader({
      title: 'Teams',
      route: '/teams',
      items: [
        { title: 'Overview', to: ''},
        { title: 'Users', badge: overview ? overview.users : undefined, to: '/users'},
        { title: 'Messages', badge: overview ? overview.messages : undefined, to: '/messages'},
        { title: 'Hydrate', to: '/hydrate'},
        { title: 'Backfill', to: '/backfill'},
        { title: 'Events', to: '/events'},
//        { title: 'Process', to: '/process'},
      ],
    }, [overview]);

  return (
    <>
      <Route path='/teams/backfill' component={Backfill} />
      <Route path='/teams/hydrate' component={() => <Queue url={'api/events/hydrate'} />} />
      <Route path='/teams/process' component={() => <Queue url={'api/teams/process'} />} />
      <Route path='/teams/users' component={() => (
        <Users
          url={`api/teams/users`} 
          searchPlaceholder="Search by name or job title"
        />
      )} />
      <Route path='/teams/messages' component={() => (
        <Messages
          url={`api/teams/messages`} 
          searchPlaceholder="Search by sender or message content"
        />
      )} />

      { error && <Alert severity="error">{error}</Alert> }

      { location.pathname === "/teams" && overview && (<Overview {...overview} />) }
    </>
  );
};

export default Teams;
