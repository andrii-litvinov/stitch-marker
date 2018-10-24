import { call, put, takeEvery, takeLatest, all } from 'redux-saga/effects'
import { MARK_BACKSTITCHES, UNMARK_STITCHES, MARK_STITCHES, UNMARK_BACKSTITCHES, INIT_STORE } from '../pattern-store/pattern-actions';
import { patternStore } from '../pattern-store/pattern-store';

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
      backstitches: getBackstitchCoordinates(actionData.backstitches)
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
      backstitches: getBackstitchCoordinates(actionData.backstitches)
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
      stitches: getStitchCoordinates(actionData.stitches)
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
      stitches: getStitchCoordinates(actionData.stitches)
    });
    try {
      await http.post(SM.apiUrl + JSON.parse(localStorage.getItem('patternInfo'))
        .links.find(link => link.rel === 'unmarkStitches').href, data);
    }
    catch (e) { console.log(`Error in fetch: ${e}`); }
  }));
}

function getStitchCoordinates(indexes) {
  return indexes.map(index => {
    let stitch = patternStore.getState().pattern.stitches[index];
    return { x: stitch.x, y: stitch.y }
  });
}

function getBackstitchCoordinates(indexes) {
  return indexes.map(index => {
    let backstitch = patternStore.getState().pattern.backstitches[index];
    return { x1: backstitch.x1, y1: backstitch.y1, x2: backstitch.x2, y2: backstitch.y2 }
  });
}


export function* rootSaga() {
  yield all([
    watchMarkStitch(),
    watchUnmarkStitch(),
    watchUnmarkBackstitch(),
    watchMarkBackstitch()
  ])
}
