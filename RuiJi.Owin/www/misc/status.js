define(['jquery', 'utils'], function ($, utils) {
    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/status.html", false);
            $("#tab_panel_status").html(tmp);
        }
    };

    module.init();
});