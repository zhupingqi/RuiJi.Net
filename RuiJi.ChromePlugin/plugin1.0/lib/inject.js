/// <reference path="jquery-1.11.0-vsdoc.js" />
var template = "";
var items = [{
    title: '栏&nbsp;&nbsp;&nbsp;&nbsp;目',
    name: 'railling'
}, {
    title: '标&nbsp;&nbsp;&nbsp;&nbsp;题',
    name: 'title'
}, {
    title: '日&nbsp;&nbsp;&nbsp;&nbsp;期',
    name: 'newsdate'
}, {
    title: '来&nbsp;&nbsp;&nbsp;&nbsp;源',
    name: 'sourcemedia'
}, {
    title: '记&nbsp;&nbsp;&nbsp;&nbsp;者',
    name: 'reporter'
}, {
    title: '正&nbsp;&nbsp;&nbsp;&nbsp;文',
    name: 'content'
}, {
    title: '翻&nbsp;&nbsp;&nbsp;&nbsp;页',
    name: 'page'
}, {
    title: '阅读量',
    name: 'reads'
}];

var itemTypes = [{
    name: 'csquery',
    template: 'csquery',
    'default': ["", "", "html"],
    callback: function (elem, v, isr) {
        var result;
        if (isr) {
            elem = elem || $(document);
            var el = $("<doc>" + elem + "</doc>");
            el.find(v[0]).remove();
            result = el;
        } else {
            elem = $("<doc>" + elem + "</doc>") || $(document);
            result = $(elem).find(v[0]);
        }

        switch (v[2]) {
            case "html": {
                var r = "";
                $.each(result, function (i, n) {
                    r += $(n).html() + "\r\n";
                });
                return r;
                break;
            }
            case "text": {
                var r = "";
                $.each(result, function (i, n) {
                    r += $(n).text() + "\r\n";
                });
                return r;
                break;
            }
            case "attr": {
                var r = "";
                $.each(result, function (i, n) {
                    r += $(n).attr(v[1]) + "\r\n";
                });
                return r;
                break;
            }
            default: {
                return result.text();
                break;
            }
        }
    }
}, {
    name: 'regex',
    template: 'regex',
    callback: function (elem, v, isr) {
        var r = new RegExp(v[0], "gi");
        if (isr) {
            return elem.replace(r, "");
        } else {
            var val = v[1].split(",");
            var result = "";
            var rexec = r.exec(elem)
            $.each(val, function (i, n) {
                if (!isNaN(n)) {
                    result += rexec[parseInt(n)] + " ";
                }
            });
            return result;
        }

    }
}, {
    name: 'split',
    template: 'split',
    callback: function (elem, v, isr) {
        if (!elem || elem.length < 1)
            return "";
        var val = v[1].split(",");
        var result = "";
        var separator = new RegExp(v[0], "gi").exec(elem)[0];
        var splitArr = skipEmptyElementForArray(elem.split(separator));
        if (isr) {
            $.each(splitArr, function (i, n) {
                var index = $.inArray(i.toString(), val);
                if (index == -1) {
                    result += n + " ";
                }
            });
        } else {
            $.each(val, function (i, n) {
                result += splitArr[parseInt(n)] + " ";
            });
        }
        return result;
    }
}, {
    name: 'text',
    template: 'text',
    callback: function (elem, v, isr) {
        if (!elem || elem.length < 1)
            return "";
        var result = elem;
        var indexVal = new RegExp(v[0], "gi").exec(elem)[0];
        var lastIndexVal = new RegExp(v[1], "gi").exec(elem)[0];
        if (isr) {
            var start = indexVal == "" ? 0 : elem.indexOf(indexVal) >= 0 ? elem.indexOf(indexVal) + indexVal.length : 0;
            var end = lastIndexVal == "" ? elem.length - 1 : elem.lastIndexOf(lastIndexVal) > 0 ? elem.lastIndexOf(lastIndexVal) - 1 : elem.length - 1;
            result = elem.replace(elem.substring(start, end), "");
        } else {
            var start = indexVal == "" ? 0 : elem.indexOf(indexVal) >= 0 ? elem.indexOf(indexVal) + indexVal.length : 0;
            var end = lastIndexVal == "" ? elem.length - 1 : elem.lastIndexOf(lastIndexVal) > 0 ? elem.lastIndexOf(lastIndexVal) : elem.length;
            result = elem.substring(start, end);
        }


        return result;
    }
}, {
    name: 'exclude',
    template: 'exclude',
    callback: function (elem, v) {
        if (!elem || elem.length < 1)
            return "";

        var result = elem;
        switch (v[1]) {
            case "start":
                if (new RegExp("^" + v[0] + ".*").test(elem)) {
                    var reg = new RegExp("^" + v[0]);
                    result = elem.replace(reg, "");
                }
                break;
            case "end":
                if (new RegExp(".*" + v[0] + "$").test(elem)) {
                    var last = elem.lastIndexOf(v[0]);
                    var reg = new RegExp(v[0] + "$");
                    result = elem.replace(reg, "");
                }
                break;
            default:
                var reg = new RegExp(v[0], "g");
                result = elem.replace(reg, "");
                break;
        }
        return result;
    }
}, {
    name: 'replace',
    template: 'replace',
    callback: function (elem, v) {
        var reg = new RegExp(v[0], "g");
        result = elem.replace(reg, v[1]);
        return result;
    }
}];

