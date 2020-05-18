import React from 'react';
import { Grid } from "@material-ui/core"
import { Field } from 'formik';
import { TextField } from 'formik-material-ui';
import { queue } from '../../api';
import { AccountState } from '../../store/Account';
import { useSelector } from '../../store/useSelector';
import { QueueJob } from '../QueueJob';


export interface Values {
    token: string;
}

interface Props {
    sourceId: string;
}

export const Backfill = ({ sourceId }: Props) => {
    const { password, userName } = useSelector<AccountState>(state => state.account);

    const apiCall = async (values: Values) => {
        var response = await queue('api/yammer/backfill', {
            password,
            userName,
            token: values.token,
            sourceId,
        });
        return response;
    }

    const validate = (values: Values) => {
        const errors: Partial<Values> = {};
        if (!values.token) {
            errors.token = 'Required';
        }
        return errors;
    };

    const formFields = () => (
        <Grid item xs={12}>
            <Field
                component={TextField}
                name="token"
                type="text"
                label="token"
            />
        </Grid>
    );

    return (
        <QueueJob 
            fields={formFields}
            initialValues={{
                token: '',
            }}
            apiCall={apiCall}
            validate={validate}
        />
    );

}
