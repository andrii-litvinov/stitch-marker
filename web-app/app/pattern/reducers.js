import { combineReducers } from 'redux'
import backstitches from './backstitch-reducer';
import stitches from './stitch-reducer';
import basePatternData from './base-reducer';

export default combineReducers({
  basePatternData,
  backstitches,
  stitches
})
