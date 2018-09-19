import { call, put, takeEvery, takeLatest, all } from 'redux-saga/effects'
import { INCREMENT, DECREMENT, DECREMENT_ASYNC } from '../actions';
import { delay } from 'redux-saga';

function* saga() {
  yield takeEvery(DECREMENT, mark);
}

function mark() {
  console.log('saga');
}

function* decrementAsync() {
  yield delay(1000);
  yield put({ type: DECREMENT });
}


function* watchIncrementAsync() {
  yield takeEvery('DECREMENT_ASYNC', decrementAsync);
}

// function* fetchDogAsync() {
//   try {
//     yield put(requestDog());
//     const data = yield call(() => {
//       return fetch('https://dog.ceo/api/breeds/image/random')
//               .then(res => res.json())
//       }
//     );
//     yield put(requestDogSuccess(data));
//   } catch (error) {
//     yield put(requestDogError());
//   }
// }

function* rootSaga() {
  yield all([
    saga(),
    watchIncrementAsync()
  ])
}

export default rootSaga;
