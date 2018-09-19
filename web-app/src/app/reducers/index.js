import { INCREMENT, DECREMENT } from '../actions';

const reducer = (state = {clicks: 0, value: 0}, action) => {
  switch (action.type) {
    case INCREMENT:
      return {
        'clicks': state.clicks + 1,
        'value': state.value + 1
      };
    case DECREMENT:
      return {
        'clicks': state.clicks + 1,
        'value': state.value - 1
      };
    default:
      return state;
  }
};

export default reducer;
