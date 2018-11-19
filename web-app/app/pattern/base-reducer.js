import { INIT_STORE } from './actions';

const basePatternData = (state = {}, action) => {
    switch (action.type) {
        case INIT_STORE:
            return { ...state, pattern: action.pattern };

        default:
            return state;
    }
};

export default basePatternData;
