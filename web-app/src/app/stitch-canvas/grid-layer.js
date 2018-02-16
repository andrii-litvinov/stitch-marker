class GridLayer {
  constructor(scene) {
    this.scene = scene;
    this.ctx = this.createContext();

    let sceneEventListeners = {
      resize: e => this.resize(e),
      patternmodechanged: e => this.render(e),
      render: e => this.render(e),
      zoom: e => this.render(e)
    }

    for (const type in sceneEventListeners) {
      this.scene.addEventListener(type, sceneEventListeners[type]);
    }
  }

  dispose() {
    this.scene.component.shadowRoot.removeChild(this.ctx.canvas);
  }

  createContext() {
    let canvas = document.createElement("canvas");
    this.scene.component.shadowRoot.appendChild(canvas);
    return canvas.getContext("2d");
  }

  resize(event) {
    this.ctx.canvas.width = event.detail.width;
    this.ctx.canvas.height = event.detail.height;
    this.render();
  }

  render() {
    this.ctx.clearRect(0, 0, this.ctx.canvas.width, this.ctx.canvas.height);
    this.drawVerticalLines();
    this.drawHorizontalLines();

  }

  drawVerticalLines() {
    const stitchSize = Math.floor(this.scene.zoom * config.stitchSize);
    const xTo = Math.min(this.scene.pattern.width * stitchSize, this.scene.width - this.scene.x);
    const yTo = Math.min(this.scene.pattern.height * stitchSize + this.scene.y, this.scene.height);
    const yFrom = this.scene.y;
    const lineCountX = Math.ceil(xTo / stitchSize) + 1;

    for (let i = 0; i < lineCountX; i++) {
      const xFrom = this.scene.x + stitchSize * i;
      this.ctx.beginPath();
      this.ctx.moveTo(xFrom, yFrom);
      this.ctx.lineTo(xFrom, yTo);
      this.ctx.stroke();
    }
  }

  drawHorizontalLines() {
    const stitchSize = Math.floor(this.scene.zoom * config.stitchSize);
    const xTo = Math.min(this.scene.pattern.width * stitchSize + this.scene.x, this.scene.width);
    const yTo = Math.min(this.scene.pattern.height * stitchSize, this.scene.height - this.scene.y);
    const xFrom = this.scene.x;
    const lineCountY = Math.ceil(yTo / stitchSize) + 1;

    for (let i = 0; i < lineCountY; i++) {
      const yFrom = this.scene.y + stitchSize * i;
      this.ctx.beginPath();
      this.ctx.moveTo(xFrom, yFrom);
      this.ctx.lineTo(xTo, yFrom);
      this.ctx.stroke();
    }
  }
}
