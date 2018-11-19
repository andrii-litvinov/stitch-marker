export const INIT_STORE = 'INIT_STORE';
export const MARK_STITCHES = 'MARK_STITCHES';
export const UNMARK_STITCHES = 'UNMARK_STITCHES';
export const TAP_STITCHES = 'TAP_STITCHES';
export const MARK_BACKSTITCHES = 'MARK_BACKSTITCHES';
export const UNMARK_BACKSTITCHES = 'UNMARK_BACKSTITCHES';

export const initStore = (pattern) => {
  return {
    type: INIT_STORE,
    pattern
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

