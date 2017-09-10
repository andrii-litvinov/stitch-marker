class Grid {
  constructor() {
  }

  draw(ctx, begin, end, size) {
    // TODO: Draw sharp lines when zoom is small.
    // TODO: Draw sharp lines on scroll. Consider to make scroll fixed or rounded or something.
    
    // cached last arguments for redrawing grid
    if (arguments.length) {
      this.draw._prevArguments = [...arguments];
    }

    ctx.strokeStyle = 'lightgray';
    ctx.lineWidth = 1;
    ctx.beginPath();

    let x, y;
    for (x = begin.x + 0.5; x <= end.x + 1; x += size) {
      ctx.moveTo(x, begin.y);
      ctx.lineTo(x, end.y);
    }

    for (y = begin.y + 0.5; y <= end.y + 1; y += size) {
      ctx.moveTo(begin.x, y);
      ctx.lineTo(end.x, y);
    }

    ctx.stroke();

    ctx.strokeStyle = 'black';
    ctx.beginPath();

    for (let x = begin.x + 0.5; x <= end.x + 1; x += size) {
      if (x % (10 * size) === 0.5) {
        ctx.moveTo(x, begin.y);
        ctx.lineTo(x, end.y);
      }
    }

    for (let y = begin.y + 0.5; y <= end.y + 1; y += size) {
      if (y % (10 * size) === 0.5) {
        ctx.moveTo(begin.x, y);
        ctx.lineTo(end.x, y);
      }
    }

    ctx.stroke();
  }
  redraw() {
    this.draw(...this.draw._prevArguments);
  }
}