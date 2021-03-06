import React, { useState, useEffect } from 'react';
import { useLocation, Route } from 'react-router-dom'
import { useHeader } from '../../store/useHeader';
import { Backfill } from './Backfill';
import { Queue } from '../Queue';
import { postResponse } from '../../api';
import { useSelector } from '../../store/useSelector';
import { AccountState } from '../../store/Account';
import { Users } from '../Users/Users';
import { Events } from '../Events/Events';
import { Messages } from '../Messages/Messages';
import Alert from '@material-ui/lab/Alert';
import { OverviewProps, Overview } from '../Overview';
import { useSource } from '../useSource';

const Yammer = () => {
  const [overview, setOverview] = useState<OverviewProps>();
  const { password, userName } = useSelector<AccountState>(state => state.account);
  const [error, setError] = useState<string>("");
  const location = useLocation();
  const { sourceId, sourceName } = useSource('Yammer');

  useEffect(() => {
    postResponse<OverviewProps>(`api/events/overview`, {userName, password, sourceId})
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
        { title: 'Events', to: '/events', badge: overview ? overview.events.map(m => m.count).reduce((prev, cur) => prev + cur, 0) : undefined},
        { title: 'Hydrate', to: '/hydrate'},
        { title: 'Backfill', to: '/backfill'},
        { title: 'Process', to: '/process'},
      ],
    }, [overview]);

  return (
    <>
      <Route path='/yammer/backfill' component={() => <Backfill sourceId={sourceId} />} />
      <Route path='/yammer/hydrate' component={() => <Queue url={'api/events/hydrate'} sourceId={sourceId} />} />
      <Route path='/yammer/process' component={() => <Queue url={'api/yammer/process'} sourceId={sourceId} />} />
      <Route path='/yammer/events' component={() => (
        <Events
          url={`api/events/events`} 
          searchPlaceholder="Search events"
          network='Yammer'
        />
      )} />
      <Route path='/yammer/users' component={() => (
        <Users
          url={`api/events/users`} 
          searchPlaceholder="Search by name or job title"
          network="Yammer"
        />
      )} />
      <Route path='/yammer/messages' component={() => (
        <Messages
          url={`api/events/messages`} 
          searchPlaceholder="Search by sender or message content"
          network='Yammer'
        />
      )} />

      { error && <Alert severity="error">{error}</Alert> }

      { location.pathname === "/yammer" && overview && (<Overview {...overview} />) }
    </>
  );
};

export default Yammer;
