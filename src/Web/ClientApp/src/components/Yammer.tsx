import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as YammerStore from '../store/Yammer';
import { Users } from './Users'
import { Messages } from './Messages'
import { useLocation } from 'react-router-dom'
;import { bindActionCreators } from 'redux';
import * as HeaderStore from '../store/Header';

type YammerProps =
  YammerStore.YammerState
  & typeof YammerStore.actionCreators
  & typeof HeaderStore.actionCreators;

const Yammer = (props: YammerProps) => {
  const requestUsers = (search: string, pageNumber: number, pageSize: number) => props.requestUsers(pageNumber, pageSize, search);
  const requestMessages = (search: string, pageNumber: number, pageSize: number) => props.requestMessages(pageNumber, pageSize, search);
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
      <Users users={props.users} searchPlaceholder="Search by name or job title" searchRequest={requestUsers} />
    );
    case '/yammer/messages': return (
      <Messages messages={props.messages} searchPlaceholder="Search by sender or message content" searchRequest={requestMessages} />
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
