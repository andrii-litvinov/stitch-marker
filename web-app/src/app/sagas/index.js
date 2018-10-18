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
      await http.post(SM.apiUrl + JSON.parse(localStorage.getItem('patternInfo'))
        .links.find(link => link.rel === 'markBackstitches').href, data);
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

      await http.post(SM.apiUrl + JSON.parse(localStorage.getItem('patternInfo'))
        .links.find(link => link.rel === 'unmarkBackstitches').href, data);
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
      await http.post(SM.apiUrl + JSON.parse(localStorage.getItem('patternInfo'))
        .links.find(link => link.rel === 'markStitches').href, data);
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
      await http.post(SM.apiUrl + JSON.parse(localStorage.getItem('patternInfo'))
        .links.find(link => link.rel === 'unmarkStitches').href, data);
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
