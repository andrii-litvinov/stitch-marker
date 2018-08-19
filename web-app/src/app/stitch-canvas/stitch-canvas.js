class EventDispatcher {
  constructor() {
    this.listeners = {};
  }

  addEventListener(type, listener) {
    this.listeners[type] = this.listeners[type] || [];
    this.listeners[type].push(listener);
  }

  removeEventListener(type, listener) {
    if (this.listeners[type]) {
      const index = this.listeners[type].indexOf(listener);
      this.listeners[type].splice(index, 1);
    }
  }

  dispatchEvent(event) {
    if (this.listeners[event.type])
      this.listeners[event.type].forEach(listener => listener(event));
  }
}
//https://stackoverflow.com/questions/5560248/programmatically-lighten-or-darken-a-hex-color-or-rgb-and-blend-colors
//https:stackoverflow.com/questions/11867545/change-text-color-based-on-brightness-of-the-covered-background-area
function shadeBlendConvert(p, from, to) {
  if (typeof (p) != "number" || p < -1 || p > 1 || typeof (from) != "string" || (from[0] != 'r' && from[0] != '#') || (typeof (to) != "string" && typeof (to) != "undefined")) return null; //ErrorCheck
  if (!this.sbcRip) this.sbcRip = (d) => {
    let l = d.length, RGB = new Object();
    if (l > 9) {
      d = d.split(",");
      if (d.length < 3 || d.length > 4) return null;//ErrorCheck
      RGB[0] = i(d[0].slice(4)), RGB[1] = i(d[1]), RGB[2] = i(d[2]), RGB[3] = d[3] ? parseFloat(d[3]) : -1;
    } else {
      if (l == 8 || l == 6 || l < 4) return null; //ErrorCheck
      if (l < 6) d = "#" + d[1] + d[1] + d[2] + d[2] + d[3] + d[3] + (l > 4 ? d[4] + "" + d[4] : ""); //3 digit
      d = i(d.slice(1), 16), RGB[0] = d >> 16 & 255, RGB[1] = d >> 8 & 255, RGB[2] = d & 255, RGB[3] = l == 9 || l == 5 ? r(((d >> 24 & 255) / 255) * 10000) / 10000 : -1;
    }
    return RGB;
  }
  var i = parseInt, r = Math.round, h = from.length > 9, h = typeof (to) == "string" ? to.length > 9 ? true : to == "c" ? !h : false : h, b = p < 0, p = b ? p * -1 : p, to = to && to != "c" ? to : b ? "#000000" : "#FFFFFF", f = this.sbcRip(from), t = this.sbcRip(to);
  if (!f || !t) return null; //ErrorCheck
  if (h) return "rgb(" + r((t[0] - f[0]) * p + f[0]) + "," + r((t[1] - f[1]) * p + f[1]) + "," + r((t[2] - f[2]) * p + f[2]) + (f[3] < 0 && t[3] < 0 ? ")" : "," + (f[3] > -1 && t[3] > -1 ? r(((t[3] - f[3]) * p + f[3]) * 10000) / 10000 : t[3] < 0 ? f[3] : t[3]) + ")");
  else return "#" + (0x100000000 + (f[3] > -1 && t[3] > -1 ? r(((t[3] - f[3]) * p + f[3]) * 255) : t[3] > -1 ? r(t[3] * 255) : f[3] > -1 ? r(f[3] * 255) : 255) * 0x1000000 + r((t[0] - f[0]) * p + f[0]) * 0x10000 + r((t[1] - f[1]) * p + f[1]) * 0x100 + r((t[2] - f[2]) * p + f[2])).toString(16).slice(f[3] > -1 || t[3] > -1 ? 1 : 3);
}

