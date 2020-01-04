import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as YammerStore from '../store/Yammer';
import { UserSearch } from './UserSearch';
import { useLocation } from 'react-router-dom'
;import { bindActionCreators } from 'redux';
import * as HeaderStore from '../store/Header';

type YammerProps =
  YammerStore.YammerState // ... state we've requested from the Redux store
  & typeof YammerStore.actionCreators
  & typeof HeaderStore.actionCreators
  & RouteComponentProps; // ... plus action creators we've requested

const Yammer = (props: YammerProps) => {
  const searchRequest = (search: string, pageNumber: number, pageSize: number) => props.requestUsers(pageNumber, pageSize, search);
  const location = useLocation();

  React.useEffect(() => {
    props.setHeader({
      title: 'Yammer',
      items: [
        { title: 'Overview', to: '/yammer'},
        { title: 'Users', to: '/yammer/users'},
        { title: 'Messages', to: '/yammer/messages'},
        { title: 'Notifications', to: '/yammer/notifications'},
      ],
    })
  }, [])

  switch (location.pathname) {
    case '/yammer/users': return (
      <UserSearch users={props.users} searchPlaceholder="Search by name or job title" searchRequest={searchRequest} />
    );
    default: return (<div>{location.pathname}</div>)
  }
};

export default connect(
  (state: ApplicationState) => state.yammer, // Selects which state properties are merged into the component's props
  (dispatch) => ({
    ...bindActionCreators(YammerStore.actionCreators, dispatch),
    ...bindActionCreators(HeaderStore.actionCreators, dispatch),
   }) // Selects which action creators are merged into the component's props
)(Yammer as any);
