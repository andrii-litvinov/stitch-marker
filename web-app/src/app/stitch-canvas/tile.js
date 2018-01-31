class Tile {
  static get size() { return 256; }

  constructor(config, stitchSize) {
    this.config = config;
    this.stitchSize = stitchSize;
    this.stitches = [];
  }

  add(stitch) {
    this.stitches.push(stitch);
  }

  render(ctx, offsetX, offsetY, patternMode) {
    new Stitch(this.config, this.stitchSize, this.stitches).draw(ctx, offsetX, offsetY, patternMode);
    ctx.beginPath();
    ctx.rect(0, 0, Tile.size, Tile.size);
    ctx.stroke();
  }
}
