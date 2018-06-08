define(['jquery', 'utils', 'sweetAlert', 'bootstrapDialog', 'bootstrapTable', 'bootstrapEditable', 'bootstrapTableEditable'], function ($, utils) {

    var proxyUrl = "";

    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/setting/funcs.html", false);
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

            $(document).on("click", "#tb_funcs .fa-edit", function () {
                var ele = $(this);
                var id = ele.closest("tr").find("td").eq(1).text();
                var f = $(func);
                f.find("input[name='id']").val(id);

                $.getJSON("http://" + proxyUrl + "/api/func?id=" + id, function (d) {
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

            tmp = $(tmp);
            tmp.find("#tb_funcs").attr("data-url", "http://" + proxyUrl + "/api/funcs");
            $("#tab_panel_funcs").html(tmp.prop("outerHTML"));

            var $table = $('#tb_funcs').bootstrapTable({
                //url: "http://" + proxyUrl + "/api/funcs",
                method: 'get',
                toolbar: '#toolbar_funcs',
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
                        $('#tb_funcs > tbody > tr').map(function (i, m) {
                            $(m).find("td:last").html("<i class='fa fa-edit'></i>");
                        });
                    }
                }
                //,
                //onEditableSave: function (field, row, oldValue, $el) {
                //    $.ajax({
                //        type: "post",
                //        url: "api/funcs/update",
                //        data: row,
                //        dataType: 'JSON',
                //        success: function (data, status) {
                //            if (data) {
                //                alert('提交数据成功');
                //            }
                //            else {
                //                $el.text(oldValue);
                //            }
                //            $table = $('#tb_funcs').bootstrapTable("resetView");
                //        },
                //        error: function () {
                //            $el.text(oldValue);
                //            //swal('编辑失败');
                //        },
                //        complete: function () {

                //        }

                //    });
                //}
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
        },
        getProxy: function (fn) {
            $.getJSON("/api/zoo/feedproxy", function (url) {
                proxyUrl = url;

                fn();
            });
        }
    };

    module.getProxy(function () {
        module.init();
    });
});