import { Action, Reducer } from 'redux';

export interface AccountState {
    userName?: string;
    password?: string;
}

const SET_ACCOUNT = 'SET_ACCOUNT';

interface SetAccountAction {
    type: typeof SET_ACCOUNT;
    userName: string;
    password: string;
}

type KnownAction = SetAccountAction;

export const actionCreators = {
    setAccount: (userName: string, password: string) => ({ type: SET_ACCOUNT, userName, password } as SetAccountAction)
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
        }
    } 
    return state;
};
