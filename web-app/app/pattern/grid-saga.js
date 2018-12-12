import { takeEvery } from 'redux-saga/effects'
import { RENDER_GRID } from '../pattern/actions';

export function* watchGridRender() {
    yield takeEvery(RENDER_GRID, GridRender);
}

function GridRender(action) {
    const ctx = action.ctx;
    const scene = action.scene;

    let boundsX = getBoundsX(scene);
    let boundsY = getBoundsY(scene);
    ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
    ctx.translate(0.5, 0.5);

    if (scene.scale >= 0.5) {
        renderGrid(boundsX, boundsY, ctx, scene);
        renderDecGrid(boundsX, boundsY, ctx, scene);
    }
    else {
        ctx.strokeRect(boundsX.xFrom, boundsY.yFrom, boundsX.xTo - boundsX.xFrom, boundsY.yTo - boundsY.yFrom);
    }

    ctx.setTransform(1, 0, 0, 1, 0, 0);
}

function getBoundsX(scene) {
    let patternCanvasWidth = (Math.floor(scene.pattern.width / 10) + 1) * 10;
    let patternCanvasHeight = (Math.floor(scene.pattern.height / 10) + 1) * 10;

    const xTo = Math.min(patternCanvasWidth * scene.stitchSize + scene.x, scene.width);
    const yTo = Math.min(patternCanvasHeight * scene.stitchSize, scene.height - scene.y);
    const lineCountY = Math.ceil(yTo / scene.stitchSize) + 1;
    const xFrom = scene.x;
    return { xTo: xTo, xFrom: xFrom, lineCountY: lineCountY }
}

function getBoundsY(scene) {
    let patternCanvasWidth = (Math.floor(scene.pattern.width / 10) + 1) * 10;
    let patternCanvasHeight = (Math.floor(scene.pattern.height / 10) + 1) * 10;

    const xTo = Math.min(patternCanvasWidth * scene.stitchSize, scene.width - scene.x);
    const yTo = Math.min(patternCanvasHeight * scene.stitchSize + scene.y, scene.height);
    const yFrom = scene.y;
    const lineCountX = Math.ceil(xTo / scene.stitchSize) + 1;
    return { yTo: yTo, yFrom: yFrom, lineCountX: lineCountX }
}

function renderGrid(boundsX, boundsY, ctx, scene) {
    ctx.strokeStyle = "lightgray";

    for (let i = 0; i < boundsY.lineCountX; i++) {
        const xFrom = scene.x + scene.stitchSize * i;
        ctx.beginPath();
        ctx.moveTo(xFrom, boundsY.yFrom);
        ctx.lineTo(xFrom, boundsY.yTo);
        ctx.stroke();
    }

    for (let i = 0; i < boundsX.lineCountY; i++) {
        const yFrom = scene.y + scene.stitchSize * i;
        ctx.beginPath();
        ctx.moveTo(boundsX.xFrom, yFrom);
        ctx.lineTo(boundsX.xTo, yFrom);
        ctx.stroke();
    }
}

function renderDecGrid(boundsX, boundsY, ctx, scene) {
    ctx.strokeStyle = "black";

    for (let j = 0; j < boundsX.lineCountY; j += 10) {
        let yFrom = scene.y + scene.stitchSize * j;
        ctx.beginPath();
        ctx.moveTo(boundsX.xFrom, yFrom);
        ctx.lineTo(boundsX.xTo, yFrom);
        ctx.stroke();
    }

    for (let j = 0; j < boundsY.lineCountX; j += 10) {
        let xFrom = scene.x + scene.stitchSize * j;
        ctx.beginPath();
        ctx.moveTo(xFrom, boundsY.yFrom);
        ctx.lineTo(xFrom, boundsY.yTo);
        ctx.stroke();
    }
}