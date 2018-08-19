import { PolymerElement, html } from '@polymer/polymer'
import '@polymer/paper-toast'
import { WebAuth } from 'auth0-js'

const webAuth = new WebAuth({
  domain: 'stitch-marker.auth0.com',
  clientID: 'AZHrqJ4Qu2tfZ0F4oxljBtaLSv3cJQD1',
  responseType: 'token id_token',
  audience: 'http://localhost:5000/api/',
  scope: 'openid email profile',
  redirectUri: `${location.origin}/auth`,
  prompt: 'none'
});

class Auth extends PolymerElement {
  static get template() {
    return html`
    <style>
      :host {
        display: block;
      }
    </style>
    <app-location id="location" route="{{route}}"></app-location>
    <paper-toast id="toast"></paper-toast>
    `};

  static get is() { return 'stitch-auth'; }

  static get properties() {
    return {
      active: {
        type: Boolean,
        reflectToAttribute: true,
        observer: 'activeChanged'
      }
    }
  }

  async activeChanged() {
    if (!this.active) return;

    if (window.location.hash) {
      webAuth.parseHash({ hash: window.location.hash }, async (error, authResult) => {
        if (error) {
          this.$.toast.text = `Error reading hash: ${error}`;
          this.$.toast.open();
          return;
        }
        this.saveAuthData(authResult);
        await this.redirect();
      });
    } else if (this.isAuthenticated()) {
      webAuth.logout({
        returnTo: `${location.origin}`,
        clientID: 'AZHrqJ4Qu2tfZ0F4oxljBtaLSv3cJQD1'
      });
      localStorage.removeItem('authData');
      clearTimeout(this.timeoutId);
    } else {
      webAuth.authorize();
    }
  }

  async redirect() {
    await Promise.yield();
    window.location.hash = '';
    this.set("route.path", "/");
  }

  isAuthenticated() {
    const authData = JSON.parse(localStorage.getItem("authData"));
    return authData && authData.expiresOn > new Date().getTime();
  }

  saveAuthData(authResult) {
    const authData = {
      accessToken: authResult.accessToken,
      idToken: authResult.idToken,
      expiresOn: authResult.expiresIn * 1000 + new Date().getTime(),
      userEmail: authResult.idTokenPayload.email,
      userName: authResult.idTokenPayload.name
    };

    localStorage.setItem('authData', JSON.stringify(authData));
    this.scheduleRenewal();
  }

  renewToken() {
    webAuth.checkSession({},
      (error, authResult) => {
        if (error) {
          this.$.toast.text = `Error occurred while checking session: ${error}`;
          this.$.toast.open();
        } else {
          this.saveAuthData(authResult);
        }
      }
    );
  }

  scheduleRenewal() {
    const delay = JSON.parse(localStorage.getItem('authData')).expiresOn - Date.now();
    this.timeoutId = setTimeout(() => { this.renewToken(); }, delay);
  }
}

customElements.define(Auth.is, Auth);
