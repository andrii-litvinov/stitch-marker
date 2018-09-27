export const INIT_STORE = 'INIT_STORE';
export const FETCH_INIT_STATE = 'FETCH_INIT_STATE';
export const UPDATE_BACKSTITCHES = 'UPDATE_BACKSTITCHES';
export const UPDATE_STITCH_TILES = 'UPDATE_STITCH_TILES';

export const initStore = (backstitches, stitchTiles) => {
  return {
    type: INIT_STORE,
    backstitches,
    stitchTiles
  };
};

export const fetchInitState = (patternId) => {
  return {
    type: FETCH_INIT_STATE,
    patternId
  };
};

export const updateBackstitches = (backstitches) => {
  return {
    type: UPDATE_BACKSTITCHES,
    backstitches
  };
};

export const updateStitchTiles = (stitchTiles) => {
  return {
    type: UPDATE_STITCH_TILES,
    stitchTiles
  };
};

