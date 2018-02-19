class GridLayer {
  constructor(scene) {
    this.scene = scene;
    this.ctx = this.createContext();

    let sceneEventListeners = {
      resize: e => this.resize(e),
      patternmodechanged: () => this.render(),
      render: () => this.render(),
      zoom: () => this.render()
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
    let boundsX = this.getBoundsX();
    let boundsY = this.getBoundsY();
    this.ctx.clearRect(0, 0, this.ctx.canvas.width, this.ctx.canvas.height);
    this.ctx.translate(0.5, 0.5);

    if (this.scene.zoom >= 0.5) {
      this.renderGrid(boundsX, boundsY);
      this.renderDecGrid(boundsX, boundsY);
    }
    else {
      this.ctx.strokeRect(boundsX.xFrom, boundsY.yFrom, boundsX.xTo - boundsX.xFrom, boundsY.yTo - boundsY.yFrom);
    }

    this.ctx.setTransform(1, 0, 0, 1, 0, 0);
  }

  renderGrid(boundsX, boundsY) {
    this.ctx.strokeStyle = "lightgray";

    for (let i = 0; i < boundsY.lineCountX; i++) {
      const xFrom = this.scene.x + this.scene.stitchSize * i;
      this.ctx.beginPath();
      this.ctx.moveTo(xFrom, boundsY.yFrom);
      this.ctx.lineTo(xFrom, boundsY.yTo);
      this.ctx.stroke();
    }

    for (let i = 0; i < boundsX.lineCountY; i++) {
      const yFrom = this.scene.y + this.scene.stitchSize * i;
      this.ctx.beginPath();
      this.ctx.moveTo(boundsX.xFrom, yFrom);
      this.ctx.lineTo(boundsX.xTo, yFrom);
      this.ctx.stroke();
    }
  }

  renderDecGrid(boundsX, boundsY) {
    this.ctx.strokeStyle = "black";

    for (let j = 0; j < boundsX.lineCountY; j += 10) {
      let yFrom = this.scene.y + this.scene.stitchSize * j;
      this.ctx.beginPath();
      this.ctx.moveTo(boundsX.xFrom, yFrom);
      this.ctx.lineTo(boundsX.xTo, yFrom);
      this.ctx.stroke();
    }

    for (let j = 0; j < boundsY.lineCountX; j += 10) {
      let xFrom = this.scene.x + this.scene.stitchSize * j;
      this.ctx.beginPath();
      this.ctx.moveTo(xFrom, boundsY.yFrom);
      this.ctx.lineTo(xFrom, boundsY.yTo);
      this.ctx.stroke();
    }
  }

  getBoundsX() {
    let patternCanvasWidth = (Math.floor(this.scene.pattern.width / 10) + 1) * 10;
    let patternCanvasHeight = (Math.floor(this.scene.pattern.height / 10) + 1) * 10;

    const xTo = Math.min(patternCanvasWidth * this.scene.stitchSize + this.scene.x, this.scene.width);
    const yTo = Math.min(patternCanvasHeight * this.scene.stitchSize, this.scene.height - this.scene.y);
    const lineCountY = Math.ceil(yTo / this.scene.stitchSize) + 1;
    const xFrom = this.scene.x;
    return { xTo: xTo, xFrom: xFrom, lineCountY: lineCountY }
  }

  getBoundsY() {
    let patternCanvasWidth = (Math.floor(this.scene.pattern.width / 10) + 1) * 10;
    let patternCanvasHeight = (Math.floor(this.scene.pattern.height / 10) + 1) * 10;

    const xTo = Math.min(patternCanvasWidth * this.scene.stitchSize, this.scene.width - this.scene.x);
    const yTo = Math.min(patternCanvasHeight * this.scene.stitchSize + this.scene.y, this.scene.height);
    const yFrom = this.scene.y;
    const lineCountX = Math.ceil(xTo / this.scene.stitchSize) + 1;
    return { yTo: yTo, yFrom: yFrom, lineCountX: lineCountX }
  }
}
