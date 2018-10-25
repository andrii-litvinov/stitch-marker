import { INIT_STORE, MARK_STITCHES, UNMARK_STITCHES, MARK_BACKSTITCHES, UNMARK_BACKSTITCHES } from './pattern-actions';
import Backstitch from '../stitch-canvas/backstitch.js';
import Stitch from '../stitch-canvas/stitch.js';

const reducer = (state = {}, action) => {
  switch (action.type) {
    case INIT_STORE:
      let backstitches = [];
      let stitches = [];
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

      action.pattern.stitches.forEach(s => {
        const stitch = new Stitch(action.pattern.configurations[s.configurationIndex], s);
        stitches[stitch.x * action.pattern.height + stitch.y] = stitch;
      });

      action.pattern.backstitches = backstitches;
      action.pattern.backstitchesMap = backstitchesMap;
      action.pattern.stitches = stitches;
      return { ...state, pattern: action.pattern };

    case UNMARK_BACKSTITCHES:
      action.backstitches.forEach(actionBackstitch => {
        state.pattern.backstitches[actionBackstitch].marked = false;
      });
      return { ...state };

    case MARK_BACKSTITCHES:
      action.backstitches.forEach(actionBackstitch => {
        state.pattern.backstitches[actionBackstitch].marked = true;
      });
      return { ...state };

    case UNMARK_STITCHES:
      action.stitches.forEach(actionStitch => {
        state.pattern.stitches[actionStitch].marked = false;
      });
      return { ...state };

    case MARK_STITCHES:
      action.stitches.forEach(actionStitch => {
        state.pattern.stitches[actionStitch].marked = true;
      });
      return { ...state };

    default:
      return state;
  }
};

export default reducer;
