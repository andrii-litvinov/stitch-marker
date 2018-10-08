import { createStore, applyMiddleware } from 'redux';
import createSagaMiddleware from 'redux-saga';

import {rootSaga} from '../sagas';
import reducer from '../reducers';

const sagaMiddleware = createSagaMiddleware();

export const patternStore = createStore(reducer, applyMiddleware(sagaMiddleware));

sagaMiddleware.run(rootSaga);

