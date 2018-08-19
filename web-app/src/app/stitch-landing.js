import {PolymerElement, html} from '@polymer/polymer'

class Landing extends PolymerElement {
  static get template() {
    return html`
    <style>
      :host {
        display: block;
      }

      stitch-header {
        margin-bottom: 50px;
      }

      main {
        font-family: 'Roboto';
        display: flex;
        flex-direction: row;
        justify-content: space-around;
        text-align: center;
        align-content: center;
        margin-bottom: 100px;
      }

      main>div {
        max-width: 250px;
      }

      img {
        max-width: 110px;
      }

      h4 {
        font-size: 18px;
      }

      #buttons {
        display: flex;
        flex-direction: row;
        justify-content: center;
      }

      paper-button {
        background-color: black;
        color: white;
        text-align: center;
        font-size: 14px;
        margin: 10px 10px 100px;
        height: 48px;
        width: 200px;
      }

      stitch-social {
        margin-top: 70px;
      }
    </style>

    <main>
      <div>
        <img src="/images/landing/add-icon.svg" alt="add">
        <h4>Добавляйте схемы</h4>
        <p>Возможность добавлять схемы любым удобным для Вас способом.</p>
      </div>
      <div>
        <img src="/images/landing/marker-icon.svg" alt="mark">
        <h4>Отмечайте вышитое</h4>
        <p>Функция, которая облегчает процесс всем, кто любит вышивать.</p>
      </div>
      <div>
        <img src="/images/landing/enjoy-icon.svg" alt="enjoy">
        <h4>Наслаждайтесь процессом</h4>
        <p>Процесс вышивания становится намного удобнее вместе со Stitch&nbsp;Marker.</p>
      </div>
    </main>

    <div id="buttons">
      <paper-button raised="" id="Start">Начать вышивать</paper-button>
    </div>

    <stitch-social> </stitch-social>
`;
  }

  static get is() { return "stitch-landing"; }
}
customElements.define(Landing.is, Landing);
