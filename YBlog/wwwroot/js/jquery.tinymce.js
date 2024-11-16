(function ($) {
    $.fn.initTinymce = function (options) {
        var settings = $.extend({
            height: 500,
            images_upload_url: '/common/upload'
        }, options);

        return this.each(function () {
            tinymce.init({
                selector: '#' + this.id,
                image_dimensions: false,
                height: settings.height,
                menubar: false,
                plugins: settings.plugins ?? [
                    'advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview',
                    'anchor', 'searchreplace', 'visualblocks', 'code', 'fullscreen',
                    'insertdatetime', 'media', 'table', 'wordcount', 'codesample'
                ],
                toolbar: settings.toolbar ?? 'undo redo | formatselect | blocks | ' +
                    'bold italic backcolor | alignleft aligncenter ' +
                    'alignright alignjustify | bullist numlist outdent indent | ' +
                    'removeformat | image | codesample | table | code',
                codesample_languages: settings.codesample_languages ?? [
                    { text: 'C#', value: 'csharp' },
                    { text: 'HTML/XML', value: 'markup' },
                    { text: 'JavaScript', value: 'javascript' },
                    { text: 'CSS', value: 'css' },
                    { text: 'Bash', value: 'bash' },
                    { text: 'Json', value: 'json' },
                    { text: 'Sql', value: 'sql' },
                    { text: 'PHP', value: 'php' },
                    //{ text: 'Ruby', value: 'ruby' },
                    { text: 'Python', value: 'python' },
                    { text: 'Java', value: 'java' },
                    { text: 'C', value: 'c' },
                    { text: 'C++', value: 'cpp' }
                ],
                // https://www.tiny.cloud/docs/plugins/opensource/codesample/
                /* enable title field in the Image dialog*/
                image_title: true,
                /* enable automatic uploads of images represented by blob or data URIs*/
                images_upload_handler: function imageUploadHandler(blobInfo, success, failure, progress) {
                    var xhr, formData;

                    xhr = new XMLHttpRequest();
                    xhr.withCredentials = false;
                    xhr.open('POST', settings.images_upload_url);

                    xhr.upload.onprogress = function (e) {
                        progress(e.loaded / e.total * 100);
                    };

                    xhr.onload = function () {
                        var json;

                        if (xhr.status === 401) {
                            failure('HTTP Error: ' + xhr.status, { remove: true });
                            return;
                        }

                        if (xhr.status === 403) {
                            failure('HTTP Error: ' + xhr.status, { remove: true });
                            return;
                        }

                        if (xhr.status < 200 || xhr.status >= 300) {
                            failure('HTTP Error: ' + xhr.status, { remove: true });
                            return;
                        }

                        json = JSON.parse(xhr.responseText);

                        if (!json || typeof json.location != 'string') {
                            failure('Invalid JSON: ' + xhr.responseText);
                            return;
                        }

                        success(json.location);
                    };

                    xhr.onerror = function () {
                        failure('Image upload failed due to a XHR Transport error. Code: ' + xhr.status);
                    };

                    formData = new FormData();
                    formData.append('file', blobInfo.blob(), blobInfo.filename());

                    xhr.send(formData);
                },
                automatic_uploads: true,
                /*
                    URL of our upload handler (for more details check: https://www.tiny.cloud/docs/configure/file-image-upload/#images_upload_url)
                    images_upload_url: 'postAcceptor.php',
                    here we add custom filepicker only to Image dialog
                */
                images_upload_url: settings.images_upload_url,
                convert_urls: false,
                file_picker_types: 'image',
                /* and here's our custom image picker*/
                file_picker_callback: (cb, value, meta) => {
                    const input = document.createElement('input');
                    input.setAttribute('type', 'file');
                    input.setAttribute('accept', 'image/*');

                    input.addEventListener('change', (e) => {
                        const file = e.target.files[0];

                        const reader = new FileReader();
                        reader.addEventListener('load', () => {
                            /*
                            Note: Now we need to register the blob in TinyMCEs image blob
                            registry. In the next release this part hopefully won't be
                            necessary, as we are looking to handle it internally.
                            */
                            const id = 'blobid' + (new Date()).getTime();
                            const blobCache = tinymce.activeEditor.editorUpload.blobCache;
                            const base64 = reader.result.split(',')[1];
                            const blobInfo = blobCache.create(id, file, base64);
                            blobCache.add(blobInfo);

                            /* call the callback and populate the Title field with the file name */
                            var fileName = file.name;
                            var titleValue = $('input[name="Title"]').val();
                            if (titleValue) {
                                fileName = titleValue;
                            }
                            cb(blobInfo.blobUri(), { title: fileName, alt: fileName });
                        });
                        reader.readAsDataURL(file);
                    });
                    input.click();
                },
                content_style: 'body { font-family:Helvetica,Arial,sans-serif; font-size:16px }',
                init_instance_callback: function (editor) {
                    editor.on("Change", function (e) {
                        tinyMCE.triggerSave();
                        // $(editor.targetElm).valid();
                    });
                }
            });
        });
    }
}(jQuery));