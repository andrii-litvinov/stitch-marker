import { MARK_STITCHES, UNMARK_STITCHES, TAP_STITCHES, unmarkStitches, markStitches, REARRANGE_TILES, RENDER, INIT_STITCHES } from './actions';
import Stitch from '../stitch-canvas/stitch.js';
import Tile from '../stitch-canvas/tile.js';

const stitches = (state = {}, action) => {
    switch (action.type) {
        case INIT_STITCHES:
            let stitches = [];

            action.pattern.stitches.forEach(s => {
                const stitch = new Stitch(action.pattern.configurations[s.configurationIndex], s);
                stitches[stitch.x * action.pattern.height + stitch.y] = stitch;
            });

            action.pattern.stitches = stitches;
            return { ...state, stitches };

        case REARRANGE_TILES:
            let tiles = [];
            let stitchesPerTile = Tile.size / action.scene.stitchSize;

            state.stitches.forEach(stitch => {
                let column = Math.floor(stitch.x / stitchesPerTile);
                let row = Math.floor(stitch.y / stitchesPerTile);
                const spanMultipleTilesX = (stitch.x + 1) * action.scene.stitchSize > (column + 1) * Tile.size;
                const spanMultipleTilesY = (stitch.y + 1) * action.scene.stitchSize > (row + 1) * Tile.size;

                tiles = addStitchToTile(row, column, stitch, tiles, action.scene.pattern.height, action.stitchesLayer);
                if (spanMultipleTilesX) tiles = addStitchToTile(row, column + 1, stitch, tiles, action.scene.pattern.height, action.stitchesLayer);
                if (spanMultipleTilesY) tiles = addStitchToTile(row + 1, column, stitch, tiles, action.scene.pattern.height, action.stitchesLayer);
                if (spanMultipleTilesY && spanMultipleTilesX) tiles = addStitchToTile(row + 1, column + 1, stitch, tiles, action.scene.pattern.height, action.stitchesLayer);
            });
            return { ...state, tiles };

        case RENDER:
            const startRow = action.bounds.row;
            const startColumn = action.bounds.column;
            const rowCount = action.bounds.row + action.bounds.rowCount;
            const columnCount = action.bounds.column + action.bounds.columnCount;
            const patternHeight = action.scene.pattern.height;

            for (let row = startRow; row < rowCount; row++) {
                for (let column = startColumn; column < columnCount; column++) {
                    let tile = state.tiles[row * patternHeight + column];
                    tile && tile.render();
                }
            }
            return { ...state };

        case TAP_STITCHES:
            action.stitches.forEach(index => {
                let stitch = state.stitches[index];
                stitch.marked = !stitch.marked;
            });
            return { ...state };

        default:
            return state;
    }
};

function addStitchToTile(row, column, stitch, tiles, height, stitchesLayer) {
    let tile = tiles[row * height + column];
    if (!tile) {
        tile = new Tile(stitchesLayer, row, column);
        tiles[row * height + column] = tile;
    }
    tile.add(stitch);
    return tiles;
}

export default stitches;
