import {PolymerElement, html} from '@polymer/polymer'

import './stitch-pattern-card.js';
import './stitch-upload.js';

class Patterns extends PolymerElement {
  static get template() {
    return html`
    <style>
      :host {
        display: block;
      }

      #container {
        display: flex;
        flex-direction: row;
        flex-wrap: wrap;
        margin: auto;
      }

      @media only screen and (max-width: 720px) {
        #container {
          width: 320px;
          justify-content: center;
        }
      }

      @media only screen and (min-width: 720px) {
        #container {
          width: 660px;
        }
      }

      @media only screen and (min-width: 1024px) {
        #container {
          width: 990px;
        }
      }

      @media only screen and (min-width: 1366px) {
        #container {
          width: 1320px;
        }
      }

      @media only screen and (min-width: 1824px) {
        #container {
          width: 1650px;
        }
      }

      stitch-pattern-card {
        margin: 0 15px 30px 15px;
      }
    </style>
    <div id="container">
      <template is="dom-repeat" items="[[patternInfos]]" as="pattern">
        <stitch-pattern-card pattern-info="[[pattern]]" on-delete="delete"></stitch-pattern-card>
      </template>
    </div>
    <stitch-upload on-upload="upload" on-error="error"></stitch-upload>
    <paper-toast id="toast"></paper-toast>
`;
  }

  static get is() {
    return 'stitch-patterns';
  }

  static get properties() {
    return {
      patternInfos: {
        type: Array,
        value: []
      },
      active: {
        type: Boolean,
        reflectToAttribute: true,
        observer: 'activeChanged'
      }
    }
  }

  activeChanged() {
    const authData = JSON.parse(localStorage.getItem("authData"));
    if (!authData && this.patternInfos.length > 0) {
      this.patternInfos = [];
      return;
    }
    if (this.active) {
      this.getPatterns();
    }
  }

  async getPatterns() {
    const response = await http.get(`${SM.apiUrl}/api/patterns`);
    if (response.status === 200) {
      this.patternInfos = await response.json();
    }
  }

  delete(event) {
    var index = this.patternInfos.indexOf(event.target.patternInfo);
    this.splice('patternInfos', index, 1);
  }

  async upload(event) {
    const patternInfo = event.detail;
    this.push('patternInfos', patternInfo);

    await Promise.yield();

    var cards = this.$.container.querySelectorAll('stitch-pattern-card');
    var latestCard = cards[cards.length - 1];
    var boundingRect = latestCard.getBoundingClientRect();
    if (!(boundingRect.top >= 0 && boundingRect.bottom <= window.innerHeight)) {
      this.smoothScroll(latestCard.offsetTop, 1000);
    }
  }

  error(event) {
    this.$.toast.text = "Pattern upload failed.";
    this.$.toast.open();
  }
}

customElements.define(Patterns.is, Patterns);
