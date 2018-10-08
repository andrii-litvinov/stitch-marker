import { INIT_STORE, MARK_STITCHES, UNMARK_STITCHES, MARK_BACKSTITCHES, UNMARK_BACKSTITCHES } from '../actions';

const reducer = (state = { pattern: {} }, action) => {
  switch (action.type) {
    case INIT_STORE:
      return {
        ...state,
        pattern: action.pattern,
      };
    case UNMARK_BACKSTITCHES:
    case MARK_BACKSTITCHES:
      return {
        ...state,
        pattern: updateBackstitch(state.pattern, action)
      };

    case UNMARK_STITCHES:
    case MARK_STITCHES:
      return {
        ...state,
        pattern: updateStitch(state.pattern, action)
      };

    default:
      return state;
  }
};

const updateBackstitch = (state, action) => {
  switch (action.type) {
    case MARK_BACKSTITCHES:
      
      return {
        ...state
      };

    case UNMARK_BACKSTITCHES:
     
      return {
        ...state
      };

    default:
      return state;
  }
};

const updateStitch = (state, action) => {
  const stitch = state.stitches[action.x * state.height + action.y];
  switch (action.type) {
    case MARK_STITCHES:
      state.stitches.forEach((element, index) => {
        if (element === stitch) {
          element.marked = true;
        }
      });
      return {
        ...state
      };

    case UNMARK_STITCHES:
      state.stitches.forEach((element, index) => {
        if (element === stitch) {
          element.marked = false;
        }
      });
      return {
        ...state
      };

    default:
      return state;
  }
};


export default reducer;