var rules;

var itemTypeDefault = new Array("csquery", "regex", "split", "text", "exclude", "replace");
var csqueryResults = new Array("html", "text", "attr");
var inject = false;

var processResult;
//var excludeUrls = [{
//    host: "localhost"
//}, {
//    host: "baidu.com"
//}];

(function (window) {
    if (self != top)
        return;

    //for (var i in excludeUrls) {
    //    var es = excludeUrls[i];
    //    if (es.pathname) {
    //        if (document.location.host == es.host && document.location.pathname.toLowerCase() == es.pathname) {
    //            return;
    //        }
    //    } else {
    //        if (document.location.host == es.host)
    //            return;
    //    }
    //}

    chrome.extension.sendRequest({ cmd: "getTemplate" }, function (response) {
        template = "<mj-se-template>" + response.html + "</mj-se-template>";
        init();
    });

    $(document).on("mousedown", "ul.inject-item-type-ul a", function () {
        var $this = $(this);
        var t = $this.attr("t");

        switch (t) {
            case "add": {
                var item = $(getTemplate("template-tool-item-csquery")).find(".inject-input-group");
                item.attr("t", "csquery");
                $this.closest("div.inject-row").append(item);
                loadItemType($this.closest("div.inject-row"));
                break;
            }
            case "remove": {
                $this.closest(".inject-input-group").remove();
                break;
            }
            default: {
                changeItemType($this.closest(".inject-input-group"), $this, t);
            }
        }
    });

    $(document).on("click", ".csquery-result-type i", function () {
        var $this = $(this);
        var cls = $this.attr("class").replace("fa csquery-result-", "");
        var index = csqueryResults.indexOf(cls);
        if (index == csqueryResults.length - 1) {
            index = 0;
        } else {
            index++;
        }

        var t = csqueryResults[index];
        $this.removeAttr("class");
        $this.addClass("fa csquery-result-" + t);

        var g = $(this).closest("div.inject-input-group");
        g.find("input.form-csquery-result-type").val(t);

        if (t == "attr") {
            g.find(":text").addClass("input-item-6");
            g.find(":text").removeClass("inject-hide");
        } else {
            g.find(":text").removeClass("input-item-6");
            g.find(":text").last().addClass("inject-hide");
        }
    });

    $(document).on("click", "div.inject-list span", function () {
        if ($(this).hasClass("fa-plus")) {
            if (rules.length == $("div.inject-list span").size() - 1) {
                $("<span>" + (rules.length + 1) + "</span>").insertBefore("div.inject-list span.fa-plus");
            }
            $("div.inject-list span").eq(rules.length).click();
        } else {
            load(parseInt($(this).text()) - 1);

            $("div.inject-list span").removeClass("active");
            $(this).addClass("active");
        }
    });

    $(document).on("click", "span.inject-input-group-btn > button", function () {
        var $this = $(this);
        if ($this.find("i.fa-bullseye").size() == 0)
            return;

        $("span.inject-input-group-btn > button").not($this).removeClass("active");
        $this.toggleClass("active");
        if ($this.hasClass("active") && !inject) {
            $("div.inject-footer-tool span:eq(0)").click();
        }
    });

    $(document).on("click", function (e) {
        if (!inject)
            return;

        var t = $(e.target);
        if (t.closest(".jsPanel,.sweet-alert").size() > 0)
            return false;

        if (t.text() == "")
            return false;

        $(".inject-selected").removeClass("inject-selected");

        selectElement(t);
        e.stopPropagation();
        return false;
    });

    $(document).on("click", "div.inject-footer-tool span:eq(0)", function () {
        if ($(this).hasClass("fa-unlink")) {
            inject = true;
            $(this).removeClass("fa-unlink").addClass("fa-link");
        } else {
            inject = false;
            $(this).removeClass("fa-link").addClass("fa-unlink");
        }

        if (!inject) {
            $(".inject-selected").removeClass("inject-selected");
        }
    });

    $(document).on("click", "div.inject-footer-tool span.fa-save", function () {
        var data = getData();
        if (data.length > 0) {
            $.ajax({
                url: 'http://inspect.setup.miaojian.net/api/extractRule/add',
                data: JSON.stringify(data),
                type: 'post',
                dataType: 'json',
                headers: {
                    Accept: "application/json",
                    "Content-Type": "application/json"
                },
                processData: false,
                cache: false
            }).done(function (result) {
                if (result.data > 0) {
                    var p = $("div.inject-list span[class='active']").text();
                    if (!rules[p]) {
                        var iditem = $.Enumerable.From(data).SingleOrDefault(-1, "x=>x.name == 'id'");
                        iditem.values[0].values = [result.data];
                        rules.push(data);
                    } else {
                        rules[p] = data;
                    }
                    swal({ title: "保存成功", type: "success", timer: 3000 });
                } else {
                    swal({ title: result.msg, type: "error", timer: 3000 });
                }
            });
        }
    });

    $(document).on("click", "div.inject-footer-tool span.fa-external-link", function () {
        var url = $("div.inject-row[inject-item-name='source'] input").val();
        window.open(url);
    });

    $(document).on("click", "div.inject-footer-tool span.fa-eye", function () {
        window.open("http://inspect.setup.miaojian.net/api/extractJson3?url=" + encodeURIComponent(window.location));
    });

    $(document).on("click", "div.inject-footer-tool span.fa-times-circle", function () {
        swal({
            title: "确定是否要删除此条规则？",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "确定删除",
            cancelButtonText: "取消",
            closeOnConfirm: false
        }, function (isConfirm) {
            if (isConfirm) {
                var id = $(".inject-row[inject-item-name='id'] input").val();
                if (id != "" && id > 0) {
                    $.get("http://inspect.setup.miaojian.net/api/extractRule/remove?id=" + id, function (result) {
                        if (result.data) {
                            swal({ title: "删除成功", type: "success", timer: 3000 });
                            var t = $("div.inject-list span[class='active']");
                            var p = parseInt(t.text());
                            rules.splice(p - 1, 1);
                            initPage();
                        } else {
                            swal({ title: "删除失败", text: result.msg, type: "error", timer: 3000 });
                        }
                    });
                } else {
                    swal({ title: "删除成功", type: "success", timer: 3000 });
                    initPage();
                }

            }
        });
    });

    $(document).on("click", "div.excludtype-btn-group button", function () {
        var th = $(this);
        th.closest("div.excludtype-btn-group").find("button").removeClass("active");
        th.addClass("active");
        th.closest("div.inject-input-group").find(".inject-excludeType").val(th.attr("t"));
    });

    $(document).on("click", "div.select-actions button", function () {
        var act = $(this).attr("class").split(" ")[1];
        var t = $(".inject-selected");
        if (t.size() == 0)
            return;

        switch (act) {
            case "fa-outdent": {
                t = t.parent();
                break;
            }
            case "fa-indent": {
                t = t.children().first();
                break;
            }
            case "fa-arrow-up": {
                t = t.prev();
                break;
            }
            case "fa-arrow-down": {
                t = t.next();
                break;
            }
        }

        if (t.length == 0 || t.is("html")) {
            swal({ title: "选取失败", type: "warning", timer: 3000 });
        } else {
            $(".inject-selected").removeClass("inject-selected");
            selectElement(t);
        }
    });

    $(document).on("click", "div.inject-input-group .inject-item-title", function () {
        var g = $(this).closest("div.inject-input-group");
        if (g.attr("t") == "csquery") {
            $(".inject-selected").removeClass("inject-selected");
            selectElement($(document).find(g.find("input").eq(0).val()));
        }

    });

    $(document).on("click", "div.inject-row[inject-item-name] input", function () {
        var $this = $(this);
        //var row = $this.closest("div.inject-row");
        var groups = [];
        var $thisGroup = $this.closest("div.inject-input-group")
        $("div.inject-step").html("");
        var prevGroup = $thisGroup;
        var tmp;
        for (; ;) {
            groups.unshift(prevGroup);
            prevGroup = prevGroup.prev("div.inject-input-group");
            if (prevGroup.size() == 0) {
                break;
            }
        }

        try {
            $.each(groups, function (i, n) {
                n = $(n);
                var t = n.attr("t");
                var isremove = n.find(".fa-minus-square").hasClass("isremove");
                var v = n.find("input").map(function () {
                    return $(this).val()
                });

                var type = $.Enumerable.From(itemTypes).SingleOrDefault(-1, "x=>x.name == '" + t + "'");
                if (type != -1) {
                    tmp = i == 0 ? $(document.body).html() : tmp;
                    tmp = type.callback(tmp, v, isremove);
                }
            });
        }
        catch (e) { }

        $(".inject-step").val(tmp);
    });

    $(document).on("click", ".inject-input-group .fa-minus-square", function () {
        $(this).toggleClass("isremove");
    });

    $(document).on("focus", "button.dropdown-toggle", function () {
        var $this = $(this);
        var g = $this.closest("span");
        var isOpen = g.hasClass("open");

        $("button.dropdown-toggle").parent().removeClass("open");
        if (!isOpen)
            g.addClass("open");
    });

    $(document).on("blur", "button.dropdown-toggle", function () {
        $("button.dropdown-toggle").parent().removeClass("open");
    });

})(window);

