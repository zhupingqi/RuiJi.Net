define(['jquery', 'utils', 'sweetAlert', 'bootstrapDialog', 'bootstrapTable'], function ($, utils) {


    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/setting/proxys.html", false);

            tmp = $(tmp);
            tmp.find("#tb_proxys").attr("data-url", "http://" + proxyUrl + "/api/proxys");

            $("#tab_panel_proxys").html(tmp.prop("outerHTML"));

            var $table = $('#tb_proxys').bootstrapTable({
                toolbar: '#toolbar_proxy',
                pagination: true,
                queryParams: module.queryParams,
                sidePagination: "server",
                showColumns: true,
                showRefresh: true,
                height: 530,
                onPostBody: function (e) {
                    if (e.length > 0) {
                        $('#tb_proxys > tbody > tr').map(function (i, m) {
                            $(m).find("td:last").html("<i class='fa fa-edit'></i></i>");
                        });
                    }
                }
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

            $(document).on("click", "#tb_proxys .fa-edit", function () {
                var ele = $(this);
                var id = ele.closest("tr").find("td").eq(1).text();
                var f = $(tmp);
                f.find("input[name='id']").val(id);

                $.getJSON("http://" + proxyUrl + "/api/proxy?id=" + id, function (d) {
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
                        title: 'Edit Func',
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

            $(document).on("click", "#proxy_dialog .btn-mjdark2", function () {
                var ele = $(this);
                ele.closest(".input-group").find("input").val(ele.find("input").val());
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

            $("input.required", "#proxy_dialog").each(function (index, e) {
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

            $("input[name]:not(:disabled),textarea", "#proxy_dialog").each(function (index, e) {
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
                url: "http://" + proxyUrl + "/api/proxy/update",
                data: JSON.stringify(d),
                type: 'POST',
                contentType: "application/json",
                success: function (res) {
                    swal("完成");
                    dialog.close();
                    $('#tb_proxys').bootstrapTable("refresh");
                }
            });
        }
    };

    module.getProxy(function () {
        module.init();
    });
});