function getContrastYIQ(hexcolor) {
  if (hexcolor.charAt(0) === "#")
    hexcolor = hexcolor.slice(1, hexcolor.length);
  var r = parseInt(hexcolor.substr(0, 2), 16);
  var g = parseInt(hexcolor.substr(2, 2), 16);
  var b = parseInt(hexcolor.substr(4, 2), 16);
  var yiq = ((r * 299) + (g * 587) + (b * 114)) / 1000;
  return (yiq >= 128) ? 'dark' : 'bright';
}
const config = {
  stitchSize: 30
};class Stitch extends EventDispatcher {
  constructor(config, data) {
    super();

    this.config = config;
    Object.assign(this, data);
  }

  tap() {
    this.marked = !this.marked;
    this.dispatchEvent(new CustomEvent("change"));
  }

  draw(ctx, stitchSize) {
    if (this.marked) {
      ctx.fillStyle = "#fafafa";
      ctx.fillRect(
        this.x * stitchSize,
        this.y * stitchSize,
        stitchSize,
        stitchSize);
    } else {
      ctx.fillStyle = this.config.hexColor;
      ctx.fillRect(
        this.x * stitchSize,
        this.y * stitchSize,
        stitchSize,
        stitchSize);
    }

    if (this.marked)
      ctx.fillStyle = "lightgray";
    else {
      const brightness = getContrastYIQ(this.config.hexColor);
      if (brightness === "bright")
        ctx.fillStyle = shadeBlendConvert(.3, this.config.hexColor);
      else
        ctx.fillStyle = shadeBlendConvert(-.3, this.config.hexColor);
    }

    ctx.textBaseline = "middle";
    ctx.font = stitchSize * 0.8 + "px CrossStitch3";
    var metrics = ctx.measureText(this.config.symbol);
    ctx.fillText(
      this.config.symbol,
      this.x * stitchSize + (stitchSize - metrics.width) / 2,
      this.y * stitchSize + stitchSize / 2);
  }
}
class Backstitch extends EventDispatcher {
  static get scaleFactor() { return 40; }

  constructor(config, strands, data, scale, marked) {
    super();
    this.strands = strands;
    this.config = config;
    this.width = Math.round(Math.sqrt(this.strands.backstitch * Backstitch.scaleFactor) * scale);
    Object.assign(this, data);
    this.marked = marked;
  }

  draw(ctx, stitchSize, scale) {
    ctx.strokeStyle = this.marked ? "grey" : this.config.hexColor;
    ctx.lineCap = "none";
    ctx.lineWidth = Math.round(Math.sqrt(this.strands.backstitch * Backstitch.scaleFactor) * scale);
    this.width = ctx.lineWidth;
    ctx.beginPath();
    ctx.moveTo(this.x1 * stitchSize / 2, this.y1 * stitchSize / 2);
    ctx.lineTo(this.x2 * stitchSize / 2, this.y2 * stitchSize / 2);
    ctx.stroke();
  }
}
class BackstitchMarker extends EventDispatcher {
  constructor(ctx, scene, backstitch, touchX, touchY) {
    super();

    this.ctx = ctx;
    this.scene = scene;
    this.backstitch = backstitch;
    this.epsilon = backstitch.width;
    if (!backstitch.marked) {
      this.markerColor = "grey";
      this.backstitchColor = backstitch.config.hexColor;
    } else {
      this.markerColor = backstitch.config.hexColor;
      this.backstitchColor = "grey";
    }

    this.setBackstitchPoints(backstitch, touchX, touchY);
    const sceneEventListeners = {
      move: this.move.bind(this),
      touchend: this.touchend.bind(this)
    }

    for (const type in sceneEventListeners) {
      this.scene.addEventListener(type, sceneEventListeners[type]);
    }
  }

  touchend() {
    this.stopDrawing();
    this.dispose();
  }

  move(e) {
    const x = e.detail.x;
    const y = e.detail.y;

    const x1 = this.startPoint.x * this.scene.stitchSize / 2 + this.scene.x;
    const x2 = this.endPoint.x * this.scene.stitchSize / 2 + this.scene.x;
    const y1 = this.startPoint.y * this.scene.stitchSize / 2 + this.scene.y;
    const y2 = this.endPoint.y * this.scene.stitchSize / 2 + this.scene.y;

    // http://www.cyberforum.ru/cpp-beginners/thread1503781.html
    const k = ((x - x1) * (x2 - x1) + (y - y1) * (y2 - y1)) / (Math.pow(x2 - x1, 2) + Math.pow(y2 - y1, 2));
    const backstitchX = x1 + k * (x2 - x1);
    const backstitchY = y1 + k * (y2 - y1);
    const distanceToBackstitch = Math.sqrt(Math.pow(backstitchX - x, 2) + Math.pow(backstitchY - y, 2));

    if (distanceToBackstitch < this.epsilon) {
      const inbetween = Math.min(x1, x2) <= backstitchX && backstitchX <= Math.max(x1, x2) && Math.min(y1, y2) <= backstitchY && backstitchY <= Math.max(y1, y2);
      if (inbetween) {
        let distanceToEnd = Math.sqrt(Math.pow(x2 - backstitchX, 2) + Math.pow(y2 - backstitchY, 2));

        // TODO: Consider how to solve issue when both backstitches have same direction and distance to end decreased for both.
        if (this.distanceToEndPrev && this.distanceToEndPrev > distanceToEnd) {
          this.dispatchEvent(new CustomEvent("progress", { detail: { backstitch: this.backstitch } }));
          this.draw(backstitchX, backstitchY, x1, y1, x2, y2, distanceToEnd);
        }
        else {
          this.distanceToEndPrev = distanceToEnd;
        }
      }
    }
  }

