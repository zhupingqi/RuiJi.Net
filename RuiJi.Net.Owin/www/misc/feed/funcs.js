define(['jquery', 'utils', 'sweetAlert', 'bootstrapDialog', 'bootstrapTable', 'bootstrapEditable', 'bootstrapTableEditable'], function ($, utils) {

    var proxyUrl = "";

    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/feed/funcs.html", false);
            var func = utils.loadTemplate("/misc/feed/func.html", false);

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

            tmp = $(tmp);
            $("#tab_panel_funcs").html(tmp.prop("outerHTML"));

            var $table = $('#tb_funcs').bootstrapTable({
                url: "http://" + proxyUrl + "/api/funcs",
                method: 'get',
                toolbar: '#toolbar_funcs',
                striped: true,
                cache: false,
                pagination: true,
                sortable: false,
                sortOrder: "asc",
                queryParams: module.queryParams,
                sidePagination: "server",
                columns: [{
                    field: "name",
                    title: "Name",
                    editable: {
                        type: 'text',
                        title: 'Name',
                        validate: function (v) {
                            if (!v) return 'name not null';

                        }
                    }
                }, {
                    field: "code",
                    title: "Code",
                    editable: {
                        type: 'text',
                        title: 'Code',
                        validate: function (v) {
                            if (!v) return 'code not null';

                        }
                    }

                }, {
                    field: "sample",
                    title: "Sample",
                    editable: {
                        type: 'text',
                        title: 'Sample'
                    }
                }, {
                    field: "actions",
                    title: "Actions"
                }],
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
                onEditableSave: function (field, row, oldValue, $el) {
                    $.ajax({
                        type: "post",
                        url: "api/funcs/update",
                        data: row,
                        dataType: 'JSON',
                        success: function (data, status) {
                            if (data) {
                                alert('提交数据成功');
                            }
                            else {
                                $el.text(oldValue);
                            }
                            $table = $('#tb_funcs').bootstrapTable("resetView");
                        },
                        error: function () {
                            $el.text(oldValue);
                            //swal('编辑失败');
                        },
                        complete: function () {

                        }

                    });
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
        test: function () {
            var d = {};
            var t = $("#func_form").serializeArray();

            $.each(t, function () {
                d[this.name] = this.value.replace(/"/g, "&quot;");
            });

            $.ajax({
                url: "/api/funcs/test",
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
                url: "/api/funcs/update",
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