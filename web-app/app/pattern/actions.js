export const INIT_STORE = 'INIT_STORE';
export const MARK_STITCHES = 'MARK_STITCHES';
export const UNMARK_STITCHES = 'UNMARK_STITCHES';
export const TAP_STITCHES = 'TAP_STITCHES';
export const MARK_BACKSTITCHES = 'MARK_BACKSTITCHES';
export const UNMARK_BACKSTITCHES = 'UNMARK_BACKSTITCHES';
export const REARRANGE_TILES = 'REARRANGE_TILES';
export const RENDER_STITCH = 'RENDER';
export const INIT_STITCHES = 'INIT_STITCHES';
export const INIT_BACKSTITCHES = 'INIT_BACKSTITCHES';
export const RENDER_BACKSTITCH = 'RENDER_BACKSTITCH';
export const BACKSTITCH_TOUCHSTART = 'BACKSTITCH_TOUCHSTART';
export const BACKSTITCH_PROCESS = 'BACKSTITCH_PROCESS';
export const BACKSTITCH_COMPLETE = 'BACKSTITCH_COMPLETE';

export const initStore = (pattern) => {
  return {
    type: INIT_STORE,
    pattern
  };
};

export const initStitches = (pattern) => {
  return {
    type: INIT_STITCHES,
    pattern
  };
};

export const initBackstitches = (pattern, ctx, scene) => {
  return {
    type: INIT_BACKSTITCHES,
    pattern, ctx, scene
  };
};

export const processBackstitch = (e) => {
  return {
    type: BACKSTITCH_PROCESS,
    e
  };
};

export const completeBackstitch = (e) => {
  return {
    type: BACKSTITCH_COMPLETE,
    e
  };
};

export const backstitchTouchStart = (e, context, scene) => {
  return {
    type: BACKSTITCH_TOUCHSTART,
    e, context, scene
  };
};

export const renderStitch = (bounds, scene) => {
  return {
    type: RENDER_STITCH,
    bounds, scene
  };
};

export const renderBackstitch = (context, scene) => {
  return {
    type: RENDER_BACKSTITCH,
    context, scene
  };
};

export const rearrangeTiles = (scene, stitchesLayer) => {
  return {
    type: REARRANGE_TILES,
    scene, stitchesLayer
  };
};

export const markStitches = (stitches) => {
  return {
    type: MARK_STITCHES,
    stitches
  };
};

export const unmarkStitches = (stitches) => {
  return {
    type: UNMARK_STITCHES,
    stitches
  };
};

export const tapStitches = (stitches) => {
  return {
    type: TAP_STITCHES,
    stitches
  };
};

export const markBackstitches = (backstitches) => {
  return {
    type: MARK_BACKSTITCHES,
    backstitches
  };
};

export const unmarkBackstitches = (backstitches) => {
  return {
    type: UNMARK_BACKSTITCHES,
    backstitches
  };
};

