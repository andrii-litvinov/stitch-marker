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
      action.backstitches.forEach(actionBs => {
        state.backstitches.forEach((bs, index) => {
          if (bs.X1 == actionBs.X1 &&
            bs.Y1 == actionBs.Y1 &&
            bs.X2 == actionBs.X2 &&
            bs.Y2 == actionBs.Y2)
            bs.marked = true;
        })
      });
      return {
        ...state
      };

    case UNMARK_BACKSTITCHES:
      action.backstitches.forEach(actionBs => {
        state.backstitches.forEach((bs, index) => {
          if (bs.X1 == actionBs.X1 &&
            bs.Y1 == actionBs.Y1 &&
            bs.X2 == actionBs.X2 &&
            bs.Y2 == actionBs.Y2)
            bs.marked = true;
        })
      });
      return {
        ...state
      };

    default:
      return state;
  }
};

const updateStitch = (state, action) => {
  switch (action.type) {
    case MARK_STITCHES:
      action.stitches.forEach(actionS => {
        state.stitches.forEach((s, index) => {
          if (s.X == actionS.X &&
            s.Y == actionS.Y)
            s.marked = true;
        })
      });
      return {
        ...state
      };

    case UNMARK_STITCHES:
      action.stitches.forEach(actionS => {
        state.stitches.forEach((s, index) => {
          if (s.X == actionS.X &&
            s.Y == actionS.Y)
            s.marked = false;
        })
      });
      return {
        ...state
      };

    default:
      return state;
  }
};


export default reducer;
