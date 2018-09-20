import { UPDATE_BACKSTITCHES, UPDATE_STITCH_TILES, INIT_STORE } from '../actions';

const reducer = (state = { backstitches: {}, stitchTiles: {} }, action) => {
  switch (action.type) {
    case INIT_STORE:
      return {
        backstitches: action.backstitches,
        stitchTiles: action.stitchTiles
      };
    case UPDATE_BACKSTITCHES:
      return {
        ...state,
        backstitches: action.backstitches
      };
    case UPDATE_STITCH_TILES:
      return {
        ...state,
        stitchTiles: action.stitchTiles
      };

    default:
      return state;
  }
};

export default reducer;
