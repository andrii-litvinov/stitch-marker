class Scene {
  constructor(ctx, pattern, patternMode, zoom = 1, x = 0, y = 0) {
    this.ctx = ctx;
    this.tiles = [];
    this.patternMode = patternMode;
    this.x = x;
    this.y = y;
    this.width = pattern.width;
    this.height = pattern.height;

    let stitchSize = Math.floor(zoom * config.stitchSize);
    let stitchesPerTile = Tile.size / stitchSize;

    pattern.stitches.forEach((stitch) => {
      let column = Math.floor(stitch.point.x / stitchesPerTile);
      let spanAcrossColumns = stitch.point.x * stitchSize > (column + 1) * Tile.size;

      let row = Math.floor(stitch.point.y / stitchesPerTile);
      let spanAcrossRows = stitch.point.y * stitchSize > (row + 1) * Tile.size;

      let tile = this.tiles[row * this.height + column];
      if (!tile) {
        tile = new Tile(pattern.configurations, stitchSize);
        this.tiles[row * this.height + column] = tile;
      }
      tile.add(stitch);

      // TODO: Add stitches spanning across tiles. Set offets.

    });
  }

  translate(x, y) {
    this.x += x;
    this.y += y;
    this.render();
  }

  render() {
    this.ctx.clearRect(0, 0, this.ctx.canvas.width, this.ctx.canvas.height);
    this.ctx.translate(this.x, this.y);

    let horizontal = this.getTilesBounds(this.y, this.height, this.ctx.canvas.height);
    let vertical = this.getTilesBounds(this.x, this.width, this.ctx.canvas.width);
    let rendered = 0;

    for (let row = horizontal.start; row < horizontal.end; row++) {
      for (let column = vertical.start; column < vertical.end; column++) {
        let tile = this.tiles[row * this.height + column];
        if (tile) {
          const offsetX = column * Tile.size;
          const offsetY = row * Tile.size
          tile.render(this.ctx, offsetX, offsetY, this.patternMode);         
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
