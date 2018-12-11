import { MARK_BACKSTITCHES, UNMARK_BACKSTITCHES, abortBackstitch, BACKSTITCH_CREATEMARKER, INIT_BACKSTITCHES, completeBackstitch, BACKSTITCH_TOUCHSTART, processBackstitch, renderBackstitch } from './actions';
import Backstitch from '../stitch-canvas/backstitch.js';
import BackstitchMarker from '../stitch-canvas/backstitch-marker';
import { patternStore } from '../pattern/store.js';

const backstitches = (state = {}, action) => {
    switch (action.type) {
        case INIT_BACKSTITCHES:
            let backstitches = [];
            let backstitchesMap = [];

            const height = action.pattern.height;
            action.pattern.backstitches.forEach(bs => {
                const config = action.pattern.configurations[bs.configurationIndex];
                const strands = config.strands || action.pattern.strands;
                const backstitch = new Backstitch(config, strands, bs, action.scale, bs.marked);
                [
                    { x: backstitch.x1, y: backstitch.y1 },
                    { x: backstitch.x2, y: backstitch.y2 }
                ].forEach(point => {
                    const index = point.x * height + point.y;
                    backstitchesMap[index] = backstitchesMap[index] || [];
                    backstitchesMap[index].push(backstitch);
                    backstitches.push(backstitch);
                });
            });
            return { ...state, items: backstitches, maps: backstitchesMap, activeBackstitch: {}, markers: [], ctx: action.ctx, scene: action.scene };

        case BACKSTITCH_TOUCHSTART:
            const x = Math.floor((action.e.detail.x - action.scene.x) / action.scene.stitchSize * 2);
            const y = Math.floor((action.e.detail.y - action.scene.y) / action.scene.stitchSize * 2);

            // check 4 points, near user tap, for available backstitches
            for (let i = 0; i <= 1; i++)
                for (let j = 0; j <= 1; j++) {
                    let xCoord = x + i;
                    let yCoord = y + j;

                    let point = state.maps[xCoord * action.scene.pattern.height + yCoord];
                    if (point) {
                        let distToPoint = Math.sqrt(Math.pow((xCoord * action.scene.stitchSize / 2) - (action.e.detail.x - action.scene.x), 2) + Math.pow((yCoord * action.scene.stitchSize / 2) - (action.e.detail.y - action.scene.y), 2));
                        if (distToPoint < action.scene.stitchSize / 2 - 1) {
                            createBackstitchMarkers(point, xCoord, yCoord, state);
                        }
                    };
                };
            return { ...state };

        case BACKSTITCH_CREATEMARKER:
            createBackstitchMarkers(action.point, action.touchX, action.touchY, state);
            return { ...state };

        default:
            return state;
    }
};

function createBackstitchMarkers(point, touchX, touchY, state) {
    const markerEventListeners = {
        progress: progress.bind(this),
        complete: backstitchComplete.bind(this),
        abort: abort.bind(this)
    };

    point.forEach(backstitch => {
        state.markers.push(new BackstitchMarker(state.ctx, state.scene, backstitch, touchX, touchY));
    });
    for (const type in markerEventListeners) {
        state.markers.forEach(marker => {
            marker.addEventListener(type, markerEventListeners[type]);
        });
    }
}

function backstitchComplete(e) {
    patternStore.dispatch(completeBackstitch(e));
}

function progress(e) {
    patternStore.dispatch(processBackstitch(e));
}

function abort(e) {
    patternStore.dispatch(abortBackstitch(e));
}

export default backstitches;
