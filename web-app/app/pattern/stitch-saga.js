import { call, takeEvery,  put } from 'redux-saga/effects'
import { UNMARK_STITCHES, MARK_STITCHES, TAP_STITCHES } from '../pattern/actions';
import { patternStore } from '../pattern/store';

export function* watchUnmarkStitch() {
    yield takeEvery(UNMARK_STITCHES, unmarkStitches);
}

export function* watchMarkStitch() {
    yield takeEvery(MARK_STITCHES, markStitches);
}

export function* watchTapStitches() {
    yield takeEvery(TAP_STITCHES, tapStitches);
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