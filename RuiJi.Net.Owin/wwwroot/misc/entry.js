
define(['jquery', 'tabs'], function ($, tabs) {
    var module = {
        init: function () {

            var conf = [
                {
                    id: 'status',
                    text: 'Status',
                    type: 'lib',
                    lib: 'status'
                },
                {
                    id: 'node',
                    text: 'Node',
                    type: 'lib',
                    lib: 'node'
                },
                {
                    id: 'cluster',
                    text: 'Cluster',
                    type: 'lib',
                    lib: 'cluster'
                },
                {
                    id: 'log',
                    text: 'Log',
                    type: 'lib',
                    lib: 'log'
                },
                {
                    id: 'feed',
                    text: 'Feed&Rule',
                    type: 'lib',
                    lib: 'feed/entry'
                },
                {
                    id: 'content',
                    text: 'GrabResult',
                    type: 'lib',
                    lib: 'content'
                },
                {
                    id: 'setting',
                    text: 'Setting',
                    type: 'lib',
                    lib: 'setting/entry'
                }
            ];

            $.getJSON("/api/alone", function (d) {
                if (d) {
                    conf.splice(1, 2);
                }

                tabs.init({
                    container: '#main_menu_tabs',
                    direction: 'vertical',
                    current: 0,
                    tabs: conf
                });
            });
        }
    };

    module.init();
});