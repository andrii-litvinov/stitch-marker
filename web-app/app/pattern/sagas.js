import { all } from 'redux-saga/effects'
import { watchGridRender } from '../pattern/grid-saga'
import { watchUnmarkStitch, watchMarkStitch, watchTapStitches } from '../pattern/stitch-saga';
import { watchMarkBackstitch, watchUnmarkBackstitch, watchBackstitchComplete, watchBackstitchRender, 
  watchBackstitchProgress, watchBackstitchAbort } from '../pattern/backstitch-saga';

export function* rootSaga() {
  yield all([
    watchTapStitches(),
    watchUnmarkBackstitch(),
    watchMarkBackstitch(),
    watchMarkStitch(),
    watchUnmarkStitch(),
    watchBackstitchRender(),
    watchGridRender(),
    watchBackstitchProgress(),
    watchBackstitchComplete(),
    watchBackstitchAbort()
  ])
}
