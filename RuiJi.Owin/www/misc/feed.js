define(['jquery', 'utils', 'tabs'], function ($, utils, tabs) {
    var module = {
        init: function () {
            tabs.init({
                container: '#tab_panel_feed',
                current: 0,
                tabs: [
                    {
                        id: 'statistics',
                        text: '统计',
                        type: 'lib',
                        lib: 'statistics'
                    },
                    {
                        id: 'feeds',
                        text: '新闻源',
                        type: 'lib',
                        lib: 'feeds'
                    },
                    {
                        id: 'rules',
                        text: '规则',
                        type: 'lib',
                        lib: 'rules'
                    },
                    {
                        id: 'plugin',
                        text: '插件',
                        type: 'lib',
                        lib: 'plugin'
                    },
                    {
                        id: 'setting',
                        text: '设置',
                        type: 'lib',
                        lib: 'setting'
                    }
                ]
            });
        }
    };

    module.init();
});