class Backstitch extends EventDispatcher {
  static get scaleFactor() { return 40; }

  constructor(config, zoom, strands, data) {
    super();
    this.zoom = zoom;
    this.strands = strands;
    this.config = config;
    Object.assign(this, data);
  }

  draw(ctx, stitchSize) {
    ctx.strokeStyle = this.config.hexColor;
    ctx.lineCap = "round";
    ctx.lineWidth = Math.round(
      Math.sqrt(this.strands.backstitch * Backstitch.scaleFactor) * this.zoom);
    ctx.beginPath();
    ctx.moveTo(this.x1 * stitchSize / 2, this.y1 * stitchSize / 2);
    ctx.lineTo(this.x2 * stitchSize / 2, this.y2 * stitchSize / 2);
    ctx.stroke();
  }
}
