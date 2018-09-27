import { PolymerElement, html } from '@polymer/polymer';
import { GestureEventListeners } from '@polymer/polymer/lib/mixins/gesture-event-listeners.js';
import * as Gestures from '@polymer/polymer/lib/utils/gestures.js';
import Scene from './scene.js';
import * as _ from 'webfontloader'
import { store } from '../stitch-store.js';
import { fetchInitState } from '../actions/index.js';

class Canvas extends GestureEventListeners(PolymerElement) {
  static get template() {
    return html`
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
      resolve();
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

    const onEnd = () => {
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

    window.addEventListener('resize', () => this.resize());
    Gestures.addListener(this, 'tap', onTap);
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

    /// update scene data here
    store.dispatch(fetchInitState());

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
