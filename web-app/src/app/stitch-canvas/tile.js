class Tile {
  static get size() { return 256; }

  constructor(scene, config, stitchSize, row, column) {
    this.scene = scene;
    this.config = config;
    this.stitchSize = stitchSize;
    this.row = row;
    this.column = column;
    this.stitches = [];
  }

  add(stitch) {
    this.stitches.push(stitch);
  }

  draw(ctx) {
    this.stitches.forEach(stitch => {
      ctx.fillStyle = this.config[stitch.configurationIndex].hexColor;
      ctx.fillRect(this.scene.x + stitch.point.x * this.stitchSize, this.scene.y + stitch.point.y * this.stitchSize, this.stitchSize, this.stitchSize);
    });

    // ctx.beginPath();
    // ctx.rect(this.scene.x - this.column * Tile.size, this.scene.y - this.row * Tile.size, Tile.size, Tile.size);
    // ctx.stroke();
  }
}
