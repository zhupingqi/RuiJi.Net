define(['jquery', 'utils', 'sweetAlert', 'bootstrapDialog', 'bootstrapTable', 'jsonViewer'], function ($, utils) {
    var proxyUrl = "";

    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/feed/rules.html", false);
            var ruleTmp = utils.loadTemplate("/misc/feed/rule.html", false);

            //#region event
            $(document).on("click", "#add_rule", function () {
                BootstrapDialog.show({
                    title: '添加 Rule',
                    message: ruleTmp,
                    closable: false,
                    nl2br: false,
                    buttons: [{
                        label: 'Test',
                        action: function (dialog) {
                            module.test();
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

            $(document).on("click", "#rule_dialog ul.dropdown-menu a", function () {
                var menu = $(this);
                var h = menu.closest(".input-group").find(":hidden");
                var v = menu.attr("data-bind") ? menu.attr("data-bind") : menu.text();

                h.val(v);
                h.next().val(menu.text());
            });

            $(document).on("click", "#tb_rules .fa-edit", function () {
                var ele = $(this);
                var id = ele.closest("tr").find("td").eq(1).text();
                var f = $(ruleTmp);
                f.find("input[name='id']").val(id);

                $.getJSON("http://" + proxyUrl + "/api/rule?id=" + id, function (d) {
                    for (var p in d) {
                        var v = d[p];
                        var ele = f.find("[name='" + p + "']").eq(0);

                        ele.attr("value", v);
                        ele.text(v);

                        if (ele.is(":hidden")) {
                            ele.next().attr("value", v);
                        }
                    }

                    BootstrapDialog.show({
                        title: '修改 Rule',
                        message: f.prop("outerHTML"),
                        closable: false,
                        nl2br: false,
                        buttons: [{
                            label: 'Test',
                            action: function (dialog) {
                                module.test();
                            }
                        }, {
                            label: 'Ok',
                            action: function (dialog) {
                                module.update();
                                dialog.close();
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

            $(document).on("click", "#rule_dialog ul.nav li", function () {
                var ele = $(this);
                ele.addClass("active").siblings().removeClass("active");

                ele.closest("div").find("textarea").eq(ele.index()).show().siblings("textarea").hide();
            });

            $(document).on("click", "#toolbar_rules a[remove]", function () {
                swal({
                    title: "确定删除吗？",
                    text: "你将无法恢复该规则！",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "确定删除！",
                    cancelButtonText: "取消删除！",
                    closeOnConfirm: false
                },
                    function () {
                        var ids = $("#rules_list td :checked").parent().next().map(function () { return $(this).text(); }).get().join(",");
                        if(ids != "")
                            module.remove(ids);
                        else
                            swal("错误！", "未选择任何规则。", "success");
                    });
            });
            //#endregion

            tmp = $(tmp);
            tmp.find("#tb_rules").attr("data-url", "http://" + proxyUrl + "/api/rules");

            $("#tab_panel_rules").html(tmp.prop("outerHTML"));

            var $table = $('#tb_rules').bootstrapTable({
                toolbar: '#toolbar_rules',
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
                        $('#tb_rules > tbody > tr').map(function (i, m) {
                            $(m).find("td:last").html("<i class='fa fa-edit'></i><i class='fa fa-history'></i>");
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

            $("input.required", "#rule_dialog").each(function (index, e) {
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

            $("input[name]:not(:disabled),textarea", "#rule_dialog").each(function (index, e) {
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
                url: "http://" + proxyUrl + "/api/rule/update",
                data: JSON.stringify(d),
                type: 'POST',
                contentType: "application/json",
                success: function (res) {
                    swal("完成");
                    dialog.close();
                    $('#tb_rules').bootstrapTable("refresh");
                }
            });
        },
        test: function () {
            var d = {};
            var validate = true;
            var msg = "need";

            $("input.required", "#rule_dialog").each(function (index, e) {
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

            $("input[name]:not(:disabled),textarea", "#rule_dialog").each(function (index, e) {
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
                url: "/api/rule/test",
                data: JSON.stringify(d),
                type: "POST",
                contentType: "application/json",
                success: function (res) {
                    var options = {
                        collapsed: false,
                        withQuotes: true
                    };

                    $('#rule_test_result').jsonViewer(res, options);
                }
            });
        },
        remove: function (ids) {
            var url = "http://" + proxyUrl + "/api/rule/remove?ids=" + ids;

            $.getJSON(url, function (res) {
                if (res) {
                    swal("删除！", "规则已经被删除。", "success");
                    $('#tb_rules').bootstrapTable("refresh");
                } else {
                    swal("删除失败！", "删除规则发生错误。", "success");
                }
            });
        }
    };

    module.getProxy(function () {
        module.init();
    });
});