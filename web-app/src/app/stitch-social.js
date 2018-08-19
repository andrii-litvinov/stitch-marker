class Social extends Polymer.Element {
  static get template() {
    return Polymer.html`
    <style>
       :host {
        display: block;
      }

      div {
        display: flex;
        flex-direction: row;
        justify-content: center;
        align-content: center;
      }

      img {
        width: 30px;
      }
    </style>
    <div>
      <a href="https://www.instagram.com/stitchmarker/" target="_blank">
      <img src="/images/instagram-logo.svg" alt="Instagram">
     </a>
      <a href="/">
      <img src="/images/facebook-logo.svg" alt="Facebook">
     </a>
    </div>
`;
  }

  static get is() {
    return "stitch-social";
  }
}
customElements.define(Social.is, Social);
