class Scene extends EventDispatcher {
  constructor(component, pattern, patternMode, zoom = 1, x = 0, y = 0) {
    super();

    this.component = component;
    this.pattern = pattern;
    this.patternMode = patternMode;
    this.zoom = zoom;
    this.x = x;
    this.y = y;
    this.layers = [];
    this.layers.push(new StitchesLayer(this));
  }

  dispose() {
    this.layers.forEach(layer => layer.dispose());
  }

  resize(width, height) {
    this.width = width;
    this.height = height;
    this.dispatchEvent(new CustomEvent("resize", { detail: { width: width, height: height, bounds: this.getBounds() } }));
  }

  setPatternMode(patternMode) {
    this.patternMode = patternMode;
    this.dispatchEvent(new CustomEvent("patternmodechange", { detail: { patternMode: patternMode, bounds: this.getBounds() } }));
  }

  setZoom(zoom) {
    this.zoom = zoom;
    this.dispatchEvent(new CustomEvent("zoom", { detail: { zoom: zoom, bounds: this.getBounds() } }));
  }

  translate(x, y) {
    this.x += x;
    this.y += y;
    this.render();
  }

  tap(x, y) {
    this.dispatchEvent(new CustomEvent("tap", { detail: { x: x, y: y } }));
  }

  render() {
    this.dispatchEvent(new CustomEvent("render", { detail: { bounds: this.getBounds() } }));
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
