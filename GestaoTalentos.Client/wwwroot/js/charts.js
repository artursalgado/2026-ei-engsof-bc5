// =============================================================================
// Helpers de JS Interop para gráficos do Dashboard
// Usa ApexCharts (https://apexcharts.com) carregado via CDN no index.html
// Cada função: cria o gráfico no elemento com o id passado, com os dados
// recebidos do Blazor. Reaproveita instâncias guardadas em window._dashboardCharts
// para que re-render limpe o anterior em vez de empilhar.
// =============================================================================

window._dashboardCharts = window._dashboardCharts || {};

function _destruir(elementId) {
    if (window._dashboardCharts[elementId]) {
        try { window._dashboardCharts[elementId].destroy(); } catch { /* ignora */ }
        delete window._dashboardCharts[elementId];
    }
}

function _temasCores() {
    return ['#6366f1', '#8b5cf6', '#ec4899', '#f59e0b', '#10b981', '#06b6d4', '#84cc16', '#f43f5e'];
}

// Barras verticais — Perfis por Área
window.renderPerfisPorArea = function (elementId, labels, valores) {
    _destruir(elementId);
    const options = {
        chart: { type: 'bar', height: 320, toolbar: { show: false }, fontFamily: 'Inter, sans-serif' },
        series: [{ name: 'Perfis', data: valores }],
        xaxis: { categories: labels },
        colors: ['#6366f1'],
        plotOptions: { bar: { borderRadius: 6, columnWidth: '55%', distributed: false } },
        dataLabels: { enabled: false },
        grid: { borderColor: '#e5e7eb', strokeDashArray: 4 },
        tooltip: { theme: 'light' }
    };
    const chart = new ApexCharts(document.getElementById(elementId), options);
    chart.render();
    window._dashboardCharts[elementId] = chart;
};

// Donut — Distribuição por País
window.renderPaisesDonut = function (elementId, labels, valores) {
    _destruir(elementId);
    const options = {
        chart: { type: 'donut', height: 320, fontFamily: 'Inter, sans-serif' },
        series: valores,
        labels: labels,
        colors: _temasCores(),
        legend: { position: 'bottom' },
        plotOptions: { pie: { donut: { size: '65%' } } },
        dataLabels: { enabled: true, formatter: (val) => val.toFixed(1) + '%' }
    };
    const chart = new ApexCharts(document.getElementById(elementId), options);
    chart.render();
    window._dashboardCharts[elementId] = chart;
};

// Linha temporal — Propostas criadas por mês
window.renderPropostasPorMes = function (elementId, labels, valores) {
    _destruir(elementId);
    const options = {
        chart: { type: 'line', height: 320, toolbar: { show: false }, fontFamily: 'Inter, sans-serif', zoom: { enabled: false } },
        series: [{ name: 'Propostas criadas', data: valores }],
        xaxis: { categories: labels },
        colors: ['#10b981'],
        stroke: { curve: 'smooth', width: 3 },
        markers: { size: 5 },
        dataLabels: { enabled: false },
        grid: { borderColor: '#e5e7eb', strokeDashArray: 4 }
    };
    const chart = new ApexCharts(document.getElementById(elementId), options);
    chart.render();
    window._dashboardCharts[elementId] = chart;
};

// Barras horizontais — Top 5 Skills mais usadas
window.renderTopSkills = function (elementId, labels, valores) {
    _destruir(elementId);
    const options = {
        chart: { type: 'bar', height: 320, toolbar: { show: false }, fontFamily: 'Inter, sans-serif' },
        series: [{ name: 'Perfis', data: valores }],
        xaxis: { categories: labels },
        colors: ['#f59e0b'],
        plotOptions: { bar: { horizontal: true, borderRadius: 6, barHeight: '60%' } },
        dataLabels: { enabled: true, style: { colors: ['#fff'] } },
        grid: { borderColor: '#e5e7eb', strokeDashArray: 4 }
    };
    const chart = new ApexCharts(document.getElementById(elementId), options);
    chart.render();
    window._dashboardCharts[elementId] = chart;
};
