import { INIT_STORE, MARK_BACKSTITCHES, UNMARK_BACKSTITCHES} from './actions';
import { combineReducers } from 'redux'
import backstitches from '../pattern/backstitch-reducer';
import stitches from '../pattern/stitch-reducer'

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
  backstitches
})

export default rootReducer;
