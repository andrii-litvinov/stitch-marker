import EventDispatcher from './event-dispatcher.js';
import { shadeBlendConvert, getContrastYIQ } from './colors.js';
import { patternStore } from '../pattern-store/pattern-store.js';
import { markStitches, unmarkShitches } from '../pattern-store/pattern-actions.js';

export default class Stitch extends EventDispatcher {
  constructor(config, data) {
    super();

    this.config = config;
    Object.assign(this, data);
  }

  tap() {
    patternStore.dispatch(this.marked ?
      unmarkStitches([{ x: this.x, y: this.y }]) :
      markStitches([{ x: this.x, y: this.y }]));
    this.marked = !this.marked;
    this.dispatchEvent(new CustomEvent("change"));
  }

  draw(ctx, stitchSize) {
    if (this.marked) {
      ctx.fillStyle = "#fafafa";
      ctx.fillRect(
        this.x * stitchSize,
        this.y * stitchSize,
        stitchSize,
        stitchSize);
    } else {
      ctx.fillStyle = this.config.hexColor;
      ctx.fillRect(
        this.x * stitchSize,
        this.y * stitchSize,
        stitchSize,
        stitchSize);
    }

    if (this.marked)
      ctx.fillStyle = "lightgray";
    else {
      const brightness = getContrastYIQ(this.config.hexColor);
      if (brightness === "bright")
        ctx.fillStyle = shadeBlendConvert(.3, this.config.hexColor);
      else
        ctx.fillStyle = shadeBlendConvert(-.3, this.config.hexColor);
    }

    ctx.textBaseline = "middle";
    ctx.font = stitchSize * 0.8 + "px CrossStitch3";
    var metrics = ctx.measureText(this.config.symbol);
    ctx.fillText(
      this.config.symbol,
      this.x * stitchSize + (stitchSize - metrics.width) / 2,
      this.y * stitchSize + stitchSize / 2);
  }
}
