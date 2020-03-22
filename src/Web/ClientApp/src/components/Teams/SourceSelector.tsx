import React from 'react';
import { actionCreators } from '../../store/Account';
import { useSource } from '../useSource';
import { FormControl, InputLabel, Select } from '@material-ui/core';
import { useDispatch } from 'react-redux';

type SourceSelectorProps = {
    network: string;
}

export const SourceSelector = ({ network }: SourceSelectorProps) => {
  const dispatch = useDispatch();
  const handleChange = (a: any) => {
    dispatch(actionCreators.setSource(network, a.target.value));
  };
  const { sourceId, sources } = useSource(network);
  return (<FormControl>
    <InputLabel htmlFor="source-selector">Source</InputLabel>
    <Select native value={sourceId} onChange={handleChange} inputProps={{
      name: 'source-selector',
      id: 'source-selector',
    }}>
      {sources.map(s => (<option value={s.id}>{s.name}</option>))}
    </Select>
  </FormControl>);
};
