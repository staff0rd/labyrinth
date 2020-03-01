import { Action, Reducer } from 'redux';

export interface AccountState {
    userName?: string;
    password?: string;
}

const SET_ACCOUNT = 'SET_ACCOUNT';
const CLEAR_ACCOUNT = 'CLEAR_ACCOUNT';

interface SetAccountAction {
    type: typeof SET_ACCOUNT;
    userName: string;
    password: string;
}

interface ClearAccountAction {
    type: typeof CLEAR_ACCOUNT;
}

type KnownAction = SetAccountAction | ClearAccountAction;

export const actionCreators = {
    setAccount: (userName: string, password: string) => ({ type: SET_ACCOUNT, userName, password } as SetAccountAction),
    clearAccount: () => ({ type: CLEAR_ACCOUNT } as ClearAccountAction)
};

const initialState: AccountState = { };

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
                }
        }
    } 
    return state;
};
