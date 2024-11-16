(function ($) {
    $.fn.initForm = function (options) {
        var settings = $.extend({
            verify: undefined,
            success: undefined,
            successMessage: '提交成功',
            errorMessage: '提交失败',
            error: undefined,
            complete: undefined,
            autoMessage: true,
            successMessageType: 'alert',
            errorMessageType: 'alert',
            captchaId: 'Captcha',
            captchaName: 'Captcha',
            hasCaptcha: false
        }, options);

        var showAutoMessage = (isSuccess, response) => {
            if (isSuccess) {
                if (settings.successMessageType === 'msg') {
                    layui.layer.msg(settings.successMessage, { icon: 1 }, function () {
                        if (settings.success) {
                            settings.success(response);
                        }
                    });
                } else {
                    var successIndex = layui.layer.alert(settings.successMessage, { icon: 1 }, function () {
                        layer.close(successIndex);
                        if (settings.success) {
                            settings.success(response);
                        }
                    });
                }
            } else {
                if (response?.status === 401) {
                    if (settings.errorMessageType === 'msg') {
                        layui.layer.msg('请先登录', { icon: 2 }, function () {
                            document.location = '/login?from=' + encodeURIComponent(window.location.href);
                        });
                    } else {
                        layui.layer.alert('请先登录', { icon: 2 }, function () {
                            document.location = '/login?from=' + encodeURIComponent(window.location.href);
                        });
                    }
                } else {
                    if (settings.errorMessageType === 'msg') {
                        layui.layer.msg(response.responseJSON?.message ?? settings.errorMessage, { icon: 2 }, function () {
                            if (settings.error) {
                                settings.error(response);
                            }
                        });
                    } else {
                        var errorIndex = layui.layer.alert(response.responseJSON?.message ?? settings.errorMessage, { icon: 2 }, function () {
                            layer.close(errorIndex);
                            if (settings.error) {
                                settings.error(response);
                            }
                        });
                    }
                }
            }
        }

        return this.each(function () {
            layui.use(function () {
                var form = layui.form;
                if (settings.verify) {
                    form.verify(settings.verify);
                }
                form.on('submit', function (data) {
                    var formData = new FormData(data.form);
                    if (settings.hasCaptcha) {
                        if (!formData.has(settings.captchaName)) {
                            var submitBtn = $(data.form).find('button[type="submit"]');
                            var captchaWrapper = document.createElement('div');
                            captchaWrapper.className = 'captcha-wrapper';
                            var captchaImg = document.createElement('img');
                            captchaImg.className = 'captcha-img';
                            captchaImg.setAttribute('src', '/common/captcha?rnd=' + Math.random());
                            captchaImg.onclick = (event) => {
                                $('.captcha-img').attr('src', '/common/captcha?rnd=' + Math.random());
                                $('input[name="' + settings.captchaName + '"]').val('');
                            };
                            var captchaInput = document.createElement('input');
                            captchaInput.type = 'text';
                            captchaInput.name = settings.captchaName;
                            captchaInput.id = settings.captchaId;
                            captchaInput.className = 'layui-input captcha-input';
                            captchaInput.setAttribute('lay-verify', 'required');
                            captchaInput.setAttribute('placeholder', '验证码');
                            captchaInput.setAttribute('lay-reqtext', '请填写验证码');
                            captchaInput.setAttribute('autocomplete', 'off');
                            captchaWrapper.append(captchaImg);
                            captchaWrapper.append(captchaInput);
                            submitBtn.before(captchaWrapper);
                            return false;
                        }
                    }
                    var action = data.form.getAttribute('action');
                    var type = data.form.getAttribute('method') ?? 'post';
                    $.ajax({
                        type: type,
                        url: action,
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        beforeSend: function () {
                            data.elem.setAttribute('disabled', 'disabled');
                            data.elem.classList.add('layui-btn-disabled');
                        },
                        success: function (response) {
                            if (response && response.id) {
                                $('input[name="Id"]').val(response.id);
                            }
                            if (settings.autoMessage) {
                                showAutoMessage(true, response);
                            } else {
                                if (settings.success) {
                                    settings.success(response);
                                }
                            }
                        },
                        error: function (response) {
                            if (settings.autoMessage) {
                                showAutoMessage(false, response);
                            } else {
                                if (settings.error) {
                                    settings.error(response);
                                }
                            }
                        },
                        complete: function () {
                            data.elem.removeAttribute('disabled');
                            data.elem.classList.remove('layui-btn-disabled');
                            if (settings.complete) {
                                settings.complete();
                            }
                        }
                    });
                    return false; // 阻止默认 form 跳转
                });
            });
        });
    }
}(jQuery));