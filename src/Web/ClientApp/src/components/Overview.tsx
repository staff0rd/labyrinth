import React from 'react';
import { Paper, TableContainer, Table, TableHead, TableRow, TableCell, TableBody, makeStyles } from '@material-ui/core';
import { EventCounts } from './EventCounts';

export type OverviewProps = {
    groups: number;
    messages: number;
    threads: number;
    users: number;
    events: EventCounts[];
  };

const useStyles = makeStyles(theme => ({
    container: {
        display: 'inline-block',
        width: 'auto',
    },
    table: {
        width: "auto",
        tableLayout: "auto",
    },
}));

export const Overview = (props: OverviewProps) => {
    const {
        groups,
        messages,
        threads,
        users,
        events,
    } = props;
    const classes = useStyles();
    return (
        <TableContainer className={classes.container} component={Paper}>
            <Table className={classes.table}>
                <TableHead>
                    <TableRow>
                        <TableCell>Type</TableCell>
                        <TableCell align="right">#</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    <TableRow><TableCell>Groups</TableCell><TableCell align="right">{groups}</TableCell></TableRow>
                    <TableRow><TableCell>Messages</TableCell><TableCell align="right">{messages}</TableCell></TableRow>
                    <TableRow><TableCell>Threads</TableCell><TableCell align="right">{threads}</TableCell></TableRow>
                    <TableRow><TableCell>Users</TableCell><TableCell align="right">{users}</TableCell></TableRow>
                </TableBody>
                <TableHead>
                    <TableRow>
                        <TableCell colSpan={2}>Events</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {events.map(row => (
                        <TableRow key={row.eventName}>
                            <TableCell component="th" scope="row">
                                {row.eventName}
                            </TableCell>
                            <TableCell align="right">{row.count}</TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
}