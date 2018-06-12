class BackstitchesLayer extends BaseLayer {
  constructor(scene) {
    super(scene)

    this.scene = scene;
    this.ctx = this.createContext();
    this.generateBackstitches();
    this.markers = [];

    const sceneEventListeners = {
      render: this.render.bind(this),
      resize: this.resize.bind(this),
      zoom: this.render.bind(this),
      touchstart: this.touchStart.bind(this)
    };

    this.markerEventListeners = {
      progress: this.progress.bind(this),
      complete: this.backstitchComplete.bind(this),
      abort: this.abort.bind(this)
    };

    for (const type in sceneEventListeners) {
      this.scene.addEventListener(type, sceneEventListeners[type]);
    }
  }

  dispose() {
    this.scene.component.shadowRoot.removeChild(this.ctx.canvas);
  }

  disposeMarkers() {
    this.markers.forEach(marker => {
      marker.dispose();
    });
    this.markers.length = 0;
  }

  abort() {
    this.render();
    this.disposeMarkers();
  }

  backstitchComplete(e) {
    let index = this.backstitches.indexOf(this.activeBackstitch);
    this.backstitches[index].marked = !this.backstitches[index].marked;

    this.disposeMarkers();

    let point = this.backstitchesMap[e.detail.x * this.scene.pattern.height + e.detail.y];
    if (point) {
      point.forEach(backstitch => {
        if (backstitch != this.activeBackstitch) {
          this.markers.push(new BackstitchMarker(this.ctx, this.scene, backstitch, e.detail.x, e.detail.y));
        }
      });
      for (const type in this.markerEventListeners) {
        this.markers.forEach(marker => {
          marker.addEventListener(type, this.markerEventListeners[type]);
        });
      }
    };

    this.render();
  }

  progress(e) {
    if (this.activeBackstitch != e.detail.backstitch) {
      this.activeBackstitch = e.detail.backstitch;
    }
  }

  touchStart(e) {
    const x = Math.floor((e.detail.x - this.scene.x) / this.scene.stitchSize * 2);
    const y = Math.floor((e.detail.y - this.scene.y) / this.scene.stitchSize * 2);

    for (let i = 0; i <= 1; i++)
      for (let j = 0; j <= 1; j++) {
        let xCoord = x + i;
        let yCoord = y + j;

        let point = this.backstitchesMap[xCoord * this.scene.pattern.height + yCoord];
        if (point) {
          let distToPoint = Math.sqrt(Math.pow((xCoord * this.scene.stitchSize / 2) - e.detail.x, 2) + Math.pow((yCoord * this.scene.stitchSize / 2) - e.detail.y, 2));
          if (distToPoint < this.scene.stitchSize / 2 - 1) {
            point.forEach(backstitch => {
              this.markers.push(new BackstitchMarker(this.ctx, this.scene, backstitch, xCoord, yCoord));
            });
            for (const type in this.markerEventListeners) {
              this.markers.forEach(marker => {
                marker.addEventListener(type, this.markerEventListeners[type]);
              });
            }
          };
        };
      };
  }

  render() {
    this.ctx.clearRect(0, 0, this.ctx.canvas.width, this.ctx.canvas.height);
    this.ctx.translate(this.scene.x + 0.5, this.scene.y + 0.5);
    this.backstitches.forEach(backstitch => {
      backstitch.draw(this.ctx, this.scene.stitchSize, this.scene.scale, backstitch.marked ? "grey" : backstitch.config.hexColor);
    });
    this.ctx.setTransform(1, 0, 0, 1, 0, 0);
  }

  generateBackstitches() {
    const height = this.scene.pattern.height;
    this.backstitchesMap = [];
    this.backstitches = [];
    this.scene.pattern.backstitches.forEach(bs => {
      const config = this.scene.pattern.configurations[bs.configurationIndex];
      const strands = config.strands || this.scene.pattern.strands;
      const backstitch = new Backstitch(config, strands, bs, this.scene.scale, false);
      [
        { x: backstitch.x1, y: backstitch.y1 },
        { x: backstitch.x2, y: backstitch.y2 }
      ].forEach(point => {
        const index = point.x * height + point.y;
        this.backstitchesMap[index] = this.backstitchesMap[index] || [];
        this.backstitchesMap[index].push(backstitch);
        this.backstitches.push(backstitch);
      });
    });
  }

  createContext() {
    let canvas = document.createElement("canvas");
    this.scene.component.shadowRoot.appendChild(canvas);
    return canvas.getContext("2d");
  }
}
