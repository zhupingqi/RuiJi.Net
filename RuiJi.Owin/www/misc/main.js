requirejs.config({
    urlArgs: 'ver=1.0.0.0',
    baseUrl : "/misc",
    paths: {
        jquery: '/scripts/jquery-3.1.1.min',
        template: '/scripts/template-web',
        bootstrap: '/scripts/bootstrap-3.3.5/js/bootstrap',
        bootstrapTable: '/scripts/bootstrap-table/bootstrap-table.min',
        'tabs': '/scripts/require-tabs/require.tabs',
        'tree': '/scripts/jstree/jstree.min'
    },
    map: {
        '*': {
            'css': '/scripts/css.min.js'
        }
    },
    shim: {
        'bootstrap': {
            exports: 'bootstrap'
        },
        'bootstrapTable': {
            exports: 'bootstrapTable',
            deps: ['bootstrap', 'css!/scripts/bootstrap-table/bootstrap-table.min.css']
        },
        'tabs': {
            deps: ['css!/scripts/require-tabs/tabs.css', 'css!/misc/font-awesome.min.css']
        },
        'tree': {
            deps: ['css!/scripts/jstree/themes/default/style.min.css','jquery']
        }
    }
});

require(['proto']);
require(['entry']);