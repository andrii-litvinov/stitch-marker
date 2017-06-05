class Point {
  constructor(x, y) {
    this.x = x;
    this.y = y;
  }

  toRelativeUnit(ratio) {
    return {
      x: Math.floor(this.x / ratio),
      y: Math.floor(this.y / ratio)
    };
  }

  add(point) {
    this.x = this.x + point.x;
    this.y = this.y + point.y;
  }
}
