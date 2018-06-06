define(['jquery', 'utils', 'sweetAlert', 'bootstrapTable', 'bootstrapEditable', 'bootstrapTableEditable'], function ($, utils) {

    var proxyUrl = "";

    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/feed/func.html", false);

            tmp = $(tmp);
            //tmp.find("#tb_funcs").attr("data-url", "http://" + proxyUrl + "/api/funcs");
            $("#tab_panel_func").html(tmp.prop("outerHTML"));

            var $table = $('#tb_funcs').bootstrapTable({
                url: "/api/funcs",
                method: 'get',
                toolbar: '#toolbar_funcs',
                striped: true,
                cache: false,
                pagination: true,
                sortable: false,
                sortOrder: "asc",
                queryParams: "",
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
                        title: 'Name',
                        validate: function (v) {
                            if (!v) return 'name not null';

                        }
                    }

                }, {
                    field: "examples",
                    title: "Examples"
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
                onPostBody: function (e) {
                    if (e.length > 0) {
                        $('#tb_funcs > tbody > tr').map(function (i, m) {
                            $(m).find("td:last").html("<i class='fa fa-edit'></i><i class='fa fa-eye'></i><i class='fa fa-history'></i><i class='fa fa-random'></i>");
                        });
                    }
                },
                onEditableSave: function (field, row, oldValue, $el) {
                    $.ajax({
                        type: "post",
                        url: "/Editable/Edit",
                        data: row,
                        dataType: 'JSON',
                        success: function (data, status) {
                            if (status == "success") {
                                alert('提交数据成功');
                            }
                        },
                        error: function () {
                            alert('编辑失败');
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