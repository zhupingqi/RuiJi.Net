define('langs/core', ['jquery', 'object-path'], function ($, objectPath) {
    var langs = {
        'zh': '中文',
        'en': 'English',
        'kor': '한국어',
        'jp': '日本語',
        'de': 'Deutsch',
        'fra': 'Français'
    };
    var path = "/misc/v3/langs/";
    var attrs = ['title', 'placeholder', 'alt', 'value'];
    var def = "zh";

    var module = {
        init: function () {
            var lang = module.get();
            module.set(lang);
        },
        set: function (name) {
            if (langs[name]) {
                for (p in module) {
                    if (!$.isFunction(module[p])) {
                        delete module[p];
                    }
                }
                var d = module.load(path + name + ".js");
                $.extend(module, d);
                window.localStorage.setItem("language", name);
            }
        },
        get: function () {
            return window.localStorage.getItem("language") || def;
        },
        trans: function (obj, opt) {
            if (typeof (obj) === "string") {
                if ($.trim(obj) == "")
                    return "";

                var $obj = $("<div>" + obj + "</div>");
                if ($obj.html() == obj) {
                    if (!module[obj])
                        return obj;

                    return module[obj];
                } else {
                    opt = opt || "i18n";
                    var elems = $obj.find("*[" + opt + "]");
                    $.map(elems, function (elem) {
                        elem = $(elem);
                        var t = elem.text();
                        if (t == elem.html())
                            elem.text(module.trans(t));

                        $.map(attrs, function (a) {
                            if (elem.attr(a)) {
                                elem.attr(a, module.trans(elem.attr(a)));
                            }
                        });
                    });

                    return $obj.html();
                }
            }

            if (obj instanceof jQuery) {
                opt = opt || "i18n";
                var elems = obj.find("*[" + opt + "]");
                $.map(elems, function (elem) {
                    elem = $(elem);
                    var t = elem.text();
                    elem.text(module.trans(t));

                    $.map(attrs, function (a) {
                        if (elem.attr(a)) {
                            elem.attr(a, module.trans(elem.attr(a)));
                        }
                    });
                });
                //return;
            }

            if (typeof (obj) === "object") {
                if (!opt) {
                    return;
                }

                if (obj.length) {
                    for (var i = 0; i < obj.length; i++) {
                        module.trans(obj[i], opt);
                    }
                }
                else {
                    if (typeof (opt) === "string") {
                        var name = objectPath.get(obj, opt);
                        var t = module.trans(name);

                        objectPath.set(obj, opt, t);
                    }

                    if (opt instanceof Array) {
                        $.map(opt, function (o) {
                            var name = objectPath.get(obj, o);
                            var t = module.trans(name);

                            objectPath.set(obj, o, t);
                        });
                    }
                }
            }
        },
        getLangs: function () {
            return langs;
        },
        load: function (url) {
            var d = {};

            $.ajax({
                url: url,
                dataType: 'json',
                async: false,
                success: function (json) {
                    d = json;
                }
            });

            return d;
        }
    };

    module.init();

    return module;
});