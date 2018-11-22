import { INIT_STORE, MARK_BACKSTITCHES, UNMARK_BACKSTITCHES} from './actions';
import { combineReducers } from 'redux'
import Backstitch from '../stitch-canvas/backstitch.js';
import stitches from '../pattern/stitch-reducer'

const reducer = (state = {}, action) => {
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

      action.pattern.backstitches = backstitches;
      action.pattern.backstitchesMap = backstitchesMap;
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

    default:
      return state;
  }
};

const rootReducer = combineReducers({
  reducer,
  stitches
})

export default rootReducer;
