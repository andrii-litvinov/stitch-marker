import { call, put, takeEvery, takeLatest, all } from 'redux-saga/effects'
import { UPDATE_BACKSTITCHES, UPDATE_STITCH_TILES, FETCH_INIT_STATE, INIT_STORE } from '../actions';
import { delay } from 'redux-saga';
import { store } from '../stitch-store';

export function* watchUpdateBackstichesAsync() {
  yield takeLatest(UPDATE_BACKSTITCHES, PostDataAsync);
}

export function* watchUpdateStitchTilesAsync() {
  yield takeLatest(UPDATE_STITCH_TILES, PostDataAsync);
}

export function* watchInitStoreAsync() {
  yield takeEvery(FETCH_INIT_STATE, InitStoreAsync);
}

export function* PostDataAsync() {
  yield call((async () => {
    const data = JSON.stringify({
      patternId: store.getState().patternId,
      backstitches: store.getState().backstitches,
      stitchTiles: store.getState().stitchTiles
    });
    const response = await http.post(`${SM.apiUrl}/api/patterns/store`, data);
  }));
}

export function* InitStoreAsync() {
  yield call((async () => {
    const response = await http.get(`${SM.apiUrl}/api/patterns/store/${store.getState().patternId}`);
    const content = await response.json();
    put({ type: 'INIT_STORE', content });
  }));
}

export function* rootSaga() {
  yield all([
    watchUpdateStitchTilesAsync(),
    watchUpdateBackstichesAsync(),
    watchInitStoreAsync()
  ])
}
