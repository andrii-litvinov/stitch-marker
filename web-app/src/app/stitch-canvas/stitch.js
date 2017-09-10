class Stitch {
  static toPixel(n, ratio) { return n * ratio }
  
  constructor(data) {
    this.selected = false;
    this.x = null;
    this.y = null;
    this.symbol = '';
    this.color = 'white';
    
    Object.assign(this, data);
  }

  draw(ctx, size, drawSymbol = true, drawColor = true) {
    // cached last arguments for redrawing grid
    if (arguments.length) {
      this.draw._prevArguments = [...arguments];
    }
    
    let
      cx = Stitch.toPixel(this.x, size),
      cy = Stitch.toPixel(this.y, size);


    // draw color
    if (drawColor) {
      ctx.fillStyle = this.color;
      ctx.fillRect(cx, cy, size, size);
    }

    // draw symbol
    if (drawSymbol && this.symbol) {
      const symbolWidth = ctx.measureText(this.symbol).width;
      ctx.fillStyle = 'black';
      ctx.font = size * .9 + "px Arial";

      ctx.fillText(
        this.symbol,
        cx + (size - symbolWidth) / 2,
        cy + size * .9
      );
    }

    if (this.selected) {
      const lineWidth = 2;
      ctx.lineWidth = lineWidth;
      ctx.strokeStyle = 'red';
      ctx.strokeRect(cx + (lineWidth + 0.5), cy + (lineWidth + 0.5), size - (2 * lineWidth + 0.5), size - (2 * lineWidth + 0.5));
    }
  }
  redraw() {
    this.draw(...this.draw._prevArguments);
  }
  toggle() {
    this.selected = !this.selected;
    this.redraw();
  }
}