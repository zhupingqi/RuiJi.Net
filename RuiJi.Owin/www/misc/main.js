requirejs.config({
    urlArgs: 'ver=1.0.0.0',
    baseUrl : "/misc",
    paths: {
        jquery: '/scripts/jquery-3.1.1.min',
        template: '/scripts/template-web',
        'tabs': '/scripts/require-tabs/require.tabs',
        'tree': '/scripts/jstree/jstree.min'
    },
    map: {
        '*': {
            'css': '/scripts/css.min.js'
        }
    },
    shim: {
        'tabs': {
            deps: ['css!/scripts/require-tabs/tabs.css', 'css!fonts/font-awesome.min.css']
        },
        'tree': {
            deps: ['css!/scripts/jstree/themes/default/style.min.css','jquery']
        }
    }
});

require(['proto']);
require(['entry']);