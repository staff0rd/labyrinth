import * as React from 'react';
import { Route } from 'react-router';
import Home from './components/Paperbase/Content';
import Counter from './components/Counter';
import FetchData from './components/FetchData';
import LinkedIn from './components/LinkedIn';
import Paperbase from './components/Paperbase/Paperbase'

import './custom.css'
import { Paper } from '@material-ui/core';

export default () => (
    <Paperbase>
        <Route exact path='/' component={Home} />
        <Route path='/linkedin' component={LinkedIn} />
    </Paperbase>
    // <Layout>
    //     <Route path='/counter' component={Counter} />
    //     <Route path='/fetch-data/:startDateIndex?' component={FetchData} />
    //     <Route path='/linkedin' component={LinkedIn} />

    // </Layout>
);
