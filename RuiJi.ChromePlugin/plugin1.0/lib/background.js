/// <reference path="jquery-1.11.0-vsdoc.js" />

var version = '';
var keywordMap = {};
var newVersion = false;
var appId;

; (function (window) {
    if (!chrome.cookies) {
        chrome.cookies = chrome.experimental.cookies;
    }

    appId = chrome.extension.getURL("/").replace("chrome-extension://", "").replace("/", "");

    chrome.management.get(appId, function (d) {
        if (!d) return !1;
        version = d.version;
    });

    bgMonitor();


    if (!chrome.extension.onRequest.hasListener(requestListener)) {
        chrome.extension.onRequest.addListener(requestListener);
    }
})(this);

function bgMonitor() {
    var hasListener = chrome.browserAction.onClicked.hasListener(clickListener);
    if (!hasListener) {
        chrome.browserAction.onClicked.addListener(clickListener);
    }
}

function clickListener(e) {
    window.open("http://www.ruijihg.com/2018/05/08/ruiji-net/");
}

function requestListener(request, sender, sendResponse) {

    if (request.cmd == "getTemplate") {
        sendResponse({ html: $("#template").html() });
    }

    if (request.cmd == "copy") {
        copy(request.urls);
        sendResponse({ result: true });
    }

    if (request.cmd == "getVersion") {
        sendResponse({ version: version });
    }

    if (request.cmd == "getAppId") {
        sendResponse({ appId: appId });
    }

    if (request.cmd == "getUpdateLog") {
        $.get(chrome.extension.getURL("/") + "log.html", function (h) {
            sendResponse({ body: h });
        });
    }
}

function copy(text) {
    $("#clipboard").val(text);
    $("#clipboard").select();
    document.execCommand("Copy");
}

function xmlToJson(xml) {

    // Create the return object
    var obj = {};

    if (xml.nodeType == 1) { // element
        // do attributes
        if (xml.attributes.length > 0) {
            obj["@attributes"] = {};
            for (var j = 0; j < xml.attributes.length; j++) {
                var attribute = xml.attributes.item(j);
                obj["@attributes"][attribute.nodeName] = attribute.nodeValue;
            }
        }
    } else if (xml.nodeType == 3) { // text
        obj = xml.nodeValue;
    }

    // do children
    if (xml.hasChildNodes()) {
        for (var i = 0; i < xml.childNodes.length; i++) {
            var item = xml.childNodes.item(i);
            var nodeName = item.nodeName;
            if (typeof (obj[nodeName]) == "undefined") {
                obj[nodeName] = xmlToJson(item);
            } else {
                if (typeof (obj[nodeName].length) == "undefined") {
                    var old = obj[nodeName];
                    obj[nodeName] = [];
                    obj[nodeName].push(old);
                }
                obj[nodeName].push(xmlToJson(item));
            }
        }
    }
    return obj;
};