define(['jquery', 'tree', 'utils'], function ($, tree, utils) {
    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/node.html", false);
            $("#tab_panel_node").html(tmp);
            $(".setting").hide();

            $(document).on("click", "#save_ips", function () {
                var ips = $("#tab_panel_node :checked").map(function () {
                    return $(this).val()
                }).get();

                var crawler = $("#active_node_crawler").text();
                var url = "/api/crawler/ips?baseUrl=" + crawler;

                $.ajax({
                    url: url,
                    data: JSON.stringify(ips),
                    type: 'POST',
                    contentType: "application/json",
                    success: function (res) {
                        alert("完成");
                    }
                });
            });

            $(document).on("click", "#save_feed", function () {
                var pages = $("#node_pages").val();
                var feed = $("#active_node_feed").text();

                var url = "/api/feed/set?baseUrl=" + feed;

                $.ajax({
                    url: url,
                    data: JSON.stringify(pages),
                    type: 'POST',
                    contentType: "application/json",
                    success: function (res) {
                        alert("完成");
                    }
                });
            });

            $('#tree')
                .on('changed.jstree', function (e, data) {
                    $(".setting").hide();
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
                            $("#crawler_node_set").show();
                            var crawler = path.replace("/config/crawler/", "");
                            var url = "/api/crawler/ips?baseUrl=" + crawler;

                            $("#active_node_crawler").text(crawler);

                            $.getJSON(url, function (cips) {
                                var ips = $.parseJSON(d.data).ips;

                                $.map(cips, function (ip) {
                                    var input = $("<div><input type='checkbox' value='" + ip + "' />" + ip + "</div>");
                                    if ($.inArray(ip, ips) != -1) {
                                        input.find("input").attr("checked", "checked");
                                    }

                                    $("#ips_set").append(input);
                                })
                            });
                        }

                        if (path.startWith("/config/feed/")) {
                            $("#feed_node_set").show();
                            var feed = path.replace("/config/feed/", "");
                            var url = "/api/feed/get?baseUrl=" + feed;

                            $("#active_node_feed").text(feed);

                            $.getJSON(url, function (pages) {
                                $("#node_pages").val(pages);
                            });
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