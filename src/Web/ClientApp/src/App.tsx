import * as React from 'react';
import { Route } from 'react-router';
import Home from './components/Paperbase/Content';
import { LinkedIn } from './components/LinkedIn';
import Accounts from './components/Accounts';
import Yammer from './components/Yammer';
import Paperbase from './components/Paperbase/Paperbase'
import { About } from './components/About'

import './custom.css'

export default () => (
    <Paperbase>
        <Route exact path='/' component={Home} />
        <Route path='/linkedin' component={LinkedIn as any} />
        <Route path='/yammer' component={Yammer} />
        <Route path='/about' component={About} />
        <Route path='/accounts' component={Accounts} />
    </Paperbase>
);
