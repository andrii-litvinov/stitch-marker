import {
  createStore,
  compose as origCompose,
  applyMiddleware,
  combineReducers
} from 'redux';
import createSagaMiddleware from 'redux-saga'
import rootSaga from './sagas/index'

import app from './reducers/app.js';

const sagaMiddleware = createSagaMiddleware({ rootSaga })

export const store = createStore(
  app,
  applyMiddleware(sagaMiddleware)
);

