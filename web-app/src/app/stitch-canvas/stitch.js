class Stitch {

  constructor(config, stitchSize, stitches) {
    this.config = config;
    this.stitchSize = stitchSize;
    this.stitches = stitches;
  }

  draw(ctx, offsetX, offsetY, patternMode) {
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
  }
}
