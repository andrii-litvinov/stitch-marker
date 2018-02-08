class Tile {
  static get size() { return 256; }

  constructor() {
    this.stitches = [];
  }

  add(stitch) {
    this.stitches.push(stitch);
  }

  createContext() {
    var canvas = document.createElement('canvas');
    canvas.width = Tile.size;
    canvas.height = Tile.size;
    return canvas.getContext("2d");
  }

  render(scene, offsetX, offsetY) {
    if (!this.ctx || this.patternMode != scene.patternMode) {
      this.patternMode = scene.patternMode;
      this.ctx = this.createContext();
      
      this.ctx.translate(-offsetX, -offsetY);
      this.stitches.forEach(stitch => stitch.draw(this.ctx, scene));
      this.ctx.resetTransform();

      this.ctx.beginPath();
      this.ctx.rect(0, 0, Tile.size, Tile.size);
      this.ctx.stroke();
    }
    scene.ctx.drawImage(this.ctx.canvas, offsetX, offsetY);
  }
}
