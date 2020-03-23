import React from 'react';
import { queue, postResponse } from '../api';
import { AccountState } from '../store/Account';
import { useSelector } from '../store/useSelector';
import { QueueJob } from './QueueJob';
import { Grid } from '@material-ui/core';
import { TextField } from 'formik-material-ui';
import { Field } from 'formik';

type Props = {
    url: string
    sourceId: string
    token?: boolean;
}

export interface Values {
    token: string;
}

export const Queue = ({url, sourceId, token: usesToken}: Props) => {
    const { password, userName } = useSelector<AccountState>(state => state.account);

    const apiCall = async (values: Values) => {
        var response = await queue(url, {
            password,
            userName,
            sourceId,
            token: values.token
        });
        return response;
    }

    const validate = (values: Values) => {
        const errors: Partial<Values> = {};
        if (usesToken && !values.token) {
            errors.token = 'Required';
        }
        return errors;
    };

    const fields = () => usesToken ? (
        <Grid item xs={12}>
            <Field
                component={TextField}
                name="token"
                type="text"
                label="token"
            />
        </Grid>
    ) : (<></>);

    return (
        <QueueJob 
            fields={fields}
            initialValues={{}}
            apiCall={apiCall}
            validate={validate}
        />
    );

}
