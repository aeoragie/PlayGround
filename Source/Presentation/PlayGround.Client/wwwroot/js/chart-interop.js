window.ChartInterop = {
    create: function (canvasId, config) {
        const ctx = document.getElementById(canvasId);
        if (ctx._chart) ctx._chart.destroy();
        ctx._chart = new Chart(ctx, config);
    },
    update: function (canvasId, data) {
        const ctx = document.getElementById(canvasId);
        if (ctx._chart) {
            ctx._chart.data = data;
            ctx._chart.update();
        }
    },
    destroy: function (canvasId) {
        const ctx = document.getElementById(canvasId);
        if (ctx._chart) ctx._chart.destroy();
    }
};
