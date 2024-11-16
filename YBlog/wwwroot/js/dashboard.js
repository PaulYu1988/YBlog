$.ajax({
    type: 'GET',
    url: '/manage/home/statistics',
    dataType: 'json',
    success: function (result) {
        renderChart1(result.articleStatistics);
        renderChart2(result.dailyStatistics);
    },
    error: function (XMLHttpRequest, textStatus, errorThrown) {
        $('#chart1').html('获取文章数量统计数据失败');
        $('#chart2').html('获取阅读评论统计数据失败');
    }
});

var chart1Dom = document.getElementById('chart1');
var myChart1 = echarts.init(chart1Dom);
function renderChart1(items) {
    var chart1Data = [];
    items.forEach(function (item) {
        chart1Data.push({
            value: item.articleCount,
            name: item.categoryName
        });
    });
    var option1 = {
        title: {
            text: '文章数量统计',
            subtext: '',
            left: 'left'
        },
        tooltip: {
            trigger: 'item'
        },
        legend: {
            orient: 'vertical',
            left: 'right'
        },
        series: [
            {
                name: '文章数量',
                type: 'pie',
                center: ['40%', '60%'],
                radius: '50%',
                data: chart1Data,
                emphasis: {
                    itemStyle: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                }
            }
        ]
    };
    myChart1.setOption(option1);
}

var chart2Dom = document.getElementById('chart2');
var myChart2 = echarts.init(chart2Dom);
function renderChart2(items) {
    var dates = [];
    var readCounts = [];
    var commentCounts = [];
    items.forEach(function (item) {
        dates.push(item.date);
        readCounts.push(item.readCount);
        commentCounts.push(item.commentCount);
    });
    var option2 = {
        title: {
            text: '阅读/评论统计'
        },
        xAxis: {
            type: 'category',
            data: dates
        },
        legend: {
            data: ['阅读', '评论'],
            left: 'right'
        },
        tooltip: {
            trigger: 'axis'
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        yAxis: {
            type: 'value'
        },
        series: [
            {
                name: '阅读',
                data: readCounts,
                type: 'line'
            },
            {
                name: '评论',
                data: commentCounts,
                type: 'line'
            }
        ]
    };
    myChart2.setOption(option2);
}

window.addEventListener('resize', function () {
    myChart1.resize();
    myChart2.resize();
});

$('.show-log').on('click', function () {
    var layer = layui.layer;
    layer.alert($(this).html(), {
        title: '详情'
    });
});