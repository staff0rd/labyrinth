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
import { SourceSelector } from './SourceSelector';
import { Grid } from '@material-ui/core';

const Teams = () => {
  const [overview, setOverview] = useState<OverviewProps>();
  const { password, userName } = useSelector<AccountState>(state => state.account);
  const [error, setError] = useState<string>("");
  const location = useLocation();
  const { sourceId, sourceName } = useSource('Teams');
  
  useEffect(() => {
    postResponse<OverviewProps>(`api/events/overview?network=Teams`, {userName, password, sourceId})
    .then(data => {
      if (data.isError)
        setError(data.message!);
      else
        data && setOverview(data.response);
    });
  }, [sourceId])

  useHeader({
      title: `Teams - ${sourceName}`,
      route: '/teams',
      items: [
        { title: 'Overview', to: ''},
        { title: 'Users', badge: overview ? overview.users : undefined, to: '/users'},
        { title: 'Messages', badge: overview ? overview.messages : undefined, to: '/messages'},
        { title: 'Hydrate', to: '/hydrate'},
        { title: 'Backfill', to: '/backfill'},
        { title: 'Events', to: '/events'},
        { title: 'Process', to: '/process'},
      ],
    }, [overview]);

  return (
    <>
      <Route path='/teams/backfill' component={Backfill} />
      <Route path='/teams/hydrate' component={() => <Queue url={'api/events/hydrate'} sourceId={sourceId} />} />
      <Route path='/teams/process' component={() => <Queue url={'api/teams/process'} sourceId={sourceId} token/>} />
      <Route path='/teams/users' component={() => (
        <Users
          url={`api/events/users`} 
          searchPlaceholder="Search by name or job title"
          network='Teams'
        />
      )} />
      <Route path='/teams/events' component={() => (
        <Events
          url={`api/events/events`} 
          searchPlaceholder="Search events"
          network='Teams'
        />
      )} />
      <Route path='/teams/messages' component={() => (
        <Messages
          url={`api/events/messages`} 
          searchPlaceholder="Search by sender or message content"
          network='Teams'
        />
      )} />

      { location.pathname === "/teams" && (
        <Grid container>
          <Grid item xs={12}>
            <SourceSelector network='Teams' />
          </Grid>
          
          { error && (
            <Grid item xs={12}>
              <Alert severity="error">{error}</Alert>
            </Grid>
          )}
          
          { overview && (
            <Grid item xs={12}>
              <Overview {...overview} />
            </Grid>
          )}
        </Grid>
      )}

      

      
    </>
  );
};

export default Teams;