  draw(x, y, x1, y1, x2, y2, distanceToEnd) {
    if (distanceToEnd < this.epsilon) {
      this.finalize();
    } else {
      if (this.backstitch.width / 2 != 0) this.ctx.setTransform(1, 0, 0, 1, 0.5, 0.5);

      this.ctx.beginPath();
      this.ctx.lineCap = 'none';
      this.ctx.moveTo(x1, y1);
      this.ctx.lineTo(x, y);
      this.ctx.lineWidth = this.backstitch.width;
      this.ctx.strokeStyle = this.markerColor;
      this.ctx.stroke();
      this.ctx.closePath();

      this.ctx.beginPath();
      this.ctx.lineCap = 'none';
      this.ctx.moveTo(x, y);
      this.ctx.lineTo(x2, y2);
      this.ctx.lineWidth = this.backstitch.width;
      this.ctx.strokeStyle = this.backstitchColor;
      this.ctx.stroke();
      this.ctx.closePath();

      this.ctx.setTransform(1, 0, 0, 1, 0, 0);
    }
  }

  setBackstitchPoints(backstitch, touchX, touchY) {
    if (backstitch.x1 == touchX && backstitch.y1 == touchY) {
      this.startPoint = { x: backstitch.x1, y: backstitch.y1 };
      this.endPoint = { x: backstitch.x2, y: backstitch.y2 };
    } else {
      this.startPoint = { x: backstitch.x2, y: backstitch.y2 };
      this.endPoint = { x: backstitch.x1, y: backstitch.y1 };
    }
  }

  finalize() {
    this.dispatchEvent(new CustomEvent("complete", { detail: this.endPoint }));
  }

  stopDrawing() {
    this.dispatchEvent(new CustomEvent("abort"));
  }

  dispose() {
    this.scene.removeEventListener("move", this.move);
    this.scene.removeEventListener("touchend", this.touchend);
  }
}
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
class BaseLayer {
  constructor(scene) {
    this.scene = scene;
    this.ctx = this.createContext();
  }
  resize(event) {
    this.ctx.canvas.width = event.detail.width;
    this.ctx.canvas.height = event.detail.height;
    this.render(event.detail.bounds);
  }
  createContext() {
    let canvas = document.createElement("canvas");
    this.scene.component.shadowRoot.appendChild(canvas);
    return canvas.getContext("2d");
  }
  dispose() {
    this.scene.component.shadowRoot.removeChild(this.ctx.canvas);
  }
}
class StitchesLayer extends BaseLayer {
  constructor(scene) {
    super(scene);

    this.tiles = [];

    this.generateStitches();
    this.rearrangeTiles();

    const sceneEventListeners = {
      render: e => this.render(e.detail.bounds),
      resize: e => this.resize(e),
      zoom: e => this.zoom(e),
      tap: e => this.tap(e)
    }

    for (const type in sceneEventListeners) {
      this.scene.addEventListener(type, sceneEventListeners[type]);
    }
  }

  generateStitches() {
    this.stitches = [];
    this.scene.pattern.stitches.forEach(s => {
      const stitch = new Stitch(this.scene.pattern.configurations[s.configurationIndex], s);
      this.stitches[stitch.x * this.scene.pattern.height + stitch.y] = stitch;
    });
  }

