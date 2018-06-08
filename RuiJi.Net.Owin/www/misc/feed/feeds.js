define(['jquery', 'utils', 'sweetAlert', 'bootstrapDialog', 'bootstrapTable','jsonViewer'], function ($, utils) {
    var proxyUrl = "";

    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/feed/feeds.html", false);
            var feed = utils.loadTemplate("/misc/feed/feed.html", false);
            var crawl = utils.loadTemplate("/misc/feed/crawl.html", false);

            $(document).on("click", "#add_feed", function () {
                BootstrapDialog.show({
                    title: '添加Feed',
                    message: feed,
                    closable: false,
                    nl2br: false,
                    buttons: [{
                        label: 'Test',
                        action: function (dialog) {
                            module.test();
                        }
                    },{
                        label: 'Ok',
                        action: function (dialog) {
                            module.update(dialog);
                        }
                    }, {
                        label: 'Close',
                        action: function (dialog) {
                            dialog.close();
                        }
                    }]
                });
            });

            $(document).on("click", "#feed_dialog ul.dropdown-menu a", function () {
                var menu = $(this);
                var h = menu.closest(".input-group").find(":hidden");
                var v = menu.attr("data-bind") ? menu.attr("data-bind") : menu.text();

                h.val(v);
                h.next().val(menu.text());
            });

            $(document).on("click", "#tb_feeds .fa-edit", function () {
                var ele = $(this);
                var id = ele.closest("tr").find("td").eq(1).text();
                var f = $(feed);
                f.find("input[name='id']").val(id);

                $.getJSON("http://" + proxyUrl + "/api/feed?id=" + id, function (d) {
                    for (var p in d) {
                        var v = d[p];
                        var ele = f.find("[name='" + p + "']").eq(0);

                        ele.attr("value", v);
                        ele.text(v);

                        if (ele.is(":hidden")) {
                            ele.next().attr("value", v);
                        }

                        if (p == "status" && v == "OFF") {
                            f.find(":radio[value='ON']").parent().removeClass("active");
                            f.find(":radio[value='OFF']").parent().addClass("active");
                        }
                    }

                    BootstrapDialog.show({
                        title: '修改 Feed',
                        message: f.prop("outerHTML"),
                        closable: false,
                        nl2br: false,
                        buttons: [{
                            label: 'Test',
                            action: function (dialog) {
                                module.test();
                            }
                        },{
                            label: 'Ok',
                            action: function (dialog) {
                                module.update(dialog);
                            }
                        }, {
                            label: 'Close',
                            action: function (dialog) {
                                dialog.close();
                            }
                        }]
                    });
                });
            });

            $(document).on("click", "#tb_feeds .fa-random", function () {
                var ele = $(this);
                var id = ele.closest("tr").find("td").eq(1).text();
                var f = $(crawl);
                f.find("input[name='id']").val(id);

                $.getJSON("http://" + proxyUrl + "/api/feed?id=" + id, function (d) {
                    f.find("#crawl_info").html("正在使用 " + d.address + " <br/>进行即时抓取，点击Start开始");

                    BootstrapDialog.show({
                        title: '即时抓取 Feed',
                        message: f.prop("outerHTML"),
                        closable: false,
                        nl2br: false,
                        buttons: [{
                            label: 'Start',
                            action: function (dialog) {
                                module.start(id);
                            }
                        }, {
                            label: 'Close',
                            action: function (dialog) {
                                dialog.close();
                            }
                        }]
                    });
                });
            });

            $(document).on("click", "#feed_dialog ul.nav li", function () {
                var ele = $(this);
                ele.addClass("active").siblings().removeClass("active");

                ele.closest("div").find("textarea").eq(ele.index()).show().siblings("textarea").hide();
            });

            $(document).on("click", "#feed_dialog .btn-mjdark2", function () {
                var ele = $(this);
                ele.closest(".input-group").find("input").val(ele.find("input").val());
            });

            tmp = $(tmp);
            tmp.find("#tb_feeds").attr("data-url", "http://" + proxyUrl + "/api/feeds");

            $("#tab_panel_feeds").html(tmp.prop("outerHTML"));

            var $table = $('#tb_feeds').bootstrapTable({
                toolbar: '#toolbar_feeds',
                striped: true,
                cache: false,
                pagination: true,
                sortable: false,
                sortOrder: "asc",
                queryParams: module.queryParams,
                sidePagination: "server",
                pageNumber: 1,
                pageSize: 10,
                pageList: [10, 25, 50, 100],
                showColumns: true,
                showRefresh: true,
                minimumCountColumns: 2,
                clickToSelect: true,
                height: 500,
                uniqueId: "ID",
                cardView: false,
                detailView: false,
                onPostBody: function (e) {
                    if (e.length > 0) {
                        $('#tb_feeds > tbody > tr').map(function (i, m) {
                            $(m).find("td:last").html("<i class='fa fa-edit'></i><i class='fa fa-eye'></i><i class='fa fa-history'></i><i class='fa fa-random'></i>");
                        });
                    }
                }
            });
        },
        queryParams: function (params) {
            var temp = {
                limit: params.limit,
                offset: params.offset,
                departmentname: $("#txt_search_departmentname").val(),
                statu: $("#txt_search_statu").val()
            };
            return temp;
        },
        getProxy: function (fn) {
            $.getJSON("/api/zoo/feedproxy", function (url) {
                proxyUrl = url;

                fn();
            });
        },
        update: function (dialog) {
            var d = {};
            var validate = true;
            var msg = "need";

            $("input.required", "#feed_dialog").each(function (index, e) {
                e = $(e);
                var v = e.val();
                if ($.trim(e.val()) == "") {
                    validate = false;
                    msg += "\n" + e.attr("name");
                }
            });

            if (!validate) {
                swal(msg);
                return;
            }

            $("input[name]:not(:disabled),textarea", "#feed_dialog").each(function (index, e) {
                e = $(e);
                var v = e.val();
                if (v == "true")
                    d[e.attr("name")] = true;
                else if (v == "false")
                    d[e.attr("name")] = false;
                else
                    d[e.attr("name")] = v;
            });

            $.ajax({
                url: "http://" + proxyUrl + "/api/feed/update",
                data: JSON.stringify(d),
                type: 'POST',
                contentType: "application/json",
                success: function (res) {
                    swal("完成");
                    dialog.close();
                    $('#tb_feeds').bootstrapTable("refresh");
                }
            });
        },
        test: function () {
            var d = {};
            var validate = true;
            var msg = "need";

            $("input.required", "#feed_dialog").each(function (index, e) {
                e = $(e);
                var v = e.val();
                if ($.trim(e.val()) == "") {
                    validate = false;
                    msg += "\n" + e.attr("name");
                }
            });

            if (!validate) {
                swal(msg);
                return;
            }

            $("input[name]:not(:disabled),textarea", "#feed_dialog").each(function (index, e) {
                e = $(e);
                var v = e.val();
                if (v == "true")
                    d[e.attr("name")] = true;
                else if (v == "false")
                    d[e.attr("name")] = false;
                else
                    d[e.attr("name")] = v;
            });

            $.ajax({
                url: "/api/feed/test",
                data: JSON.stringify(d),
                type: "POST",
                contentType: "application/json",
                success: function (res) {
                    var options = {
                        collapsed: false,
                        withQuotes: true
                    };

                    $('#feed_test_result').jsonViewer(res, options);
                }
            });
        },
        start: function (feedId, taskId) {

            var url = "http://" + proxyUrl + "/api/feed/crawl?feedId=" + feedId;
            if (taskId && taskId > 0) {
                url += "&taskId=" + taskId;
            }

            $.getJSON(url, function (d) {
                $("#crawl_msg").text(d.state);

                if (!d.completed) {
                    module.start(feedId, d.taskId);
                } else {
                    var options = {
                        collapsed: false,
                        withQuotes: true
                    };
                    $('#crawl_result').jsonViewer(d.result, options);
                }
            });
        }
    };

    module.getProxy(function () {
        module.init();
    });
});