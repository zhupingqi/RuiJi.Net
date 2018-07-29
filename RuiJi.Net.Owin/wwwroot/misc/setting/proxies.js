define(['jquery', 'utils', 'sweetAlert', 'bootstrapDialog', 'bootstrapTable'], function ($, utils) {
    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/setting/proxies.html", false);

            tmp = $(tmp);
            tmp.find("#tb_proxies").attr("data-url", "/api/setting/proxy/list");

            $("#tab_panel_proxies").html(tmp.prop("outerHTML"));

            var $table = $('#tb_proxies').bootstrapTable({
                toolbar: '#toolbar_proxy',
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
            var tmp = utils.loadTemplate("/misc/setting/proxy.html", false);

            $(document).on("click", "#add_proxy", function () {
                BootstrapDialog.show({
                    title: 'Add Func',
                    message: tmp,
                    closable: false,
                    nl2br: false,
                    buttons: [{
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

            $(document).on("click", "#proxy_dialog ul.dropdown-menu a", function () {
                var menu = $(this);
                var h = menu.closest(".input-group").find(":hidden");
                var v = menu.attr("data-bind") ? menu.attr("data-bind") : menu.text();

                h.val(v);
                h.next().val(menu.text());
            });

            $(document).on("click", "#tb_proxies .fa-edit", function () {
                var ele = $(this);
                var id = ele.closest("tr").find("td").eq(1).text();
                var f = $(tmp);
                f.find("input[name='id']").val(id);

                $.getJSON("/api/setting/proxy?id=" + id, function (d) {
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
                        title: 'Edit Proxy',
                        message: f.prop("outerHTML"),
                        closable: false,
                        nl2br: false,
                        buttons: [{
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

            $(document).on("click", "#tb_proxies .fa-terminal", function () {
                var ele = $(this);
                var id = ele.closest("tr").find("td").eq(1).text();
                ele.next().html("<img src='/Content/images/loading.0.gif' />");
                $.getJSON("/api/test/proxy?id=" + id, function (d) {
                    if (d.code == 200)
                        ele.next().html("&nbsp;&nbsp;" + d.elspsed + " ms");
                    else {
                        ele.next().html("&nbsp;&nbsp;" + d.code + " " + d.msg);
                    }
                });
            });

            //batch operate
            $(document).on("click", "#toolbar_proxy a[type]", function () {
                switch ($(this).attr("type")) {
                    case "enable":
                        module.batchOpConfirm("Do you confirm the enable?", "The proxies will be enable", "warning", module.enable);
                        break;
                    case "disable":
                        module.batchOpConfirm("Do you confirm the disable?", "The proxies will be disabled", "warning", module.disable);
                        break;
                    case "remove":
                        module.batchOpConfirm("Do you confirm the deletion?", "The proxies will not be restored!", "warning", module.remove);
                        break;
                }
            });

            $(document).on("click", "#proxy_dialog .btn-mjdark2", function () {
                var ele = $(this);
                ele.closest(".input-group").find("input").val(ele.find("input").val());
            });
        },
        queryParams: function (params) {
            var temp = {
                limit: params.limit,
                offset: params.offset
            };
            return temp;
        },
        update: function (dialog) {
            var d = {};
            var validate = true;
            var msg = "need";

            $("input.required", "#proxy_dialog").each(function (index, e) {
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

            $("input[name]:not(:disabled),textarea", "#proxy_dialog").each(function (index, e) {
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
                url: "/api/setting/proxy/update",
                data: JSON.stringify(d),
                type: 'POST',
                contentType: "application/json",
                success: function (res) {
                    swal("success!", "The proxy have been update", "success");
                    dialog.close();
                    $('#tb_proxies').bootstrapTable("refresh");
                }
            });
        },
        batchOpConfirm: function (title, text, type, callback) {
            var selects = $("#tb_proxies").bootstrapTable('getSelections');
            if (selects.length <= 0) {
                swal("Unchecked any proxies!", "", "warning");
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
        enable: function (ids) {
            var url = "/api/setting/proxy/status?ids=" + ids + "&status=ON";

            $.getJSON(url, function (res) {
                if (res) {
                    swal("Enable sucess!", "The proxies have been enable.", "success");
                    $('#tb_proxies').bootstrapTable("refresh");
                } else {
                    swal("Enable failed!", "A mistake in the enable proxy.", "error");
                }
            });
        },
        disable: function (ids) {
            var url = "/api/setting/proxy/status?ids=" + ids + "&status=OFF";

            $.getJSON(url, function (res) {
                if (res) {
                    swal("Disable sucess!", "The proxies have been disable.", "success");
                    $('#tb_proxies').bootstrapTable("refresh");
                } else {
                    swal("Disable failed!", "A mistake in the disable proxy.", "error");
                }
            });
        },
        remove: function (ids) {
            var url = "/api/setting/proxy/remove?ids=" + ids;

            $.getJSON(url, function (res) {
                if (res) {
                    swal("Delete sucess!", "The proxies have been deleted.", "success");
                    $('#tb_proxies').bootstrapTable("refresh");
                } else {
                    swal("Delete failed!", "A mistake in the deletion proxy.", "error");
                }
            });
        }
    };

    module.init();
});