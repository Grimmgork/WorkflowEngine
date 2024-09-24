
fabric.FunctionNode = fabric.util.createClass(fabric.Object, {
    initialize: function (x, y) {
        this.x = x || 0;
        this.y = y || 0;
    },
    toString: function () {
        return this.x + '/' + this.y;
    }
});

class WorkflowEditor {
    constructor(canvasElement) {
        this.canvasElement;
        var parent = canvasElement.parentElement;
        var canvas = new fabric.Canvas(canvasElement);
        canvas.selection = false;

        this.resizeObserver = new ResizeObserver((entries) => {
            this.canvas.setHeight(parent.getBoundingClientRect().height - 2);
            this.canvas.setWidth(parent.getBoundingClientRect().width - 2);
        });

        this.resizeObserver.observe(parent);

        canvas.on('mouse:wheel', function (opt) {
            var delta = opt.e.deltaY;
            var zoom = canvas.getZoom();
            zoom *= 0.999 ** delta;
            if (zoom > 20) zoom = 20;
            if (zoom < 0.01) zoom = 0.01;
            canvas.zoomToPoint({ x: opt.e.offsetX, y: opt.e.offsetY }, zoom);
            opt.e.preventDefault();
            opt.e.stopPropagation();
        });


        canvas.on('mouse:down', function (opt) {
            var evt = opt.e;
            if (evt.altKey === true) {
                this.isDragging = true;
                this.lastPosX = evt.clientX;
                this.lastPosY = evt.clientY;
            }
        });

        canvas.on('mouse:move', function (opt) {
            if (this.isDragging) {
                var e = opt.e;
                var vpt = this.viewportTransform;
                vpt[4] += e.clientX - this.lastPosX;
                vpt[5] += e.clientY - this.lastPosY;
                this.requestRenderAll();
                this.lastPosX = e.clientX;
                this.lastPosY = e.clientY;
            }
        });

        canvas.on('mouse:up', function (opt) {
            this.setViewportTransform(this.viewportTransform);
            this.isDragging = false;
        });

        this.canvas = canvas;
    }

    addFunctionNode() {
        // "add" rectangle onto canvas
        var rect = new fabric.FunctionNode(1, 2);
        this.canvas.add(rect);

        rect = new fabric.Rect({
            left: 100,
            top: 100,
            fill: 'red',
            width: 20,
            height: 20
        });
        this.canvas.add(rect);
    }

    dispose() {
        this.resizeObserver.disconnect();
    }
}

function initWorkflowEditor(canvasElement) {
    return new WorkflowEditor(canvasElement);
}
