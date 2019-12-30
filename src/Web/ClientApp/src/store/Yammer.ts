import { Action, Reducer } from 'redux';
import { AppThunkAction } from '.';
import { User } from './User';
import { Paged } from './Paged';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface YammerState {
    isLoading: boolean;
    users: Paged<User>;
}

const REQUEST_YAMMER_USERS = 'REQUEST_YAMMER_USERS';
const RECEIVE_YAMMER_USERS = 'RECEIVE_YAMMER_USERS';

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

interface RequestUsersAction {
    type: typeof REQUEST_YAMMER_USERS;
    pageSize: number;
    page: number;
}

interface ReceiveUsersAction {
    type: typeof RECEIVE_YAMMER_USERS;
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
        fetch(`api/yammer?pageNumber=${page}&pageSize=${pageSize}&search=${search}`)
            .then(response => response.json() as Promise<Paged<User>>)
            .then(data => {
                dispatch({ type: RECEIVE_YAMMER_USERS, users: data });
            });

        dispatch({ type: REQUEST_YAMMER_USERS, page, pageSize });
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: YammerState = { users: { rows: [], page: 0, pageSize: 20, totalRows: 0}, isLoading: false };

export const reducer: Reducer<YammerState> = (state: YammerState | undefined, incomingAction: Action): YammerState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
    if (action) {
        switch (action.type) {
            case REQUEST_YAMMER_USERS:
                return {
                    ...state,
                    isLoading: true
                };
            case RECEIVE_YAMMER_USERS:
                return {
                    ...state,
                    users: action.users,
                    isLoading: false
                };
        }
    } 
    return state;
};
