define(['jquery', 'utils'], function ($, utils) {
    var module = {
        init: function () {
            var tmp = utils.loadTemplate("/misc/setting.html", false);

            $("#tab_panel_setting").html(tmp);
        }
    };

    module.init();
});