'use strict';
var page = require('webpage').create(), system = require('system'), fs = require('fs'), address, path;

address = system.args[1];
path = system.args[2];

page.settings.javascriptEnabled = true;
page.settings.loadImages = false;
page.settings.webSecurityEnabled = true;
page.settings.XSSAuditingEnabled = false;
page.settings.userAgent = 'Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.137 Safari/537.36 LBBROWSER';

phantom.addCookie({});

function onPageReady() {
    var htmlContent = page.evaluate(function () {
        return document.documentElement.outerHTML;
    });

    try {
        fs.write(path, htmlContent, 'w');
    } catch (e) {
        console.log(e);
    }

    phantom.exit();
}

page.onResourceReceived = function (res) {
    if (res.id == 1) {
        res.charset = document.characterSet;
        res.cookie = document.cookie;
        fs.write(path + ".json", JSON.stringify(res), 'w');
    }
};

page.open(address, function (status) {
    function checkReadyState() {
        setTimeout(function () {
            var readyState = page.evaluate(function () {
                return document.readyState;
            });

            if ("complete" === readyState) {
                onPageReady();
            } else {
                checkReadyState();
            }
        }, 1000);
    }

    checkReadyState();
});   