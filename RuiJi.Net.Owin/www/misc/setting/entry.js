define(['jquery', 'utils', 'tabs'], function ($, utils, tabs) {
    var module = {
        init: function () {
            tabs.init({
                container: '#tab_panel_setting',
                current: 0,
                tabs: [
                    {
                        id: 'crawler_node',
                        text: '节点',
                        type: 'lib',
                        lib: 'setting/node'
                    },
                    {
                        id: 'storage',
                        text: '存储',
                        type: 'lib',
                        lib: 'setting/storage'
                    },
                    {
                        id: 'funcs',
                        text: '函数',
                        type: 'lib',
                        lib: 'setting/funcs'
                    },
                    {
                        id: 'ua',
                        text: 'UA',
                        type: 'lib',
                        lib: 'setting/ua'
                    },
                    {
                        id: 'proxys',
                        text: '代理',
                        type: 'lib',
                        lib: 'setting/proxys'
                    },
                    {
                        id: 'plugin',
                        text: '插件',
                        type: 'lib',
                        lib: 'setting/plugin'
                    }                    
                ]
            });

            $(document).keydown(function (event) {
                if (event.keyCode == 9 && event.target.tagName == "TEXTAREA") {
                    event.returnValue = false;
                    var textEl = event.target;
                    var tab = '\t';
                    var txt = $(textEl).val();
                    var startPos = textEl.selectionStart;
                    var endPos = textEl.selectionEnd;

                    txt = txt.substring(0, startPos) + tab + txt.substring(endPos, txt.length);
                    $(textEl).val(txt);

                    textEl.selectionStart = endPos + 1;
                    textEl.selectionEnd = endPos + 1;

                    return false;
                }
            });
        }
    };

    module.init();
});