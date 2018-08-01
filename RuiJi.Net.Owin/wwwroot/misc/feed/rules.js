define(['jquery', 'utils', 'sweetAlert', 'bootstrapDialog', 'bootstrapTable', 'jsonViewer'], function ($, utils) {
    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/feed/rules.html", false);

            tmp = $(tmp);
            //tmp.find("#tb_rules").attr("data-url", "/api/rules");

            $("#tab_panel_rules").html(tmp.prop("outerHTML"));

            var $table = $('#tb_rules').bootstrapTable({
                toolbar: '#toolbar_rules',
                url: "/api/fp/rule/list",
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
            var ruleTmp = utils.loadTemplate("/misc/feed/rule.html", false);

            //add dialog open
            $(document).on("click", "#add_rule", function () {
                BootstrapDialog.show({
                    title: 'Add Rule',
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

            //dialog dropdown-menu item click
            $(document).on("click", "#rule_dialog ul.dropdown-menu a", function () {
                var menu = $(this);
                var h = menu.closest(".input-group").find(":hidden");
                var v = menu.attr("data-bind") ? menu.attr("data-bind") : menu.text();

                h.val(v);
                h.next().val(menu.text());
            });

            //modify dialog 
            $(document).on("click", "#tb_rules .fa-edit", function () {
                var ele = $(this);
                var id = ele.closest("tr").find("td").eq(1).text();
                var f = $(ruleTmp);
                f.find("input[name='id']").val(id);

                $.getJSON("/api/fp/rule?id=" + id, function (d) {
                    for (var p in d) {
                        var v = d[p];
                        var ele = f.find("[name='" + p + "']").eq(0);

                        ele.attr("value", v);
                        ele.text(v);

                        if (ele.is(":hidden")) {
                            ele.next().attr("value", v);
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
                        title: 'Edit Rule',
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

            $(document).on("click", "#tb_rules .fa-history", function () {
                swal("Coming soon!", "The module is going online!", "warning");
            });

            //batch operate
            $(document).on("click", "#toolbar_rules a[type]", function () {
                switch ($(this).attr("type")) {
                    case "enable":
                        module.batchOpConfirm("Do you confirm the enable?", "The rules will be enable", "warning", module.enable);
                        break;
                    case "disable":
                        module.batchOpConfirm("Do you confirm the disable?", "The rules will be disabled", "warning", module.disable);
                        break;
                    case "remove":
                        module.batchOpConfirm("Do you confirm the deletion?", "The rules will not be restored!", "warning", module.remove);
                        break;
                }
            });

            //change type|status
            $(document).on("click", "#rule_dialog .btn-mjdark2", function () {
                var ele = $(this);
                ele.closest(".input-group").find(":hidden").val(ele.find("input").val());
            });

            //rules search dropdown-menu change
            $(document).on("click", "#rules_search_group .dropdown-menu a", function () {
                $(this).closest("ul").prev("button").html($(this).text() + "<span class=\"caret\"></span>");
            });

            //rules search
            $(document).on("click", "#rules_search_group .rules-search", function () {
                $("#tb_rules").bootstrapTable('refresh');
            });
        },
        batchOpConfirm: function (title, text, type, callback) {
            var selects = $("#tb_rules").bootstrapTable('getSelections');
            if (selects.length <= 0) {
                swal("Unchecked any rules!", "", "warning");
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
        queryParams: function (params) {
            var temp = {
                limit: params.limit,
                offset: params.offset,
                key: $.trim($("#rules_search_group .key").val()),
                type: $.trim($("#rules_search_group .type").text()),
                status: $.trim($("#rules_search_group .status").text())
            };
            return temp;
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
                swal("missing field", msg, "error");
                return;
            }

            $("input[name]:not(:disabled),textarea", "#rule_dialog").each(function (index, e) {
                e = $(e);
                var v = e.val();
                if ($.trim(v) == "")
                    return true;

                if (v == "true")
                    d[e.attr("name")] = true;
                else if (v == "false")
                    d[e.attr("name")] = false;
                else
                    d[e.attr("name")] = v;
            });

            $.ajax({
                url: "/api/fp/rule/update",
                data: JSON.stringify(d),
                type: 'POST',
                contentType: "application/json",
                success: function (res) {
                    swal("success!", "", "success");
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
                if ($.trim(v) == "")
                    return true;

                if (v == "true")
                    d[e.attr("name")] = true;
                else if (v == "false")
                    d[e.attr("name")] = false;
                else
                    d[e.attr("name")] = v;
            });

            $.ajax({
                url: "/api/test/rule",
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
        enable: function (ids) {
            var url = "/api/fp/rule/status?ids=" + ids + "&status=ON";

            $.getJSON(url, function (res) {
                if (res) {
                    swal("Enable sucess!", "The rules have been enable.", "success");
                    $('#tb_rules').bootstrapTable("refresh");
                } else {
                    swal("Enable failed!", "A mistake in the enable rule.", "error");
                }
            });
        },
        disable: function (ids) {
            var url = "/api/fp/rule/status?ids=" + ids + "&status=OFF";

            $.getJSON(url, function (res) {
                if (res) {
                    swal("Disable sucess!", "The rules have been disable.", "success");
                    $('#tb_rules').bootstrapTable("refresh");
                } else {
                    swal("Disable failed!", "A mistake in the disable rule.", "error");
                }
            });
        },
        remove: function (ids) {
            var url = "/api/fp/rule/remove?ids=" + ids;

            $.getJSON(url, function (res) {
                if (res) {
                    swal("Delete sucess!", "The rules have been deleted.", "success");
                    $('#tb_rules').bootstrapTable("refresh");
                } else {
                    swal("Delete failed!", "A mistake in the deletion rule.", "error");
                }
            });
        }
    };

    module.init();
});