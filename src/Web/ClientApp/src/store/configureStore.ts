import { applyMiddleware, combineReducers, compose, createStore } from 'redux';
import thunk from 'redux-thunk';
import { routerMiddleware } from 'connected-react-router';
import { History } from 'history';
import { ApplicationState } from './';
import rootReducer from "./reducers";
import { LoggerMiddleware } from './LoggerMiddleware';

export default function configureStore(history: History, initialState?: ApplicationState) {
    const middleware = [
        LoggerMiddleware,
        thunk,
        routerMiddleware(history)
    ];

    const enhancers = [];
    const windowIfDefined = typeof window === 'undefined' ? null : window as any;
    if (windowIfDefined && windowIfDefined.__REDUX_DEVTOOLS_EXTENSION__) {
        enhancers.push(windowIfDefined.__REDUX_DEVTOOLS_EXTENSION__());
    }

    const store = createStore(
        rootReducer(history),
        initialState,
        compose(applyMiddleware(...middleware), ...enhancers)
    );

    if (process.env.NODE_ENV !== 'production' && module.hot) {
        module.hot.accept('./reducers', () => store.replaceReducer(rootReducer(history)))
    }

    return store;
}
