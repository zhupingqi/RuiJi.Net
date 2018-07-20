define(['jquery', 'utils', 'sweetAlert', 'bootstrapDialog', 'bootstrapTable', 'jsonViewer'], function ($, utils) {
    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/feed/feeds.html", false);

            tmp = $(tmp);
            //tmp.find("#tb_feeds").attr("data-url", "/api/feeds");

            $("#tab_panel_feeds").html(tmp.prop("outerHTML"));

            var $table = $('#tb_feeds').bootstrapTable({
                toolbar: '#toolbar_feeds',
                url: "/api/fp/feed/list",
                pagination: true,
                queryParams: module.queryParams,
                sidePagination: "server",
                showColumns: true,
                showRefresh: true,
                height: 530
            });

            module.initEvent();
        },
        initEvent: function () {
            var feed = utils.loadTemplate("/misc/feed/feed.html", false);
            var crawl = utils.loadTemplate("/misc/feed/crawl.html", false);

            $(document).on("click", "#add_feed", function () {
                BootstrapDialog.show({
                    title: 'Add Feed',
                    message: feed,
                    closable: false,
                    nl2br: false,
                    buttons: [{
                        label: 'Test',
                        action: function (dialog) {
                            module.test();
                        }
                    }, {
                        label: 'Download Target',
                        action: function (dialog) {
                            module.test(true);
                        }
                    }, {
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
                h.next().val(v);
            });

            $(document).on("click", "#feed_dialog ul.dropdown-menu[method] a", function () {
                var menu = $(this);
                var v = menu.attr("data-bind") ? menu.attr("data-bind") : menu.text();
                if (v == "POST") {
                    $("#feed_dialog .method-group").removeClass("hide");
                } else {
                    $("#feed_dialog .method-group").addClass("hide");
                }
            });

            $(document).on("click", "#feed_dialog ul.dropdown-menu[contentType] a", function () {
                var menu = $(this);
                var v = menu.attr("data-bind") ? menu.attr("data-bind") : menu.text();
                if (v == "application/json") {
                    $("#feed_dialog textarea[name='data']").attr("placeholder", "example:{\"name\":\"zhangsan\",\"age\":20}");
                } else {
                    $("#feed_dialog textarea[name='data']").attr("placeholder", "example:name=zhangsan&age=20");
                }
            });

            $(document).on("click", "#tb_feeds .fa-edit", function () {
                var ele = $(this);
                var id = ele.closest("tr").find("td").eq(1).text();
                var f = $(feed);
                f.find("input[name='id']").val(id);

                $.getJSON("/api/fp/feed?id=" + id, function (d) {
                    for (var p in d) {
                        var v = d[p];
                        var ele = f.find("[name='" + p + "']").eq(0);

                        ele.attr("value", v);
                        ele.text(v);

                        if (ele.is(":hidden")) {
                            ele.next().attr("value", v);
                        }

                        if (p == "method" && v == "POST") {
                            $(".method-group", f ).removeClass("hide");
                        }

                        if (p == "contentType" && v == "application/json") {
                            $("textarea[name='data']", f).attr("placeholder", "example:a=1&b=2");
                        }

                        if (p == "status" && v == "OFF") {
                            f.find("div.status :radio[value='ON']").parent().removeClass("active");
                            f.find("div.status :radio[value='OFF']").parent().addClass("active");
                        }

                        if (p == "runJs" && v == "ON") {
                            f.find("div.runjs :radio[value='OFF']").parent().removeClass("active");
                            f.find("div.runjs :radio[value='ON']").parent().addClass("active");
                        }
                    }

                    BootstrapDialog.show({
                        title: 'Edit Feed',
                        message: f.prop("outerHTML"),
                        closable: false,
                        nl2br: false,
                        buttons: [{
                            label: 'Test',
                            action: function (dialog) {
                                module.test();
                            }
                        }, {
                            label: 'Download Target',
                            action: function (dialog) {
                                module.test(true);
                            }
                        }, {
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

            $(document).on("click", "#tb_feeds .fa-eye", function () {
                var id = $(this).closest("tr").find("td").eq(1).text();
                require(["content"], function (m) {
                    m.feedResult(id);
                    $("#main_menu_tabs [href='#content']").trigger("click");
                });
            });

            $(document).on("click", "#tb_feeds .fa-history", function () {
                swal("Coming soon!", "", "warning");
            });

            $(document).on("click", "#tb_feeds .fa-random", function () {
                var ele = $(this);
                var id = ele.closest("tr").find("td").eq(1).text();
                var f = $(crawl);
                f.find("input[name='id']").val(id);

                $.getJSON("/api/fp/feed?id=" + id, function (d) {
                    f.find("#crawl_info").html("Using " + d.address + " to grab immediately,<br/>click Start to start.");

                    BootstrapDialog.show({
                        title: 'Grab Feed Now',
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

            //batch operate
            $(document).on("click", "#toolbar_feeds a[type]", function () {
                switch ($(this).attr("type")) {
                    case "enable":
                        module.batchOpConfirm("Do you confirm the enable?", "The feeds will be enable", "warning", module.enable);
                        break;
                    case "disable":
                        module.batchOpConfirm("Do you confirm the disable?", "The feeds will be disabled", "warning", module.disable);
                        break;
                    case "remove":
                        module.batchOpConfirm("Do you confirm the deletion?", "The feeds will not be restored!", "warning", module.remove);
                        break;
                }
            });

            $(document).on("click", "#feed_dialog .btn-mjdark2", function () {
                var ele = $(this);
                ele.closest(".input-group").find(":hidden").val(ele.find("input").val());
            });

            $(document).on("click", "#feeds_search_group .dropdown-menu a", function () {
                $(this).closest("ul").prev("button").html($(this).text() + "<span class=\"caret\"></span>");
            });

            $(document).on("click", "#feeds_search_group .feed-search", function () {
                $("#tb_feeds").bootstrapTable('refresh');
            });
        },
        queryParams: function (params) {
            var temp = {
                limit: params.limit,
                offset: params.offset,
                key: $.trim($("#feeds_search_group .key").val()),
                method: $.trim($("#feeds_search_group .method").text()),
                type: $.trim($("#feeds_search_group .type").text()),
                status: $.trim($("#feeds_search_group .status").text())
            };
            return temp;
        },
        batchOpConfirm: function (title, text, type, callback) {
            var selects = $("#tb_feeds").bootstrapTable('getSelections');
            if (selects.length <= 0) {
                swal("Unchecked any feeds!", "", "warning");
                return;
            }
            var ids = selects.map(function (s) { return s.id }).join(",");
            swal(
                {
                    title: title,
                    text: text,
                    type: type,
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Confirm",
                    cancelButtonText: "Cancel",
                    closeOnConfirm: false
                },
                function () {
                    callback(ids);
                }
            );
        },
        update: function (dialog) {
            var d = {};
            var validate = true;
            var msg = "need";

            $(".required:visible", "#feed_dialog").each(function (index, e) {
                e = $(e);
                var v = e.val();
                if ($.trim(e.val()) == "") {
                    validate = false;
                    msg += "\n" + e.attr("name");
                }
            });

            if (!validate) {
                swal("missing field", msg, "error");
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
                url: "/api/fp/feed/update",
                data: JSON.stringify(d),
                type: 'POST',
                contentType: "application/json",
                success: function (res) {
                    swal("success!", "The feed have been update.", "success");
                    dialog.close();
                    $('#tb_feeds').bootstrapTable("refresh");
                }
            });
        },
        test: function (down) {
            down = down || false;

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
                url: "/api/test/feed?down=" + down,
                data: JSON.stringify(d),
                type: "POST",
                contentType: "application/json",
                success: function (res) {
                    var options = {
                        collapsed: false,
                        withQuotes: true
                    };

                    $('#feed_test_result').jsonViewer(res, options);
                    if (down) {
                        swal("Download completion!", "", "success");
                        window.open("/download");
                    }
                }
            });
        },
        start: function (feedId, taskId) {

            var url = "/api/test/crawl?feedId=" + feedId;
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
        },
        enable: function (ids) {
            var url = "/api/fp/feed/status?ids=" + ids + "&status=ON";

            $.getJSON(url, function (res) {
                if (res) {
                    swal("Enable sucess!", "The feeds have been enable.", "success");
                    $('#tb_feeds').bootstrapTable("refresh");
                } else {
                    swal("Enable failed!", "A mistake in the enable feed.", "error");
                }
            });
        },
        disable: function (ids) {
            var url = "/api/fp/feed/status?ids=" + ids + "&status=OFF";

            $.getJSON(url, function (res) {
                if (res) {
                    swal("Disable sucess!", "The feeds have been disable.", "success");
                    $('#tb_feeds').bootstrapTable("refresh");
                } else {
                    swal("Disable failed!", "A mistake in the disable feed.", "error");
                }
            });
        },
        remove: function (ids) {
            var url = "/api/fp/feed/remove?ids=" + ids;

            $.getJSON(url, function (res) {
                if (res) {
                    swal("Delete sucess!", "The feeds have been deleted.", "success");
                    $('#tb_feeds').bootstrapTable("refresh");
                } else {
                    swal("Delete failed!", "A mistake in the deletion feed.", "error");
                }
            });
        }
    };

    module.init();
});