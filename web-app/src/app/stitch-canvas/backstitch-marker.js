class BackstitchMarker extends EventDispatcher {
  constructor(ctx, scene, backstitch, touchX, touchY) {
    super();

    this.ctx = ctx;
    this.scene = scene;
    this.backstitch = backstitch;
    this.epsilon = backstitch.width;
    this.aliasingCorrection = backstitch.width / 2 == 0 ? 0 : 0.5;
    if (!backstitch.marked) {
      this.markerColor = "grey";
      this.backstitchColor = backstitch.config.hexColor;
    } else {
      this.markerColor = backstitch.config.hexColor;
      this.backstitchColor = "grey";
    }

    this.setBackstitchPoints(backstitch, touchX, touchY);
    const sceneEventListeners = {
      move: this.move.bind(this),
      touchend: this.touchend.bind(this)
    }

    for (const type in sceneEventListeners) {
      this.scene.addEventListener(type, sceneEventListeners[type]);
    }
  }

  touchend() {
    this.stopDrawing();
    this.dispose();
  }

  move(e) {
    const x = e.detail.x;
    const y = e.detail.y;

    const x1 = this.startPoint.x * this.scene.stitchSize / 2;
    const x2 = this.endPoint.x * this.scene.stitchSize / 2;
    const y1 = this.startPoint.y * this.scene.stitchSize / 2;
    const y2 = this.endPoint.y * this.scene.stitchSize / 2;

    // http://www.cyberforum.ru/cpp-beginners/thread1503781.html
    const k = ((x - x1) * (x2 - x1) + (y - y1) * (y2 - y1)) / (Math.pow(x2 - x1, 2) + Math.pow(y2 - y1, 2));
    const backstitchX = x1 + k * (x2 - x1);
    const backstitchY = y1 + k * (y2 - y1);
    const distanceToBackstitch = Math.sqrt(Math.pow(backstitchX - x, 2) + Math.pow(backstitchY - y, 2));

    if (distanceToBackstitch < this.epsilon) {
      const inbetween = Math.min(x1, x2) <= backstitchX && backstitchX <= Math.max(x1, x2) && Math.min(y1, y2) <= backstitchY && backstitchY <= Math.max(y1, y2);
      if (inbetween) {
        let distanceToEnd = Math.sqrt(Math.pow(x2 - backstitchX, 2) + Math.pow(y2 - backstitchY, 2));

        // TODO: Consider how to solve issue when both backstitches have same direction and distance to end decreased for both.
        if (this.distanceToEndPrev && this.distanceToEndPrev > distanceToEnd) {
          this.dispatchEvent(new CustomEvent("progress", { detail: { backstitch: this.backstitch } }));
          this.draw(backstitchX, backstitchY, x1, y1, x2, y2, distanceToEnd);
        }
        else {
          this.distanceToEndPrev = distanceToEnd;
        }
      }
    }
  }

  draw(x, y, x1, y1, x2, y2, distanceToEnd) {
    if (distanceToEnd < this.epsilon) {
      this.finalize();
    } else {
      this.ctx.beginPath();
      this.ctx.lineCap = 'round';
      this.ctx.moveTo(x1 + this.aliasingCorrection, y1 + this.aliasingCorrection);
      this.ctx.lineTo(x + this.aliasingCorrection, y + this.aliasingCorrection);
      this.ctx.lineWidth = this.backstitch.width;
      this.ctx.strokeStyle = this.markerColor;
      this.ctx.stroke();
      this.ctx.closePath();

      this.ctx.beginPath();
      this.ctx.lineCap = 'butt';
      this.ctx.moveTo(x + this.aliasingCorrection, y + this.aliasingCorrection);
      this.ctx.lineTo(x2 + this.aliasingCorrection, y2 + this.aliasingCorrection);
      this.ctx.lineWidth = this.backstitch.width;
      this.ctx.strokeStyle = this.backstitchColor;
      this.ctx.stroke();
      this.ctx.closePath();

      this.ctx.beginPath();
      this.ctx.lineCap = 'round';
      this.ctx.moveTo(x2 + this.aliasingCorrection, y2 + this.aliasingCorrection);
      this.ctx.lineTo(x2 + this.aliasingCorrection, y2 + this.aliasingCorrection);
      this.ctx.lineWidth = this.backstitch.width;
      this.ctx.strokeStyle = this.backstitchColor;
      this.ctx.stroke();
      this.ctx.closePath();
    }
  }

  setBackstitchPoints(backstitch, touchX, touchY) {
    if (backstitch.x1 == touchX && backstitch.y1 == touchY) {
      this.startPoint = { x: backstitch.x1, y: backstitch.y1 };
      this.endPoint = { x: backstitch.x2, y: backstitch.y2 };
    } else {
      this.startPoint = { x: backstitch.x2, y: backstitch.y2 };
      this.endPoint = { x: backstitch.x1, y: backstitch.y1 };
    }
  }

  finalize() {
    this.dispatchEvent(new CustomEvent("complete", { detail: this.endPoint }));
  }

  stopDrawing() {
    this.dispatchEvent(new CustomEvent("abort"));
  }

  dispose() {
    this.scene.removeEventListener("move", this.move);
    this.scene.removeEventListener("touchend", this.touchend);
  }
}
