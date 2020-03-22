import { Action, Reducer } from 'redux';
import { Source } from './Source';

export interface AccountState {
    userName?: string;
    password?: string;
    sources: Source[]|undefined;
    source: Dictionary<string>;
}

interface Dictionary<T> {
    [key: string]: T;
  }

const SET_ACCOUNT = 'SET_ACCOUNT';
const CLEAR_ACCOUNT = 'CLEAR_ACCOUNT';
const SET_SOURCES = 'SET_SOURCES';
const SET_SOURCE = 'SET_SOURCE';
const ADD_SOURCE = 'ADD_SOURCE';

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

interface SetSourceAction {
    type: typeof SET_SOURCE;
    network: string;
    id: string;
}

interface AddSourceAction {
    type: typeof ADD_SOURCE;
    source: Source;
}

type KnownAction = SetAccountAction | ClearAccountAction | SetSourcesAction | SetSourceAction | AddSourceAction;

export const actionCreators = {
    setAccount: (userName: string, password: string) => ({ type: SET_ACCOUNT, userName, password } as SetAccountAction),
    clearAccount: () => ({ type: CLEAR_ACCOUNT } as ClearAccountAction),
    setSources: (sources: Source[]) => ({ type: SET_SOURCES, sources } as SetSourcesAction),
    setSource: (network: string, id: string) => ({ type: SET_SOURCE, network, id} as SetSourceAction),
    addSource: (source: Source) => ({ type: ADD_SOURCE, source} as AddSourceAction)
};

const initialState: AccountState = { sources: undefined, source: {} };

const setSelectedSource = (selectedSource: Dictionary<string>, network: string, id: string) => {
    selectedSource[network] = id;
    return selectedSource;
}

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
                };
            case SET_SOURCES:
                const selectedSource: Dictionary<string> = {}
                for (var source of action.sources) {
                    if (!selectedSource[source.network]) {
                        selectedSource[source.network] = source.id;
                    }
                }
                return {
                    ...state,
                    sources: action.sources,
                    source: selectedSource,
                };
            case SET_SOURCE: 
                return {
                    ...state,
                    source: setSelectedSource(state.source, action.network, action.id),
                };
            case ADD_SOURCE:
                return {
                    ...state,
                    sources: [...(state.sources || []), action.source],
                    source: setSelectedSource(state.source, action.source.network, action.source.id)
                }
        }
    } 
    return state;
};
