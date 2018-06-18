define(['jquery', 'utils'], function ($, utils) {
    var module = {
        init: function () {
            setInterval(function () {
                module.log();
            }, 5000);
        },
        log: function () {
            if ($("#tab_panel_log").is(":visible")) {
                $.getJSON('/api/log', function (d) {
                    $.map(d, function (v) {
                        var $d = $("<div></div>");
                        $d.text(v);
                        $("#tab_panel_log").append($d);
                    });
                });
            }
        }
    };

    module.init();
});