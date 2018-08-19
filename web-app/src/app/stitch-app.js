import './stitch-header.js';
import './stitch-landing.js';
import './stitch-social.js';
class App extends Polymer.Element {
  static get template() {
    return Polymer.html`
    <style>
      :host {
        display: block;
      }
    </style>

    <app-location id="location" route="{{route}}"></app-location>
    <app-route id="route" route="{{route}}" tail="{{subroute}}" pattern="/:view" data="{{routeData}}"></app-route>

    <stitch-header id="header"></stitch-header>
    
    <iron-pages role="main" selected="[[view]]" attr-for-selected="name" selected-attribute="active">
      <stitch-landing name="landing"></stitch-landing>
      <stitch-marker name="marker" route="{{subroute}}"></stitch-marker>
      <stitch-home name="home" route="{{subroute}}"></stitch-home>
      <stitch-auth name="auth"></stitch-auth>
    </iron-pages>
`;
  }

  static get is() { return "stitch-app"; }

  static get properties() {
    return {
      view: {
        type: String,
        reflectToAttribute: true,
        observer: 'viewChanged'
      },
    }
  }

  static get observers() {
    return [
      'routeViewChanged(routeData.view)'
    ]
  }

  routeViewChanged(view) {
    this.view = view || 'landing';
  }

  viewChanged(view) {
    let resolvedUrl = this.resolveUrl(`stitch-${view}.html`);
    Polymer.importHref(resolvedUrl, null, null, true);
    this.$.header.hidden = view === "marker";
  }
}

customElements.define(App.is, App);
