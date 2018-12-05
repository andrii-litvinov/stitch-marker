import { call, takeEvery, all, put } from 'redux-saga/effects'
import { MARK_BACKSTITCHES, UNMARK_STITCHES, MARK_STITCHES, UNMARK_BACKSTITCHES, TAP_STITCHES, RENDER_BACKSTITCH, BACKSTITCH_PROCESS, BACKSTITCH_COMPLETE } from '../pattern/actions';
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
        .links.find(link => link.rel === 'mark-stitches').href, JSON.stringify({
          id: patternStore.getState().pattern.id,
          stitches: action.toMark
        }));
    }
    catch (e) { console.log(`Error in fetch: ${e}`); }
  });
}

function* unmarkStitches(action) {
  yield call(async () => {
    try {
      await http.put(SM.apiUrl + JSON.parse(localStorage.getItem('patternInfo'))
        .links.find(link => link.rel === 'unmark-stitches').href, JSON.stringify({
          id: patternStore.getState().pattern.id,
          stitches: action.toUnmark
        }));
    }
    catch (e) { console.log(`Error in fetch: ${e}`); }
  });
}

function* watchTapStitches() {
  yield takeEvery(TAP_STITCHES, tapStitches);
}

function* tapStitches(action) {
  let toMark = [];
  let toUnmark = [];

  action.stitches.forEach(index => {
    const stitch = patternStore.getState().pattern.stitches[index];
    stitch.marked ? toMark.push({ x: stitch.x, y: stitch.y }) : toUnmark.push({ x: stitch.x, y: stitch.y });
    stitch.tap();
  });

  if (toUnmark.length > 0) yield put({ type: "UNMARK_STITCHES", toUnmark });
  if (toMark.length > 0) yield put({ type: "MARK_STITCHES", toMark });
}

function* watchBackstitchRender() {
  yield takeEvery(RENDER_BACKSTITCH, BackstitchRender);
}

function BackstitchRender() {
  const context = patternStore.getState().backstitches.ctx;
  const scene = patternStore.getState().backstitches.scene;
  const items = patternStore.getState().backstitches.items;
  const activeBackstitch = patternStore.getState().backstitches.activeBackstitch;

  context.clearRect(0, 0, context.canvas.width, context.canvas.height);
  context.translate(scene.x + 0.5, scene.y + 0.5);
  items.forEach(backstitch => {
    if (activeBackstitch != backstitch) {
      backstitch.draw(context, scene.stitchSize, scene.scale);
    }
  });
  context.setTransform(1, 0, 0, 1, 0, 0);
}

function* watchBackstitchProgress() {
  yield takeEvery(BACKSTITCH_PROCESS, BackstitchProgress);
}

function BackstitchProgress(action) {
  const activeBackstitch = patternStore.getState().backstitches.activeBackstitch;
  if (activeBackstitch != action.e.detail.backstitch) {
    patternStore.getState().backstitches.activeBackstitch = action.e.detail.backstitch;
  }
  BackstitchRender();
}

function* watchBackstitchComplete() {
  yield takeEvery(BACKSTITCH_COMPLETE, BackstitchComplete);
}

function BackstitchComplete(action) {
  let backstitches = patternStore.getState().backstitches;
  let index = backstitches.items.indexOf(backstitches.activeBackstitch);
  const backstitch = backstitches.items[index];

  backstitch.marked = !backstitch.marked;
  // patternStore.dispatch(backstitch.marked
  //     ? unmarkBackstitches([index])
  //     : markBackstitches([index]));

  disposeMarkers(backstitches);
  backstitches.activeBackstitch = null;
  BackstitchRender();

  let point = backstitches.maps[action.e.detail.x * backstitches.scene.pattern.height + action.e.detail.y];
  if (point) {
    // createBackstitchMarkers(point, e.detail.x, e.detail.y);
  };
}

function disposeMarkers(backstitches) {
  backstitches.markers.forEach(marker => {
    marker.dispose();
  });
  backstitches.markers.length = 0;
}

export function* rootSaga() {
  yield all([
    watchTapStitches(),
    watchUnmarkBackstitch(),
    watchMarkBackstitch(),
    watchMarkStitch(),
    watchUnmarkStitch(),
    watchBackstitchRender(),
    watchBackstitchProgress(),
    watchBackstitchComplete()
  ])
}
