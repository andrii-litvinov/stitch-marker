import { MARK_BACKSTITCHES, UNMARK_BACKSTITCHES, INIT_STORE } from './actions';
import Backstitch from '../stitch-canvas/backstitch.js';

const backstitches = (state = {}, action) => {
    switch (action.type) {
        case INIT_STORE:
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
            return { ...state, backstitches: backstitches, backstitchesMap: backstitchesMap };

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

export default backstitches;
