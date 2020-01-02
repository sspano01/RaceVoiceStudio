var Selectors = {
    Speed: function(val) { return val.s; },
    Time: function(val) { return val.ti; },
    Distance: function(val) { return val.ld; },
    Throttle: function(val) { return val.tp; },
    Rpm: function(val) { return val.rpm; },
    BrakesFront: function(val) { return val.bf; },
    BrakesRear: function(val) { return val.br; },
    Brakes: function(val) { return (val.br + val.bf) / 2; },
}

var RawData = 0;

var LineCharts = {};
var BoxCharts = {};


function buildBoxChartData(data, xSelector, ySelector) {
    var min = [];
    var max = [];
    var labels = [];

    var longest = 0;
    var longestIdx = 0;
    for (var i = 0; i < data.length; i++) {
        if (data[i].length > longest) {
            longest = data[i].length;
            longestIdx = i;
        }
    }

    for (var n = 0; n < longest; n++) {
        var cMin = Number.MAX_VALUE;
        var cMax = Number.MIN_VALUE;

        var xVal = xSelector(data[longestIdx][n]);
        labels.push(xVal);

        for (var i = 0; i < data.length; i++) {
            var values = data[i];
            var yVal = 0;

            if (n < values.length) {
                yVal = ySelector(values[n]);
                
                if (yVal < cMin) {
                    cMin = yVal;
                }

                if (yVal > cMax) {
                    cMax = yVal;
                }
            }
        }

        min.push(cMin);
        max.push(cMax-cMin);
    }
    
    return { Labels: labels, Min: min, Max: max };
}

function buildMinMaxLapData(data, xSelector, ySelector) {
    var min = [];
    var max = [];
    var laps = []
    var labels = [];

    var longest = 0;
    var longestIdx = 0;
    for (var i = 0; i < data.length; i++) {
        if (data[i].length > longest) {
            longest = data[i].length;
            longestIdx = i;
        }
        laps.push([]);
    }

    for (var n = 0; n < longest; n++) {
        var cMin = { x: 0, y: Number.MAX_VALUE };
        var cMax = { x: 0, y: Number.MIN_VALUE };
        labels.push('');

        var xVal = xSelector(data[longestIdx][n]);

        for (var i = 0; i < data.length; i++) {
            var values = data[i];
            var yVal = 0;

            if (n < values.length) {
                yVal = ySelector(values[n]);
                
                if (yVal < cMin.y) {
                    cMin.x = xVal;
                    cMin.y = yVal;
                }

                if (yVal > cMax.y) {
                    cMax.x = xVal;
                    cMax.y = yVal;
                }
            }

            laps[i].push({ x: xVal, y: yVal });
        }

        min.push(cMin);
        max.push(cMax);
    }
    
    return { Labels: labels, Min: min, Max: max, Laps: laps };
}



var colors = ['rgba(237,28,36,1)', 'rgba(0,128,0,1)', 'rgba(0,0,255,1)', 'rgba(255,0,128,1)', 'rgba(255,128,0,1)'];
function makeDataset(label, color, lap) { 
    return {
        label: label,
        data: lap,
        fill: false,
        borderColor: color,
        borderWidth: 1
    }
}

function renderLineChart(data, ctx, xSelector, ySelector) {
    var parsed = buildMinMaxLapData(data, xSelector, ySelector);
    var datasets = [];
    var checkboxes = $('.checkcontainer > input:checked');
    for (var i = 0; i < parsed.Laps.length; i++) {
        datasets.push(makeDataset("Lap " + checkboxes[i].value, colors[i%colors.length], parsed.Laps[i]));
    }
    console.log(datasets);
    
    if (data.length > 1) {
        var minSet = makeDataset("Min", 'rgba(128,128,128,1)', parsed.Min);
        minSet.fill = '+1';
        datasets.push(minSet);
        var maxSet = makeDataset("Max", 'rgba(128,128,128,1)', parsed.Max);
        datasets.push(maxSet);
    }

    if (LineCharts[ctx]) {
        LineCharts[ctx].destroy();
    }

    var chart = new Chart($(ctx), {
        type: 'line',
        data: { 
            labels: parsed.Labels,
            datasets: datasets
        },
        options: {
            showLines: true,
            responsive: true,
            maintainAspectRatio: false,
            animation: {
                duration: 0, // general animation time
            },
            hover: {
                animationDuration: 0, // duration of animations when hovering an item
            },
            tooltips: {
                intersect: false
            },
            responsiveAnimationDuration: 0, // animation duration after a resize

            scales: {
                xAxes: [{
                    gridLines: {
                        display:true 
                    },
                    ticks: {
                        callback: function(label, index, labels) {
                            return secsToTime(label);
                        }
                    },
                    type: 'linear',
                }],
                yAxes: [{
                    //type: 'linear'
                }]
            },
        }
    });

    LineCharts[ctx] = chart;
}

