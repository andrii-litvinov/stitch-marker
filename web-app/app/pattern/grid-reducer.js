import { INIT_GRID, RENDER_GRID } from './actions';

const grid = (state = {}, action) => {
    switch (action.type) {
        case INIT_GRID:

            return { ...state, grid };

        case RENDER_GRID:
            
            return { ...state, grid };

        default:
            return state;
    }
};

export default grid;
