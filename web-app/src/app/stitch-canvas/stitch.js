class Stitch {

  constructor(config, stitchSize, data) {
    this.config = config;
    this.stitchSize = stitchSize;

    Object.assign(this, data);
  }

  draw(ctx, patternMode) {
    switch (patternMode) {
      case "color":
        ctx.fillStyle = this.config.hexColor;
        ctx.fillRect(
          this.point.x * this.stitchSize,
          this.point.y * this.stitchSize,
          this.stitchSize, this.stitchSize);
        break;
      case "symbol":
        ctx.fillStyle = 'black';
        ctx.textBaseline = "middle";
        ctx.font = this.stitchSize * 0.8 + "px Arial";
        var metrics = ctx.measureText(this.config.symbol);
        ctx.fillText(this.config.symbol,
          this.point.x * this.stitchSize + (this.stitchSize - metrics.width) / 2,
          this.point.y * this.stitchSize + this.stitchSize / 2);
        break;
      default:
        throw `PatternMode '${patternMode}' is not supported.`;
        break;
    }
  }
}
