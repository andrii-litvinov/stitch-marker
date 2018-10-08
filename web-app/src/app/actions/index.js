export const INIT_STORE = 'INIT_STORE';
export const MARK_STITCHES = 'MARK_STITCHES';
export const UNMARK_STITCHES = 'UNMARK_STITCHES';
export const MARK_BACKSTITCHES = 'MARK_BACKSTITCHES';
export const UNMARK_BACKSTITCHES = 'UNMARK_BACKSTITCHES';

export const initStore = (pattern) => {
  return {
    type: INIT_STORE,
    pattern
  };
};

export const markStitches = (x, y) => {
  return {
    type: MARK_STITCHES,
    x, y
  };
};

export const unmarkShitches = (x, y) => {
  return {
    type: UNMARK_STITCHES,
    x, y
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

