import {
  reducer,
  backgroundTaskLogUpdate,
  backgroundTaskProgressUpdate,
 } from './BackgroundTasks';

describe('BackgroundTasks', () => {
  describe('backgroundTaskProgressUpdate reducer', () => {
    it ('should add new task', () => {
      const state = reducer([], backgroundTaskProgressUpdate({ id: "1", progress: 1}));
      expect(state).toHaveLength(1);
      expect(state[0].id).toBe("1");
      expect(state[0].progress).toBe(1);
    });
    it ('should update old task', () => {
      const state = reducer([{id: "1", progress: 0, logs: []}], backgroundTaskProgressUpdate({id: "1", progress: 2}));
      expect(state).toHaveLength(1);
      expect(state[0].id).toBe("1");
      expect(state[0].progress).toBe(2);
    });
  })
  describe('backgroundTaskLogUpdate reducer', () => {
    it ('should add new task', () => {
      const state = reducer([], backgroundTaskLogUpdate({ id: "1", log: "a test", severity: 1}));
      expect(state).toHaveLength(1);
      expect(state[0].id).toBe("1");
      expect(state[0].logs).toHaveLength(1);
    });
    it ('should update old task', () => {
      const state = reducer([{id: "1", progress: 0, logs: []}], backgroundTaskLogUpdate({ id: "1", log: "a test", severity: 1}));
      expect(state).toHaveLength(1);
      expect(state[0].id).toBe("1");
      expect(state[0].logs).toHaveLength(1);
    });
  })
});