class Backstitch extends EventDispatcher {
  constructor(config, data) {
    super();

    this.config = config;
    Object.assign(this, data);
  }

  draw(ctx, stitchSize) {
    ctx.strokeStyle = this.config.hexColor;
    ctx.lineCap = "round";
    ctx.lineWidth = 5;
    ctx.beginPath();
    ctx.moveTo(this.x1 * stitchSize / 2, this.y1 * stitchSize / 2);
    ctx.lineTo(this.x2 * stitchSize / 2, this.y2 * stitchSize / 2);
    ctx.stroke();
  }
}
