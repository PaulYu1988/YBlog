(function ($) {
    $.fn.Order = function (options) {
        var settings = $.extend({
            cols: []
        }, options);

        return this.each(function () {
            if (!settings.cols) {
                return;
            }
            var params = new URLSearchParams(window.location.search);
            var orderBy = params.get('orderby');
            var orderDirection = params.get('orderdirection');
            var cols = $(this).find('[orderby]');
            if (cols.length > 0) {
                cols.each(function () {
                    var currOrderBy = $(this).attr('orderby');
                    if (settings.cols.includes(currOrderBy)) {
                        $(this).css('cursor', 'pointer');
                        if (orderBy == currOrderBy) {
                            if (orderDirection == 'ASC') {
                                $(this).append('<i class="layui-icon layui-icon-up"></i>');
                                $(this).on('click', function () {
                                    params.set('orderdirection', 'DESC');
                                    document.location = window.location.origin + window.location.pathname + '?' + params.toString();
                                });
                            } else {
                                $(this).append('<i class="layui-icon layui-icon-down"></i>');
                                $(this).on('click', function () {
                                    params.set('orderdirection', 'ASC');
                                    document.location = window.location.origin + window.location.pathname + '?' + params.toString();
                                });
                            }
                        } else {
                            $(this).on('click', function () {
                                params.set('orderdirection', 'DESC');
                                params.set('orderby', currOrderBy);
                                params.delete('page');
                                params.delete('query.page');
                                document.location = window.location.origin + window.location.pathname + '?' + params.toString();
                            });
                        }
                    }
                });
            }
        });
    };
}(jQuery));