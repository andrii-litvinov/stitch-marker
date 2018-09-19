import { call, put, takeEvery, takeLatest } from 'redux-saga/effects'
import { INCREMENT, DECREMENT } from '../actions';

function* saga() {
  yield takeEvery(INCREMENT, mark);
}

function mark() {
  console.log('saga');
}

export default saga;
