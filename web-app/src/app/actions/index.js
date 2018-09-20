export const INIT_STORE = 'INIT_STORE';
export const UPDATE_BACKSTITCHES = 'UPDATE_BACKSTITCHES';
export const UPDATE_STITCH_TILES = 'UPDATE_STITCH_TILES';

export const initStore = (scene) => {
  return {
    type: INIT_STORE,
    scene
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

