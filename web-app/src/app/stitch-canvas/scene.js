class Scene {
  constructor(ctx, pattern, patternMode, zoom = 1, x = 0, y = 0) {
    this.ctx = ctx;
    this.pattern = pattern;
    this.patternMode = patternMode;
    this.zoom = zoom;
    this.x = x;
    this.y = y;

    this.generateStitches();
    this.rearrangeTiles();
  }

  generateStitches() {
    this.stitches = [];
    this.pattern.stitches.forEach(s => {
      const stitch = new Stitch(this.pattern.configurations[s.configurationIndex], s);
      this.stitches[stitch.point.x * this.pattern.height + stitch.point.y] = stitch;
    });
  }

  rearrangeTiles() {
    this.tiles = [];
    this.stitchSize = Math.floor(this.zoom * config.stitchSize);
    let stitchesPerTile = Tile.size / this.stitchSize;

    this.stitches.forEach(stitch => {
      let column = Math.floor(stitch.point.x / stitchesPerTile);
      let row = Math.floor(stitch.point.y / stitchesPerTile);
      const spanMultipleTilesX = (stitch.point.x + 1) * this.stitchSize > (column + 1) * Tile.size;
      const spanMultipleTilesY = (stitch.point.y + 1) * this.stitchSize > (row + 1) * Tile.size;

      this.addStitchToTile(row, column, stitch);
      if (spanMultipleTilesX) this.addStitchToTile(row, column + 1, stitch);
      if (spanMultipleTilesY) this.addStitchToTile(row + 1, column, stitch);
      if (spanMultipleTilesY && spanMultipleTilesX) this.addStitchToTile(row + 1, column + 1, stitch);
    });
  }

  addStitchToTile(row, column, stitch) {
    let tile = this.tiles[row * this.pattern.height + column];
    if (!tile) {
      tile = new Tile();
      this.tiles[row * this.pattern.height + column] = tile;
    }
    tile.add(stitch);
  }

  setPatternMode(patternMode) {
    this.patternMode = patternMode;
    this.render();
  }

  setZoom(zoom) {
    this.zoom = zoom;
    this.rearrangeTiles();
    this.render();
  }

  translate(x, y) {
    this.x += x;
    this.y += y;
    this.render();
  }

  markStitch(x, y) {
    let coordX = Math.floor((x - this.x) / this.stitchSize);
    let coordY = Math.floor((y - this.y) / this.stitchSize);
    let stitch = this.stitches[coordX * this.pattern.height + coordY];
  }

  render() {
    this.ctx.clearRect(0, 0, this.ctx.canvas.width, this.ctx.canvas.height);
    this.ctx.translate(this.x, this.y);

    let horizontal = this.getTilesBounds(this.y, this.pattern.height, this.ctx.canvas.height);
    let vertical = this.getTilesBounds(this.x, this.pattern.width, this.ctx.canvas.width);
    let rendered = 0;

    for (let row = horizontal.start; row < horizontal.end; row++) {
      for (let column = vertical.start; column < vertical.end; column++) {
        let tile = this.tiles[row * this.pattern.height + column];
        if (tile) {
          const offsetX = column * Tile.size;
          const offsetY = row * Tile.size
          tile.render(this, offsetX, offsetY);
          rendered++;
        }
      }
    }

    console.log(`rendered: ${rendered}`);
    this.ctx.resetTransform();
  }

  getTilesBounds(coordinate, size, canvasSize) {
    let startCoordinate = Math.abs(Math.min(coordinate, 0));
    let current = Math.floor(startCoordinate / Tile.size);
    let fittingCount = Math.ceil((canvasSize - Math.max(coordinate, 0)) / Tile.size);

    if (startCoordinate % Tile.size !== 0) fittingCount++;

    return { start: current, end: Math.min(current + fittingCount, size) };
  }
}
