define(['jquery', 'utils', 'sweetAlert', 'bootstrapDialog', 'bootstrapTable'], function ($, utils) {
    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/setting/funcs.html", false);

            tmp = $(tmp);
            tmp.find("#tb_funcs").attr("data-url", "/api/funcs");
            $("#tab_panel_funcs").html(tmp.prop("outerHTML"));

            var $table = $('#tb_funcs').bootstrapTable({
                toolbar: '#toolbar_funcs',
                pagination: true,
                queryParams: module.queryParams,
                sidePagination: "server",
                showColumns: true,
                showRefresh: true,
                height: 530,
                onPostBody: function (e) {
                    if (e.length > 0) {
                        $('#tb_funcs > tbody > tr').map(function (i, m) {
                            $(m).find("td:last").html("<i class='fa fa-edit'></i><i class='fa fa-minus-circle'></i>");
                        });
                    }
                }
            });

            module.initEvent();
        },
        initEvent: function () {
            var func = utils.loadTemplate("/misc/setting/func.html", false);

            $(document).on("click", "#add_func", function () {
                BootstrapDialog.show({
                    title: 'Add Func',
                    message: func,
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

            $(document).on("click", "#delete_funcs", function () {
                var select = $("#tb_funcs").bootstrapTable('getSelections');
                if (select.length <= 0) {
                    swal("请至少选中一行");
                }
                else {
                    if (confirm("确认删除选中函数？")) {
                        var ids = "";
                        $.map(select, function (n) {
                            ids += n.id + ",";
                        })

                        $.getJSON("api/func/remove?ids=" + ids, function (d) {
                            if (d) {
                                swal("完成");
                                $('#tb_funcs').bootstrapTable("refresh");
                            }
                        });
                    }
                }
            });

            $(document).on("click", "#tb_funcs .fa-minus-circle", function () {
                var ele = $(this);
                var id = ele.closest("tr").find("td").eq(1).text();

                if (confirm("确认删除此条函数？")) {
                    $.getJSON("api/func/remove?ids=" + id, function (d) {
                        if (d) {
                            swal("完成");
                            $('#tb_funcs').bootstrapTable("refresh");
                        }
                    });
                }
            });

            $(document).on("click", "#tb_funcs .fa-edit", function () {
                var ele = $(this);
                var id = ele.closest("tr").find("td").eq(1).text();
                var f = $(func);
                f.find("input[name='id']").val(id);

                $.getJSON("/api/func?id=" + id, function (d) {
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
        test: function () {
            var d = {};
            var t = $("#func_form").serializeArray();

            $.each(t, function () {
                d[this.name] = this.value.replace(/"/g, "&quot;");
            });

            $.ajax({
                url: "/api/func/test",
                data: JSON.stringify(d),
                type: "POST",
                contentType: "application/json",
                success: function (res) {
                    $("#func_form .msg").text(res);
                }
            });
        },
        update: function (dialog) {
            var d = {};
            var t = $("#func_form").serializeArray();
            var msg = "";

            $.each(t, function () {
                d[this.name] = this.value.replace(/"/g, "&quot;");
                if ($.trim(d[this.name]) == "") {
                    msg += "\nneed " + this.name;
                }
            });

            if (msg != "") {
                swal(msg);
                return;
            }

            $.ajax({
                url: "/api/func/update",
                data: JSON.stringify(d),
                type: "POST",
                contentType: "application/json",
                success: function (res) {
                    $table = $('#tb_funcs').bootstrapTable("refresh");
                    if (res) {
                        swal("完成");
                        dialog.close();
                    }
                    else
                        swal("添加失败");
                }
            });
        }
    };

    module.init();
});