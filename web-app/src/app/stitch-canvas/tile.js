class Tile {
  static get size() { return 256; }

  constructor(layer, row, column) {
    this.layer = layer;
    this.row = row;
    this.column = column;
    this.stitches = [];
    this.rerender = () => this.render(true);
  }

  add(stitch) {
    this.stitches.push(stitch);
    stitch.addEventListener("change", this.rerender);
  }

  dispose() {
    this.stitches.forEach(stitch => stitch.removeEventListener("change", this.rerender));
  }

  createContext() {
    var canvas = document.createElement('canvas');
    canvas.width = Tile.size;
    canvas.height = Tile.size;
    return canvas.getContext("2d");
  }

  render(rerender) {
    let scene = this.layer.scene;
    const offsetX = this.column * Tile.size;
    const offsetY = this.row * Tile.size;

    if (rerender || !this.ctx) {
      this.ctx = this.createContext();
      this.ctx.translate(-offsetX, -offsetY);
      this.stitches.forEach(stitch => stitch.draw(this.ctx, scene.stitchSize));
      this.ctx.setTransform(1, 0, 0, 1, 0, 0);

      // this.ctx.beginPath();
      // this.ctx.rect(0, 0, Tile.size, Tile.size);
      // this.ctx.stroke();
    }

    this.layer.ctx.translate(scene.x, scene.y);
    this.layer.ctx.clearRect(offsetX, offsetY, this.ctx.canvas.width, this.ctx.canvas.height);
    this.layer.ctx.drawImage(this.ctx.canvas, offsetX, offsetY);
    this.layer.ctx.translate(-scene.x, -scene.y);
  }
}
