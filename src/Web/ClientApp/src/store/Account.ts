import { Action, Reducer } from 'redux';
import { Source } from './Source';

export interface AccountState {
    userName?: string;
    password?: string;
    sources: Source[]|undefined;
}

const SET_ACCOUNT = 'SET_ACCOUNT';
const CLEAR_ACCOUNT = 'CLEAR_ACCOUNT';
const SET_SOURCES = 'SET_SOURCES';

interface SetAccountAction {
    type: typeof SET_ACCOUNT;
    userName: string;
    password: string;
}

interface ClearAccountAction {
    type: typeof CLEAR_ACCOUNT;
}

interface SetSourcesAction {
    type: typeof SET_SOURCES;
    sources: Source[];
}

type KnownAction = SetAccountAction | ClearAccountAction | SetSourcesAction;

export const actionCreators = {
    setAccount: (userName: string, password: string) => ({ type: SET_ACCOUNT, userName, password } as SetAccountAction),
    clearAccount: () => ({ type: CLEAR_ACCOUNT } as ClearAccountAction),
    setSources: (sources: Source[]) => ({ type: SET_SOURCES, sources } as SetSourcesAction)
};

const initialState: AccountState = { sources: undefined };

export const reducer: Reducer<AccountState> = (state: AccountState | undefined, incomingAction: Action): AccountState => {
    if (state === undefined) {
        return initialState;
    }

    const action = incomingAction as KnownAction;
    if (action) {
        switch (action.type) {
            case SET_ACCOUNT:
                return {
                    ...state,
                    userName: action.userName,
                    password: action.password,
                };
            case CLEAR_ACCOUNT:
                return {
                    ...state,
                    userName: undefined,
                    password: undefined,
                    sources: undefined,
                }
            case SET_SOURCES:
                return {
                    ...state,
                    sources: action.sources
                }
        }
    } 
    return state;
};
