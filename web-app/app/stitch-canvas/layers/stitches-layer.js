import BaseLayer from './base-layer.js';
import { patternStore } from '../../pattern/store.js';
import { tapStitches, renderStitch, initStitches, rearrangeTiles } from '../../pattern/actions.js';

export default class StitchesLayer extends BaseLayer {
  constructor(scene) {
    super(scene);

    patternStore.dispatch(initStitches(patternStore.getState().pattern));
    patternStore.dispatch(rearrangeTiles(scene, this));

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

  zoom(event) {
    patternStore.dispatch(rearrangeTiles(this.scene, this));
    this.render(event.detail.bounds);
  }

  tap(event) {
    let coordX = Math.floor((event.detail.x - this.scene.x) / this.scene.stitchSize);
    let coordY = Math.floor((event.detail.y - this.scene.y) / this.scene.stitchSize);
    let index = coordX * this.scene.pattern.height + coordY
    patternStore.dispatch(tapStitches([index]));
  }

  render(bounds) {
    this.ctx.clearRect(0, 0, this.ctx.canvas.width, this.ctx.canvas.height);
    patternStore.dispatch(renderStitch(bounds, this.scene));
  }
}
