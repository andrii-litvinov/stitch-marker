import { call, put, takeEvery, takeLatest, all } from 'redux-saga/effects'
import { MARK_BACKSTITCHES, UNMARK_STITCHES, MARK_STITCHES, UNMARK_BACKSTITCHES, INIT_STORE } from '../actions';
import { patternStore } from '../stores/patternStore.js';

export function* watchUpdateBackstichesAsync() {
  yield takeLatest(MARK_BACKSTITCHES, PostDataAsync);
}

export function* watchUpdateStitchTilesAsync() {
  yield takeLatest(UNMARK_STITCHES, PostDataAsync);
}

export function* watchInitStoreAsync() {
  yield takeEvery(MARK_STITCHES, InitStoreAsync);
}

export function* PostDataAsync() {
  // yield call((async () => {
  //   const data = JSON.stringify({
  //     patternId: store.getState().patternId,
  //     backstitches: store.getState().backstitches,
  //     stitchTiles: store.getState().stitchTiles
  //   });
  //   const response = await http.post(`${SM.apiUrl}/api/patterns/store`, data);
  // }));
}

export function* InitStoreAsync() {
  // yield call((async () => {
  //   const response = await http.get(`${SM.apiUrl}/api/patterns/store/${store.getState().patternId}`);
  //   const content = await response.json();
  //   put({ type: 'INIT_STORE', content });
  // }));
}

export function* rootSaga() {
  yield all([
    watchUpdateStitchTilesAsync(),
    watchUpdateBackstichesAsync(),
    watchInitStoreAsync()
  ])
}
