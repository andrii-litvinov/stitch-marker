import { createStore, applyMiddleware } from 'redux';
import createSagaMiddleware from 'redux-saga';

import {rootSaga} from './pattern-sagas';
import reducer from './pattern-reducers';

const sagaMiddleware = createSagaMiddleware();

export const patternStore = createStore(reducer, applyMiddleware(sagaMiddleware));

sagaMiddleware.run(rootSaga);

