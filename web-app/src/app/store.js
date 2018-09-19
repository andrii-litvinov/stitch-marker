import { createStore, applyMiddleware } from 'redux';
import createSagaMiddleware from 'redux-saga';

import saga from './sagas';
import reducer from './reducers';

const sagaMiddleware = createSagaMiddleware(); //createSagaMiddleware({mySaga})

export const store = createStore(reducer, applyMiddleware(sagaMiddleware));

sagaMiddleware.run(saga);

