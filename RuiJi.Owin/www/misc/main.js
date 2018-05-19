requirejs.config({
    urlArgs: 'ver=1.0.0.0',
    paths: {
        jquery: '/scripts/jquery-3.1.1.min',
        'tabs': '/scripts/require-tabs/require.tabs'
    },
    map: {
        '*': {
            'css': '/scripts/css.min.js'
        }
    },
    shim: {
        'tabs': {
            deps: ['css!/scripts/require-tabs/tabs.css', 'css!fonts/font-awesome.min.css']
        }
    }
});

require(['/misc/v1/entry.js']);