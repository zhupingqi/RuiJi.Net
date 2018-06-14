'use strict';
var page = require('webpage').create(), system = require('system'), fs = require('fs'), host, port, address, path, username, password;

address = system.args[1];
path = system.args[2];
host = system.args[3];
port = system.args[4];
username = system.args[5];
password = system.args[6];

page.settings = {
    javascriptEnabled: true,
    loadImages: false,
    webSecurityEnabled: false,
    userAgent: 'Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.137 Safari/537.36 LBBROWSER'
};

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

if (host !== "" && port !== "")
    phantom.setProxy(host, port, 'manual', username, password);

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
        },1000);
    }

    checkReadyState();
});   