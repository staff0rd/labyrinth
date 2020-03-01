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
    oldPassword: string;
    password: string;
    passwordConfirm: string;
}

export const ChangePassword = () => {
    const [result, setResult] = useState<Result>();
    const dispatch = useDispatch();
    const { userName } = useSelector<AccountState>(state => state.account);

    const showResult = () => {
        if (result) {
            if (result.isError) {
                return <Alert severity="error">{result.message}</Alert>
            } else {
                return <Alert severity="success">Password changed</Alert>
            }
        }
    }

    return (
        <Formik
            initialValues={{
                oldPassword: '',
                password: '',
                passwordConfirm: '',
            }}
            validate={values => {
                const errors: Partial<Values> = {};
                if (!values.oldPassword) {
                    errors.oldPassword = 'Required';
                }
                if (!values.password) {
                    errors.password = 'Required';
                }
                if (!values.passwordConfirm) {
                    errors.passwordConfirm = 'Required';
                } else if (values.password != values.passwordConfirm) {
                    errors.passwordConfirm = 'New passwords do not match';
                }
                return errors;
            }}
            onSubmit={async (values, { setSubmitting }) => {
                try {
                    setResult(undefined);
                    const response = await post('api/accounts/change-password', {
                        userName: userName!,
                        oldPassword: values.oldPassword,
                        newPassword: values.password,
                    });
                    setResult(response);
                    if (!response.isError) {
                        dispatch(accountActions.setAccount(userName!, values.password));
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
                            name="oldPassword"
                            type="password"
                            label="Current password"
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
                            Change
                        </Button>
                    </Grid>
                </Grid>
            </Form>
            )}
        </Formik>
    );
    }
    