function getTemplate(name) {
    var html = $(template).find("#" + name).html();
    return html;
}

function init() {
    var contentTemplate = getTemplate("template-tool");
    var item_template = getTemplate("template-tool-item-csquery");
    var item_type_template = getTemplate("template-tool-item-type");
    var footer_template = getTemplate("template-tool-footer");
    var actions = getTemplate("template-tool-actions");

    $(document.body).append("<inject-bootscrap></inject-bootscrap>");

    var section = $("inject-bootscrap");

    var main = $.jsPanel({
        //container : section,
        headerControls: {
            maximize: 'remove',
            minimize: 'remove',
            close: 'disable'
        },
        theme: 'primary',
        content: contentTemplate,
        contentSize: {
            width: 400,
            height: 650
        },
        headerTitle: 'title',
        position: {
            my: 'right-top',
            at: 'right-top',
            of: 'window',
            offsetY: 60,
            offsetX: -20
        },
        draggable: {
            stop: function (event, ui) {
                tools.reposition(toolsPosition);
            }
        },
        resizable: {
            stop: function (event, ui) {
                tools.reposition(toolsPosition);
            }
        },
        onsmallified: function () {
            $(tools).hide();
        },
        onunsmallified: function () {
            $(tools).show();
            tools.reposition(toolsPosition);
        },
        contentOverflow: { horizontal: 'hidden', vertical: 'auto' },
        footerToolbar: function (footer) {
            return footer_template;
        }
    });

    var toolsPosition = {
        my: 'right-top',
        at: 'left-top',
        of: main,
        offsetY: 45,
        offsetX: -2
    };

    var tools = $.jsPanel({
        //container: section,
        contentSize: { width: 200, height: 400 },
        headerRemove: true,
        resizable: 'disabled',
        draggable: { handle: 'div.jsPanel-content' },
        content: actions,
        position: toolsPosition,
        contentOverflow: { horizontal: 'hidden', vertical: 'auto' },
        callback: function (panel) {

        }
    });

    $(window).scroll(function () {
        tools.reposition(toolsPosition);
    });

    $.each(items, function (i, n) {
        var $item = $(item_template);
        $item.find(".inject-item-title").html(n.title);
        $item.closest("div.inject-row").attr("inject-item-name", n.name);
        $item.find("div.inject-input-group").attr("t", "csquery");
        main.content.append($item);
    });

    //var d = [{
    //    name: 'for',
    //    values: [{
    //        type: 'text',
    //        values: ["http://www.baidu.com/"]
    //    }]
    //}, {
    //    name: 'feature',
    //    values: [{
    //        type: 'include',
    //        values: ["以下内容来自微信公众平台"]
    //    }]
    //}, {
    //    name: 'region',
    //    values: [{
    //        type: 'csquery',
    //        values: ["body"]
    //    }, {
    //        type: 'text',
    //        values: ["<body>", "</body>"]
    //    }, {
    //        type: 'text',
    //        values: ["<body>", "</body>"]
    //    }]
    //}];

    $.get("http://inspect.setup.miaojian.net/api/extractRule/getbyurl?url=" + window.location, function (result) {
        rules = result.data;
        load();

        initPage();

        //if (result.data && result.data.length > 0) {
        //    d = result.data;

        //    load(d[0]);
        //    initPage(d.length);
        //} else {
        //    load([]);
        //    initPage(1);
        //}
    });
}

