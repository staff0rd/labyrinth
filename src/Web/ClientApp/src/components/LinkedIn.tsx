import * as React from 'react';
import { useEffect} from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as LinkedInStore from '../store/LinkedIn';

// At runtime, Redux will merge together...
type LinkedInProps =
  LinkedInStore.LinkedInState // ... state we've requested from the Redux store
  & typeof LinkedInStore.actionCreators // ... plus action creators we've requested
  & RouteComponentProps<{ startDateIndex: string }>; // ... plus incoming routing parameters


const UserTable = (props: LinkedInProps) => {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th></th> 
            <th>Name</th>
            <th>Occupation</th>
          </tr>
        </thead>
        <tbody>
          {props.users.map((user: LinkedInStore.User) =>
            <tr key={user.profileUrl}>
              <td><img src={user.mugshotUrl} /></td>
              <td>{user.name}</td>
              <td>{user.occupation}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
}

const LinkedIn = (props: LinkedInProps) => {
  useEffect(() => {
    props.requestUsers();
  }, []);

  return (
    <React.Fragment>
      <h1 id="tabelLabel">LinkedIn</h1>
      <p>This component demonstrates fetching data from the server and working with URL parameters.</p>
      <UserTable {...props} />
      {/* {this.renderPagination()} */}
    </React.Fragment>
  );

  // private renderPagination() {
  //   const prevStartDateIndex = (this.props.startDateIndex || 0) - 5;
  //   const nextStartDateIndex = (this.props.startDateIndex || 0) + 5;

  //   return (
  //     <div className="d-flex justify-content-between">
  //       <Link className='btn btn-outline-secondary btn-sm' to={`/fetch-data/${prevStartDateIndex}`}>Previous</Link>
  //       {this.props.isLoading && <span>Loading...</span>}
  //       <Link className='btn btn-outline-secondary btn-sm' to={`/fetch-data/${nextStartDateIndex}`}>Next</Link>
  //     </div>
  //   );
  // }
}

export default connect(
  (state: ApplicationState) => state.linkedIn, // Selects which state properties are merged into the component's props
  LinkedInStore.actionCreators // Selects which action creators are merged into the component's props
)(LinkedIn as any);
