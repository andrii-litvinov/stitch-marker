import { MARK_BACKSTITCHES, UNMARK_BACKSTITCHES, INIT_BACKSTITCHES, RENDER_BACKSTITCH } from './actions';
import Backstitch from '../stitch-canvas/backstitch.js';

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
            return { ...state, items: backstitches, maps: backstitchesMap, activeBackstitch: {} };

        case RENDER_BACKSTITCH:
            render(action.context, action.scene, state);
            return { ...state };

        case UNMARK_BACKSTITCHES:
            action.backstitches.forEach(actionBackstitch => {
                state.backstitches[actionBackstitch].marked = false;
            });
            return { ...state };

        case MARK_BACKSTITCHES:
            action.backstitches.forEach(actionBackstitch => {
                state.backstitches[actionBackstitch].marked = true;
            });
            return { ...state };

        default:
            return state;
    }
};

function backstitchComplete(e) {
    let index = this.backstitches.indexOf(this.activeBackstitch);
    const backstitch = this.backstitches[index];

    patternStore.dispatch(backstitch.marked
        ? unmarkBackstitches([index])
        : markBackstitches([index]));

    this.disposeMarkers();
    this.activeBackstitch = null;
    render();

    let point = this.backstitchesMap[e.detail.x * this.scene.pattern.height + e.detail.y];
    if (point) {
        createBackstitchMarkers(point, e.detail.x, e.detail.y);
    };
}

function createBackstitchMarkers(point, touchX, touchY) {
    point.forEach(backstitch => {
        this.markers.push(new BackstitchMarker(this.ctx, this.scene, backstitch, touchX, touchY));
    });
    for (const type in this.markerEventListeners) {
        this.markers.forEach(marker => {
            marker.addEventListener(type, this.markerEventListeners[type]);
        });
    }
}

function render(context, scene, state) {
    context.clearRect(0, 0, context.canvas.width, context.canvas.height);
    context.translate(scene.x + 0.5, scene.y + 0.5);
    state.items.forEach(backstitch => {
        if (state.activeBackstitch != backstitch) {
            backstitch.draw(context, scene.stitchSize, scene.scale);
        }
    });
    context.setTransform(1, 0, 0, 1, 0, 0);
}

export default backstitches;
