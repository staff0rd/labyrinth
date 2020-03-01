import React from 'react';
import { Route } from 'react-router-dom'
import { useHeader } from '../../store/useHeader';
import { Create } from './Create';
import { Login } from './Login';
import { Overview } from './Overview';
import { Logout } from './Logout';
import { ChangePassword } from './ChangePassword';
import { useLocation} from "react-router";
import * as AccountStore from '../../store/Account';
import * as HeaderStore from '../../store/Header';
import { useSelector } from '../../store/useSelector';

const Accounts = () => {
  const location = useLocation();
  const { userName } = useSelector<AccountStore.AccountState>(state => state.account);

  const includeIfLoggedIn = (item: HeaderStore.HeaderItem) => {
    return userName ? [item] : [];
  }

  const includeIfLoggedOut = (item: HeaderStore.HeaderItem) => {
    return userName ? [] : [item];
  }

  useHeader({
    title: 'Accounts',
    route: '/accounts',
    items: [
      ...includeIfLoggedIn({ title: 'Overview', to: ''}),
      ...includeIfLoggedOut({ title: 'Login', to: '/login'}),
      ...includeIfLoggedIn({ title: 'Logout', to: '/logout'}),
      ...includeIfLoggedOut({ title: 'Create', to: '/create'}),
      ...includeIfLoggedIn({ title: 'Change Password', to: '/change-password'}),
    ],
  }, [userName]);

  return (
    <>
      <Route path='/accounts/create' component={Create} />
      <Route path='/accounts/login' component={Login} />
      <Route path='/accounts/change-password' component={ChangePassword} />
      <Route path='/accounts/logout' component={Logout} />
      { location.pathname === '/accounts' && <Overview /> }
    </>
  );
};

export default Accounts;
