import { call, takeEvery, all } from 'redux-saga/effects'
import { MARK_BACKSTITCHES, UNMARK_STITCHES, MARK_STITCHES, UNMARK_BACKSTITCHES } from '../pattern/actions';
import { patternStore } from '../pattern/store';

function* watchMarkBackstitch() {
  yield takeEvery(MARK_BACKSTITCHES, markBackstitch);
}

function* watchUnmarkBackstitch() {
  yield takeEvery(UNMARK_BACKSTITCHES, unmarkBackstitch);
}

function* markBackstitch(action) {
  yield call(async () => {
    try {
      await http.put(SM.apiUrl + JSON.parse(localStorage.getItem('patternInfo'))
        .links.find(link => link.rel === 'mark-backstitches').href, getBackstitchRequestData(action));
    }
    catch (e) { console.log(`Error in fetch: ${e}`); }
  });
}

function* unmarkBackstitch(action) {
  yield call(async () => {
    try {
      await http.put(SM.apiUrl + JSON.parse(localStorage.getItem('patternInfo'))
        .links.find(link => link.rel === 'unmark-backstitches').href, getBackstitchRequestData(action));
    }
    catch (e) { console.log(`Error in fetch: ${e}`); }
  });
}

function getBackstitchRequestData(action) {
  return JSON.stringify({
    id: patternStore.getState().pattern.id,
    backstitches: getBackstitchCoordinates(action.backstitches)
  });
}

function getBackstitchCoordinates(indexes) {
  return indexes.map(index => {
    let backstitch = patternStore.getState().pattern.backstitches[index];
    return { x1: backstitch.x1, y1: backstitch.y1, x2: backstitch.x2, y2: backstitch.y2 }
  });
}

function* watchUnmarkStitch() {
  yield takeEvery(UNMARK_STITCHES, unmarkStitches);
}

function* watchMarkStitch() {
  yield takeEvery(MARK_STITCHES, markStitches);
}

function* markStitches(action) {
  yield call(async () => {
    try {
      await http.put(SM.apiUrl + JSON.parse(localStorage.getItem('patternInfo'))
        .links.find(link => link.rel === 'mark-stitches').href, getStitchRequestData(action));
    }
    catch (e) { console.log(`Error in fetch: ${e}`); }
  });
}

function* unmarkStitches(action) {
  yield call(async () => {
    try {
      await http.put(SM.apiUrl + JSON.parse(localStorage.getItem('patternInfo'))
        .links.find(link => link.rel === 'unmark-stitches').href, getStitchRequestData(action));
    }
    catch (e) { console.log(`Error in fetch: ${e}`); }
  });
}

function getStitchRequestData(action) {
  return JSON.stringify({
    id: patternStore.getState().pattern.id,
    stitches: getStitchCoordinates(action.stitches)
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
