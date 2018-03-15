class BackstitchMarker {
  constructor(ctx, scene, backstitch, touchX, touchY) {
    this.ctx = ctx;
    this.scene = scene;
    this.backstitch = backstitch;
    this.epsilon = backstitch.width + 3;
    this.setBackstitchPoints(backstitch, touchX, touchY);
    console.log(this.backstitch);
    console.log(this.startPoint);
    const sceneEventListeners = {
      move: this.move.bind(this),
    }

    for (const type in sceneEventListeners) {
      this.scene.addEventListener(type, sceneEventListeners[type]);
    }
  }

  move(e) {
    const x = e.detail.x;
    const y = e.detail.y;
    console.log(x, y);
    const moveContext = this.getPointOnBackstitch(x, y);
    console.log("move", moveContext);

    // TODO: Fire event that backstitch marking progressed.
    moveContext && this.draw(moveContext.x, moveContext.y);
  }

  getPointOnBackstitch(x, y) {
    const x1 = this.startPoint.x * this.scene.stitchSize / 2;
    const x2 = this.endPoint.x * this.scene.stitchSize / 2;
    const y1 = this.startPoint.y * this.scene.stitchSize / 2;
    const y2 = this.endPoint.y * this.scene.stitchSize / 2;
    const distanceToStart = Math.sqrt(Math.pow(x1 - x, 2) + Math.pow(y1 - y, 2));
    const distanceToEnd = Math.sqrt(Math.pow(x2 - x, 2) + Math.pow(y2 - y, 2));
    const backstitchLength = Math.sqrt(Math.pow(x2 - x1, 2) + Math.pow(y2 - y1, 2));
    const distanceToBackstitch = this.getDistanceToBackstith(distanceToStart, distanceToEnd, backstitchLength);

    if (distanceToBackstitch < this.epsilon) {
      const cathetus = Math.sqrt(Math.pow(distanceToStart, 2) - Math.pow(distanceToBackstitch, 2));

      let ab = Math.sqrt(Math.pow(x2 - x1, 2) + Math.pow(y2 - y1, 2));
      let bc = Math.sqrt(Math.pow(x2 - x1, 2) + Math.pow(y2 - y2, 2));
      let ac = Math.sqrt(Math.pow(x1 - x1, 2) + Math.pow(y2 - y1, 2));

      let dX = cathetus * bc / ab;
      let dY = cathetus * ac / ab;
      const deviationX = [dX, -dX];
      const deviationY = [dY, -dY];

      for (let i = 0; i < deviationX.length; i++) {
        const backstitchX = x1 + deviationX[i];
        for (let j = 0; j < deviationY.length; j++) {
          const backstitchY = y1 + deviationY[j];
          const inbetween = Math.min(x1, x2) <= backstitchX && backstitchX <= Math.max(x1, x2) && Math.min(y1, y2) <= backstitchY && backstitchY <= Math.max(y1, y2);
          const matchesDistance = Math.abs(Math.sqrt((Math.pow(x - backstitchX, 2) + Math.pow(y - backstitchY, 2))) - distanceToBackstitch) < 0.00001;
          if (inbetween && matchesDistance) {
            const pointOnBackstitch = { x: backstitchX, y: backstitchY };
            const backstitchPointToStart = Math.sqrt(Math.pow(x1 - pointOnBackstitch.x, 2) + Math.pow(y1 - pointOnBackstitch.y, 2));
            const backstitchPointToEnd = Math.sqrt(Math.pow(x2 - pointOnBackstitch.x, 2) + Math.pow(y2 - pointOnBackstitch.y, 2));
            if (backstitchPointToStart < backstitchLength && backstitchPointToEnd < backstitchLength) {

              // TODO: Fire event directly here. Move the code to move method.
              return pointOnBackstitch;
            }
            else {
              // TODO: Call dispose from layer as reaction to the event.
              this.dispose();

              // TODO: Fire event that marking was aborted.
              this.backstitch.draw(this.ctx, this.scene.stitchSize, this.scene.scale);
            }
          }
        }
      }
    } else {
      this.stopDrawing();
    }

    return null;
  }

  draw(x, y) {
    console.log("u are in draw method");
    const x1 = this.startPoint.x * this.scene.stitchSize / 2;
    const x2 = this.endPoint.x * this.scene.stitchSize / 2;
    const y1 = this.startPoint.y * this.scene.stitchSize / 2;
    const y2 = this.endPoint.y * this.scene.stitchSize / 2;
    let distanceToEnd = Math.sqrt(Math.pow(x2 - x1, 2) + Math.pow(y2 - y1, 2)) - Math.sqrt(Math.pow(x - x1, 2) + Math.pow(y - y1, 2))

    if (distanceToEnd < this.epsilon) {
      this.ctx.beginPath();
      this.ctx.moveTo(x1, y1);
      this.ctx.lineTo(x2, y2);
      this.ctx.lineWidth = this.backstitch.width;
      this.ctx.strokeStyle = "blue";
      this.ctx.stroke();
      this.ctx.closePath();
      this.finalize();
    } else {
      this.backstitch.draw(this.ctx, this.scene.stitchSize, this.scene.scale);
      this.ctx.beginPath();
      this.ctx.moveTo(x1, y1);
      this.ctx.lineTo(x, y);
      this.ctx.lineWidth = this.backstitch.width;
      this.ctx.strokeStyle = "blue";
      this.ctx.stroke();
      this.ctx.closePath();

      this.ctx.beginPath();
      this.ctx.moveTo(x, y);
      this.ctx.lineTo(x2, y2);
      this.ctx.lineWidth = this.backstitch.width;
      this.ctx.strokeStyle = this.backstitch.config.hexColor;
      this.ctx.stroke();
      this.ctx.closePath();
    }
  }

  getDistanceToBackstith(distanceToStart, distanceToEnd, backstitchLength) {
    let halfPerimeter = (distanceToStart + distanceToEnd + backstitchLength) * 0.5;
    let area = Math.sqrt(halfPerimeter * (halfPerimeter - distanceToStart) * (halfPerimeter - distanceToEnd) * (halfPerimeter - backstitchLength));
    let distanceToBackstitch = (area * 2 / backstitchLength);
    return distanceToBackstitch;
  }

  setBackstitchPoints(backstitch, touchX, touchY) {
    console.log("setBackstitchPoints");
    //normalizing epsilon
    if (backstitch.x1 - this.epsilon / this.scene.stitchSize * 2 < touchX && touchX < backstitch.x1 + this.epsilon / this.scene.stitchSize * 2 && backstitch.y1 - this.epsilon / this.scene.stitchSize * 2 < touchY && touchY < backstitch.y1 + this.epsilon / this.scene.stitchSize * 2) {
      this.startPoint = { x: backstitch.x1, y: backstitch.y1 };
      this.endPoint = { x: backstitch.x2, y: backstitch.y2 };
    } else {
      this.startPoint = { x: backstitch.x2, y: backstitch.y2 };
      this.endPoint = { x: backstitch.x1, y: backstitch.y1 };
    }
  }

  finalize() {
    // TODO: fire event that backstitch is completed

    // TODO: Dispose from layer.
    this.dispose();
  }

  stopDrawing() {
    // TODO: Call dispose from layer as reaction to the event.
    this.dispose();

    // TODO: Fire event that marking was aborted.
    this.backstitch.draw(this.ctx, this.scene.stitchSize, this.scene.scale);
  }

  dispose() {
    this.ctx.canvas.removeEventListener("mousemove", this.move);
  }
}
