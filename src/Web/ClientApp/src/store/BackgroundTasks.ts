import { createAction, createReducer } from "@reduxjs/toolkit"

function withPayloadType<T>() {
    return (t: T) => ({ payload: t })
  }
interface BackgroundTaskProgressUpdate {
  id: string;
  progress: number;
}
interface BackgroundTaskLogUpdate {
  id: string;
  log: string;
  severity: number;
}

export interface BackgroundTask {
  id: string;
  progress: number;
  logs: string[]
  complete?: boolean;
};

export const subscribeToBackgroundTask = createAction('subscribe-to-background-task', withPayloadType<string>());
export const backgroundTaskProgressUpdate = createAction('background-task-progress', withPayloadType<BackgroundTaskProgressUpdate>());
export const backgroundTaskLogUpdate = createAction('background-task-log', withPayloadType<BackgroundTaskLogUpdate>());

export const reducer = createReducer<BackgroundTask[]>([], builder =>
  builder
    .addCase(backgroundTaskProgressUpdate, (state, { payload: { id, progress } }) => {
      const task = state.find(t => t.id === id);
      if (task) return state.map(t => (t.id === id ? { ...task, progress } : t))
      return [...state, { id, progress, logs: []}];
    })
    .addCase(backgroundTaskLogUpdate, (state, { payload: { id, log, severity } }) => {
      const task = state.find(t => t.id === id);
      if (task) return state.map(t => (t.id === id ? { ...task, logs: [...t.logs, log] } : t));
      return [...state, { id, progress: 0, logs: [log]}];
    })
)