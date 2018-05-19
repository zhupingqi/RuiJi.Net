define(['jquery', 'tabs'], function ($, tabs) {
    var module = {
        init: function () {
            tabs.init({
                container: '#main_menu_tabs',
                direction: 'vertical',
                click: function (tab) {
                    
                }
            });
        }
    };

    module.init();
});