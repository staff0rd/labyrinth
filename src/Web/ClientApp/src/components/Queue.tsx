import React from 'react';
import { queue } from '../api';
import { AccountState } from '../store/Account';
import { useSelector } from '../store/useSelector';
import { QueueJob } from './QueueJob';

type Props = {
    url: string
    sourceId: string
}

export interface Values {}

export const Queue = ({url, sourceId}: Props) => {
    const { password, userName } = useSelector<AccountState>(state => state.account);

    const apiCall = async (values: Values) => {
        var response = await queue(url, {
            password,
            userName,
            sourceId
        });
        return response;
    }

    const validate = (values: Values) => {
        const errors: Partial<Values> = {};
        return errors;
    };

    const fields = () => (
        <></>
    );

    return (
        <QueueJob 
            fields={fields}
            initialValues={{}}
            apiCall={apiCall}
            validate={validate}
        />
    );

}
