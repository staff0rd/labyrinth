import React, { useState } from 'react';
import { Button, Grid, makeStyles, LinearProgress } from "@material-ui/core"
import Alert from "@material-ui/lab/Alert"
import { Formik, Form, Field } from 'formik';
import { TextField } from 'formik-material-ui';
import { post, Result } from '../../api';
import { actionCreators as accountActions } from '../../store/Account';
import { useDispatch } from 'react-redux';


interface Values {
    userName: string;
    password: string;
    passwordConfirm: string;
}

export const Create = () => {
    const [result, setResult] = useState<Result>();
    const dispatch = useDispatch();

    const showResult = () => {
        if (result) {
            if (result.isError) {
                return <Alert severity="error">{result.message}</Alert>
            } else {
                return <Alert severity="success">User created</Alert>
            }
        }
    }

    return (
        <Formik
            initialValues={{
                userName: '',
                password: '',
                passwordConfirm: '',
            }}
            validate={values => {
                const errors: Partial<Values> = {};
                if (!values.userName) {
                    errors.userName = 'Required';
                }
                if (!values.password) {
                    errors.password = 'Required';
                }
                if (!values.passwordConfirm) {
                    errors.passwordConfirm = 'Required';
                } else if (values.password != values.passwordConfirm) {
                    errors.passwordConfirm = 'Passwords do not match';
                }
                return errors;
            }}
            onSubmit={async (values, { setSubmitting }) => {
                try {
                    setResult({ isError: false });
                    const response = await post('api/accounts', values);
                    setResult(response);
                    if (!response.isError) {
                        dispatch(accountActions.setAccount(values.userName, values.password));
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
                        <Field
                            component={TextField}
                            type="password"
                            label="Confirm Password"
                            name="passwordConfirm"
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
                            Create
                        </Button>
                    </Grid>
                </Grid>
            </Form>
            )}
        </Formik>
    );
    }
    

