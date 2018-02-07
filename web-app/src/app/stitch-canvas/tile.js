class Tile {
  static get size() { return 256; }

  constructor(config, stitchSize) {
    this.config = config;
    this.stitchSize = stitchSize;
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

  render(ctx, offsetX, offsetY, patternMode) {
    if (!this.ctx) {
      this.ctx = this.createContext();
      
      this.ctx.translate(-offsetX, -offsetY);
      this.stitches.forEach(stitch => {
        stitch.draw(this.ctx, patternMode);
      });
      this.ctx.resetTransform();

      this.ctx.beginPath();
      this.ctx.rect(0, 0, Tile.size, Tile.size);
      this.ctx.stroke();
    }
    ctx.drawImage(this.ctx.canvas, offsetX, offsetY);
  }
}
