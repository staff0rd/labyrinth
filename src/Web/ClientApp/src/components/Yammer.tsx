import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as YammerStore from '../store/Yammer';
import { UserSearch } from './UserSearch';

type YammerProps =
  YammerStore.YammerState // ... state we've requested from the Redux store
  & typeof YammerStore.actionCreators; // ... plus action creators we've requested

const Yammer = (props: YammerProps) => {
  const searchPlaceholder = "Search by name or job title";
  const searchRequest = (search: string, pageNumber: number, pageSize: number) => props.requestUsers(pageNumber, pageSize, search);

  return (
    <UserSearch users={props.users} searchPlaceholder={searchPlaceholder} searchRequest={searchRequest} />
  );
};

export default connect(
  (state: ApplicationState) => state.yammer, // Selects which state properties are merged into the component's props
  YammerStore.actionCreators // Selects which action creators are merged into the component's props
)(Yammer as any);
