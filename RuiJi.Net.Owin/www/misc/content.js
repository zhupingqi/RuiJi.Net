define(['jquery', 'utils', 'sweetAlert', 'flatJson', 'bootstrapDialog', 'bootstrapTable'], function ($, utils) {
    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/content.html", false);
            tmp = $(tmp);
            tmp.find("#tb_contents").attr("data-url", "/api/fp/content");

            $("#tab_panel_content").html(tmp.prop("outerHTML"));

            module.fillShard(tmp);

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
            module.initEvent();
        },
        initEvent: function () {
            $(document).on("click", "#content_search_group .dropdown-menu a", function () {
                $(this).closest("ul").prev("button").html($(this).text() + "<span class=\"caret\"></span>");
            });

            $(document).on("click", "#content_search_group .content-search", function () {
                module.search();
            });

            //batch operate
            $(document).on("click", "#toolbar_contents a[remove]", function () {
                module.batchOpConfirm("Do you confirm the deletion?", "The results will not be restored!", "warning", module.remove);
            });
        },
        fillShard: function () {
            var url = "/api/fp/content/shards";

            var shardDropdown = $("#content_search_group button.shard").parent().find("ul.dropdown-menu");
            $.getJSON(url, function (res) {
                if (res) {
                    shardDropdown.html("");
                    res.map(function (s) {
                        shardDropdown.append("<li><a href='javascript:;'>" + s + "</a></li>");
                    });
                }
            });
        },
        search: function () {
            $("#tb_contents").bootstrapTable('refresh');
        },
        feedResult: function (feedId) {
            $("#content_search_group .feed").val(feedId);
            module.search();
        },
        queryParams: function (params) {
            var temp = {
                limit: params.limit,
                offset: params.offset,
                feedId: $.trim($("#content_search_group .feed").val()),
                shard: $.trim($("#content_search_group .shard").text()),
            };
            return temp;
        },
        batchOpConfirm: function (title, text, type, callback) {
            var selects = $("#tb_contents").bootstrapTable('getSelections');
            if (selects.length <= 0) {
                swal("Unchecked any result!", "", "warning");
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
            var selects = $("#tb_contents").bootstrapTable('getSelections');
            var shard = new Date(selects[0].cdate).format("yyyyMM");

            var url = "/api/fp/content/remove?ids=" + ids + "&shard=" + shard;

            $.getJSON(url, function (res) {
                if (res) {
                    swal("Delete sucess!", "The results have been deleted.", "success");
                    $('#tb_contents').bootstrapTable("refresh");
                } else {
                    swal("Delete failed!", "A mistake in the deletion result.", "error");
                }
            });
        }
    };

    module.init();
    return module;
});