  rearrangeTiles() {
    this.tiles.forEach(tile => tile.dispose());
    this.tiles.length = 0;

    let stitchesPerTile = Tile.size / this.scene.stitchSize;

    this.stitches.forEach(stitch => {
      let column = Math.floor(stitch.x / stitchesPerTile);
      let row = Math.floor(stitch.y / stitchesPerTile);
      const spanMultipleTilesX = (stitch.x + 1) * this.scene.stitchSize > (column + 1) * Tile.size;
      const spanMultipleTilesY = (stitch.y + 1) * this.scene.stitchSize > (row + 1) * Tile.size;

      this.addStitchToTile(row, column, stitch);
      if (spanMultipleTilesX) this.addStitchToTile(row, column + 1, stitch);
      if (spanMultipleTilesY) this.addStitchToTile(row + 1, column, stitch);
      if (spanMultipleTilesY && spanMultipleTilesX) this.addStitchToTile(row + 1, column + 1, stitch);
    });
  }

  addStitchToTile(row, column, stitch) {
    let tile = this.tiles[row * this.scene.pattern.height + column];
    if (!tile) {
      tile = new Tile(this, row, column);
      this.tiles[row * this.scene.pattern.height + column] = tile;
    }
    tile.add(stitch);
  }

  zoom(event) {
    this.rearrangeTiles();
    this.render(event.detail.bounds);
  }

  tap(event) {
    let coordX = Math.floor((event.detail.x - this.scene.x) / this.scene.stitchSize);
    let coordY = Math.floor((event.detail.y - this.scene.y) / this.scene.stitchSize);
    let stitch = this.stitches[coordX * this.scene.pattern.height + coordY];
    if (stitch) stitch.tap();
  }

  render(bounds) {
    this.ctx.clearRect(0, 0, this.ctx.canvas.width, this.ctx.canvas.height);

    const startRow = bounds.row;
    const startColumn = bounds.column;
    const rowCount = bounds.row + bounds.rowCount;
    const columnCount = bounds.column + bounds.columnCount;
    const patternHeight =  this.scene.pattern.height;

    for (let row = startRow; row < rowCount; row++) {
      for (let column = startColumn; column < columnCount; column++) {
        let tile = this.tiles[row * patternHeight + column];
        tile && tile.render();
      }
    }
  }
}
class GridLayer extends BaseLayer {
  constructor(scene) {
    super(scene);

    const sceneEventListeners = {
      resize: e => this.resize(e),
      render: () => this.render(),
      zoom: () => this.render()
    }

    for (const type in sceneEventListeners) {
      this.scene.addEventListener(type, sceneEventListeners[type]);
    }
  }

