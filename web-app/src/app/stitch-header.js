import {PolymerElement, html} from '@polymer/polymer';
import '@polymer/app-layout/app-layout.js';
import '@polymer/app-layout/app-header/app-header.js';
import '@polymer/app-layout/app-drawer/app-drawer.js';
import '@polymer/iron-selector/iron-selector.js';
import '@polymer/paper-progress';
import '@polymer/paper-button';
import '@polymer/iron-icons/iron-icons.js';
import '@polymer/iron-icons/image-icons.js';
import '@polymer/iron-icons/editor-icons.js';
import '@polymer/paper-icon-button';
import '@polymer/paper-item/paper-item.js';

class Header extends PolymerElement {
  static get template() {
    return html`
    <style>
      :host {
        display: block;
      }

      :host([hidden]) {
        display: none;
      }

      app-toolbar {
        background-color: white;
        margin-bottom: 20px;
        box-shadow: 0px 5px 6px -3px rgba(0, 0, 0, 0.1);
      }

      stitch-logo {
        margin-left: 15px;
      }

      paper-progress {
        display: block;
        width: 100%;
        --paper-progress-active-color: #D81B60;
        --paper-progress-transition-duration: 0.5s;
        --paper-progress-transition-timing-function: ease;
        --paper-progress-transition-transition-delay: 0s;
      }

      paper-icon-button {
        color: #424242;
      }

      a,
      a:visited {
        text-decoration-line: none;
        margin-top: 10px;
        font-family: 'Roboto';
        color: #616161;
        font-size: 14px;
      }

      a:focus {
        outline-color: rgba(0, 0, 0, 0);
        font-weight: bold;
      }

      a:hover {
        background-color: #E0E0E0;
      }

      a:active {
        color: #D81B60;
      }

      #menu-tabs {
        display: flex;
        flex-direction: column;
        justify-content: center;
        text-align: center;
      }

      iron-icon {
        margin-right: 20px;
        color: #616161;
      }

      #drawer-menu {
        margin: 10px 20px;
      }

      app-drawer {
        z-index: 1;
      }
    </style>
    <app-header reveals="">
      <paper-progress id="progress"></paper-progress>
      <app-toolbar>
        <stitch-logo main-title=""></stitch-logo>
        <paper-icon-button icon="menu" on-click="toggleMenu"></paper-icon-button>
      </app-toolbar>
    </app-header>
    <app-drawer id="drawer" align="right" swipe-open="">
      <iron-selector id="viewSelector" selected="{{routeData.view}}" attr-for-selected="name">
        <paper-icon-button id="drawer-menu" icon="menu" on-click="toggleMenu"></paper-icon-button>
        <div role="listbox" id="menu-tabs">
          <a href="/" on-click="toggleMenu">
            <paper-item>
              <iron-icon icon="icons:home"></iron-icon>Landing
            </paper-item>
          </a>
          <a href="/auth" on-click="toggleMenu">
            <paper-item>
              <iron-icon icon="icons:account-circle"></iron-icon>
              <span id="loginText">Login</span>
            </paper-item>
          </a>
          <a href="/home/patterns" on-click="toggleMenu">
            <paper-item>
              <iron-icon icon="image:collections"></iron-icon>Home
            </paper-item>
          </a>
          <a href="/marker" on-click="toggleMenu">
            <paper-item>
              <iron-icon icon="editor:border-color"></iron-icon>Marker
            </paper-item>
          </a>
        </div>
      </iron-selector>
    </app-drawer>
`;
  }

  static get is() { return "stitch-header"; }

  constructor() {
    super();
    this.progress = async event => {
      this.$.progress.classList.add("transiting");
      if (event.detail === 100 && this.$.progress.value < 100) {
        this.$.progress.value = event.detail;
        await Promise.delay(500);
        this.$.progress.value = 0;
      } else {
        this.$.progress.value = event.detail;
      }
      this.$.progress.classList.remove("transiting");
    };

    this.progressrollback = async event => {
      if (this.$.progress.value) {
        this.$.progress.classList.add("transiting");
        this.$.progress.value = 0;
        await Promise.delay(500);
        this.$.progress.classList.remove("transiting");
      }
    };
  }

  connectedCallback() {
    super.connectedCallback();
    window.addEventListener('progress', this.progress);
    window.addEventListener('progressrollback', this.progressrollback);
  }

  disconnectedCallback() {
    super.disconnectedCallback();
    window.removeEventListener('progress', this.progress);
    window.removeEventListener('progressrollback', this.progressrollback);
  }

  toggleMenu() {
    const authData = JSON.parse(localStorage.getItem("authData"));
    const authorized = authData && authData.expiresOn > new Date().getTime();
    this.$.loginText.innerText = authorized ? 'Logout' : 'Login';
    this.$.drawer.toggle();
  }
}
customElements.define(Header.is, Header);
