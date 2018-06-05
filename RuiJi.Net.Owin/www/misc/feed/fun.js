define(['jquery', 'utils', 'sweetAlert', 'bootstrapDialog', 'bootstrapTable'], function ($, utils) {

    var proxyUrl = "";

    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/feed/fun.html", false);
            
            tmp = $(tmp);
            tmp.find("#tb_funs").attr("data-url", "http://" + proxyUrl + "/api/rules");
            $("#tab_panel_fun").html(tmp.prop("outerHTML"));

            var $table = $('#tb_funs').bootstrapTable({
                toolbar: '#toolbar_funs',
                striped: true,
                cache: false,
                pagination: true,
                sortable: false,
                sortOrder: "asc",
                queryParams: "",
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
                        $('#tb_funs > tbody > tr').map(function (i, m) {
                            $(m).find("td:last").html("<i class='fa fa-edit'></i><i class='fa fa-eye'></i><i class='fa fa-history'></i><i class='fa fa-random'></i>");
                        });
                    }
                }
            });
        }
    };

    module.init();
});