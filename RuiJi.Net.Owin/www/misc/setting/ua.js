define(['jquery', 'utils', 'sweetAlert', 'bootstrapDialog', 'bootstrapTable'], function ($, utils) {
    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/setting/uas.html", false);

            tmp = $(tmp);

            $("#tab_panel_ua").html(tmp.prop("outerHTML"));

            module.fillGroup();

            $('#tb_uas').bootstrapTable({
                toolbar: '#toolbar_ua',
                url: "/api/uas",
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
            var tmp = utils.loadTemplate("/misc/setting/ua.html", false);
            var tmpGroup = utils.loadTemplate("/misc/setting/uaGroup.html", false);

            //#region Ua
            $(document).on("click", "#add_ua", function () {
                var $group = $("#toolbar_ua_group button.ua-group");
                var gid = $group.attr("gid") || 0;
                if (gid == 0) {
                    swal("Please select a group!", "unselected group", "error");
                    return;
                }
                var f = $(tmp);
                var groupName = $group.text();
                f.find("input[name='groupId']").val(gid);
                f.find("input[name='group']").attr("value", groupName);

                BootstrapDialog.show({
                    title: 'Add UA',
                    message: f.prop("outerHTML"),
                    closable: false,
                    nl2br: false,
                    buttons: [{
                        label: 'Ok',
                        action: function (dialog) {
                            module.update(dialog, gid);
                        }
                    }, {
                        label: 'Close',
                        action: function (dialog) {
                            dialog.close();
                        }
                    }]
                });
            });

            $(document).on("click", "#tb_uas .fa-edit", function () {
                var ele = $(this);
                var id = ele.closest("tr").find("td").eq(1).text();
                var f = $(tmp);

                f.find("input[name='id']").val(id);

                $.getJSON("/api/ua?id=" + id, function (d) {
                    if (d) {
                        var groupDropdown = $("#toolbar_ua_group button.ua-group").parent().find("ul.dropdown-menu");
                        for (var p in d) {
                            var v = d[p];
                            var ele = f.find("[name='" + p + "']").eq(0);

                            ele.attr("value", v);
                            ele.text(v);

                            if (p == "groupId") {
                                var groupName = groupDropdown.find("li > a[gid=" + v + "]").text();
                                f.find("input[name='group']").attr("value", groupName);
                            }
                        }

                        BootstrapDialog.show({
                            title: 'Edit UA',
                            message: f.prop("outerHTML"),
                            closable: false,
                            nl2br: false,
                            buttons: [{
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
                    }
                });
            });

            //batch operate
            $(document).on("click", "#toolbar_ua a[remove]", function () {
                module.batchOpConfirm("Do you confirm the deletion?", "The ua will not be restored!", "warning", module.remove);
            });
            //#endregion

            //#region uagroup
            $(document).on("click", "#add_ua_group", function () {
                BootstrapDialog.show({
                    title: 'Add UA Group',
                    message: tmpGroup,
                    closable: false,
                    nl2br: false,
                    buttons: [{
                        label: 'Ok',
                        action: function (dialog) {
                            module.updateGroup(dialog);
                        }
                    }, {
                        label: 'Close',
                        action: function (dialog) {
                            dialog.close();
                        }
                    }]
                });
            });

            $(document).on("click", "#update_ua_group", function () {
                var ele = $(this);
                var id = $("#toolbar_ua_group button.ua-group").attr("gid") || 0;
                if (id == 0) {
                    swal("Please select a group!", "unselected group", "error");
                    return;
                }
                var f = $(tmpGroup);
                f.find("input[name='id']").val(id);

                $.getJSON("/api/ua/group?id=" + id, function (d) {
                    if (d) {
                        for (var p in d) {
                            var v = d[p];
                            var ele = f.find("[name='" + p + "']").eq(0);

                            ele.attr("value", v);
                            ele.text(v);
                        }
                    }

                    BootstrapDialog.show({
                        title: 'Edit UA Group',
                        message: f.prop("outerHTML"),
                        closable: false,
                        nl2br: false,
                        buttons: [{
                            label: 'Ok',
                            action: function (dialog) {
                                module.updateGroup(dialog);
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

            $(document).on("click", "#delete_ua_group", function () {
                var id = $("#toolbar_ua_group button.ua-group").attr("gid") || 0;
                if (id == 0) {
                    swal("Please select a group!", "unselected group", "error");
                    return;
                }

                swal(
                    {
                        title: "Do you confirm the deletion?",
                        text: "The ua groups and the elements in the group will not be restored!",
                        type: "warning",
                        showCancelButton: true,
                        confirmButtonColor: "#DD6B55",
                        confirmButtonText: "Confirm",
                        cancelButtonText: "Cancel",
                        closeOnConfirm: false
                    },
                    function () {
                        module.removeGroup(id);
                    }
                );
            });

            $(document).on("click", "#toolbar_ua_group .dropdown-menu a", function () {
                $(this).closest("ul").prev("button").attr("gid", $(this).attr("gid")).html($(this).text() + "<span class=\"caret\"></span>");
                $('#tb_uas').bootstrapTable("refresh");
            });
            //#endregion
        },
        fillGroup: function () {
            var url = "/api/ua/groups";

            var groupDropdown = $("#toolbar_ua_group button.ua-group").parent().find("ul.dropdown-menu");
            $.getJSON(url, function (res) {
                groupDropdown.html("<li><a href='javascript:;' gid=0>All</a></li>");
                if (res && res.length > 0) {
                    res.map(function (s) {
                        groupDropdown.append("<li><a href='javascript:;' gid=" + s.id + ">" + s.name + "</a></li>");
                    });
                }
            });
        },
        queryParams: function (params) {
            var gid = $("#toolbar_ua_group button.ua-group").attr("gid") || 0;
            var temp = {
                limit: params.limit,
                offset: params.offset,
                groupId: gid
            };
            return temp;
        },
        updateGroup: function (dialog) {
            var d = {};
            var validate = true;
            var msg = "need";

            $("input.required", "#ua_group_dialog").each(function (index, e) {
                e = $(e);
                var v = e.val();
                if ($.trim(e.val()) == "") {
                    validate = false;
                    msg += "\n" + e.attr("name");
                }
            });

            if (!validate) {
                swal("missing field", msg, "error");
                return;
            }

            $("input[name]", "#ua_group_dialog").each(function (index, e) {
                e = $(e);
                var v = e.val();
                if (v == "true")
                    d[e.attr("name")] = true;
                else if (v == "false")
                    d[e.attr("name")] = false;
                else
                    d[e.attr("name")] = v;
            });

            $.ajax({
                url: "/api/ua/group/update",
                data: JSON.stringify(d),
                type: 'POST',
                contentType: "application/json",
                success: function (res) {
                    swal("success!", "The ua group have been update", "success");
                    if (d.id == 0) {
                        $("#toolbar_ua_group button.ua-group").parent().find("ul.dropdown-menu").append("<li><a href='javascript:;' gid=" + res + ">" + d.name + "</a></li>");
                    }
                    $("#toolbar_ua_group button.ua-group").attr("gid", res).html(d.name + "<span class='caret'></span>");
                    dialog.close();
                    $('#tb_uas').bootstrapTable("refresh");
                }
            });
        },
        update: function (dialog) {
            var d = {};
            var validate = true;
            var msg = "need";

            $("input.required", "#ua_dialog").each(function (index, e) {
                e = $(e);
                var v = e.val();
                if ($.trim(e.val()) == "") {
                    validate = false;
                    msg += "\n" + e.attr("name");
                }
            });

            if (!validate) {
                swal("missing field", msg, "error");
                return;
            }

            $("input[name]:not([unsub])", "#ua_dialog").each(function (index, e) {
                e = $(e);
                var v = e.val();
                if (v == "true")
                    d[e.attr("name")] = true;
                else if (v == "false")
                    d[e.attr("name")] = false;
                else
                    d[e.attr("name")] = v;
            });

            $.ajax({
                url: "/api/ua/update",
                data: JSON.stringify(d),
                type: 'POST',
                contentType: "application/json",
                success: function (res) {
                    swal("success!", "The ua have been update", "success");
                    dialog.close();
                    $('#tb_uas').bootstrapTable("refresh");
                }
            });
        },
        batchOpConfirm: function (title, text, type, callback) {
            var selects = $("#tb_uas").bootstrapTable('getSelections');
            if (selects.length <= 0) {
                swal("Unchecked any ua!", "", "warning");
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
        removeGroup: function (id) {
            var url = "/api/ua/group/remove?id=" + id;

            $.getJSON(url, function (res) {
                if (res) {
                    swal("Delete sucess!", "The ua group have been deleted.", "success");
                    module.fillGroup();
                    $("#toolbar_ua_group button.ua-group").attr("gid", 0).html("All <span class='caret'></span>");
                    $('#tb_uas').bootstrapTable("refresh");
                } else {
                    swal("Delete failed!", "A mistake in the deletion ua group.", "error");
                }
            });
        },
        remove: function (ids) {
            var url = "/api/ua/remove?ids=" + ids;

            $.getJSON(url, function (res) {
                if (res) {
                    swal("Delete sucess!", "The ua have been deleted.", "success");
                    $('#tb_uas').bootstrapTable("refresh");
                } else {
                    swal("Delete failed!", "A mistake in the deletion ua.", "error");
                }
            });
        }
    };

    module.init();
});