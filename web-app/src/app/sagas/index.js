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
    const rawResponse = await fetch(`${SM.apiUrl}/api/patterns/store`, {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'Authorization': "Bearer " + JSON.parse(localStorage.getItem("authData")).accessToken
      },
      body: JSON.stringify({
        backstitches: store.getState().backstitches,
        stitchTiles: store.getState().stitchTiles
      })
    });
  }));
}

export function* InitStoreAsync() {
  yield call((async () => {
    const rawResponse = await fetch(`${SM.apiUrl}/api/patterns/store`, {
      method: 'GET',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'Authorization': "Bearer " + JSON.parse(localStorage.getItem("authData")).accessToken
      }
    });

    const content = await rawResponse.json();
    // yield put({ type: 'INIT_STORE', content });
    console.log(content);
    // dispatch({ type: 'INIT_STORE', content });
  }));
}

export function* rootSaga() {
  yield all([
    watchUpdateStitchTilesAsync(),
    watchUpdateBackstichesAsync(),
    watchInitStoreAsync()
  ])
}
