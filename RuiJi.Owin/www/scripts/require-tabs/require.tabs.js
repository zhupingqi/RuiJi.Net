define(['jquery'], function ($) {
    var panelIdPrefix = "tab_panel_";
    var auto = 0;;
    var op = {
        current: -1,
        click: {}
    };

    var module = {
        init: function (opt, fn) {
            if (opt.css)
                module.loadCss(opt.css);

            opt = $.extend({}, op, opt);
            var $container = $(opt.container);
            var ul;

            $container.addClass("ui-require-tabs");

            if (opt.container && opt.tabs) {
                ul = $("<ul />");
                $container.append(ul);

                $.map(opt.tabs, function (tab) {
                    tab.id = tab.id || "auto_" + auto++;

                    var href = opt.prefix ? opt.prefix + "_" + tab.id : tab.id;
                    tab.fixId = href;
                    tab.liId = href + "_li";
                    var panelId = panelIdPrefix + href;
                    tab.panelId = panelId;
                    var li = $("<li/>");
                    li.attr("id", tab.liId);
                    var a = $("<a/>");
                    var div = $("<div/>");
                    div.attr("id", panelId);
                    li.append(a);
                    ul.append(li);
                    $container.append(div);

                    tab.li = li;
                    tab.panel = div;
                    tab.type = tab.type || "lib";
                    a.attr("type", tab.type);
                    a.text(tab.text);

                    switch (tab.type) {
                        case "lib": {
                            a.attr("href", "#" + href);
                            break;
                        }
                        case "href": {
                            a.attr("href", "#" + href);
                            a.attr("target", "_blank");
                            break;
                        }
                        case "label": {
                            a.css({ cursor: 'default' });
                            break;
                        }
                        case "fn": {
                            a.attr("href", "javascript:;");
                            break;
                        }
                        case "tab": {
                            a.attr("href", "javascript:;");
                            break;
                        }
                        case "for": {
                            a.attr("href", "javascript:;");
                            li.addClass("pull-right");
                            if (tab.for) {
                                li.attr("for", tab.for);
                            }
                            break;
                        }
                    }

                    if (tab.img) {
                        var img = $("<img/>");
                        img.attr("src", tab.img);
                        a.append(img);
                    }

                    if (tab.html) {
                        a.html(tab.html);
                    }

                    if (tab.css) {
                        $.map(tab.css, function (c) {
                            if ($.trim(c) != "")
                                require(['css!' + c]);
                        });
                    }

                    if (tab.class) {
                        li.addClass(tab.class);
                    }

                    if (tab.fa) {
                        a.prepend("<i class='fa " + tab.fa + "'></i>");
                    }

                    if (tab.title) {
                        a.attr("title", tab.title);
                    }

                    li.data("tab", tab);

                    if (tab.lib == module.getHashLib()) {
                        li.addClass("current");
                        $container.find("div:last").addClass("current");
                    }
                });
            }

            if (opt.container && !opt.tabs) {
                ul = $container.find(">ul:first");
                if (opt.direction) {
                    $container.addClass("ui-require-tabs-" + opt.direction);
                }
                if (opt.current) {
                    ul.find("li").eq(opt.current).trigger("click");
                }
            }

            ul.addClass("ui-require-tabs-nav");
            $container.find("li[for]").hide();

            if ($.isFunction(opt.ready)) {
                opt.ready();
            }

            ul.find("li a").click(function (e) {
                var li = $(this).parent();
                var tab = li.data("tab");
                var href = $(this).attr("href");

                if (!tab) {
                    tab = {
                        type: 'fn'
                    };
                }

                if ($.isFunction(opt.before)) {
                    opt.before(tab);
                }

                if ($.isFunction(tab.before)) {
                    var r = tab.before(tab);
                    if (r === undefined)
                        r = true;
                    if (!r) {
                        return false;
                    }
                }

                switch (tab.type) {
                    case "lib": {
                        li.addClass("current").siblings().removeClass("current");
                        $("#" + tab.panelId).addClass("current").siblings().removeClass("current");

                        module.requestHash(opt.tabs, "#" + tab.lib, opt.prefix);
                        if ($.isFunction(tab.onShow)) {
                            tab.onShow(tab);
                        }
                        if ($.isFunction(opt.click)) {
                            opt.click(tab, li);
                        }
                        $container.find("li[for]").hide();
                        $container.find("li[for='" + tab.id + "']").show();
                        return false;
                        break;
                    }
                    case "tab": {
                        li.addClass("current").siblings().removeClass("current");
                        $("#" + tab.panelId).addClass("current").siblings().removeClass("current");
                        window.location.hash = "";
                        break;
                    }
                    case "req": {
                        require([tab.lib]);
                        break;
                    }
                    case "fn": {
                        var result = true;

                        if ($.isFunction(tab.fn)) {
                            var r = tab.fn(tab);
                            if (r === undefined)
                                r = true;
                            if (!r) {
                                result = false;
                            }
                        }

                        if (result) {
                            li.addClass("current").siblings().removeClass("current");
                            if (tab.panelId)
                                $("#" + tab.panelId).addClass("current").siblings().removeClass("current");
                            else
                                $container.find(">div").eq(li.index()).addClass("current").siblings().removeClass("current");

                            if ($.isFunction(tab.onShow)) {
                                tab.onShow(tab);
                            }
                        }
                        break;
                    }
                }

                if ($.isFunction(opt.click)) {
                    if (tab.type != "label" && tab.type != "href") {
                        opt.click(tab, li);
                    }
                }

                if ($.isFunction(tab.after)) {
                    tab.after(tab);
                }

                if ($.isFunction(opt.after)) {
                    opts.after(tab);
                }
            });

            module.requestHash(opt.tabs, window.location.hash, opt.prefix);

            if (ul.find("li.current").length == 0) {
                var eq = 0;
                if (opt.current > -1)
                    eq = opt.current;
                ul.find("a[type='lib'],a[type='fn'],a[type='tab']").eq(eq).trigger("click");
            }

            if (ul.find("li.current").length > 0) {
                var tab = ul.find("li.current").eq(0).data("tab");
                if (tab)
                    $container.find("li[for='" + tab.id + "']").show();
            }

            if ($.isFunction(fn)) {
                fn();
            }
        },
        requestHash: function (tabs, href, prefix) {
            if (href !== "" && href.substr(0, 1) == "#") {
                var lib = href.substr(1);
                var uri = new URL(window.location.origin + "/" + lib);
                lib = uri.pathname.substr(1);

                for (var i = 0; i < tabs.length; i++) {
                    var d = tabs[i];
                    if (d.lib == lib) {
                        require([lib]);
                        uri = new URL(window.location.origin + "/" + window.location.hash.substr(1));
                        window.location.hash = d.lib + uri.search;
                        break;
                    }
                }
            }
        },
        loadCss: function (path) {
            require(['css!' + path]);
        },
        getHashLib: function () {
            if (window.location.hash.length == 0)
                return "";

            var uri = new URL(window.location.origin + "/" + window.location.hash.substr(1));
            return uri.pathname.substr(1);
        }
    };

    return module;
});