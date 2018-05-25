define(['jquery', 'utils', 'bootstrapDialog', 'bootstrapTable'], function ($, utils) {
    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/feeds.html", false);
            var feed = utils.loadTemplate("/misc/feed.html", false);

            $(document).on("click", "#add_feed", function () {
                BootstrapDialog.show({
                    title: '添加Feed',
                    message: feed,
                    closable: false,
                    nl2br: false,
                    buttons: [{
                        label: 'Ok',
                        action: function (dialog) {
                            
                        }
                    }, {
                        label: 'Close',
                        action: function (dialog) {
                            dialog.close();
                        }
                    }]
                });
            });

            $.getJSON("/api/zoo/feedproxy", function (url) {

                tmp = $(tmp);
                tmp.find("#tb_feeds").attr("data-url", "http://" + url + "/api/feeds");

                $("#tab_panel_feeds").html(tmp.prop("outerHTML"));

                var $table = $('#tb_feeds').bootstrapTable({
                    toolbar: '#toolbar_feeds',
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
                            $('#tb_feeds > tbody > tr').map(function (i,m) {
                                $(m).find("td:last").html("<i class='fa fa-edit'></i><i class='fa fa-eye'></i><i class='fa fa-history'></i><i class='fa fa-random'></i>");
                            });
                        }
                    }
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
        }
    };

    module.init();
});