function renderBoxChart(data, label, ctx, xSelector, ySelector) {
    var parsed = buildBoxChartData(data, xSelector, ySelector);

    if (BoxCharts[ctx]) {
        BoxCharts[ctx].dispose();
    }

    var chart = echarts.init(document.getElementById(ctx));
    BoxCharts[ctx] = chart;

    // specify chart configuration item and data
    var option = {
        title: {
            text: label 
        },
        tooltip: {
            show: false
        },
        animation: false,
        grid: {
            left: '4%',
            right: '4%',
            bottom: '0%',
            containLabel: true
        },
        xAxis: {
            type: 'category',
            splitLine: { show: false },
            data: parsed.Labels,
            axisLabel: {
                formatter: function (value, index) {
                    return secsToTime(value);
                }
            }
        },
        yAxis: {
            type: 'value'
        },
        series: [{
            name: 'Min',
            type: 'bar',
            data: parsed.Min,
            stack: 'a',
            itemStyle: {
                normal: {
                    barBorderColor: 'rgba(0,0,0,0)',
                    color: 'rgba(0,0,0,0)'
                },
                emphasis: {
                    barBorderColor: 'rgba(0,0,0,0)',
                    color: 'rgba(0,0,0,0)'
                }
            },
        },{
            name: 'Max',
            type: 'bar',
            data: parsed.Max,
            stack: 'a',
            label: {
                normal: {
                    show: false,
                    position: 'inside'
                }
            }
        }]
    };

    // use configuration item and data specified to show chart
    chart.setOption(option);
}

function onDataDownloaded(data) {
    Chart.defaults.global.elements.point.radius = 0;
    RawData = data;

    for (var i = 0; i < data.length; i++) {
        var $d = $('<div>').attr('class', 'checkcontainer');
        $d.text("Lap " + (i+1).toString());
        var $chk = $('<input type="checkbox" checked>').val(i+1);
        $chk.click(onLapToggleClick)
        $d.prepend($chk);
        $('#laptoggles').append($d);
    }

    reloadAllCharts(data);

    if (data.HideSpeedCharts) {
        $('#speed-charts').hide();
    }

    if (data.HideRpmCharts) {
        $('#rpm-charts').hide();
    }

    if (data.HideThrottle) {
        $('#throttle-charts').hide();
    }

    if (data.length === 1) {
        $('#rpmBoxChart').hide();
        $('#speedBoxChart').hide();
        $('#throttleBoxChart').hide();
    }
}

function onLapToggleClick() {
    var checkboxes = $('.checkcontainer > input');
    var data = []
    for (var i = 0; i < RawData.length; i++) {
       if (checkboxes[i].checked) {
           data.push(RawData[i]);
       } 
    }

    reloadAllCharts(data);
}

function selectAllLaps() {
    var checkboxes = $('.checkcontainer > input');
    for (var i = 0; i < RawData.length; i++) {
        checkboxes[i].checked = true;
    }

    onLapToggleClick();
}

function selectNoLaps() {
    var checkboxes = $('.checkcontainer > input');
    for (var i = 0; i < RawData.length; i++) {
        checkboxes[i].checked = false;
    }

    onLapToggleClick();
}

function reloadAllCharts(data) {
    if (data.length == 0) {
        return;
    }
    renderLineChart(data, '#speedChart', Selectors.Time, Selectors.Speed);
    renderBoxChart(data, 'Speed (MPH) / Time (s)', 'speedBoxChart', Selectors.Time, Selectors.Speed);

    renderLineChart(data, '#rpmChart', Selectors.Time, Selectors.Rpm);
    renderBoxChart(data, 'RPM / Time (s)', 'rpmBoxChart', Selectors.Time, Selectors.Rpm);

    renderLineChart(data, '#throttleChart', Selectors.Time, Selectors.Throttle);
    renderBoxChart(data, 'Throttle Position / Time (s)', 'throttleBoxChart', Selectors.Time, Selectors.Throttle);
}