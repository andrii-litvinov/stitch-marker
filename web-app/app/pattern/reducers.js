import { INIT_STORE } from './actions';
import { combineReducers } from 'redux'
import backstitches from '../pattern/backstitch-reducer';
import stitches from '../pattern/stitch-reducer';
import grid from '../pattern/grid-reducer';

const pattern = (state = {}, action) => {
  switch (action.type) {
    case INIT_STORE:
      return { ...state, ...action.pattern };

    default:
      return state;
  }
};

const rootReducer = combineReducers({
  pattern,
  stitches,
  backstitches,
  grid
})

export default rootReducer;
