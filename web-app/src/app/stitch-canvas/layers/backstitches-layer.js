class BackstitchesLayer extends BaseLayer {
  constructor(scene) {
    super(scene)

    this.scene = scene;
    this.ctx = this.createContext();
    this.generateBackstitches();

    const sceneEventListeners = {
      render: this.render.bind(this),
      resize: this.resize.bind(this),
      zoom: this.render.bind(this),
      touchstart: this.touchStart.bind(this)
    };

    this.markerEventListeners = {
      inprogress: this.inProgress.bind(this),
      complete: this.backstitchComplete.bind(this),
      dispose: this.disposeMarker.bind(this)
    };

    for (const type in sceneEventListeners) {
      this.scene.addEventListener(type, sceneEventListeners[type]);
    }
  }

  dispose() {
    this.scene.component.shadowRoot.removeChild(this.ctx.canvas);
  }

  disposeMarker() {
    this.markers && this.markers.forEach(marker => {
      marker.dispose();
    });
  }

  backstitchComplete() {
    //set flag for backstitch that  he is complete
  }

  inProgress(e) {
    if (this.backstitchesMap[e.detail.x * this.scene.pattern.height + e.detail.y]) {
      console.log("bs: ", this.backstitchesMap[e.detail.x * this.scene.pattern.height + e.detail.y])
      //set flag for this backstitch in bsMap but we're rendering backstitches Array
    }
  }

  touchStart(e) {
    const x = Math.floor((e.detail.x - this.scene.x) / this.scene.stitchSize * 2);
    const y = Math.floor((e.detail.y - this.scene.y) / this.scene.stitchSize * 2);
    const point = this.backstitchesMap[x * this.scene.pattern.height + y];
    if (point) {
      point.forEach(backstitch => {
        this.markers = [];
        this.markers.push(new BackstitchMarker(this.ctx, this.scene, backstitch, x, y));
      });
      for (const type in this.markerEventListeners) {
        this.markers.forEach(marker => {
          marker.addEventListener(type, this.markerEventListeners[type]);
        });
      }
    }
  }

  render() {
    this.ctx.clearRect(0, 0, this.ctx.canvas.width, this.ctx.canvas.height);
    this.ctx.translate(this.scene.x + 0.5, this.scene.y + 0.5);
    this.backstitches.forEach(backstitch => {
      backstitch.draw(this.ctx, this.scene.stitchSize, this.scene.scale);
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
      const backstitch = new Backstitch(config, strands, bs, this.scene.scale);
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
