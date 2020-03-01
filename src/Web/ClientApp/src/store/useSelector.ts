import {
    useSelector as useReduxSelector,
    TypedUseSelectorHook,
  } from 'react-redux'
  import { ApplicationState } from './'
  
  export const useSelector: TypedUseSelectorHook<ApplicationState> = useReduxSelector