function initPage() {
    var length = rules.length;

    $("div.inject-list span:not(.fa-plus)").remove();

    if (length == 0) {
        $("div.inject-list span.fa-plus").click();
        return;
    }

    var pages = "";
    for (var i = 1; i <= length; i++) {
        pages += "<span>" + i + "</span>";
    }

    $("div.inject-list").prepend(pages);
    var activeSource = window.location;
    var index = 0;

    if (rules.length > 0) {
        $.each(rules, function (i, n) {
            var source = $.Enumerable.From(n).SingleOrDefault(-1, "x=>x.name == 'source'");
            if (source.values[0].values[0] == window.location) {
                index = i;
            }
        });
    }

    $("div.inject-list span").eq(index).click();
}

function renderPage(page) {
    var len = rules.length > 0 ? rules.length : 1;
    page = page || 0;

    var pages = "";
    for (var i = 1; i <= len; i++) {
        pages += "<span>" + i + "</span>";
    }

    $("div.inject-list").prepend(pages);
    $("div.inject-list").find("span").eq(page).addClass("active");
}

function load(index) {
    index = index || 0;
    d = rules[index];
    d = d || [];

    if (d.length == 0) {
        d = [{
            name: 'id',
            values: [{
                type: 'hidden',
                values: [0]
            }]
        }, {
            name: 'source',
            values: [{
                type: 'hidden',
                values: [window.location]
            }]
        }, {
            name: 'feature',
            values: [{
                type: 'csquery',
                values: ["", "", "text"]
            }]
        }, {
            name: 'region',
            values: [{
                type: 'csquery',
                values: ["", "", "text"]
            }]
        }];

        $.each(items, function (i, n) {
            if (n.name == "content") {
                d.push({
                    name: n.name,
                    values: [{
                        type: 'csquery',
                        values: ["", "", "html"]
                    }]
                });
            }
            else {
                d.push({
                    name: n.name,
                    values: [{
                        type: 'csquery',
                        values: ["", "", "text"]
                    }]
                });
            }
        });
    }

    $(".inject-input-group > input").val("");
    $("div.inject-row").each(function (i, n) {
        $(n).find("div.inject-input-group:gt(0)").remove();
    });

    $(".inject-input-group li").removeClass("active");

    $.each(d, function (i, n) {
        var item = $("div.inject-row[inject-item-name='" + n.name + "']");

        $.each(n.values, function (j, m) {
            var im = item.find("div.inject-input-group").eq(j);
            var type = $.Enumerable.From(itemTypes).SingleOrDefault(-1, "x=>x.name == '" + m.type + "'");

            if (im.size() == 0) {
                var $item_type = $(type.template ? getTemplate("template-tool-item-" + type.template) : getTemplate("template-tool-item-common"));

                item.append($item_type.children());
                im = item.find("div.inject-input-group").eq(j);
            }

            if (m.remove) {
                im.find("span.fa-minus-square").addClass("isremove");
            }

            $.each(m.values, function (index, v) {
                im.find("input").eq(index).val(v);
                if (index == 1) {
                    if (im.find("div.excludtype-btn-group").size() > 0) {
                        im.find("div.excludtype-btn-group button[t='" + v + "']").addClass("active");
                    }
                }
                if (index == 2) {
                    if (im.find("div.csquery-result-type").size() > 0) {
                        if (v) {
                            im.find("div.csquery-result-type i").removeAttr("class");
                            im.find("div.csquery-result-type i").addClass("fa csquery-result-" + v);
                        }
                    }
                }
            });

            im.attr("t", m.type);
            if (m.type == "hidden") {
                item.hide();
            }
        });
    });

    loadItemType();
}

