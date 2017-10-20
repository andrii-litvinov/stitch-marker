class Scene {
  constructor(ctx, pattern) {
    this.ctx = ctx;
    this.tiles = [];
    this.x = 0;
    this.y = 0;
    this.width = pattern.width;
    this.height = pattern.height;


    let stitchSize = 30;
    let tileSize = Tile.size;
    let stitchesPerTile = tileSize / stitchSize;

    pattern.stitches.forEach((stitch) => {
      let column = Math.floor(stitch.point.x / stitchesPerTile);
      let spanAcrossColumns = stitch.point.x * stitchSize > (column + 1) * tileSize;

      let row = Math.floor(stitch.point.y / stitchesPerTile);
      let spanAcrossRows = stitch.point.y * stitchSize > (row + 1) * tileSize;

      let tile = this.tiles[row * this.height + column];
      if (!tile) {
        tile = new Tile(this, pattern.configurations, stitchSize, row, column);
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

    let horizontal = this.getTilesBounds(this.y, this.height, this.ctx.canvas.height);
    let vartical = this.getTilesBounds(this.x, this.width, this.ctx.canvas.width);

    let rendered = 0;
    for (let row = horizontal.start; row < horizontal.end; row++) {
      for (let column = vartical.start; column < vartical.end; column++) {
        let tile = this.tiles[row * this.height + column];
        if (tile) {
          this.ctx.setTransform(1, 0, 0, 1, 0, 0);
          const offsetX = this.x + column * Tile.size;
          const offsetY = this.y + row * Tile.size
          this.ctx.translate(offsetX, offsetY);
          tile.draw(this.ctx, offsetX, offsetY);
          this.ctx.setTransform(1, 0, 0, 1, 0, 0);
          rendered++
        }
      }
    }

    console.log(`rendered: ${rendered}`);
  }

  getTilesBounds(coordinate, size, canvasSize) {
    let startCoordinate = Math.abs(Math.min(coordinate, 0));
    let current = Math.floor(startCoordinate / Tile.size);
    let fittingCount = Math.ceil((canvasSize - Math.max(coordinate, 0)) / Tile.size);
    if (startCoordinate % Tile.size !== 0) fittingCount++;
    let count = Math.min(current + fittingCount, size);
    return { start: current, end: count };
  }
}
