import React from 'react';
import { useLocation, Route } from 'react-router-dom'
import { useHeader } from '../../store/useHeader';
import { useDispatch } from 'react-redux';
import { Backfill } from './Backfill';

// type Overview = {
//   groups: number;
//   messages: number;
//   threads: number;
//   users: number;
// }

const Yammer = () => {
  //const [overview, setOverview] = useState<Overview|undefined>(undefined);
  const dispatch = useDispatch();
  // const requestUsers = (search: string, pageNumber: number, pageSize: number) => 
  //   dispatch(YammerStore.actionCreators.requestUsers(pageNumber, pageSize, search));
  // const requestMessages = (search: string, pageNumber: number, pageSize: number) => 
  //   dispatch(YammerStore.actionCreators.requestMessages(pageNumber, pageSize, search));
  //   const { users, messages } = useSelector<YammerStore.YammerState>(state => state.yammer);
    const location = useLocation();

  // useEffect(() => {
  //   fetch(`api/yammer`)
  //   .then(response => response.json() as Promise<Overview>)
  //   .then(data => {
  //       setOverview(data);
  //   });
  // }, [])

  useHeader({
      title: 'Yammer',
      route: '/yammer',
      items: [
        // { title: 'Overview', to: ''},
        // { title: 'Users', badge: overview ? overview.users : undefined, to: '/users'},
        // { title: 'Messages', badge: overview ? overview.messages : undefined, to: '/messages'},
        // { title: 'Notifications', to: '/notifications'},
        { title: 'Backfill', to: '/backfill'}
      ],
    }, []);

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
      { location.pathname === '/yammer' && <div>{location.pathname}</div> }
    </>
  );
};

export default Yammer;
