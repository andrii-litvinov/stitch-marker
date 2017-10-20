class Tile {
  static get size() { return 256; }

  constructor(composite, config, stitchSize, row, column) {
    this.composite = composite;
    this.config = config;
    this.stitchSize = stitchSize;
    this.row = row;
    this.column = column;
    this.stitches = [];
  }

  add(stitch) {
    this.stitches.push(stitch);
  }

  draw(ctx) {
    this.stitches.forEach(function (stitch) {
      ctx.fillStyle = this.config[stitch.configurationIndex].hexColor;
      ctx.fillRect(this.composite.x + stitch.point.x * this.stitchSize, this.composite.y + stitch.point.y * this.stitchSize, this.stitchSize, this.stitchSize);
    }, this);

    ctx.beginPath();
    ctx.rect(this.composite.x + this.column * Tile.size, this.composite.y + this.row * Tile.size, Tile.size, Tile.size);
    ctx.stroke();
  }
}
