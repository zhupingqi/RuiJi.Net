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
                        lib: 'feed/statistics'
                    },
                    {
                        id: 'feeds',
                        text: '新闻源',
                        type: 'lib',
                        lib: 'feed/feeds'
                    },
                    {
                        id: 'rules',
                        text: '规则',
                        type: 'lib',
                        lib: 'feed/rules'
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