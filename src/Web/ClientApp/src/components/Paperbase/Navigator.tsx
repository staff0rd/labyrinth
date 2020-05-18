import React, { useEffect } from 'react';
import clsx from 'clsx';
import { createStyles, Theme, withStyles, WithStyles } from '@material-ui/core/styles';
import Divider from '@material-ui/core/Divider';
import Drawer, { DrawerProps } from '@material-ui/core/Drawer';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import HomeIcon from '@material-ui/icons/Home';
import LinkedInIcon from '@material-ui/icons/LinkedIn';
import AddIcon from '@material-ui/icons/Add';
import InfoIcon from '@material-ui/icons/Info';
import GitHubIcon from '@material-ui/icons/GitHub';
import PeopleIcon from '@material-ui/icons/People';
import { Omit } from '@material-ui/types';
import { Link as RouterLink } from 'react-router-dom';
import Constants from '../../Constants'
import Link from '@material-ui/core/Link';
import { useLocation, useHistory } from 'react-router';
import YammerIcon from '../Icons/YammerIcon';
import TeamsIcon from '../Icons/TeamsIcon';
import { useSelector } from '../../store/useSelector';
import { Source } from '../../store/Source';
import { AccountState } from '../../store/Account';

interface SourceLink {
  id: string;
  icon: JSX.Element;
  route: string;
}


const getLinks = (sources: Source[]) : SourceLink[] => {
  return [
    { id: 'LinkedIn', icon: <LinkedInIcon />, route: '/linkedin' },
    { id: 'Yammer', icon: <YammerIcon />, route: '/yammer' },
    { id: 'Teams', icon: <TeamsIcon />, route: '/teams' },
  ].filter(c => sources.find(s => s.network == c.id));
}

const getCategories = (sources: Source[]|undefined, loggedIn: boolean) => {
  const categories = [];

  if (loggedIn) {
    const sourceLinks = { id: 'Sources', children: [] as SourceLink[]};
    if (sources) {
      sourceLinks.children = getLinks(sources);
    }
    sourceLinks.children.push({ id: 'Add source', icon: <AddIcon />, route:'/add-source' });
    categories.push(sourceLinks);
  }

  categories.push({
    id: 'Help',
    children: [
      { id: 'About', icon: <InfoIcon />, route: '/about' },
      { id: 'Accounts', icon: <PeopleIcon />, route: '/accounts' },
      { id: 'Contribute', icon: <GitHubIcon />, route: 'https://github.com/staff0rd/labyrinth'  },
    ],
  });
  
  return categories;
};

const styles = (theme: Theme) =>
  createStyles({
    categoryHeader: {
      paddingTop: theme.spacing(2),
      paddingBottom: theme.spacing(2),
    },
    categoryHeaderPrimary: {
      color: theme.palette.common.white,
    },
    item: {
      paddingTop: 1,
      paddingBottom: 1,
      color: '#bcbcbc',
      '&:hover,&:focus': {
        backgroundColor: 'rgba(255, 255, 255, 0.08)',
      },
    },
    itemCategory: {
      backgroundColor: '#232f3e',
      boxShadow: '0 -1px 0 #404854 inset',
      paddingTop: theme.spacing(2),
      paddingBottom: theme.spacing(2),
    },
    firebase: {
      fontSize: 24,
      color: theme.palette.common.white,
    },
    itemActiveItem: {
      color: '#4fc3f7',
    },
    itemPrimary: {
      fontSize: 'inherit',
    },
    itemIcon: {
      minWidth: 'auto',
      marginRight: theme.spacing(2),
    },
    divider: {
      marginTop: theme.spacing(2),
    },
  });

  function ListItemLink(props: any) {
    const { icon, primary, to, className, classes, key } = props;
  
    const renderLink: any = React.useMemo(
      () => React.forwardRef((itemProps, ref:any) => <RouterLink to={to} ref={ref} {...itemProps} />),
      [to],
    );
  
    return (
      <li>
        <ListItem key={key} className={className} classes={classes} button component={renderLink}>
          {icon ? <ListItemIcon>{icon}</ListItemIcon> : null}
          <ListItemText primary={primary} />
        </ListItem>
      </li>
    );
  }
export interface NavigatorProps extends Omit<DrawerProps, 'classes'>, WithStyles<typeof styles> {}

function Navigator(props: NavigatorProps) {
  const { classes, ...other } = props;
  const location = useLocation();
  const sources = useSelector<Source[]|undefined>(state => state.account.sources);
  const { userName } = useSelector<AccountState>(state => state.account);
  const categories = getCategories(sources, !!userName);
  const history = useHistory();
  
  useEffect(() => {
    if (!userName) {
      history.push('/accounts/login')
    }
  }, [userName]);
  
  return (
    <Drawer variant="permanent" {...other}>
      <List disablePadding>
        <ListItem className={clsx(classes.firebase, classes.item, classes.itemCategory)}>
          { Constants.appName.toLowerCase() }
        </ListItem>
        {/* <ListItem className={clsx(classes.item, classes.itemCategory)}>
          <ListItemIcon className={classes.itemIcon}>
            <HomeIcon />
          </ListItemIcon>
          <ListItemText
            classes={{
              primary: classes.itemPrimary,
            }}
          >
            Overview
          </ListItemText>
        </ListItem> */}
        {categories.map(({ id, children }) => (
          <React.Fragment key={id}>
            <ListItem className={classes.categoryHeader}>
              <ListItemText
                classes={{
                  primary: classes.categoryHeaderPrimary,
                }}
              >
                {id}
              </ListItemText>
            </ListItem>
            {children!.map(({ id: childId, icon, route }) => { 
              const active = location.pathname === route;
              return (
              <ListItemLink
                key={childId}
                button
                className={clsx(classes.item, active && classes.itemActiveItem)}
                to={route}
                primary={childId}
                classes={{
                  primary: classes.itemPrimary,
                }}
                icon={icon}
              >
              </ListItemLink>
            );})}
            <Divider className={classes.divider} />
          </React.Fragment>
        ))}
      </List>
    </Drawer>
  );
}

export default withStyles(styles)(Navigator);
