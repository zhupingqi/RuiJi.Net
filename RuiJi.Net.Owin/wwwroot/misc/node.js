define(['jquery', 'tree', 'utils', 'sweetAlert'], function ($, tree, utils) {
    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/node.html", false);
            $("#tab_panel_node").html(tmp);
            $(".setting", "#tab_panel_node").hide();

            $('#tree').on('changed.jstree', function (e, data) {
                $(".setting", "#tab_panel_node").hide();
                $("#ips_set").html("");

                var path = data.node.id;

                $.getJSON("/api/zk/data?path=" + path, function (d) {
                    $('#node_stat').html("<table></table>");
                    for (var m in d.stat) {
                        $('#node_stat table').append("<tr><td width='150px'>" + m + "</td><td>" + d.stat[m] + "</td></tr>");
                    }
                    if (d.data == "")
                        d.data = "no data";

                    $('#node_data').html(d.data);

                    if (path.startWith("/config/crawler/")) {
                        $("#crawler_node").show();
                        var crawler = path.replace("/config/crawler/", "");

                        $("#active_node_crawler").text(crawler);
                        var ips = $.parseJSON(d.data).ips;
                        if (ips && ips.length > 0) {
                            $("#ips_set").append("<div>the IPs that current node can use:</div>");
                        }
                        $.map(ips, function (ip) {
                            $("#ips_set").append("<div>" + ip + "</div>");
                        });
                    }

                    if (path.startWith("/config/feed/")) {
                        $("#feed_node").show();
                        var feed = path.replace("/config/feed/", "");
                        $("#active_node_feed").text(feed);

                        $("#node_pages").text($.parseJSON(d.data).pages.join(","));

                    }
                })

            })
                .jstree({
                    'core': {
                        'data': {
                            'url': '/api/zk/tree',
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