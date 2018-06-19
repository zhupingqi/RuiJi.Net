
define(['jquery', 'tabs'], function ($, tabs) {
    var module = {
        init: function () {

            var conf = [
                {
                    id: 'status',
                    text: '状态',
                    type: 'lib',
                    lib: 'status'
                },
                {
                    id: 'node',
                    text: '节点',
                    type: 'lib',
                    lib: 'node'
                },
                {
                    id: 'cluster',
                    text: '集群',
                    type: 'lib',
                    lib: 'cluster'
                },
                {
                    id: 'log',
                    text: '日志',
                    type: 'lib',
                    lib: 'log'
                },
                {
                    id: 'feed',
                    text: '新闻源',
                    type: 'lib',
                    lib: 'feed/entry'
                },
                {
                    id: 'content',
                    text: '抓取结果',
                    type: 'lib',
                    lib: 'content'
                },
                {
                    id: 'setting',
                    text: '设置',
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