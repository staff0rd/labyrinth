import React, { useState, useEffect } from 'react';
import { useHeader } from '../store/useHeader';
import { useDispatch } from 'react-redux';
import Alert from '@material-ui/lab/Alert';
import { useHistory } from 'react-router';
import { Formik, Form, Field } from 'formik';
import { Result, post } from '../api';
import { v4 as uuid } from 'uuid';
import { Source } from '../store/Source';
import { useSelector } from '../store/useSelector';
import { actionCreators as accountActions, AccountState } from '../store/Account';
import { Grid, LinearProgress, Button, FormControl, InputLabel, makeStyles } from '@material-ui/core';
import { Select, TextField } from 'formik-material-ui';

interface Values {
    name: string;
    network: string;
}

const useStyles = makeStyles(theme => ({
    formControl: {
      minWidth: 120,
    },
}));

export const AddSource = () => {

    useHeader({
        route: '/add-source',
        title: 'Add source',
        items: [],
    });

    const classes = useStyles();

    const { password, userName } = useSelector<AccountState>(state => state.account);
    const [result, setResult] = useState<Result>();
    const dispatch = useDispatch();
    const history = useHistory();
    const sources = useSelector<Source[]|undefined>(state => state.account.sources);
    const [networks, setNetworks] = useState<string[]>([]);

    useEffect(() => {
        fetch('api/sources/networks')
            .then((response) => {
                return response.json();
            })
            .then((data) => {
                setNetworks(data);
            });
    }, []);

    const showResult = () => {
        if (result) {
            if (result.isError) {
                return <Alert severity="error">{result.message}</Alert>
            }
        }
     }

    return (
        <Formik
            initialValues={{
                name: '',
                network: '',
            }}
            validate={values => {
                const errors: Partial<Values> = {};
                if (!values.name) {
                    errors.name = 'Required';
                }
                if (!values.network) {
                    errors.network = 'Required';
                }
                return errors;
            }}
            onSubmit={async (values, { setSubmitting }) => {
                try {
                    setResult(undefined);
                    const source = { ...values, id: uuid(), userName, password };
                    const response = await post('api/sources/add', source);
                    if (!response.isError) {
                        dispatch(accountActions.addSource(source));
                        history.push(`/${values.network.toLowerCase()}`);
                    }
                } catch (err) {
                    setResult(err);
                    console.log('error', err);
                } finally {
                    setSubmitting(false);
                }
            }}
            >
            {({ submitForm, isSubmitting, errors, values }) => (
                <Form autoComplete="off">
                    <Grid container spacing={2}>
                    <Grid item xs={12}>
                    <FormControl className={classes.formControl} error={!!errors.network}>
                        <InputLabel htmlFor="network">Network</InputLabel>
                        <Field
                                native
                                component={Select}
                                name="network"
                                label="Network"
                                onChange={() => {
                                    console.log('why does this not get called? :('); // TODO
                                    if (values.network) {
                                        values.name = values.network;
                                    }
                                }}
                                inputProps={{
                                    name: 'network',
                                    id: 'network',
                                  }}
                            >
                                <option aria-label="None" value="" />
                                { networks.map(n => (
                                    <option value={n}>{n}</option>
                                ))}
                        </Field>
                        </FormControl>
                    </Grid>
                    <Grid item xs={12}>
                        <Field
                            component={TextField}
                            name="name"
                            type="text"
                            label="Name"
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
                            Add
                        </Button>
                    </Grid>
                </Grid>
            </Form>
            )}
        </Formik>
    );
}