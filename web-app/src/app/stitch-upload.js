class Upload extends Polymer.Element {
  static get template() {
    return Polymer.html`
    <style>
      :host {
        display: block;
      }

      paper-fab {
        position: fixed;
        bottom: 30px;
        right: 30px;
        --paper-fab-keyboard-focus-background: var(--accent-color);
      }

      div[slot=file-list] {
        display: none;
      }
    </style>
    <vaadin-upload id="upload" target="[[uploadUrl]]" max-files="1" nodrop="" on-upload-response="onUploadResponse" on-upload-progress="onUploadProgress" on-upload-error="onUploadError" headers="[[authHeader]]">
      <paper-fab slot="add-button" icon="add" title="Upload pattern"></paper-fab>
      <div slot="file-list"></div>
    </vaadin-upload>
`;
  }

  static get is() {
    return "stitch-upload";
  }

  static get properties() {
    return {
      uploadUrl: {
        type: String,
        value: `${SM.apiUrl}/api/patterns`
      }
    }
  }

  connectedCallback() {
    super.connectedCallback();
    this.$.upload.addEventListener('upload-before', (event) => {
      this.$.upload.headers = JSON.stringify(window.getAuthHeaders());
    });
  }

  onUploadProgress(event) {
    this.dispatchEvent(new CustomEvent('progress',
      { bubbles: true, composed: true, detail: event.detail.file.progress }));
  }

  onUploadResponse(event) {
    const xhr = event.detail.xhr;
    if (xhr.status === 201) {
      this.dispatchEvent(new CustomEvent('upload', { detail: JSON.parse(xhr.responseText) }))
    }
    event.target.files = [];
  }

  onUploadError(event) {
    this.dispatchEvent(new CustomEvent('error', { detail: event.detail.file.error }));
    this.dispatchEvent(new CustomEvent('progressrollback', { bubbles: true, composed: true }));
  }
}
customElements.define(Upload.is, Upload);
