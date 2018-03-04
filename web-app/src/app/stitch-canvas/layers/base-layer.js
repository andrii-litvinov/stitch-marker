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
