import { takeEvery } from 'redux-saga/effects'

export default function* rootSaga() {
  yield takeEvery('INCREMENT_ASYNC', incrementAsync)
}
