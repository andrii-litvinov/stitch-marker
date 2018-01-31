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
    switch (patternMode) {
      case "color":
        this.stitches.forEach(stitch => {
          ctx.fillStyle = this.config[stitch.configurationIndex].hexColor;
          ctx.fillRect(
            stitch.point.x * this.stitchSize + offsetX,
            stitch.point.y * this.stitchSize + offsetY,
            this.stitchSize, this.stitchSize);
        })
        break;
      case "symbol":
        this.stitches.forEach(stitch => {
          ctx.fillStyle = 'black';
          ctx.textBaseline = "middle";
          ctx.font = this.stitchSize * 0.8 + "px Arial";
          var metrics = ctx.measureText(this.config[stitch.configurationIndex].symbol);
          ctx.fillText(this.config[stitch.configurationIndex].symbol,
            stitch.point.x * this.stitchSize + (this.stitchSize - metrics.width) / 2 + offsetX,
            stitch.point.y * this.stitchSize + this.stitchSize / 2 + offsetY);
        })
        break;
      default:
        throw `PatternMode '${patternMode}' is not supported.`;
        break;
    }

    ctx.beginPath();
    ctx.rect(0, 0, Tile.size, Tile.size);
    ctx.stroke();
  }
}
