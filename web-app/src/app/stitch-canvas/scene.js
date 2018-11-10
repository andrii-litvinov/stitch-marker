import EventDispatcher from './event-dispatcher.js';
import StitchesLayer from './layers/stitches-layer.js';
import GridLayer from './layers/grid-layer.js';
import BackstitchesLayer from './layers/backstitches-layer.js';
import Tile from './tile.js';
import { config } from './config.js';
import { patternStore } from '../pattern-store/pattern-store.js';

export default class Scene extends EventDispatcher {
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

    const unsubscribe = patternStore.subscribe(this.handleChange);

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

  handleChange() {
    console.log('patternStore updated');
  }
}
