import { patternStore } from '../../pattern/store.js';
import { renderBackstitch, renderStitch, renderGrid } from '../../pattern/actions.js';

export default class BaseLayer {
  constructor(scene) {
    this.scene = scene;
    this.ctx = this.createContext();
  }
  resize(event) {
    this.ctx.canvas.width = event.detail.width;
    this.ctx.canvas.height = event.detail.height;

    patternStore.dispatch(renderBackstitch(this.ctx, this.scene));
    patternStore.dispatch(renderStitch(event.detail.bounds, this.scene));
    patternStore.dispatch(renderGrid(event.detail.bounds, this.ctx, this.scene));
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
