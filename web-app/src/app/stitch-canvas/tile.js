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

  render(ctx, offsetX, offsetY) {
    this.stitches.forEach(stitch => {
      ctx.fillStyle = this.config[stitch.configurationIndex].hexColor;
      ctx.fillRect(
        stitch.point.x * this.stitchSize + offsetX,
        stitch.point.y * this.stitchSize + offsetY,
        this.stitchSize, this.stitchSize);
    });

    ctx.beginPath();
    ctx.rect(0, 0, Tile.size, Tile.size);
    ctx.stroke();
  }
}
