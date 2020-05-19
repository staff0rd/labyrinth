import { MiddlewareAPI, Dispatch } from '@reduxjs/toolkit'
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr'
import { createReducer } from '@reduxjs/toolkit';
import { subscribeToBackgroundTask, backgroundTaskProgressUpdate, backgroundTaskLogUpdate } from './BackgroundTasks';

export const SignalRMiddleware = () => {
    
    const connection: HubConnection = new HubConnectionBuilder()
        .withUrl("/taskHub")
        .withAutomaticReconnect()
        .build();
    connection.start();
    return ({ getState, dispatch }: MiddlewareAPI) => {
        connection.on('TaskProgress', (payload) => {
            dispatch(backgroundTaskProgressUpdate(payload));
        });
        connection.on('TaskLog', (payload) => {
            dispatch(backgroundTaskLogUpdate(payload));
        });
        const invoke = (methodName: string, ...params: any[]) => {
            connection.invoke(methodName, ...params)
                .catch((err) => console.log(err));
        };
        return (next: Dispatch) => (action: any) => {
            switch (action.type) {
                case subscribeToBackgroundTask.type: {
                    invoke("Subscribe", action.payload);
                }
            }
            return next(action);
        };
    };
}