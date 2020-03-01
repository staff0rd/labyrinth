import React from 'react';
import { useSelector } from '../../store/useSelector';
import * as AccountStore from '../../store/Account';

export const Overview = () => {
    const { userName } = useSelector<AccountStore.AccountState>(state => state.account);

    return (
        <h5>{userName}</h5>
    );
}
