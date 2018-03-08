class Backstitch extends EventDispatcher {
  static get scaleFactor() { return 40; }

  constructor(config, strands, data) {
    super();
    this.strands = strands;
    this.config = config;
    Object.assign(this, data);
  }

  draw(ctx, stitchSize, scale) {
    ctx.strokeStyle = this.config.hexColor;
    ctx.lineCap = "round";
    ctx.lineWidth = Math.round(
      Math.sqrt(this.strands.backstitch * Backstitch.scaleFactor) * scale);
    ctx.beginPath();
    ctx.moveTo(this.x1 * stitchSize / 2, this.y1 * stitchSize / 2);
    ctx.lineTo(this.x2 * stitchSize / 2, this.y2 * stitchSize / 2);
    ctx.stroke();
  }
}
