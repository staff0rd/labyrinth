import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import * as HeaderStore from '../store/Header';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as LinkedInStore from '../store/LinkedIn';
import { UserSearch } from './UserSearch';
import { useLocation} from "react-router";

type LinkedInProps =
  LinkedInStore.LinkedInState // ... state we've requested from the Redux store
  & typeof LinkedInStore.actionCreators // ... plus action creators we've requested
  & typeof HeaderStore.actionCreators
  & RouteComponentProps<{ startDateIndex: string }>; // ... plus incoming routing parameters

const LinkedIn = (props: LinkedInProps) => {
  const searchRequest = (search: string, pageNumber: number, pageSize: number) => props.requestUsers(pageNumber, pageSize, search);
  const location = useLocation();

  React.useEffect(() => {
    props.setHeader({
      title: 'LinkedIn',
      items: [
        { title: 'Overview', to: '/linkedin'},
        { title: 'Users', to: '/linkedin/users'},
        { title: 'Messages', to: '/linkedin/messages'},
        { title: 'Notifications', to: '/linkedin/notifications'},
      ],
    })
  }, [])

  switch (location.pathname) {
    case '/linkedin/users': return (
      <UserSearch users={props.users} searchPlaceholder="Search by name or occupation" searchRequest={searchRequest} />
    );
    default: return (<div>{location.pathname}</div>)
  }
};

export default connect(
  (state: ApplicationState) => state.linkedIn, // Selects which state properties are merged into the component's props
  (dispatch) => ({
    ...bindActionCreators(LinkedInStore.actionCreators, dispatch),
    ...bindActionCreators(HeaderStore.actionCreators, dispatch),
   }) // Selects which action creators are merged into the component's props
)(LinkedIn as any);
