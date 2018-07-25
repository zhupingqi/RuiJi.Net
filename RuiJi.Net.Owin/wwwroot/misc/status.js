define(['jquery', 'utils'], function ($, utils) {
    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/status.html", false);
            $("#tab_panel_status").html(tmp);

            module.reloadSystem();

            setInterval(function () {
                module.reloadSystem();
            }, 2000);

            $.getJSON('/api/sys/info', function (d) {
                var t = utils.template("status_server_info", d);
                $("#server_info").append(t);
            });

            $.getJSON('/api/sys/dll', function (vs) {
                $.map(vs.versions, function (v) {
                    var $d = $("<div></div>");
                    $d.text(v);
                    $("#dll_version").append($d);
                });
            });

            $.getJSON('/api/github', function (vs) {
                var div = $("<div/>");
                $.map(vs, function (v) {
                    div.append("<img src='" + v.gravatar + "' title='" + (v.name ? v.name: v.login) + "' /><span>" + v.commits + "</span>");
                });
                $("#pulse").append(div);
            });
        },
        reloadSystem: function () {
            if ($("#system_info").is(":visible")) {
                $.getJSON('/api/sys/load', function (d) {
                    var t = utils.template("status_system_info", d);
                    $("#system_info").html(t);
                });
            }
        }
    };

    module.init();
});