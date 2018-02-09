class Tile {
  static get size() { return 256; }

  constructor(scene, row, column) {
    this.scene = scene;
    this.row = row;
    this.column = column;
    this.stitches = [];
  }

  add(stitch) {
    this.stitches.push(stitch);
    stitch.addEventListener("change", () => {
      this.render(true);
      this.scene.ctx.resetTransform();
    });
  }

  createContext() {
    var canvas = document.createElement('canvas');
    canvas.width = Tile.size;
    canvas.height = Tile.size;
    return canvas.getContext("2d");
  }

  render(rerender) {
    const offsetX = this.column * Tile.size;
    const offsetY = this.row * Tile.size;

    if (rerender || !this.ctx || this.patternMode != this.scene.patternMode) {
      this.patternMode = this.scene.patternMode;
      this.ctx = this.createContext();
      this.ctx.translate(-offsetX, -offsetY);
      this.stitches.forEach(stitch => stitch.draw(this.ctx, this.scene));
      this.ctx.resetTransform();

      this.ctx.beginPath();
      this.ctx.rect(0, 0, Tile.size, Tile.size);
      this.ctx.stroke();
    }
    this.scene.ctx.translate(this.scene.x, this.scene.y);
    this.scene.ctx.drawImage(this.ctx.canvas, offsetX, offsetY);
    this.scene.ctx.resetTransform();
  }
}
