class EventDispatcher {
  constructor() {
    this.listeners = {};
  }

  addEventListener(type, listener) {
    this.listeners[type] = this.listeners[type] || [];
    this.listeners[type].push(listener);
  }

  dispatchEvent(event) {
    if (this.listeners[event.type])
      this.listeners[event.type].forEach(listener => listener(event));
  }
}
