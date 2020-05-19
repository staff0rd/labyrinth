import React, { useState } from 'react';
import { Button, Grid, LinearProgress } from "@material-ui/core";
import Alert from "@material-ui/lab/Alert";
import { Formik, Form, FormikValues } from 'formik';
import { Command, Result } from '../api';
import { subscribeToBackgroundTask, BackgroundTask } from '../store/BackgroundTasks';
import { useDispatch } from 'react-redux';
import { useSelector } from '../store/useSelector';
import { ApplicationState } from '../store';


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
    const dispatch = useDispatch();
    const [link, setLink] = useState<Command>();
    const [result, setResult] = useState<Result>();
    const taskProgress = useSelector<BackgroundTask|undefined>(state => state.tasks.find(t => t.id === (link || {id: "0"}).id));
    console.log(taskProgress);
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
                dispatch(subscribeToBackgroundTask(response.id));
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
                        {!isSubmitting && taskProgress && (
                            <div>
                                Progress: {taskProgress.progress}
                            </div>
                        )}
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