  render() {
    let boundsX = this.getBoundsX();
    let boundsY = this.getBoundsY();
    this.ctx.clearRect(0, 0, this.ctx.canvas.width, this.ctx.canvas.height);
    this.ctx.translate(0.5, 0.5);

    if (this.scene.scale >= 0.5) {
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
class BackstitchesLayer extends BaseLayer {
  constructor(scene) {
    super(scene)

    this.scene = scene;
    this.ctx = this.createContext();
    this.generateBackstitches();
    this.markers = [];

    const sceneEventListeners = {
      render: this.render.bind(this),
      resize: this.resize.bind(this),
      zoom: this.render.bind(this),
      touchstart: this.touchStart.bind(this)
    };

    this.markerEventListeners = {
      progress: this.progress.bind(this),
      complete: this.backstitchComplete.bind(this),
      abort: this.abort.bind(this)
    };

    for (const type in sceneEventListeners) {
      this.scene.addEventListener(type, sceneEventListeners[type]);
    }
  }

  dispose() {
    this.scene.component.shadowRoot.removeChild(this.ctx.canvas);
  }

  disposeMarkers() {
    this.markers.forEach(marker => {
      marker.dispose();
    });
    this.markers.length = 0;
  }

  abort() {
    this.activeBackstitch = null;
    this.render();
    this.disposeMarkers();
  }

  backstitchComplete(e) {
    let index = this.backstitches.indexOf(this.activeBackstitch);
    this.backstitches[index].marked = !this.backstitches[index].marked;

    this.disposeMarkers();
    this.activeBackstitch = null;
    this.render();

    let point = this.backstitchesMap[e.detail.x * this.scene.pattern.height + e.detail.y];
    if (point) {
      this.createBackstitchMarkers(point, e.detail.x, e.detail.y);
    };
  }

  progress(e) {
    if (this.activeBackstitch != e.detail.backstitch) {
      this.activeBackstitch = e.detail.backstitch;
    }
    this.render();
  }

  touchStart(e) {
    const x = Math.floor((e.detail.x - this.scene.x) / this.scene.stitchSize * 2);
    const y = Math.floor((e.detail.y - this.scene.y) / this.scene.stitchSize * 2);

    // check 4 points, near user tap, for available backstitches
    for (let i = 0; i <= 1; i++)
      for (let j = 0; j <= 1; j++) {
        let xCoord = x + i;
        let yCoord = y + j;

        let point = this.backstitchesMap[xCoord * this.scene.pattern.height + yCoord];
        if (point) {
          let distToPoint = Math.sqrt(Math.pow((xCoord * this.scene.stitchSize / 2) - (e.detail.x - this.scene.x), 2) + Math.pow((yCoord * this.scene.stitchSize / 2) - (e.detail.y - this.scene.y), 2));
          if (distToPoint < this.scene.stitchSize / 2 - 1) {
            this.createBackstitchMarkers(point, xCoord, yCoord);
          }
        };
      };
  }

  createBackstitchMarkers(point, touchX, touchY) {
    point.forEach(backstitch => {
      this.markers.push(new BackstitchMarker(this.ctx, this.scene, backstitch, touchX, touchY));
    });
    for (const type in this.markerEventListeners) {
      this.markers.forEach(marker => {
        marker.addEventListener(type, this.markerEventListeners[type]);
      });
    }
  }

  render() {
    this.ctx.clearRect(0, 0, this.ctx.canvas.width, this.ctx.canvas.height);
    this.ctx.translate(this.scene.x + 0.5, this.scene.y + 0.5);
    this.backstitches.forEach(backstitch => {
      if (this.activeBackstitch != backstitch) {
        backstitch.draw(this.ctx, this.scene.stitchSize, this.scene.scale);
      }
    });
    this.ctx.setTransform(1, 0, 0, 1, 0, 0);
  }

  generateBackstitches() {
    const height = this.scene.pattern.height;
    this.backstitchesMap = [];
    this.backstitches = [];
    this.scene.pattern.backstitches.forEach(bs => {
      const config = this.scene.pattern.configurations[bs.configurationIndex];
      const strands = config.strands || this.scene.pattern.strands;
      const backstitch = new Backstitch(config, strands, bs, this.scene.scale, false);
      [
        { x: backstitch.x1, y: backstitch.y1 },
        { x: backstitch.x2, y: backstitch.y2 }
      ].forEach(point => {
        const index = point.x * height + point.y;
        this.backstitchesMap[index] = this.backstitchesMap[index] || [];
        this.backstitchesMap[index].push(backstitch);
        this.backstitches.push(backstitch);
      });
    });
  }

  createContext() {
    let canvas = document.createElement("canvas");
    this.scene.component.shadowRoot.appendChild(canvas);
    return canvas.getContext("2d");
  }
}
class Scene extends EventDispatcher {
  constructor(component, pattern, scale = 1, x = 0, y = 0) {
    super();

    this.component = component;
    this.pattern = pattern;
    this.scale = scale;
    this.x = x;
    this.y = y;
    this.stitchSize = Math.floor(this.scale * config.stitchSize);

    this.layers = [];
    this.layers.push(new StitchesLayer(this));
    this.layers.push(new GridLayer(this));
    this.layers.push(new BackstitchesLayer(this));
  }

  dispose() {
    this.layers.forEach(layer => layer.dispose());
  }

  resize(width, height) {
    this.width = width;
    this.height = height;
    this.dispatchEvent(new CustomEvent("resize", { detail: { width, height, bounds: this.getBounds() } }));
  }

  zoomToCenter(scale) {
    return this.zoomToPoint({
      x: this.width / 2 - this.x,
      y: this.height / 2 - this.y
    }, scale)
  }

  zoomToPoint(point, scale) {
    const k = scale / this.scale;
    this.scale = scale;

    let newPoint = {
      x: Math.round(point.x * k),
      y: Math.round(point.y * k),
    };

    this.stitchSize = Math.floor(this.scale * config.stitchSize);
    this.dispatchEvent(new CustomEvent("zoom", { detail: { scale, bounds: this.getBounds() } }));
    this.translate(point.x - newPoint.x, point.y - newPoint.y);
  }

  translate(x, y) {
    this.x += x;
    this.y += y;
    this.render();
  }

  draw(x, y) {
    this.dispatchEvent(new CustomEvent("move", { detail: { x, y } }));
  }

  tap(x, y) {
    this.dispatchEvent(new CustomEvent("tap", { detail: { x, y } }));
  }

  render() {
    this.dispatchEvent(new CustomEvent("render", { detail: { bounds: this.getBounds() } }));
  }

  touchStart(x, y) {
    this.dispatchEvent(new CustomEvent("touchstart", { detail: { x, y } }));
  }

  touchEnd() {
    this.dispatchEvent(new CustomEvent("touchend"));
  }

  getBounds() {
    const bounds = {};
    Object.assign(bounds, this.getBound("row", this.y, this.height));
    Object.assign(bounds, this.getBound("column", this.x, this.width));
    return bounds;
  }

  getBound(name, coordinate, dimensionSize) {
    let startCoordinate = Math.abs(Math.min(coordinate, 0));
    let current = Math.floor(startCoordinate / Tile.size);
    let count = Math.ceil((dimensionSize - Math.max(coordinate, 0)) / Tile.size);
    if (startCoordinate % Tile.size !== 0) count++;
    return { [name]: current, [name + "Count"]: Math.max(count, 0) };
  }
}
class Canvas extends Polymer.Element {
  static get template() {
    return Polymer.html`
    <style>
      :host {
        position: absolute;
        width: 100vw;
        height: 100vh;
        overflow: hidden;
        user-select: none;
      }

      canvas {
        position: absolute;
        top: 0;
        left: 0;
      }
    </style>
`;
  }

  static get is() { return 'stitch-canvas'; }

  static get properties() {
    return {
      pattern: {
        type: Object,
        reflectToAttribute: false,
        observer: "patternChanged"
      },
      zoom: {
        type: Number,
        reflectToAttribute: false,
        observer: "zoomChanged"
      }
    };
  }

  constructor() {
    super();
    this.fontLoad = new Promise((resolve, reject) => {
      WebFont.load({
        custom: {
          families: ['CrossStitch3'],
          urls: ['../../fonts/cross-stitch-3.css']
        },
        active: () => resolve(),
        inactive: () => reject()
      });
    });
  }

  connectedCallback() {
    super.connectedCallback();

    let position = null;
    let moveCanvas = false;

    const getPosition = event => {
      const e = (event.touches && event.touches[0] || event);
      return { x: Math.round(e.clientX), y: Math.round(e.clientY) };
    };

    const onStart = event => {
      if ((event.type === "mouseenter" && event.buttons & 1 === 1) || event.type !== "mouseenter") {
        position = getPosition(event);
        this.scene.touchStart(position.x, position.y);
        moveCanvas = false;
      }

      if (event.type !== "mouseenter" && (event.buttons == 3) || (event.touches && event.touches.length == 2)) {
        moveCanvas = true;
        return;
      }
    };

    const onEnd = event => {
      position = null;
      this.scene.touchEnd();
    };

    const onTap = event => {
      if (!moveCanvas) this.scene.tap(event.detail.x, event.detail.y);
    };

    const onMove = event => {
      event.preventDefault();

      if (position) {
        const newPosition = getPosition(event);

        if (!moveCanvas) {
          requestAnimationFrame(() => this.scene.draw(newPosition.x, newPosition.y));
          return;
        }

        const dx = newPosition.x - position.x;
        const dy = newPosition.y - position.y;
        requestAnimationFrame(() => this.scene.translate(dx, dy));
        position = newPosition;
      }
    };

    window.addEventListener('resize', event => this.resize());
    window.addEventListener('tap', onTap);
    this.addEventListener('contextmenu', event => event.preventDefault());

    ["mousedown", "mouseenter", "touchstart"].forEach(type => {
      this.addEventListener(type, onStart);
    });

    ["mouseup", "mouseleave", "touchend", "touchcancel"].forEach(type => {
      this.addEventListener(type, onEnd);
    });

    ["mousemove", "touchmove"].forEach(type => {
      this.addEventListener(type, onMove);
    });
  }

  async patternChanged(pattern) {
    this.scene && this.scene.dispose();
    this.scene = new Scene(this, pattern);
    await this.resize();
  }

  zoomChanged(scale) {
    this.enabled && this.scene.zoomToCenter(scale);
  }

  async resize() {
    await this.fontLoad;
    this.scene.resize(this.offsetWidth, this.offsetHeight);
  }
}
customElements.define(Canvas.is, Canvas);
