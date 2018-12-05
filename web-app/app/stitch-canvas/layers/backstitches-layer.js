import BaseLayer from './base-layer.js';
import { patternStore } from '../../pattern/store.js';
import { initBackstitches, renderBackstitch, backstitchTouchStart } from '../../pattern/actions.js';

export default class BackstitchesLayer extends BaseLayer {
  constructor(scene) {
    super(scene)

    this.scene = scene;
    this.ctx = this.createContext();
    patternStore.dispatch(initBackstitches(patternStore.getState().pattern, this.ctx, this.scene));

    const sceneEventListeners = {
      render: patternStore.dispatch(renderBackstitch(this.ctx, this.scene)),
      resize: this.resize.bind(this),
      zoom: patternStore.dispatch(renderBackstitch(this.ctx, this.scene)),
      touchstart: this.touchStart.bind(this)
    };

    for (const type in sceneEventListeners) {
      this.scene.addEventListener(type, sceneEventListeners[type]);
    }
  }

  dispose() {
    this.scene.component.shadowRoot.removeChild(this.ctx.canvas);
  }

  // disposeMarkers() {
  //   this.markers.forEach(marker => {
  //     marker.dispose();
  //   });
  //   this.markers.length = 0;
  // }

  abort() {
    this.activeBackstitch = null;
    this.render();
    this.disposeMarkers();
  }

  touchStart(e) {
    patternStore.dispatch(backstitchTouchStart(e, this.ctx, this.scene))
  }

  createContext() {
    let canvas = document.createElement("canvas");
    this.scene.component.shadowRoot.appendChild(canvas);
    return canvas.getContext("2d");
  }
}
