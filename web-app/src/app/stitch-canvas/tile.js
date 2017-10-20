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

  draw(ctx, offsetX, offsetY) {
    this.stitches.forEach(stitch => {
      ctx.fillStyle = this.config[stitch.configurationIndex].hexColor;
      ctx.fillRect(
        this.scene.x + stitch.point.x * this.stitchSize - offsetX,
        this.scene.y + stitch.point.y * this.stitchSize - offsetY,
        this.stitchSize, this.stitchSize);
    });

    ctx.beginPath();
    ctx.rect(0, 0, Tile.size, Tile.size);
    ctx.stroke();
  }
}
