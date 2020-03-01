import React from 'react';
import { useDispatch } from 'react-redux';
import { actionCreators as accountActions } from '../../store/Account';
import { Button } from '@material-ui/core';
import { useHistory } from 'react-router-dom'

export const Logout = () => {
    const dispatch = useDispatch();
    const history = useHistory();

    const logout = () => {
        dispatch(accountActions.clearAccount());
    }

    return (
        <Button
            variant="contained"
            color="primary"
            onClick={logout}
        >
            Logout
        </Button>
    );    
};  
