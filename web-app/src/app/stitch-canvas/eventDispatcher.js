class EventDispatcher {
  constructor() {
    this.listeners = {};
  }

  addEventListener(type, listener) {
    this.listeners[type] = this.listeners[type] || [];
    this.listeners[type].push(listener);
  }

  removeEventListener(type, listener) {
    if (this.listeners[type]) {
      const index = this.listeners[type].indexOf(listener);
      this.listeners[type].splice(index, 1);
    }
  }

  dispatchEvent(event) {
    if (this.listeners[event.type])
      this.listeners[event.type].forEach(listener => listener(event));
  }
}
