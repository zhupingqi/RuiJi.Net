define(['jquery', 'tree'], function ($, tabs, tree) {
    var module = {
        init: function () {
            $("#tab_panel_node").append("<div id='tree'></div>");

            $('#tree').jstree({
                'core': {
                    'data': {
                        'url': '/api/zoo/tree',
                        'data': function (node) {
                            //var path = "/";
                            //if (node.parents.length > 0) {
                            //    path = "";

                            //    $.map(node.parents, function (p) {
                            //        path += ((p == "#") ? "/" : p) + node.text;
                            //    });
                            //}
                            var path = "/";
                            if (node.id != "#")
                                path = node.id;

                            return { 'path': path };
                        }
                    }
                }
            });
        }
    };

    module.init();
});