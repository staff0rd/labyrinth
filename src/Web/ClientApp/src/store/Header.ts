import { Action, Reducer } from 'redux';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

interface HeaderItem {
    title: string;
    to: string;
}

export interface HeaderState {
    title: string;
    items: HeaderItem[];
}

const SET_HEADER = 'SET_HEADER';

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

interface SetHeaderAction {
    type: typeof SET_HEADER;
    header: HeaderState;
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = SetHeaderAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data)

export const actionCreators = {
    setHeader: (header: HeaderState) => ({ type: SET_HEADER, header } as SetHeaderAction)
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: HeaderState = { title: '', items: [] };

export const reducer: Reducer<HeaderState> = (state: HeaderState | undefined, incomingAction: Action): HeaderState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
    if (action) {
        switch (action.type) {
            case SET_HEADER:
                return {
                    ...state,
                    ...action.header,
                };
        }
    } 
    return state;
};
