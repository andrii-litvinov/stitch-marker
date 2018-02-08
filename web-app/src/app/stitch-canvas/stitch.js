class Stitch {

  constructor(config, data) {
    this.config = config;
    Object.assign(this, data);
  }

  draw(ctx, scene) {
    switch (scene.patternMode) {
      case "color":
        ctx.fillStyle = this.config.hexColor;
        ctx.fillRect(
          this.point.x * scene.stitchSize,
          this.point.y * scene.stitchSize,
          scene.stitchSize,
          scene.stitchSize);
        break;
      case "symbol":
        ctx.fillStyle = 'black';
        ctx.textBaseline = "middle";
        ctx.font = scene.stitchSize * 0.8 + "px Arial";
        var metrics = ctx.measureText(this.config.symbol);
        ctx.fillText(
          this.config.symbol,
          this.point.x * scene.stitchSize + (scene.stitchSize - metrics.width) / 2,
          this.point.y * scene.stitchSize + scene.stitchSize / 2);
        break;
      default:
        throw `PatternMode '${scene.patternMode}' is not supported.`;
        break;
    }
  }
}
