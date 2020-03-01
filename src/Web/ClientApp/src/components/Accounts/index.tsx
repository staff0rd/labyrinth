import React, { useState, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { ApplicationState } from '../../store';
import * as YammerStore from '../../store/Yammer';
import { Users } from '../Users'
import { Messages } from '../Messages'
import { useLocation, Route } from 'react-router-dom'
import { bindActionCreators } from 'redux';
import { useHeader } from '../../store/useHeader';
import { Create } from './Create';

const Accounts = () => {
  useHeader({
    title: 'Accounts',
    route: '/accounts',
    items: [
      { title: 'Overview', to: ''},
      { title: 'Create', to: '/create'},
      { title: 'Change Password', to: '/change-password'},
    ],
  });

  return (
    <Route path='/accounts/create' component={Create} />
  );
};

export default Accounts;
