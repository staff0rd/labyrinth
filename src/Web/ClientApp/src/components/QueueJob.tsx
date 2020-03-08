import React, { useState } from 'react';
import { Button, Grid, LinearProgress } from "@material-ui/core";
import Alert from "@material-ui/lab/Alert";
import { Formik, Form, FormikValues } from 'formik';
import { Command, Result } from '../api';


type Props<T extends FormikValues> = {
    initialValues: any;
    fields: any;
    apiCall: (values: T) => Promise<Command>;
    validate: (values: T) => Partial<T>;
}

export const QueueJob = <T extends FormikValues>(props: Props<T>) => {
    const {
        initialValues,
        fields,
        apiCall,
        validate,
    } = props;
    const [link, setLink] = useState<Command>();
    const [result, setResult] = useState<Result>();
    const showResult = () => {
        if (result) {
            if (result.isError) {
                return <Alert severity="error">{result.message}</Alert>;
            }
        }
    };
    
    return (
    <Formik
        initialValues={initialValues}
        validate={validate}
        onSubmit={async (values, { setSubmitting }) => {
            try {
                setResult(undefined);
                const response = await apiCall(values);
                setLink(response);
            }
            catch (err) {
                setResult(err);
                console.log('error', err);
            }
            finally {
                setSubmitting(false);
            }
        }}
    >
        {({ submitForm, isSubmitting }) => (
            <Form autoComplete="off">
                <Grid container spacing={2}>
                    {fields()}
                    <Grid item xs={12}>
                        {isSubmitting && <LinearProgress />}
                        {!isSubmitting && showResult()}
                        {!isSubmitting && link && (<Alert severity="success">Job <a target='_blank' href={`/hangfire/jobs/details/${link.id}`}>#{link.id}</a> queued</Alert>)}
                    </Grid>
                    <Grid item xs={12}>
                        <Button variant="contained" color="primary" disabled={isSubmitting} onClick={submitForm}>
                            Queue
                                </Button>
                    </Grid>
                </Grid>
            </Form>
        )}
    </Formik>);
};
