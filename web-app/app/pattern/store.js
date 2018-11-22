import { createStore, applyMiddleware } from 'redux';
import createSagaMiddleware from 'redux-saga';

import { rootSaga } from './sagas';
import rootReducer from './reducers';

const sagaMiddleware = createSagaMiddleware();

export const patternStore = createStore(rootReducer, applyMiddleware(sagaMiddleware));

sagaMiddleware.run(rootSaga);

