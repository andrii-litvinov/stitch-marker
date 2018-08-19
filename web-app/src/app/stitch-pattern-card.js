import { PolymerElement, html } from '@polymer/polymer';

class PatternCard extends PolymerElement {
  static get template() {
    return html`
    <style>
      :host {
        display: block;
      }

      paper-card {
        width: 300px;
        height: 320px;
      }

      #title {
        white-space: nowrap;
        overflow: hidden;
        font-size: 18px;
        text-overflow: ellipsis;
        padding: 0;
      }

      #size {
        font-size: 14px;
      }

      paper-button {
        font-size: 14px;
        float: left;
        color: var(--google-blue-500);
        padding: 11px 0px;
      }

      .card-actions {
        border-top: 1px solid #e8e8e8;
        padding: 5px 0;
        position: relative;
      }

      #delete {
        color: #616161;
        float: right;
        margin-right: 5px;
      }
    </style>

    <app-location id="location" route="{{route}}"></app-location>
    <paper-card image="[[imageUrl]]">
      <div class="card-content">
        <div class="edit-name">
          <div id="title" title="[[patternInfo.title]]">[[patternInfo.title]]</div>
        </div>
        <div id="size">[[patternInfo.width]] × [[patternInfo.height]]</div>
      </div>
      <div class="card-actions">
        <paper-button on-tap="open">Open</paper-button>
        <paper-icon-button id="delete" icon="delete" on-tap="delete"></paper-icon-button>
      </div>
    </paper-card>
`;
  }

  static get is() {
    return "stitch-pattern-card";
  }
  static get properties() {
    return {
      patternInfo: {
        type: Object,
        value: {},
        observer: 'patternInfoChanged'
      },
      imageUrl: {
        type: String
      }
    }
  }

  open() {
    localStorage.setItem('patternInfo', JSON.stringify(this.patternInfo));
    this.set('route.path', '/marker');
  }

  async patternInfoChanged(patternInfo) {
    const response = await http.get(SM.apiUrl + patternInfo.links.find(link => link.rel === 'thumbnail').href);
    if (response.status === 200) {
      const buffer = await response.arrayBuffer();
      if (buffer) {
        this.imageUrl = 'data:image/png;base64,' + btoa(String.fromCharCode(...new Uint8Array(buffer)));
      };
    }
  }

  async delete() {
    const response = await http.delete(SM.apiUrl + this.patternInfo.links.find(link => link.rel === 'self').href);
    if (response.status === 200)
      this.dispatchEvent(new CustomEvent('delete'));
  }
}
customElements.define(PatternCard.is, PatternCard);
