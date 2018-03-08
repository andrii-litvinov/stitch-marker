class BackstitchMarker {
  constructor(ctx, scene, backstitch, touchX, touchY) {
    this.ctx = ctx;
    this.scene = scene;
    this.backstitch = backstitch;
    this.epsilon = backstitch.width + 3;

    this.setBackstitchPoints(backstitch, touchX, touchY);

    const sceneEventListeners = {
      move: this.move.bind(this),
    }

    for (const type in sceneEventListeners) {
      this.scene.addEventListener(type, sceneEventListeners[type]);
    }
  }

  move(e) {
    const moveContext = this.getPointOnBackstitch(e.detail.x, e.detail.y);

    // TODO: Fire event that backstitch marking progressed.
    moveContext && this.draw(moveContext.x, moveContext.y);
  }

  getPointOnBackstitch(x, y) {
    const startPoint = this.startPoint;
    const endPoint = this.endPoint;
    const distanceToStart = Math.sqrt(Math.pow(startPoint.x - x, 2) + Math.pow(startPoint.y - y, 2));
    const distanceToEnd = Math.sqrt(Math.pow(endPoint.x - x, 2) + Math.pow(endPoint.y - y, 2));
    const backstitchLength = Math.sqrt(Math.pow(endPoint.x - startPoint.x, 2) + Math.pow(endPoint.y - startPoint.y, 2));
    const distanceToBackstitch = this.getDistanceToBackstith(distanceToStart, distanceToEnd, backstitchLength);

    if (distanceToBackstitch < this.epsilon) {
      const cathetus = Math.sqrt(Math.pow(distanceToStart, 2) - Math.pow(distanceToBackstitch, 2));

      let ab = Math.sqrt(Math.pow(endPoint.x - startPoint.x, 2) + Math.pow(endPoint.y - startPoint.y, 2));
      let bc = Math.sqrt(Math.pow(endPoint.x - startPoint.x, 2) + Math.pow(endPoint.y - endPoint.y, 2));
      let ac = Math.sqrt(Math.pow(startPoint.x - startPoint.x, 2) + Math.pow(startPoint.y - endPoint.y, 2));

      let dX = cathetus * bc / ab;
      let dY = cathetus * ac / ab;
      const deviationX = [dX, -dX];
      const deviationY = [dY, -dY];

      for (let i = 0; i < deviationX.length; i++) {
        const backstitchX = startPoint.x + deviationX[i];
        for (let j = 0; j < deviationY.length; j++) {
          const backstitchY = startPoint.y + deviationY[j];
          const inbetween = Math.min(startPoint.x, endPoint.x) <= backstitchX && backstitchX <= Math.max(startPoint.x, endPoint.x) && Math.min(startPoint.y, endPoint.y) <= backstitchY && backstitchY <= Math.max(startPoint.y, endPoint.y);
          const matchesDistance = Math.abs(Math.sqrt((Math.pow(x - backstitchX, 2) + Math.pow(y - backstitchY, 2))) - distanceToBackstitch) < 0.00001;
          if (inbetween && matchesDistance) {
            const pointOnBackstitch = { x: backstitchX, y: backstitchY };
            const backstitchPointToStart = Math.sqrt(Math.pow(startPoint.x - pointOnBackstitch.x, 2) + Math.pow(startPoint.y - pointOnBackstitch.y, 2));
            const backstitchPointToEnd = Math.sqrt(Math.pow(endPoint.x - pointOnBackstitch.x, 2) + Math.pow(endPoint.y - pointOnBackstitch.y, 2));
            if (backstitchPointToStart < backstitchLength && backstitchPointToEnd < backstitchLength) {

              // TODO: Fire event directly here. Move the code to move method.
              return pointOnBackstitch;
            }
            else {
              // TODO: Call dispose from layer as reaction to the event.
              this.dispose();

              // TODO: Fire event that marking was aborted.
              drawBackstitch(this.ctx, this.backstitch);
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
    const startPoint = this.startPoint;
    const endPoint = this.endPoint;
    let distanceToEnd = Math.sqrt(Math.pow(endPoint.x - startPoint.x, 2) + Math.pow(endPoint.y - startPoint.y, 2)) - Math.sqrt(Math.pow(x - startPoint.x, 2) + Math.pow(y - startPoint.y, 2))

    if (distanceToEnd < this.epsilon) {
      this.ctx.beginPath();
      this.ctx.moveTo(this.startPoint.x, this.startPoint.y);
      this.ctx.lineTo(this.endPoint.x, this.endPoint.y);
      this.ctx.lineWidth = 5;
      this.ctx.strokeStyle = "blue";
      this.ctx.stroke();
      this.ctx.closePath();
      this.finalize();
    } else {
      drawBackstitch(this.ctx, this.backstitch);
      this.ctx.beginPath();
      this.ctx.moveTo(this.startPoint.x, this.startPoint.y);
      this.ctx.lineTo(x, y);
      this.ctx.lineWidth = 5;
      this.ctx.strokeStyle = "blue";
      this.ctx.stroke();
      this.ctx.closePath();

      this.ctx.beginPath();
      this.ctx.moveTo(x, y);
      this.ctx.lineTo(this.endPoint.x, this.endPoint.y);
      this.ctx.lineWidth = 5;
      this.ctx.strokeStyle = "green";
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
    if (backstitch.x1 - this.epsilon < touchX && touchX < backstitch.x1 + this.epsilon && backstitch.y1 - this.epsilon < touchY && touchY < backstitch.y1 + this.epsilon) {
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
    drawBackstitch(this.ctx, this.backstitch);
  }

  dispose() {
    this.ctx.canvas.removeEventListener("mousemove", this.move);
  }
}
