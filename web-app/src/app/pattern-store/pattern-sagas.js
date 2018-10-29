import { call, takeEvery, all } from 'redux-saga/effects'
import { MARK_BACKSTITCHES, UNMARK_STITCHES, MARK_STITCHES, UNMARK_BACKSTITCHES } from '../pattern-store/pattern-actions';
import { patternStore } from '../pattern-store/pattern-store';

export function* watchMarkBackstitch() {
  yield takeEvery(MARK_BACKSTITCHES, markBackstitch);
}

export function* watchUnmarkBackstitch() {
  yield takeEvery(UNMARK_BACKSTITCHES, unmarkBackstitch);
}

function* markBackstitch(actionData) {
  yield call(async () => {
    try {
      await http.post(SM.apiUrl + JSON.parse(localStorage.getItem('patternInfo'))
        .links.find(link => link.rel === 'markBackstitches').href, getBackstitchRequestData(actionData));
    }
    catch (e) { console.log(`Error in fetch: ${e}`); }
  });
}

function* unmarkBackstitch(actionData) {
  yield call(async () => {
    try {
      await http.post(SM.apiUrl + JSON.parse(localStorage.getItem('patternInfo'))
        .links.find(link => link.rel === 'unmarkBackstitches').href, getBackstitchRequestData(actionData));
    }
    catch (e) { console.log(`Error in fetch: ${e}`); }
  });
}

function getBackstitchRequestData(actionData) {
  return JSON.stringify({
    patternId: patternStore.getState().pattern.id,
    backstitches: getBackstitchCoordinates(actionData.backstitches)
  });
}

function getBackstitchCoordinates(indexes) {
  return indexes.map(index => {
    let backstitch = patternStore.getState().pattern.backstitches[index];
    return { x1: backstitch.x1, y1: backstitch.y1, x2: backstitch.x2, y2: backstitch.y2 }
  });
}

export function* watchUnmarkStitch() {
  yield takeEvery(UNMARK_STITCHES, unmarkStitches);
}

export function* watchMarkStitch() {
  yield takeEvery(MARK_STITCHES, markStitches);
}

function* markStitches(actionData) {
  yield call(async () => {
    try {
      await http.post(SM.apiUrl + JSON.parse(localStorage.getItem('patternInfo'))
        .links.find(link => link.rel === 'markStitches').href, getStitchRequestData(actionData));
    }
    catch (e) { console.log(`Error in fetch: ${e}`); }
  });
}

function* unmarkStitches(actionData) {
  yield call(async () => {
    try {
      await http.post(SM.apiUrl + JSON.parse(localStorage.getItem('patternInfo'))
        .links.find(link => link.rel === 'unmarkStitches').href, getStitchRequestData(actionData));
    }
    catch (e) { console.log(`Error in fetch: ${e}`); }
  });
}

function getStitchRequestData(actionData) {
  return JSON.stringify({
    patternId: patternStore.getState().pattern.id,
    stitches: getStitchCoordinates(actionData.stitches)
  });
}

function getStitchCoordinates(indexes) {
  return indexes.map(index => {
    let stitch = patternStore.getState().pattern.stitches[index];
    return { x: stitch.x, y: stitch.y }
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
