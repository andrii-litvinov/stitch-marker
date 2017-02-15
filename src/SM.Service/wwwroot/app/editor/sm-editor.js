'use strict';

  class Editor extends Polymer.Element {
    static get is() { return 'sm-editor'; }

    static get config() {
      return {
        properties: {
          active: {
            type: Boolean,
            reflectToAttribute: true
          }
        }
      };
    }
  }

  customElements.define(Editor.is, Editor);