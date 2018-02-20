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
    ctx.moveTo(this.startPoint.x * stitchSize / 2, this.startPoint.y * stitchSize / 2);
    ctx.lineTo(this.endPoint.x * stitchSize / 2, this.endPoint.y * stitchSize / 2);
    ctx.stroke();
  }
}
