import * as WeatherForecasts from './WeatherForecasts';
import * as Counter from './Counter';
import * as LinkedIn from './LinkedIn';
import * as Yammer from './Yammer';
import * as Header from './Header';
import * as Account from './Account';

// The top-level state object
export interface ApplicationState {
    counter: Counter.CounterState | undefined;
    weatherForecasts: WeatherForecasts.WeatherForecastsState | undefined;
    linkedIn: LinkedIn.LinkedInState | undefined;
    yammer: Yammer.YammerState | undefined;
    header: Header.HeaderState;
    account: Account.AccountState;
}

// Whenever an action is dispatched, Redux will update each top-level application state property using
// the reducer with the matching name. It's important that the names match exactly, and that the reducer
// acts on the corresponding ApplicationState property type.
export const reducers = {
    counter: Counter.reducer,
    weatherForecasts: WeatherForecasts.reducer,
    linkedIn: LinkedIn.reducer,
    yammer: Yammer.reducer,
    header: Header.reducer,
    account: Account.reducer,
};

// This type can be used as a hint on action creators so that its 'dispatch' and 'getState' params are
// correctly typed to match your store.
export interface AppThunkAction<TAction> {
    (dispatch: (action: TAction) => void, getState: () => ApplicationState): void;
}
