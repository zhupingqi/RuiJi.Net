define(['jquery', 'utils'], function ($, utils) {
    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/status.html", false);
            $("#tab_panel_status").html(tmp);

            setInterval(function () {
                $.getJSON('/api/info/system', function (d) {
                    var t = utils.template("status_system_info", d);
                    $("#system_info").html(t);
                });
            }, 5000);

            $.getJSON('/api/info/server?baseUrl=' + window.location.href, function (d) {
                var t = utils.template("status_server_info", d);
                $("#server_info").prepend(t);
            });

            $.getJSON('/api/info/dll', function (vs) {
                $.map(vs.versions, function (v) {
                    var $d = $("<div></div>");
                    $d.text(v);
                    $("#dll_version").append($d);
                });
            });
        }
    };

    module.init();
});