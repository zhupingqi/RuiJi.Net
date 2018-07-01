define(['jquery', 'utils', 'tabs'], function ($, utils, tabs) {
    var module = {
        init: function () {
            var conf = [
                {
                    id: 'setting_node',
                    text: 'Node',
                    type: 'lib',
                    lib: 'setting/node'
                },
                {
                    id: 'storage',
                    text: 'Storage',
                    type: 'lib',
                    lib: 'setting/storage'
                },
                {
                    id: 'funcs',
                    text: 'Funcs',
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
                    text: 'Proxys',
                    type: 'lib',
                    lib: 'setting/proxys'
                },
                {
                    id: 'plugin',
                    text: 'Plugin',
                    type: 'lib',
                    lib: 'setting/plugin'
                }
            ];

            $.getJSON("/api/alone", function (d) {
                if (d) {
                    conf.splice(0, 1);
                }

                tabs.init({
                    container: '#tab_panel_setting',
                    current: 0,
                    tabs: conf
                });
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