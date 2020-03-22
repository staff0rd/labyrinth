import React, { useState } from 'react';
import { Button, Grid, makeStyles, LinearProgress } from "@material-ui/core"
import Alert from "@material-ui/lab/Alert"
import { Formik, Form, Field } from 'formik';
import { TextField } from 'formik-material-ui';
import { post, Result, postResponse } from '../../api';
import { actionCreators as accountActions } from '../../store/Account';
import { useDispatch } from 'react-redux';
import { Source } from '../../store/Source';


interface Values {
    userName: string;
    password: string;
}

export const Login = () => {
    const [result, setResult] = useState<Result>();
    const dispatch = useDispatch();

    const showResult = () => {
        if (result) {
            if (result.isError) {
                return <Alert severity="error">Login failed</Alert>
            } else {
                return <Alert severity="success">Login successful</Alert>
            }
        }
    }

    const getSources = (userName: string, password: string) => {
        postResponse<Source[]>(`/api/sources`, {userName, password})
            .then(data => {
                if (data && !data.isError) {
                    dispatch(accountActions.setSources(data.response));
                }
            });
    }

    return (
        <Formik
            initialValues={{
                userName: '',
                password: '',
            }}
            validate={values => {
                const errors: Partial<Values> = {};
                if (!values.userName) {
                    errors.userName = 'Required';
                }
                if (!values.password) {
                    errors.password = 'Required';
                }
                return errors;
            }}
            onSubmit={async (values, { setSubmitting }) => {
                try {
                    setResult(undefined);
                    const response = await post('api/accounts/login', values);
                    setResult(response);
                    if (!response.isError) {
                        dispatch(accountActions.setAccount(values.userName, values.password));
                        getSources(values.userName, values.password);
                    }
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
                            name="userName"
                            type="text"
                            label="Username"
                        />
                    </Grid>
                    <Grid item xs={12}>
                        <Field
                            component={TextField}
                            name="password"
                            type="password"
                            label="Password"
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
                            id='login-button'
                        >
                            Login
                        </Button>
                    </Grid>
                </Grid>
            </Form>
            )}
        </Formik>
    );
}
