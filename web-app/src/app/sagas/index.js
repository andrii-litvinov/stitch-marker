import { call, put, takeEvery, takeLatest, all } from 'redux-saga/effects'
import { MARK_BACKSTITCHES, UNMARK_STITCHES, MARK_STITCHES, UNMARK_BACKSTITCHES, INIT_STORE } from '../actions';
import { patternStore } from '../stores/patternStore.js';

export function* watchMarkBackstitch() {
  yield takeEvery(MARK_BACKSTITCHES, markBackstitch);
}

export function* watchUnmarkBackstitch() {
  yield takeEvery(UNMARK_BACKSTITCHES, unmarkBackstitch);
}

function* markBackstitch(actionData) {
  yield call((async () => {
    const data = JSON.stringify({
      patternId: patternStore.getState().pattern.id,
      backstitches: actionData.backstitches
    });
    try {
      const response = await http.post(`${SM.apiUrl}/api/patterns/markbackstitches`, data);
    }
    catch (e) { console.log(`Error in fetch: ${e}`); }
  }));
}

function* unmarkBackstitch(actionData) {
  yield call((async () => {
    const data = JSON.stringify({
      patternId: patternStore.getState().pattern.id,
      backstitches: actionData.backstitches
    });
    try {
      const response = await http.post(`${SM.apiUrl}/api/patterns/unmarkbackstitches`, data);
    }
    catch (e) { console.log(`Error in fetch: ${e}`); }
  }));
}

export function* watchUnmarkStitch() {
  yield takeEvery(UNMARK_STITCHES, unmarkStitches);
}

export function* watchMarkStitch() {
  yield takeEvery(MARK_STITCHES, markStitches);
}

function* markStitches(actionData) {
  yield call((async () => {
    const data = JSON.stringify({
      patternId: patternStore.getState().pattern.id,
      stitches: actionData.stitches
    });
    try {
      const response = await http.post(`${SM.apiUrl}/api/patterns/markstitches`, data);
    }
    catch (e) { console.log(`Error in fetch: ${e}`); }
  }));
}

function* unmarkStitches(actionData) {
  yield call((async () => {
    const data = JSON.stringify({
      patternId: patternStore.getState().pattern.id,
      stitches: actionData.stitches
    });
    try {
      const response = await http.post(`${SM.apiUrl}/api/patterns/unmarkstitches`, data);
    }
    catch (e) { console.log(`Error in fetch: ${e}`); }
  }));
}



export function* rootSaga() {
  yield all([
    watchMarkStitch(),
    watchUnmarkStitch(),
    watchUnmarkBackstitch(),
    watchMarkBackstitch()
  ])
}
