define('templateHelper', ['jquery', 'template', 'linq'], function ($, template, weiboUtility) {
    //template.helper('dateFormat', function (date, format) {
    //    if (typeof (date) === "string" && date.indexOf("/Date(") == 0)
    //        return dateFromJSON(date).format(format);
    //    if (typeof (date) === "string" && date.indexOf("/Date(") == -1) {
    //        date = date.replace("T", " ").replace("Z", "");
    //        return (new Date(date)).format(format);
    //    }
    //    if (typeof (date) == "date")
    //        return date.format(format);

    //    return "";
    //});

    //template.helper('toFixed', function (numObj, num) {
    //    return numObj.toFixed(num);
    //});

    //template.helper('formatMedia', function (media) {
    //    return media.substr(0, media.length - 3);
    //});

    //template.helper('reporterJoin', function (reporter) {
    //    if (reporter && reporter.length > 0) {
    //        return reporter.join(",");
    //    }
    //    return "-";
    //});

    //template.helper('formatWrap', function (str) {
    //    return str.replace(/\s+/g, " ").replace(/&#013;/g, "").replace("</em><em>", "");
    //});

    //template.helper('highlightTitle', function (title, key) {
    //    var keyArray = key.split(" ");
    //    $.each(keyArray, function (i, n) {
    //        var k = n.replace(/"/g, "");
    //        title = title.replace(new RegExp(k, "g"), "<em>" + k + "</em>");
    //    });
    //    return title.replace(/\s+/g, " ").replace(/&#013;/g, "");
    //});

    //template.helper('getSummary', function (sums, id) {
    //    var summary = "N.A";
    //    if (sums[id]) {
    //        if (sums[id]["content_en"]) {
    //            summary = sums[id]["content_en"][0] + "...";
    //        } else {
    //            if (sums[id]["text_en"]) {
    //                summary = sums[id]["text_en"][0] + "...";
    //            }
    //        }
    //    }
    //    return summary;
    //});

    //template.helper("emptyReturn", function (str) {
    //    if ($.trim(str).length > 0) {
    //        return str;
    //    }
    //    return "-";

    //});

    //template.helper("getMediaRemark", function (remark, ellipsis) {
    //    if ($.trim(remark).length == 0) {
    //        return "";
    //    }
    //    var $remark = $(remark);
    //    var paras = $remark.find(".para");
    //    var result = "";
    //    if (paras.length > 0) {
    //        $.map(paras, function (p) {
    //            result += $(p).text();
    //        });
    //    }
    //    if (ellipsis) {
    //        if (result == "") {
    //            result = $remark.text();
    //        }
    //        result = result.length > 130 ? result.substr(0, 130) + "..." : result;
    //    }

    //    result += "<a href='javascript:void(0)' class='media-remark'>更多</a>";

    //    return result;
    //});

    //template.helper('getLogo', function (logos, id, logoSize) {
    //    var count = $.Enumerable.From(logos).Count("x=>x.mediaId == '" + id + "'");
    //    if (count == 0)
    //        return ""

    //    var m = $.Enumerable.From(logos).First("x=>x.mediaId == '" + id + "'");
    //    return m[logoSize];
    //});

    //template.helper('getMedia', function (medias, id) {
    //    var count = $.Enumerable.From(medias).Count("x=>x.id == '" + id + "'");
    //    if (count == 0)
    //        return ""

    //    var m = $.Enumerable.From(medias).First("x=>x.id == '" + id + "'");
    //    return m["name"];
    //});

    //template.helper('getMediaTransName', function (trans, id) {
    //    var count = $.Enumerable.From(trans).Count("x=>x.mediaId == '" + id + "'");
    //    if (count == 0)
    //        return ""

    //    var m = $.Enumerable.From(trans).First("x=>x.mediaId == '" + id + "'");
    //    return m["name"];
    //});

    //template.helper('getAreas', function (areas, id) {
    //    if (areas[id]) {
    //        return areas[id];
    //    }
    //    return "";
    //});

    //template.helper('isMainMedia', function (subMediaIds, id) {
    //    var count = $.Enumerable.From(subMediaIds).Count("x=>x == '" + id + "'");
    //    if (count == 0)
    //        return "主媒体"

    //    return "从媒体";
    //});

    //template.helper('getRemarks', function (remarks, id) {
    //    if (remarks[id]) {
    //        var remark = remarks[id];
    //        if ($.trim(remark).length == 0) {
    //            return "";
    //        }
    //        var $remark = $(remark);
    //        var paras = $remark.find(".para");
    //        var result = "";
    //        if (paras.length > 0) {
    //            $.map(paras, function (p) {
    //                result += $(p).text();
    //            });
    //        }
    //        if (result == "") {
    //            result = $remark.text();
    //        }
    //        result = result.length > 250 ? result.substr(0, 250) + "..." : result;

    //        return result;
    //    }
    //    return "";
    //});

    //template.helper('jsonStringifyArray', function (datas) {
    //    if (datas) {
    //        return JSON.stringify(datas);
    //    } else {
    //        return "";
    //    }
    //});

    //template.helper('getDisExtInfo', function (id, exts, field) {
    //    var count = $.Enumerable.From(exts).Count("x=>x.mediaDistributionId == '" + id + "'");
    //    if (count == 0)
    //        return ""

    //    var m = $.Enumerable.From(exts).First("x=>x.mediaDistributionId == '" + id + "'");
    //    return m[field];
    //});

    //template.helper('getWeiboUrl', function (uid, mid) {
    //    var url = "https://weibo.com/" + uid + "/" + weiboUtility.mid2url(mid.toString());
    //    return url;
    //});
});
