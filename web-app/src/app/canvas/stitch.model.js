class Stitch {
  constructor(data) {
    Object.assign(this, data);
  }

  draw(ctx, size) {
    let x = toPixel(this.x, size),
      y = toPixel(this.y, size),
      half = size / 2;


    // draw color
    ctx.fillStyle = this.color || 'white';
    ctx.fillRect(x, y, size, size);

    // draw symbol
    if (this.symbol) {
      const symbolWidth = ctx.measureText(this.symbol).width;
      ctx.fillStyle = 'black';
      ctx.font = size * .9 + "px Arial";

      ctx.fillText(
        this.symbol,
        x + (size - symbolWidth) / 2,
        y + size * .9
      );
    }
  }

}