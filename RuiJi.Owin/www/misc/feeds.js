define(['jquery', 'utils', 'bootstrap', 'bootstrapTable'], function ($, utils, bootstrapTable) {
    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/feeds.html", false);

            $.getJSON("/api/zoo/feedproxy", function (url) {

                tmp = $(tmp);
                tmp.find("#tb_feeds").attr("data-url", "http://" + url + "/api/feeds");

                $("#tab_panel_feeds").html(tmp.prop("outerHTML"));

                $('#tb_feeds').bootstrapTable({
                    toolbar: '#toolbar',
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
                    showToggle: true,
                    cardView: false,
                    detailView: false//,
                    //columns: [{
                    //    checkbox: true
                    //},
                    //{
                    //    field: 'siteName',
                    //    title: 'SiteName'
                    //},
                    //{
                    //    field: 'railling',
                    //    title: 'Railling'
                    //},
                    //{
                    //    field: 'address',
                    //    title: 'Address'
                    //},
                    //{
                    //    field: 'type',
                    //    title: 'Type'
                    //},
                    //{
                    //    field: 'method',
                    //    title: 'Method'
                    //},
                    //{
                    //    field: 'rules',
                    //    title: 'Rules'
                    //    },
                    //    {
                    //        field: 'scheduling',
                    //        title: 'Scheduling'
                    //    },
                    //{
                    //    field: 'status',
                    //    title: 'Status'
                    //},
                    //{
                    //    title: 'Actions'
                    //}]
                });

            });
        },
        queryParams: function (params) {
            var temp = {   //这里的键的名字和控制器的变量名必须一直，这边改动，控制器也需要改成一样的
                limit: params.limit,   //页面大小
                offset: params.offset,  //页码
                departmentname: $("#txt_search_departmentname").val(),
                statu: $("#txt_search_statu").val()
            };
            return temp;
        }
    };

    module.init();
});