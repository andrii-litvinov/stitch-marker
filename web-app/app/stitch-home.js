import { PolymerElement, html } from '@polymer/polymer';
import '@polymer/paper-tabs';
import './stitch-patterns.js';

class Home extends PolymerElement {
  static get is() {
    return "stitch-home";
  }

  static get template() {
    return html`
    <style>
      :host {
        display: block;
      }

      #tabs {
        display: flex;
        flex-direction: row;
        justify-content: center;
        align-content: center;
      }

      a {
        text-decoration-line: none;
        color: #616161;
        text-decoration: none;
      }

      a:focus {
        outline-color: rgba(0, 0, 0, 0);
      }

      paper-tabs {
        font-family: 'Roboto';
        text-transform: uppercase;
        --paper-tabs-selection-bar-color: black;
        font-size: 18px;
        margin-bottom: 20px;
      }

      /* .link {
        @apply --layout-horizontal;
        @apply --layout-center-center;
      } */

      paper-tab {
        --paper-tab-ink: black;
        margin-right: 10px;
      }
    </style>

    <div id="tabs">
      <paper-tabs selected="0">
        <paper-tab link>
          <a href="/home/patterns" class="link" tabindex="-1">Мои схемы</a>
        </paper-tab>
        <paper-tab link>
          <a href="/home/library" class="link" tabindex="-1">Библиотека</a>
        </paper-tab>
      </paper-tabs>
    </div>

    <app-route id="route" route="{{route}}" pattern="/:view" data="{{routeData}}" active="{{active}}"></app-route>
    <iron-pages role="main" selected="[[routeData.view]]" attr-for-selected="name">
      <stitch-patterns name="patterns" active="{{active}}"></stitch-patterns>
      <stitch-library name="library"></stitch-library>
    </iron-pages>
    `
  };
}
customElements.define(Home.is, Home);