function loadItemType(row) {
    var rows = row || $("div.inject-row");
    var template = getTemplate("template-tool-item-type");

    $.each(rows, function (i, n) {
        n = $(n);
        var injectTypes = n.attr("inject-types");
        var ary = [];

        if (!injectTypes) {
            ary = itemTypeDefault;
        } else {
            if (injectTypes != "none") {
                ary = injectTypes.split(',');
            } else {
                return;
            }
        }

        var types = $(template);
        $.each(types.find("li > a"), function (j, m) {
            m = $(m);
            var t = m.attr("t");

            if (t == "add" || t == "remove")
                return;
            if (!ary.contains(t)) {
                m.closest("li").remove();
            }
        });

        var btn = (row ? row.find(".inject-input-group-btn") : n.find(".inject-input-group-btn")).not(":has(ul)");
        btn.prepend(types);

        n.find("div.inject-input-group").each(function (j, m) {
            m = $(m);
            var t = m.attr("t");
            if (!t || t == "") {
                t = "csquery";
            }
            m.find("a[t='" + t + "']").closest("li").addClass("active");
            m.find("button span.inject-item-type-short").text(m.find("ul a[t='" + t + "'] span:first").text());
            if (t != "csquery") {
                m.find("i.fa-bullseye").closest("button").attr("disabled", "disabled");
            } else {
                m.find("i.fa-bullseye").closest("button").removeAttr("disabled");
            }
        });
    });
}

