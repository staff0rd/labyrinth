import * as LinkedIn from './LinkedIn';
import * as Yammer from './Yammer';
import * as Header from './Header';
import * as Account from './Account';
import * as Tasks from './BackgroundTasks';
import { combineReducers } from 'redux';
import { connectRouter } from 'connected-react-router';
import { History } from 'history';

const reducers = {
    linkedIn: LinkedIn.reducer,
    yammer: Yammer.reducer,
    header: Header.reducer,
    account: Account.reducer,
    tasks: Tasks.reducer,
};

export default (history: History) => combineReducers({
    ...reducers,
    router: connectRouter(history)
});
