import { UPDATE_BACKSTITCHES, UPDATE_STITCH_TILES, INIT_STORE, FETCH_INIT_STATE } from '../actions';

const reducer = (state = { backstitches: {}, stitchTiles: {} }, action) => {
  switch (action.type) {
    case INIT_STORE:
      return {
        ...state,
        stitchTiles: action.stitchTiles,
        backstitches: action.backstitches
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
    case FETCH_INIT_STATE:
      return {
        ...state
      };

    default:
      return state;
  }
};

export default reducer;