function changeItemType(subitem, elem, t) {
    if (elem.closest("li").hasClass("active"))
        return;

    var olds = subitem.find("input");

    var type = $.Enumerable.From(itemTypes).SingleOrDefault(-1, "x=>x.name == '" + t + "'");
    var $item_type = $(type.template ? getTemplate("template-tool-item-" + t) : getTemplate("template-tool-item-common"));
    $item_type = $item_type.find("div.inject-input-group");

    var inputs = $item_type.find("input");

    $.each(olds, function (i, n) {
        n = $(n);
        if (inputs.eq(i)) {
            inputs.eq(i).val(n.val());
        }
    });

    subitem.children().not("span.inject-item-title,span.inject-input-group-btn").remove();
    $item_type.find("span.inject-item-title,span.inject-input-group-btn").remove();
    $item_type.children().insertBefore(subitem.find("span.inject-input-group-btn"));

    elem.closest("ul").prev("button").html(elem.find("span:first").text() + " <span class=\"inject-caret\">");
    elem.closest("ul").find("li").removeClass("active");
    elem.closest("li").addClass("active");

    if (t != "csquery") {
        subitem.find("i.fa-bullseye").closest("button").removeClass("active");
        subitem.find("i.fa-bullseye").closest("button").attr("disabled", "disabled");
        $(".inject-selected").removeClass("inject-selected");
        inject = false;
    } else {
        subitem.find("i.fa-bullseye").closest("button").removeAttr("disabled");
    }
    subitem.attr("t", t);
}

function selectElement(elem) {
    var es = elem.ellocate3();
    $("button.active").has("i.fa-bullseye").closest("div.inject-input-group").find("input:first").val(es.css);
    $("#inject-elem-size").text($(es.css).size());
    elem.addClass("inject-selected");
}

function getData() {
    var rows = $(".inject-row");
    var data = [];
    $.each(rows, function (i, n) {
        var $n = $(n);
        var groups = $n.find(".inject-input-group");
        if (groups.size() > 0) {
            var dd = { name: $n.attr("inject-item-name"), values: [] };
            $.each(groups, function (j, m) {
                var $m = $(m);
                var ir = $(m).find("span.fa-minus-square").hasClass("isremove");
                var ditem = { remove: ir, type: $m.attr("t"), values: $m.find("input").map(function () { return $(this).val() }).get() };
                dd.values.push(ditem);
            });
            data.push(dd);
        }
    });
    return data;
}

function skipEmptyElementForArray(arr) {
    var a = [];
    $.each(arr, function (i, n) {
        var data = $.trim(n);
        if ("" != data) {
            a.push(data);
        }
    });
    return a;
}