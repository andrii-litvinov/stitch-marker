class Point {
  constructor(x, y) {
    this.x = x;
    this.y = y;
  }

  set(x, y) {
    this.x = x !== undefined ? x : this.x;
    this.y = y !== undefined ? y : this.y;
  }

  add(point) {
    this.x += point.x;
    this.y += point.y;
  }
  equal(that) {
    return this.x === that.x && this.y === that.y;
  }
}
