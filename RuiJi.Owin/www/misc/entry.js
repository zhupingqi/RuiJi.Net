define(['jquery', 'tabs'], function ($, tabs) {
    var module = {
        init: function () {
            tabs.init({
                container: '#main_menu_tabs',
                direction: 'vertical',
                current : 0,
                click: function (tab) {
                    
                }
            });
        }
    };

    module.init();
});