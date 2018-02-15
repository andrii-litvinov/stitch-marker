class Stitch extends EventDispatcher {
  constructor(config, data) {
    super();

    this.config = config;
    Object.assign(this, data);
  }

  tap() {
    this.marked = !this.marked;
    this.dispatchEvent(new CustomEvent("change"));
  }

  draw(ctx, patternMode, stitchSize) {
    switch (patternMode) {
      case "color":
        ctx.fillStyle = this.config.hexColor;
        if (this.marked) {
          ctx.fillStyle = shadeBlendConvert(.3, this.config.hexColor);
        }
        ctx.fillRect(
          this.point.x * stitchSize,
          this.point.y * stitchSize,
          stitchSize,
          stitchSize);
        break;
      case "symbol":
        ctx.fillStyle = shadeBlendConvert(.2, this.config.hexColor);
        if (this.marked) ctx.fillStyle = shadeBlendConvert(.5, this.config.hexColor);
        ctx.textBaseline = "middle";
        ctx.font = stitchSize * 0.8 + "px CrossStitch3";
        var metrics = ctx.measureText(this.config.symbol);
        ctx.fillText(
          this.config.symbol,
          this.point.x * stitchSize + (stitchSize - metrics.width) / 2,
          this.point.y * stitchSize + stitchSize / 2);
        break;
      default:
        throw `PatternMode '${patternMode}' is not supported.`;
        break;
    }
  }
}
