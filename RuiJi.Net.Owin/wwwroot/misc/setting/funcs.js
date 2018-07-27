define(['jquery', 'utils', 'sweetAlert', 'bootstrapDialog', 'bootstrapTable'], function ($, utils) {
    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/setting/funcs.html", false);

            tmp = $(tmp);
            tmp.find("#tb_funcs").attr("data-url", "/api/setting/func/list");
            $("#tab_panel_funcs").html(tmp.prop("outerHTML"));

            var $table = $('#tb_funcs').bootstrapTable({
                toolbar: '#toolbar_funcs',
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

            $(document).on("click", "#func_dialog ul.dropdown-menu a", function () {
                var menu = $(this);
                var h = menu.closest(".input-group").find(":hidden");
                var v = menu.attr("data-bind") ? menu.attr("data-bind") : menu.text();

                h.val(v);
                h.next().val(menu.text());
            });

            $(document).on("click", "#tb_funcs .fa-edit", function () {
                var ele = $(this);
                var id = ele.closest("tr").find("td").eq(1).text();
                var f = $(func);
                f.find("input[name='id']").val(id);

                $.getJSON("/api/setting/func?id=" + id, function (d) {
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

            $(document).on("click", "#toolbar_funcs a[remove]", function () {
                module.batchOpConfirm("Do you confirm the deletion?", "The funcs will not be restored!", "warning", module.remove);
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
                d[this.name] = this.value;
            });

            $.ajax({
                url: "/api/test/func",
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
                d[this.name] = this.value;
                if ($.trim(d[this.name]) == "") {
                    msg += "\nneed " + this.name;
                }
            });

            if (msg != "") {
                swal("missing field", msg, "error");
                return;
            }

            $.ajax({
                url: "/api/setting/func/update",
                data: d,
                type: "POST",
                contentType: "application/json",
                success: function (res) {
                    $table = $('#tb_funcs').bootstrapTable("refresh");
                    if (res) {
                        swal("success!","The func have been update.","success");
                        dialog.close();
                    }
                    else
                        swal("faild", "A mistake in the disable feed.","error");
                }
            });
        },
        batchOpConfirm: function (title, text, type, callback) {
            var selects = $("#tb_funcs").bootstrapTable('getSelections');
            if (selects.length <= 0) {
                swal("Unchecked any funcs!", "", "warning");
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
        remove: function (ids) {
            var url = "api/setting/func/remove?ids=" + ids;

            $.getJSON(url, function (res) {
                if (res) {
                    swal("Delete sucess!", "The funcs have been deleted.", "success");
                    $('#tb_funcs').bootstrapTable("refresh");
                } else {
                    swal("Delete failed!", "A mistake in the deletion func.", "error");
                }
            });
        }
    };

    module.init();
});