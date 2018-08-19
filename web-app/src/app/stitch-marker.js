{/* <link rel="import" href="../bower_components/paper-icon-button/paper-icon-button.html">
<link rel="import" href="../bower_components/paper-spinner/paper-spinner.html">
<link rel="import" href="../bower_components/paper-button/paper-button.html">
<link rel="import" href="../bower_components/paper-toast/paper-toast.html">
<link rel="import" href="../bower_components/iron-icons/iron-icons.html"> */}

import { PolymerElement, html } from '@polymer/polymer';
import './stitch-canvas/stitch-canvas.js';

class Marker extends PolymerElement {
  static get is() { return 'stitch-marker'; }

  static get template() {
    return html`
    <style>
      :host {
        display: none;
        width: 100%;
        height: 100%;
      }

      :host([active]) {
        display: block;
      }

      .controls {
        position: absolute;
        display: flex;
      }

      .controls.right {
        right: 15px;
      }

      .controls.top {
        top: 50px;
      }

      .controls.column {
        flex-direction: column;
      }

      paper-button {
        background-color: var(--google-red-500);
        color: white;
      }

      paper-button[active] {
        background-color: var(--google-green-500);
        color: white;
      }

      paper-spinner {
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
      }

      [icon*="zoom"] {
        background-color: white;
        border-radius: 50%;
        margin: 5px;
        box-shadow: 0 2px 2px 0 rgba(0, 0, 0, 0.14), 0 1px 5px 0 rgba(0, 0, 0, 0.12), 0 3px 1px -2px rgba(0, 0, 0, 0.2);
      }
    </style>

    <stitch-canvas id="canvas" zoom={{zoom}}></stitch-canvas>

    <div class="controls top right column">
      <paper-icon-button icon="zoom-in" title="[[zoom]]" on-tap="zoomIn"></paper-icon-button>
      <paper-icon-button icon="zoom-out" title="[[zoom]]" on-tap="zoomOut"></paper-icon-button>
    </div>

    <paper-spinner active=[[loading]]></paper-spinner>
    <paper-toast id="toast"></paper-toast>
    `
  };

  static get zoomLevels() { return [.1, .15, .25, .33, .5, .67, .75, .8, .9, 1, 1.1, 1.25, 1.5, 1.75, 2, 3, 4, 5]; }

  static get hotKey() {
    return {
      82: 'ruler',  // r
      71: 'grid',   // g
      83: 'symbol', // s
      67: 'color'   // c
    }
  };

  static get properties() {
    return {
      active: {
        type: Boolean,
        reflectToAttribute: true,
        observer: '_activeChanged'
      },
      zoom: {
        type: Number,
        value: 1
      },
      loading: {
        type: Boolean
      }
    };
  }

  constructor() {
    super();

    this.events = {
      keyup: (e) => this._handleKeyUp(e),
    }
  }

  _setUpEvents() {
    document.addEventListener('keyup', this.events.keyup);
  }

  _removeEvents() {
    document.removeEventListener('keyup', this.events.keyup);
  }

  _activeChanged() {
    if (this.active) {
      this._fetchData();
      this._setUpEvents();
      this.zoom = 1;
    } else {
      this._removeEvents();
    }
  }

  async _fetchData() {
    this.loading = true;

    try {
      const json = localStorage.getItem("patternInfo");
      if (!json) return;
      const patternInfo = JSON.parse(json);
      const response = await http.get(SM.apiUrl + patternInfo.links.find(link => link.rel === 'self').href);
      this.$.canvas.pattern = await response.json();
      this.$.canvas.enabled = true;
    } catch (error) {
      this.$.toast.text = error;
      this.$.toast.open();
    }

    this.loading = false;
  }

  zoomIn() {
    // TODO: Handle zoom on swipe when value is not predefined.
    let zoomIndex = Math.min(Marker.zoomLevels.indexOf(this.zoom) + 1, Marker.zoomLevels.length - 1);
    this.zoom = Marker.zoomLevels[zoomIndex];
  }

  zoomOut() {
    // TODO: Handle zoom on swipe when value is not predefined.
    let zoomIndex = Math.max(Marker.zoomLevels.indexOf(this.zoom) - 1, 0);
    this.zoom = Marker.zoomLevels[zoomIndex];
  }

  _handleKeyUp(event) {
    if (event.altKey) {
      const element = Marker.hotKey[event.which];

      if (element) {
        this.set(`display.${element}`, !this.display[element])
      }
    }
  }
}

customElements.define(Marker.is, Marker);
