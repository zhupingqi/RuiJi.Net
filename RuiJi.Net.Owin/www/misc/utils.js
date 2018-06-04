define('utils', ['jquery', 'template'], function ($, templateJs) {
    var templates = {};

    return {
        loadTemplate: function (url, auto) {
            if (auto === undefined)
                auto = true;

            if (templates[url] === undefined) {
                if (!url.startWith("/"))
                    url = requirejs.s.contexts["_"].config.baseUrl + url;

                $.ajax({
                    url: url,
                    async: false,
                    success: function (res) {
                        templates[url] = res;
                        if (auto) {
                            if ($("#templates").length == 0) {
                                $(document.body).append("<div id=\"templates\"></div>");
                            }
                            $("#templates").append(res);
                        }
                    }
                });
            }
            return templates[url];
        },
        template: function (s, data) {
            var html = "";
            if (data)
                html = templateJs(s, data);
            else
                html = templateJs(s)();

            return html;
        },
        loadCss: function (path) {
            require(['css!' + path]);
        },
        getQuery: function (name) {
            var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
            if (result == null || result.length < 1) {
                return "";
            }
            return decodeURI(result[1]);
        },
        getHashLib: function () {
            if (window.location.hash.length == 0)
                return "";

            var uri = new URL(window.location.origin + "/" + window.location.hash.substr(1));
            return uri.pathname.substr(1);
        },
        getHashQuery: function (name, url) {
            url = url || window.location;
            var hash = new URL(url).hash;
            var result = hash.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
            if (result == null || result.length < 1) {
                return "";
            }
            return decodeURI(result[1]);
        },
        setHashQuery: function (name, value) {
            var hash = location.hash.replace(new RegExp("([\?\&])" + name + "=[^\&]+", 'g'), '$1' + name + "=" + value);
            location.hash = hash;
        },
        showLoading: function ($elem, opt) {
            opt = $.extend({
                text: true,
                type: "0"
            }, opt);

            switch (opt.type) {
                case "0": {
                    var div = $("<div><div><img src='/Content/images/loading.0.gif'style='margin-right:5px' /></div></div>");
                    div.find("div").css({
                        position: 'absolute',
                        top: '50%',
                        left: '50%',
                        transform: 'translateX(-50%) translateY(-50%)'
                    });
                    if (opt.text) {
                        div.find("div").append("<label i18n class='t-cap'>正在加载</label>...");
                    }
                    var css = $.extend({
                        position: 'relative',
                        width: '100%',
                        height: '100%'
                    }, opt.css);

                    div.css(css);

                    langs.trans(div);

                    $elem.html(div);
                    break;
                }
                case "1": {
                    var div = $("<div><img src='/Content/images/loading.0.gif' /></div>");
                    var css = $.extend({}, opt.css);

                    div.css(css);

                    $elem.html(div);
                    break;
                }
            }
        }
    };
});