import * as LinkedIn from './LinkedIn';
import * as Yammer from './Yammer';
import * as Header from './Header';
import * as Account from './Account';

// The top-level state object
export interface ApplicationState {
    linkedIn: LinkedIn.LinkedInState;
    yammer: Yammer.YammerState;
    header: Header.HeaderState;
    account: Account.AccountState;
}

// This type can be used as a hint on action creators so that its 'dispatch' and 'getState' params are
// correctly typed to match your store.
export interface AppThunkAction<TAction> {
    (dispatch: (action: TAction) => void, getState: () => ApplicationState): void;
}
