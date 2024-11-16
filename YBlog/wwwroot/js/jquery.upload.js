(function ($) {
    $.fn.initUpload = function (options) {
        var settings = $.extend({
            name: '',
            uploadButtonText: 'Choose File',
            labelText: 'Upload',
            defaultUrl: ''
        }, options);

        return this.each(function () {
            if (!settings.name) {
                return;
            }

            var label = document.createElement('label');
            label.className = 'layui-form-label';
            label.textContent = settings.labelText;

            var divContainer = document.createElement('div');
            divContainer.className = 'layui-input-block';

            var uploadButton = document.createElement('button');
            uploadButton.type = 'button';
            uploadButton.className = 'layui-btn layui-btn-normal';
            uploadButton.onclick = () => {
                inputFile.click();
            };
            var uploadButtonIcon = document.createElement('i');
            uploadButtonIcon.className = 'layui-icon layui-icon-upload';
            uploadButton.append(uploadButtonIcon);
            uploadButton.append(settings.uploadButtonText);

            var inputHidden = document.createElement('input');
            inputHidden.type = 'hidden';
            inputHidden.name = settings.name;
            inputHidden.value = settings.defaultUrl;

            var deleteButton = document.createElement('button');
            deleteButton.type = 'button';
            deleteButton.className = 'layui-btn layui-btn-primary layui-border-red';
            deleteButton.textContent = '删除';
            deleteButton.onclick = () => {
                deleteButton.classList.add('layui-hide');
                preview.classList.add('layui-hide');
                previewImg.removeAttribute('src');
                inputFile.value = '';
                inputHidden.value = '';
            };
            if (!settings.defaultUrl) {
                deleteButton.classList.add('layui-hide');
            }

            var preview = document.createElement('blockquote');
            preview.className = 'layui-elem-quote layui-quote-nm';
            preview.style.marginTop = '16px';
            if (!settings.defaultUrl) {
                preview.classList.add('layui-hide');
            }
            var previewDiv = document.createElement('div');
            previewDiv.className = 'layui-upload-list';
            var previewImg = document.createElement('img');
            previewImg.style.maxWidth = '100%';
            if (settings.defaultUrl) {
                previewImg.src = settings.defaultUrl;
            }
            previewDiv.append(previewImg);
            preview.append(previewDiv);

            var inputFile = document.createElement('input');
            inputFile.type = 'file';
            inputFile.setAttribute('accept', 'image/*');
            inputFile.name = settings.name + 'File';
            inputFile.className = 'layui-hide';
            inputFile.onchange = () => {
                var file = inputFile.files[0];
                if (settings.extensionPattern) {
                    if (!settings.extensionPattern.test(file.name)) {
                        layui.layer.alert('不支持的文件后缀名。');
                        return;
                    }
                }
                if (!file) {
                    if (settings.defaultUrl) {
                        previewImg.src = settings.defaultUrl;
                    } else {
                        if (!$(preview).hasClass('layui-hide')) {
                            $(preview).addClass('layui-hide');
                        }
                        if (!$(deleteButton).hasClass('layui-hide')) {
                            $(deleteButton).addClass('layui-hide');
                        }
                    }
                    return;
                }
                if (file.type.startsWith('image')) {
                    if (window.URL && typeof window.URL.createObjectURL === 'function') {
                        if ($(preview).hasClass('layui-hide')) {
                            $(preview).removeClass('layui-hide');
                        }
                        if ($(deleteButton).hasClass('layui-hide')) {
                            $(deleteButton).removeClass('layui-hide');
                        }
                        previewImg.src = window.URL.createObjectURL(file);
                    } else {
                        layui.layer.alert('浏览器不支持URL.createObjectURL()，无法预览。');
                    }
                } else {
                    if (!$(preview).hasClass('layui-hide')) {
                        $(preview).addClass('layui-hide');
                    }
                    if (!$(deleteButton).hasClass('layui-hide')) {
                        $(deleteButton).addClass('layui-hide');
                    }
                    layui.layer.alert('文件不支持预览');
                }
            };

            divContainer.append(uploadButton);
            divContainer.append(deleteButton);
            divContainer.append(inputHidden);
            divContainer.append(inputFile);
            divContainer.append(preview);
            $(this).append(label);
            $(this).append(divContainer);
        });
    };
}(jQuery));