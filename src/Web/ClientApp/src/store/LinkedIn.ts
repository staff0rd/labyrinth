import { Action, Reducer } from 'redux';
import { AppThunkAction } from '.';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface LinkedInState {
    isLoading: boolean;
    users: Paged<User>;
}

export interface User {
    mugshotUrl: string;
    name: string;
    occupation: number;
    profileUrl: string;
    connected: Date;    
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

interface RequestUsersAction {
    type: 'REQUEST_USERS';
    pageSize: number;
    page: number;
}

interface Paged<T> {
    rows: T[]; 
    pageSize: number; 
    page: number; 
    totalRows: number;
}

interface ReceiveUsersAction {
    type: 'RECEIVE_USERS';
    users: Paged<User>;
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = RequestUsersAction | ReceiveUsersAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data)

export const actionCreators = {
    requestUsers: (page: number, pageSize: number, search = ""): AppThunkAction<KnownAction> => (dispatch, getState) => {
        fetch(`api/linkedin?pageNumber=${page}&pageSize=${pageSize}&search=${search}`)
            .then(response => response.json() as Promise<Paged<User>>)
            .then(data => {
                dispatch({ type: 'RECEIVE_USERS', users: data });
            });

        dispatch({ type: 'REQUEST_USERS', page, pageSize });
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: LinkedInState = { users: { rows: [], page: 0, pageSize: 20, totalRows: 0}, isLoading: false };

export const reducer: Reducer<LinkedInState> = (state: LinkedInState | undefined, incomingAction: Action): LinkedInState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'REQUEST_USERS':
            return {
                ...state,
                isLoading: true
            };
        case 'RECEIVE_USERS':
            // Only accept the incoming data if it matches the most recent request. This ensures we correctly
            // handle out-of-order responses.
            return {
                ...state,
                users: action.users,
                isLoading: false
            };
        break;
    }
    return state;
};
