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

  add(p) {
    this.x = this.x + p.x;
    this.y = this.y + p.y;
  }
}
