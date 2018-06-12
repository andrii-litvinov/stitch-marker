class Backstitch extends EventDispatcher {
  static get scaleFactor() { return 40; }

  constructor(config, strands, data, scale, marked) {
    super();
    this.strands = strands;
    this.config = config;
    this.width = Math.round(Math.sqrt(this.strands.backstitch * Backstitch.scaleFactor) * scale);
    Object.assign(this, data);
    this.marked = marked;
  }

  draw(ctx, stitchSize, scale) {
    ctx.strokeStyle = this.marked ? "grey" : this.config.hexColor;
    ctx.lineCap = "round";
    ctx.lineWidth = Math.round(Math.sqrt(this.strands.backstitch * Backstitch.scaleFactor) * scale);
    this.width = ctx.lineWidth;
    ctx.beginPath();
    ctx.moveTo(this.x1 * stitchSize / 2, this.y1 * stitchSize / 2);
    ctx.lineTo(this.x2 * stitchSize / 2, this.y2 * stitchSize / 2);
    ctx.stroke();
  }
}
