import React, { useState } from 'react';
import { Button, Grid, makeStyles, LinearProgress } from "@material-ui/core"
import Alert from "@material-ui/lab/Alert"
import { Formik, Form, Field } from 'formik';
import { TextField } from 'formik-material-ui';
import { post, Result } from '../../api';
import { actionCreators as accountActions, AccountState } from '../../store/Account';
import { useDispatch } from 'react-redux';
import { useSelector } from '../../store/useSelector';


interface Values {
    token: string;
}

export const Backfill = () => {
    const [result, setResult] = useState<Result>();
    const dispatch = useDispatch();
    const { password, userName } = useSelector<AccountState>(state => state.account);

    const showResult = () => {
        if (result) {
            if (result.isError) {
                return <Alert severity="error">Login failed</Alert>
            } else {
                return <Alert severity="success">Login successful</Alert>
            }
        }
    }

    return (
        <Formik
            initialValues={{
                token: '',
            }}
            validate={values => {
                const errors: Partial<Values> = {};
                if (!values.token) {
                    errors.token = 'Required';
                }
                return errors;
            }}
            onSubmit={async (values, { setSubmitting }) => {
                try {
                    setResult(undefined);
                    const response = await post('api/yammer/backfill', {
                        password,
                        userName,
                        token: values.token,
                    });
                    setResult(response);
                } catch (err) {
                    setResult(err);
                    console.log('error', err);
                } finally {
                    setSubmitting(false);
                }
            }}
            >
            {({ submitForm, isSubmitting }) => (
                <Form autoComplete="off">
                    <Grid container spacing={2}>
                    <Grid item xs={12}>
                        <Field
                            component={TextField}
                            name="token"
                            type="text"
                            label="token"
                        />
                    </Grid>
                    <Grid item xs={12}>
                        {isSubmitting && <LinearProgress />}
                        { !isSubmitting && showResult() }
                    </Grid>
                    <Grid item xs={12}>
                        <Button
                            variant="contained"
                            color="primary"
                            disabled={isSubmitting}
                            onClick={submitForm}
                        >
                            Queue
                        </Button>
                    </Grid>
                </Grid>
            </Form>
            )}
        </Formik>
    );
}