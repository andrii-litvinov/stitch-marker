export const INCREMENT = 'INCREMENT';
export const DECREMENT = 'DECREMENT';
export const DECREMENT_ASYNC = 'DECREMENT_ASYNC';

export const increment = () => {
  return {
    type: INCREMENT
  };
};

export const decrement = () => {
  return {
    type: DECREMENT
  };
};

export const decrementAsync = () => {
  return {
    type: DECREMENT_ASYNC
  };
};
