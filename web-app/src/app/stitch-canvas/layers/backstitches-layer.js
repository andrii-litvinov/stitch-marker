class BackstitchesLayer extends BaseLayer {
  constructor(scene) {
    super(scene)

    const sceneEventListeners = {
      render: () => this.render(),
      resize: e => this.resize(e),
      zoom: () => this.render(),
      tap: e => this.tap(e)
    }

    for (const type in sceneEventListeners) {
      this.scene.addEventListener(type, sceneEventListeners[type]);
    }
  }


  tap(event) {
    const x = event.detail.x;
    const y = event.detail.y;
  }

  render() {
    this.ctx.clearRect(0, 0, this.ctx.canvas.width, this.ctx.canvas.height);
    this.ctx.translate(this.scene.x + 0.5, this.scene.y + 0.5);
    this.scene.pattern.backstitches.forEach(bs => {
      const config = this.scene.pattern.configurations[bs.configurationIndex];
      const strands = config.strands || this.scene.pattern.strands;
      const backstitch = new Backstitch(config, this.scene.zoom, strands, bs);
      backstitch.draw(this.ctx, this.scene.stitchSize);
    });
    this.ctx.setTransform(1, 0, 0, 1, 0, 0);
  }
}
