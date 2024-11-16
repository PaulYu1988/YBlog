(function ($) {
    $.fn.initTags = function (options) {
        var settings = $.extend({
            name: '',
            addButtonText: 'Add',
            labelText: 'Tags',
            selected: [],
            tags: []
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

            var selectedTags = document.createElement('div');
            selectedTags.className = 'tag-container selected-tags';
            if (settings.selected.length > 0) {
                for (var i = 0; i < settings.selected.length; i++) {
                    var item = settings.selected[i];
                    var selectedItem = document.createElement('span');
                    selectedItem.className = 'layui-badge layui-bg-green';
                    selectedItem.innerHTML = item;
                    selectedItem.onclick = (event) => {
                        event.currentTarget.remove();
                        settings.selected = settings.selected.filter(x => x != $(event.currentTarget).text());
                        $('input[name="' + settings.name + '"]').val(settings.selected.join(','));
                    };
                    var selectedItemButton = document.createElement('i');
                    selectedItemButton.className = 'layui-icon layui-icon-close layui-font-12';
                    selectedItem.append(selectedItemButton);
                    selectedTags.append(selectedItem);
                }
            } else {
                selectedTags.classList.add('layui-hide');
            }

            var inputContainer = document.createElement('div');
            inputContainer.className = 'layui-inline';
            var input = document.createElement('input');
            input.type = 'text';
            input.className = 'layui-input tag-input';
            input.setAttribute('autocomplete', 'off');
            input.onkeydown = (event) => {
                var notAllowed = ['\'', ',', '\"', '<', '>', '/', '\\'];
                if (notAllowed.indexOf(event.key) > -1) {
                    event.preventDefault();
                    return;
                }
            };
            input.onkeyup = (event) => {
                var value = $(event.currentTarget).val().trim();
                if (value) {
                    var items = $('.match-tags span');
                    if (items.length > 0) {
                        var regex = new RegExp(value, 'gi');
                        $.each(items, function (i, item) {
                            var curr = $(item);
                            var currValue = curr.text();
                            if (currValue.match(regex)) {
                                $('.match-tags').removeClass('layui-hide');
                                if (curr.hasClass('layui-hide')) {
                                    curr.removeClass('layui-hide');
                                }
                            } else {
                                if (!curr.hasClass('layui-hide')) {
                                    curr.addClass('layui-hide');
                                }
                            }
                        });
                    }
                } else {
                    if (!$('.match-tags').hasClass('layui-hide')) {
                        $('.match-tags').addClass('layui-hide');
                    }
                }
            };
            inputContainer.append(input);

            var addButtonContainer = document.createElement('div');
            addButtonContainer.className = 'layui-inline';
            var addButton = document.createElement('button');
            addButton.type = 'button';
            addButton.textContent = settings.addButtonText;
            addButton.className = 'layui-btn layui-btn-primary';
            addButton.onclick = (event) => {
                var tag = $('.tag-input').val().trim();
                if (tag) {
                    if (settings.selected.indexOf(tag) === -1) {
                        settings.selected.push(tag);
                        $('input[name="' + settings.name + '"]').val(settings.selected.join(','));
                        var addSpan = document.createElement('span');
                        addSpan.className = 'layui-badge layui-bg-green';
                        addSpan.innerHTML = tag;
                        var addSpanButton = document.createElement('i');
                        addSpanButton.className = 'layui-icon layui-icon-close layui-font-12';
                        addSpan.append(addSpanButton);
                        addSpan.onclick = (event) => {
                            event.currentTarget.remove();
                            settings.selected = settings.selected.filter(x => x != $(event.currentTarget).text());
                            $('input[name="' + settings.name + '"]').val(settings.selected.join(','));
                        }
                        if ($(selectedTags).hasClass('layui-hide')) {
                            $(selectedTags).removeClass('layui-hide');
                        }
                        selectedTags.append(addSpan);
                    }
                    $('.tag-input').val('');
                }
            };
            addButtonContainer.append(addButton);

            var matchTags = document.createElement('div');
            matchTags.className = 'tag-container match-tags layui-hide';
            if (settings.tags.length > 0) {
                for (var i = 0; i < settings.tags.length; i++) {
                    var item = settings.tags[i];
                    var matchItem = document.createElement('span');
                    matchItem.className = 'layui-badge layui-bg-blue';
                    matchItem.innerHTML = item;
                    matchItem.onclick = (event) => {
                        var tag = $(event.currentTarget).text();
                        var index = settings.selected.indexOf(tag);
                        if (index === -1) {
                            settings.selected.push(tag);
                            $('input[name="' + settings.name + '"]').val(settings.selected.join(','));
                            var addSpan = document.createElement('span');
                            addSpan.className = 'layui-badge layui-bg-green';
                            addSpan.innerHTML = tag;
                            var addSpanButton = document.createElement('i');
                            addSpanButton.className = 'layui-icon layui-icon-close layui-font-12';
                            addSpan.append(addSpanButton);
                            addSpan.onclick = (event) => {
                                event.currentTarget.remove();
                                settings.selected = settings.selected.filter(x => x != $(event.currentTarget).text());
                                $('input[name="' + settings.name + '"]').val(settings.selected.join(','));
                            }
                            if ($(selectedTags).hasClass('layui-hide')) {
                                $(selectedTags).removeClass('layui-hide');
                            }
                            selectedTags.append(addSpan);
                        }
                    };
                    matchTags.append(matchItem);
                }
            }

            var hiddenInput = document.createElement('input');
            hiddenInput.type = 'hidden';
            hiddenInput.name = settings.name;
            if (settings.selected.length > 0) {
                hiddenInput.value = settings.selected.join(',');
            }

            divContainer.append(selectedTags);
            divContainer.append(inputContainer);
            divContainer.append(addButtonContainer);
            divContainer.append(matchTags);
            divContainer.append(hiddenInput);

            $(this).append(label);
            $(this).append(divContainer);
        });
    };
}(jQuery));