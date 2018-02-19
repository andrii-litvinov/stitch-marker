class StitchesLayer {
  constructor(scene) {
    this.scene = scene;
    this.ctx = this.createContext();
    this.tiles = [];

    this.generateStitches();
    this.rearrangeTiles();

    this.sceneEventListeners = {
      patternmodechange: e => this.render(e.detail.bounds),
      render: e => this.render(e.detail.bounds),
      resize: e => this.resize(e),
      zoom: e => this.zoom(e),
      tap: e => this.tap(e)
    }

    for (let type in this.sceneEventListeners) {
      this.scene.addEventListener(type, this.sceneEventListeners[type]);
    }
  }

  generateStitches() {
    this.stitches = [];
    this.scene.pattern.stitches.forEach(s => {
      const stitch = new Stitch(this.scene.pattern.configurations[s.configurationIndex], s);
      this.stitches[stitch.point.x * this.scene.pattern.height + stitch.point.y] = stitch;
    });
  }

  rearrangeTiles() {
    this.tiles.forEach(tile => tile.dispose());
    this.tiles.length = 0;

    let stitchesPerTile = Tile.size / this.scene.stitchSize;

    this.stitches.forEach(stitch => {
      let column = Math.floor(stitch.point.x / stitchesPerTile);
      let row = Math.floor(stitch.point.y / stitchesPerTile);
      const spanMultipleTilesX = (stitch.point.x + 1) * this.scene.stitchSize > (column + 1) * Tile.size;
      const spanMultipleTilesY = (stitch.point.y + 1) * this.scene.stitchSize > (row + 1) * Tile.size;

      this.addStitchToTile(row, column, stitch);
      if (spanMultipleTilesX) this.addStitchToTile(row, column + 1, stitch);
      if (spanMultipleTilesY) this.addStitchToTile(row + 1, column, stitch);
      if (spanMultipleTilesY && spanMultipleTilesX) this.addStitchToTile(row + 1, column + 1, stitch);
    });
  }

  addStitchToTile(row, column, stitch) {
    let tile = this.tiles[row * this.scene.pattern.height + column];
    if (!tile) {
      tile = new Tile(this, row, column);
      this.tiles[row * this.scene.pattern.height + column] = tile;
    }
    tile.add(stitch);
  }

  resize(event) {
    this.ctx.canvas.width = event.detail.width;
    this.ctx.canvas.height = event.detail.height;
    this.render(event.detail.bounds);
  }

  zoom(event) {
    this.rearrangeTiles();
    this.render(event.detail.bounds);
  }

  tap(event) {
    let coordX = Math.floor((event.detail.x - this.scene.x) / this.scene.stitchSize);
    let coordY = Math.floor((event.detail.y - this.scene.y) / this.scene.stitchSize);
    let stitch = this.stitches[coordX * this.scene.pattern.height + coordY];
    if (stitch) stitch.tap();
  }

  render(bounds) {
    this.ctx.clearRect(0, 0, this.ctx.canvas.width, this.ctx.canvas.height);

    const startRow = bounds.row;
    const startColumn = bounds.column;
    const rowCount = bounds.row + bounds.rowCount;
    const columnCount = bounds.column + bounds.columnCount;
    const patternHeight =  this.scene.pattern.height;

    for (let row = startRow; row < rowCount; row++) {
      for (let column = startColumn; column < columnCount; column++) {
        let tile = this.tiles[row * patternHeight + column];
        tile && tile.render();
      }
    }
  }

  dispose() {
    this.scene.component.shadowRoot.removeChild(this.ctx.canvas);
  }

  createContext() {
    var canvas = document.createElement('canvas');
    this.scene.component.shadowRoot.appendChild(canvas);
    return canvas.getContext("2d");
  }
}
