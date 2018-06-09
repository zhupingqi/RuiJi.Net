define(['jquery', 'utils', 'sweetAlert','flatJson', 'bootstrapDialog', 'bootstrapTable'], function ($, utils) {
    var proxyUrl = "";

    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/content.html", false);
            tmp = $(tmp);
            tmp.find("#tb_contents").attr("data-url", "http://" + proxyUrl + "/api/fp/content");

            $("#tab_panel_content").html(tmp.prop("outerHTML"));

            var $table = $('#tb_contents').bootstrapTable({
                toolbar: '#toolbar_contents',
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
                height: 550,
                uniqueId: "ID",
                cardView: false,
                detailView: false,
                flat: true,
                flatSeparator: '.',
                onPostBody: function (e) {
                    
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
            $.getJSON("/api/zoo/proxys", function (proxys) {
                proxyUrl = proxys["feed proxy"];

                fn();
            });
        }
    };

    module.getProxy(function () {
        module.init();
    });
});