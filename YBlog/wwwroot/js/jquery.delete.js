(function ($) {
    $.fn.delete = function (options) {
        var settings = $.extend({
            path: '',
            resultUrl: ''
        }, options);

        return this.each(function () {
            if (!settings.path) {
                return;
            }
            var id = $(this).attr('data-id');
            if (!id) {
                return;
            }
            this.onclick = function () {
                layer.confirm('删除确认？', {
                    btn: ['确定', '关闭']
                }, function () {
                    var data = 'id=' + id;
                    $.ajax({
                        type: 'DELETE',
                        url: settings.path,
                        data: data,
                        dataType: 'json',
                        success: function (result) {
                            layer.msg('删除成功', { icon: 1 }, function () {
                                if (settings.resultUrl) {
                                    document.location = settings.resultUrl;
                                } else {
                                    document.location.reload();
                                }
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            layer.msg('删除失败', { icon: 2 }, function () {
                                if (settings.resultUrl) {
                                    document.location = settings.resultUrl;
                                } else {
                                    document.location.reload();
                                }
                            });
                        }
                    });
                }, function () {

                });
            };
        });
    }
}(jQuery));