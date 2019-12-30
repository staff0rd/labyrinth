import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as LinkedInStore from '../store/LinkedIn';
import { UserSearch } from './UserSearch';

type LinkedInProps =
  LinkedInStore.LinkedInState // ... state we've requested from the Redux store
  & typeof LinkedInStore.actionCreators // ... plus action creators we've requested
  & RouteComponentProps<{ startDateIndex: string }>; // ... plus incoming routing parameters

const LinkedIn = (props: LinkedInProps) => {
  const searchPlaceholder = "Search by name or occupation";
  const searchRequest = (search: string, pageNumber: number, pageSize: number) => props.requestUsers(pageNumber, pageSize, search);

  return (
    <UserSearch users={props.users} searchPlaceholder={searchPlaceholder} searchRequest={searchRequest} />
  );
};

export default connect(
  (state: ApplicationState) => state.linkedIn, // Selects which state properties are merged into the component's props
  LinkedInStore.actionCreators // Selects which action creators are merged into the component's props
)(LinkedIn as any);
