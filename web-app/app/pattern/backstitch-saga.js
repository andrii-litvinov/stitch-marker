import { call, takeEvery } from 'redux-saga/effects'
import { backstitchCreateMarker, MARK_BACKSTITCHES, UNMARK_BACKSTITCHES, RENDER_BACKSTITCH, BACKSTITCH_PROCESS, BACKSTITCH_COMPLETE, BACKSTITCH_ABORT } from '../pattern/actions';
import { patternStore } from '../pattern/store';

export function* watchMarkBackstitch() {
    yield takeEvery(MARK_BACKSTITCHES, markBackstitch);
}

export function* watchUnmarkBackstitch() {
    yield takeEvery(UNMARK_BACKSTITCHES, unmarkBackstitch);
}

export function* watchBackstitchComplete() {
    yield takeEvery(BACKSTITCH_COMPLETE, BackstitchComplete);
}

export function* watchBackstitchRender() {
    yield takeEvery(RENDER_BACKSTITCH, BackstitchRender);
}

export function* watchBackstitchProgress() {
    yield takeEvery(BACKSTITCH_PROCESS, BackstitchProgress);
}

export function* watchBackstitchAbort() {
    yield takeEvery(BACKSTITCH_ABORT, BackstitchAbort);
}

function* markBackstitch(indexes) {
    yield call(async () => {
        try {
            await http.put(SM.apiUrl + JSON.parse(localStorage.getItem('patternInfo'))
                .links.find(link => link.rel === 'mark-backstitches').href, getBackstitchRequestData(indexes));
        }
        catch (e) { console.log(`Error in fetch: ${e}`); }
    });
}

function* unmarkBackstitch(indexes) {
    yield call(async () => {
        try {
            await http.put(SM.apiUrl + JSON.parse(localStorage.getItem('patternInfo'))
                .links.find(link => link.rel === 'unmark-backstitches').href, getBackstitchRequestData(indexes));
        }
        catch (e) { console.log(`Error in fetch: ${e}`); }
    });
}

function getBackstitchRequestData(indexes) {
    return JSON.stringify({
        id: patternStore.getState().pattern.id,
        backstitches: getBackstitchCoordinates(indexes)
    });
}

function getBackstitchCoordinates(indexes) {
    return indexes.map(index => {
        let backstitch = patternStore.getState().backstitches.items[index];
        return { x1: backstitch.x1, y1: backstitch.y1, x2: backstitch.x2, y2: backstitch.y2 }
    });
}

function BackstitchRender(action) {
    const context = action.ctx;
    const scene = action.scene;
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

function BackstitchProgress(action) {
    const activeBackstitch = patternStore.getState().backstitches.activeBackstitch;
    if (activeBackstitch != action.e.detail.backstitch) {
        patternStore.getState().backstitches.activeBackstitch = action.e.detail.backstitch;
    }
    BackstitchRender();
}

function* BackstitchAbort(action) {
    patternStore.getState().backstitches.activeBackstitch = null;
    BackstitchRender();
    disposeMarkers(patternStore.getState().backstitches);
}

function* BackstitchComplete(action) {
    let backstitches = patternStore.getState().backstitches;
    let index = backstitches.items.indexOf(backstitches.activeBackstitch);
    const backstitch = backstitches.items[index];

    backstitch.marked = !backstitch.marked;
    patternStore.getState().backstitches.items[index] = backstitch;
    backstitch.marked
        ? yield markBackstitch([index])
        : yield unmarkBackstitch([index]);

    disposeMarkers(backstitches);
    backstitches.activeBackstitch = null;
    BackstitchRender();

    let point = backstitches.maps[action.e.detail.x * backstitches.scene.pattern.height + action.e.detail.y];
    if (point) {
        patternStore.dispatch(backstitchCreateMarker(point, action.e.detail.x, action.e.detail.y));
    };
}

function disposeMarkers(backstitches) {
    backstitches.markers.forEach(marker => {
        marker.dispose();
    });
    backstitches.markers.length = 0;
}