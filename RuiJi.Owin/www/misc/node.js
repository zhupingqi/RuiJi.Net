define(['jquery', 'tree', 'utils'], function ($, tree, utils) {
    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/node.html", false);
            $("#tab_panel_node").html(tmp);
            $("#node_set").hide();

            $('#tree')
                .on('changed.jstree', function (e, data) {
                    $("#node_set").hide();
                    $("#ips_set").html("");

                    var path = data.node.id;

                    $.getJSON("/api/zoo/node?path=" + path, function (d) {
                        $('#node_stat').html("<table></table>");
                        for (var m in d.stat) {
                            $('#node_stat table').append("<tr><td width='150px'>" + m + "</td><td>" + d.stat[m] + "</td></tr>");
                        }
                        if (d.data == "")
                            d.data = "no data";

                        $('#node_data').html(d.data);

                        if (path.startWith("/config/crawler/")) {
                            $("#node_set").show();
                            var crawler = path.replace("/config/crawler/", "");
                            var url = "http://" + crawler + "/api/crawler/info";

                            $.getJSON(url, function (info) {
                                var ips = $.parseJSON(d.data).ips;

                                $.map(info.ips, function (ip) {
                                    var input = $("<div><input type='checkbox' value='" + ip + "' />" + ip + "</div>");
                                    if ($.inArray(ip, ips) != -1) {
                                        input.find("input").attr("checked", "checked");
                                    }

                                    $("#ips_set").append(input);
                                })
                            });
                        }
                    })

                })
                .jstree({
                    'core': {
                        'data': {
                            'url': '/api/zoo/tree',
                            'data': function (node) {
                                var path = "/";
                                if (node.id != "#")
                                    path = node.id;

                                return { 'path': path };
                            }
                        }
                    }
                });
        }
    };

    module.init();
});