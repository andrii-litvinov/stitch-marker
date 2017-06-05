class Stitch {
  constructor(data) {
    Object.assign(this, data);
  }

  draw(ctx, size, drawSymbol = true, drawColor = true) {
    let
      x = toPixel(this.x, size),
      y = toPixel(this.y, size);

    // draw color
    if (drawColor) {
      ctx.fillStyle = this.color || 'white';
      ctx.fillRect(x, y, size, size);
    }

    // draw symbol
    if (drawSymbol && this.symbol) {
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