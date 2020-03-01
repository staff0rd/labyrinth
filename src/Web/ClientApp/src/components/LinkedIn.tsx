import * as React from 'react';
import { useHeader } from '../store/useHeader';
import { useDispatch } from 'react-redux';
import { useSelector } from '../store/useSelector';
import { ApplicationState } from '../store';
import * as LinkedInStore from '../store/LinkedIn';
import { Users } from './Users';
import { useLocation} from "react-router";

export const LinkedIn = () => {
  const dispatch = useDispatch();
  const users = useSelector(state => state.linkedIn && state.linkedIn.users);
  const searchRequest = (search: string, pageNumber: number, pageSize: number) => 
    dispatch(LinkedInStore.actionCreators.requestUsers(pageNumber, pageSize, search));

  const location = useLocation();
  useHeader({
    title: 'LinkedIn',
    route: '/linkedin',
    items: [
      { title: 'Overview', to: ''},
      { title: 'Users', to: '/users'},
      { title: 'Messages', to: '/messages'},
      { title: 'Notifications', to: '/notifications'},
    ],
  });

  switch (location.pathname) {
    case '/linkedin/users': return users && (
      <Users users={users} searchPlaceholder="Search by name or occupation" searchRequest={searchRequest} />
    );
    default: return (<div>{location.pathname}</div>)
  }
};
