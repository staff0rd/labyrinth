import { useEffect } from 'react';
import { actionCreators, HeaderState } from './Header';
import { useDispatch } from 'react-redux';

export const useHeader = (header: HeaderState, dependencies: any[] = []) => {
  const dispatch = useDispatch();
  useEffect(() => {
    dispatch(actionCreators.setHeader(header));
  }, [...dependencies]);
}