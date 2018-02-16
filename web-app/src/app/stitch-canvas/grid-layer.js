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
    this.ctx.translate(0.5, 0.5);
    this.drawVerticalLines();
    this.drawHorizontalLines();
    this.drawTileLines();
    this.ctx.setTransform(1, 0, 0, 1, 0, 0);
  }

  drawVerticalLines() {
    let patternCanvasWidth = (Math.floor(this.scene.pattern.width / 10) + 1) * 10;
    let patternCanvasHeight = (Math.floor(this.scene.pattern.height / 10) + 1) * 10;

    const stitchSize = Math.floor(this.scene.zoom * config.stitchSize);
    const xTo = Math.min(patternCanvasWidth * stitchSize, this.scene.width - this.scene.x);
    const yTo = Math.min(patternCanvasHeight * stitchSize + this.scene.y, this.scene.height);
    const yFrom = this.scene.y;
    const lineCountX = Math.ceil(xTo / stitchSize) + 1;
    this.ctx.strokeStyle = "lightgray";

    for (let i = 0; i < lineCountX; i++) {
      const xFrom = this.scene.x + stitchSize * i;
      this.ctx.beginPath();
      this.ctx.moveTo(xFrom, yFrom);
      this.ctx.lineTo(xFrom, yTo);
      this.ctx.stroke();
    }
  }

  drawHorizontalLines() {
    let patternCanvasWidth = (Math.floor(this.scene.pattern.width / 10) + 1) * 10;
    let patternCanvasHeight = (Math.floor(this.scene.pattern.height / 10) + 1) * 10;

    const stitchSize = Math.floor(this.scene.zoom * config.stitchSize);
    const xTo = Math.min(patternCanvasWidth * stitchSize + this.scene.x, this.scene.width);
    const yTo = Math.min(patternCanvasHeight * stitchSize, this.scene.height - this.scene.y);
    const xFrom = this.scene.x;
    const lineCountY = Math.ceil(yTo / stitchSize) + 1;
    this.ctx.strokeStyle = "lightgray";

    for (let i = 0; i < lineCountY; i++) {
      const yFrom = this.scene.y + stitchSize * i;
      this.ctx.beginPath();
      this.ctx.moveTo(xFrom, yFrom);
      this.ctx.lineTo(xTo, yFrom);
      this.ctx.stroke();
    }


  }

  drawTileLines() {
    let patternCanvasWidth = (Math.floor(this.scene.pattern.width / 10) + 1) * 10;
    let patternCanvasHeight = (Math.floor(this.scene.pattern.height / 10) + 1) * 10;

    const stitchSize = Math.floor(this.scene.zoom * config.stitchSize);
    let xTo = Math.min(patternCanvasWidth * stitchSize + this.scene.x, this.scene.width);
    let yTo = Math.min(patternCanvasHeight * stitchSize, this.scene.height - this.scene.y);
    let xFrom = this.scene.x;
    let lineCountY = Math.ceil(yTo / stitchSize) + 1;

    for (let j = 0; j < lineCountY; j += 10) {
      let yFrom = this.scene.y + stitchSize * j;
      this.ctx.strokeStyle = "black";
      this.ctx.beginPath();
      this.ctx.moveTo(xFrom, yFrom);
      this.ctx.lineTo(xTo, yFrom);
      this.ctx.stroke();
    }

    xTo = Math.min(patternCanvasWidth * stitchSize, this.scene.width - this.scene.x);
    yTo = Math.min(patternCanvasHeight * stitchSize + this.scene.y, this.scene.height);
    let yFrom = this.scene.y;
    let lineCountX = Math.ceil(xTo / stitchSize) + 1;


    for (let j = 0; j < lineCountX; j += 10) {
      xFrom = this.scene.x + stitchSize * j;
      this.ctx.strokeStyle = "black";
      this.ctx.beginPath();
      this.ctx.moveTo(xFrom, yFrom);
      this.ctx.lineTo(xFrom, yTo);
      this.ctx.stroke();
    }
  }
}
