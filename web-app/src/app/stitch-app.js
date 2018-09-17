import { PolymerElement, html } from '@polymer/polymer';
import '@polymer/app-route/app-route.js';
import '@polymer/app-route/app-location.js';
import '@polymer/iron-pages'

import './stitch-header.js';
import './stitch-social.js';

import reducer from './reducers'
import rootSaga from './sagas'

const sagaMiddleware = createSagaMiddleware({ sagaMonitor })
const store = createStore(reducer, applyMiddleware(sagaMiddleware))
sagaMiddleware.run(rootSaga)

const action = type => store.dispatch({ type })

class App extends PolymerElement {
  static get template() {
    return html`
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
      <stitch-auth name="auth"></stitch-auth>
      <stitch-home name="home" route="{{subroute}}"></stitch-home>
      <stitch-marker name="marker" route="{{subroute}}"></stitch-marker>
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

  async viewChanged(view) {
    await import(`./stitch-${view}.js`);
    this.$.header.hidden = view === "marker";
  }
}

customElements.define(App.is, App);
