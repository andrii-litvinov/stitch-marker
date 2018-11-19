import { MARK_STITCHES, UNMARK_STITCHES, INIT_STORE, TAP_STITCHES, unmarkStitches, markStitches } from './actions';
import Stitch from '../stitch-canvas/stitch.js';
import Tile from '../stitch-canvas/tile.js';
import { patternStore } from '../pattern/store.js';

const stitches = (state = {}, action) => {
    switch (action.type) {
        case INIT_STORE:
            let stitches = [];
            let tiles = [];

            action.pattern.stitches.forEach(s => {
                const stitch = new Stitch(action.pattern.configurations[s.configurationIndex], s);
                stitches[stitch.x * action.pattern.height + stitch.y] = stitch;
            });
            action.pattern.stitches = stitches;

            // state.tiles.forEach(tile => tile.dispose());
            return { ...state, stitches: stitches, tiles: rearrangeTiles(state, action).tiles};

        case TAP_STITCHES:
            action.stitches.forEach(index => {
                patternStore.dispatch(state.stitches[index].marked ? unmarkStitches([index]) : markStitches([index]));
            });
            return { ...state };

        case UNMARK_STITCHES:
            action.stitches.forEach(index => {
                state.stitches[index].marked = false;
            });
            return { ...state };

        case MARK_STITCHES:
            action.stitches.forEach(index => {
                state.stitches[index].marked = true;
            });
            return { ...state };

        default:
            return state;
    }
};

function rearrangeTiles(state = {}, action) {
    // state.tiles.length = 0;

    let stitchesPerTile = Tile.size / action.pattern.stitchSize;

    action.pattern.stitches.forEach(stitch => {
        let column = Math.floor(stitch.x / stitchesPerTile);
        let row = Math.floor(stitch.y / stitchesPerTile);
        const spanMultipleTilesX = (stitch.x + 1) * action.pattern.stitchSize > (column + 1) * Tile.size;
        const spanMultipleTilesY = (stitch.y + 1) * action.pattern.stitchSize > (row + 1) * Tile.size;

        addStitchToTile(row, column, stitch, action.pattern);
        if (spanMultipleTilesX) this.addStitchToTile(row, column + 1, stitch, action.pattern);
        if (spanMultipleTilesY) this.addStitchToTile(row + 1, column, stitch, action.pattern);
        if (spanMultipleTilesY && spanMultipleTilesX) this.addStitchToTile(row + 1, column + 1, stitch, action.pattern);
    });

    return state;
  }

function addStitchToTile(row, column, stitch , state) {
    let tile = state.tiles[row * state.height + column];
    if (!tile) {
        tile = new Tile(this, row, column);
        state.tiles[row * state.height + column] = tile;
    }
    tile.add(stitch);
}

export default stitches;
