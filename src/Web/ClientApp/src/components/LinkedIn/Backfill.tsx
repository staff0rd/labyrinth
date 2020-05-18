import React from 'react';
import { Grid } from "@material-ui/core"
import { Field } from 'formik';
import { TextField } from 'formik-material-ui';
import { queue } from '../../api';
import { AccountState } from '../../store/Account';
import { useSelector } from '../../store/useSelector';
import { QueueJob } from '../QueueJob';


export interface Values {
    userName: string;
    password: string;
}

interface Props {
    sourceId: string;
}

export const Backfill = ({ sourceId }: Props) => {
    const { password, userName } = useSelector<AccountState>(state => state.account);

    const apiCall = async (values: Values) => {
        var response = await queue('api/linkedin/backfill', {
            password,
            userName,
            externalIdentifier: values.userName,
            externalSecret: values.password,
            sourceId,
        });
        return response;
    }

    const validate = (values: Values) => {
        const errors: Partial<Values> = {};
        if (!values.userName) {
            errors.userName = 'Required';
        }
        if (!values.password) {
            errors.password = 'Required';
        }
        return errors;
    };

    const formFields = () => (
        <>
        <Grid item xs={12}>
            <Field
                component={TextField}
                name="userName"
                type="text"
                label="LinkedIn Username"
            />
        </Grid>
        <Grid item xs={12}>
            <Field
                component={TextField}
                name="password"
                type="password"
                label="LinkedIn Password"
            />
        </Grid>
        </>
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
