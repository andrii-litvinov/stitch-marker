import BaseLayer from './base-layer.js';
import { patternStore } from '../../pattern/store.js';
import { markStitches, unmarkStitches } from '../../pattern/actions.js';

export default class StitchesLayer extends BaseLayer {
  constructor(scene) {
    super(scene);
    
    this.stitches = patternStore.getState().stitches.stitches; 

    const sceneEventListeners = {
      render: e => this.render(e.detail.bounds),
      resize: e => this.resize(e),
      zoom: e => this.zoom(e),
      tap: e => this.tap(e)
    }

    for (const type in sceneEventListeners) {
      this.scene.addEventListener(type, sceneEventListeners[type]);
    }
  }

  zoom(event) {
    this.rearrangeTiles();
    this.render(event.detail.bounds);
  }

  tap(event) {
    let coordX = Math.floor((event.detail.x - this.scene.x) / this.scene.stitchSize);
    let coordY = Math.floor((event.detail.y - this.scene.y) / this.scene.stitchSize);
    let index = coordX * this.scene.pattern.height + coordY
    let stitch = this.stitches[index];

    dispatch(stitch.marked ? unmarkStitches([index]) : markStitches([index]));

    if (stitch) stitch.tap();
  }

  render(bounds) {
    this.ctx.clearRect(0, 0, this.ctx.canvas.width, this.ctx.canvas.height);

    const startRow = bounds.row;
    const startColumn = bounds.column;
    const rowCount = bounds.row + bounds.rowCount;
    const columnCount = bounds.column + bounds.columnCount;
    const patternHeight = this.scene.pattern.height;

    for (let row = startRow; row < rowCount; row++) {
      for (let column = startColumn; column < columnCount; column++) {
        let tile = this.tiles[row * patternHeight + column];
        tile && tile.render();
      }
    }
  }
}
