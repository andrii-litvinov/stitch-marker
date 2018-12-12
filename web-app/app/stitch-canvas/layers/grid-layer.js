import BaseLayer from './base-layer.js';
import { patternStore } from '../../pattern/store';
import { renderGrid } from '../../pattern/actions.js';

export default class GridLayer extends BaseLayer {
  constructor(scene) {
    super(scene);

    const sceneEventListeners = {
      // resize: e => this.resize(e),
      // render: () => patternStore.dispatch(renderGrid(null, this.context, this.scene)),
      // zoom: () => patternStore.dispatch(renderGrid(null,this.context, this.scene))
    }

    for (const type in sceneEventListeners) {
      this.scene.addEventListener(type, sceneEventListeners[type]);
    }
  }
}
