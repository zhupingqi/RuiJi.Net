define(['jquery', 'tree', 'utils', '/scripts/echarts.min.js'], function ($, tree, utils, echarts) {
    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/cluster.html", false);
            $("#tab_panel_cluster").html(tmp);

            $(document).on("click", ".node", function () {

            });

            var myChart = echarts.init(document.getElementById('cluster_cloud'));
            myChart.showLoading();

            $.getJSON('/api/zoo/cluster', function (d) {
                myChart.hideLoading();

                var data = {
                    name: 'Cloud',
                    children: [
                        {
                            name: 'crawler proxy',
                            children: [],
                            label: {
                                position: 'right',
                                verticalAlign: 'middle',
                                align: 'left'
                            }
                        },
                        {
                            name: 'extracter proxy',
                            children: [],
                            label: {
                                position: 'right',
                                verticalAlign: 'middle',
                                align: 'left'
                            }
                        }
                    ]
                };

                data.children[0].children = d.where(function (m) {
                    return m.path.indexOf("proxy") != -1 && m.data == "crawler proxy";
                }).select(function (m) {
                    var name = m.path.split("/").last();

                    return {
                        "name": name,
                        label: {
                            position: 'right',
                            verticalAlign: 'middle',
                            align: 'left',
                            fontWeight: module.fontWeight(d, name)
                        },
                        children: d.where(function (m) {
                            return m.data.indexOf("\"proxy\":\"" + name + "\"") != -1;
                        }).select(function (m) {
                            var name = m.path.split("/").last();

                            return {
                                "name": name,
                                label: {
                                    fontWeight: module.fontWeight(d, name)
                                }
                            };
                        })
                    };
                });

                data.children[1].children = d.where(function (m) {
                    return m.path.indexOf("proxy") != -1 && m.data == "extracter proxy";
                }).select(function (m) {
                    var name = m.path.split("/").last();

                    return {
                        "name": name,
                        label: {
                            position: 'right',
                            verticalAlign: 'middle',
                            align: 'left',
                            fontWeight: module.fontWeight(d, name)
                        },
                        children: d.where(function (m) {
                            return m.data.indexOf("\"proxy\":\"" + name + "\"") != -1;
                        }).select(function (m) {
                            var name = m.path.split("/").last();

                            return {
                                "name": name,
                                label: {
                                    fontWeight: module.fontWeight(d, name)
                                }
                            };
                        })
                    };
                });

                myChart.setOption(option = {
                    series: [
                        {
                            type: 'tree',

                            data: [data],

                            top: '1%',
                            left: '7%',
                            bottom: '1%',
                            right: '20%',

                            symbolSize: 7,

                            label: {
                                normal: {
                                    position: 'left',
                                    verticalAlign: 'middle',
                                    align: 'right',
                                    fontSize: 9
                                }
                            },

                            leaves: {
                                label: {
                                    normal: {
                                        position: 'right',
                                        verticalAlign: 'middle',
                                        align: 'left'
                                    }
                                }
                            },

                            expandAndCollapse: false,
                            animationDuration: 550,
                            animationDurationUpdate: 750
                        }
                    ]
                });

                myChart.on('click', function (params) {
                    // 控制台打印数据的名称
                    if (params.name.indexOf(":") != -1)
                        window.location = "http://" + params.name;
                });
            });
        },
        fontWeight: function (data, name) {
            var node = data.where(function (m) {
                if (m.path.indexOf("/live_nodes/") != -1 && m.path.indexOf(name) != -1)
                    return m;
            });

            return node.length > 0 ? "bold" : "normal";
        }
    };

    module.init();
});