$(function () {
    $('.nav-more').on("click", function (event) {
        var nav = $('.header-nav');
        if (nav.is(":hidden")) {
            nav.removeClass('layui-hide-xs');
        } else {
            nav.addClass('layui-hide-xs');
        }
    });
    $('.btn-back').on('click', function () {
        const previousUrl = document.referrer;
        if (previousUrl) {
            document.location = previousUrl;
        } else {
            window.history.back();
        }
    });
    $('.btn-search').on('click', function () {
        searchArticle();
    });
    $(".input-search").on("keypress", function (event) {
        if (event.key === 'Enter') {
            searchArticle();
        }
    });

    var navLinks = document.querySelectorAll(".summary-list a");
    const sections = document.querySelectorAll(".article-content h2, h3");
    if (navLinks.length > 0 && sections.length > 0) {
        navLinks[0].classList.add("summary-active");
        summaryActive(navLinks, sections);
        window.addEventListener("scroll", () => {
            summaryActive(navLinks, sections);
        });
    }

    $('.mode-container .layui-form-switch').click(function () {
        var checked = $(this).hasClass('layui-form-onswitch');
        if (checked) {
            $('html').addClass('dark');
            $('head').append('<link id="dark-css" rel="stylesheet" href="/assets/layui/css/layui-theme-dark.css">');
        } else {
            $('html').removeClass('dark');
            $('#dark-css').remove();
        }
        const date = new Date();
        date.setTime(date.getTime() + 365 * 24 * 60 * 60 * 1000); // 1 year
        expires = "; expires=" + date.toUTCString();
        document.cookie = 'ThemeMode' + "=" + encodeURIComponent(checked ? 'dark' : 'light') + expires + "; path=/";
    });
});
function searchArticle() {
    var val = $('.input-search').val();
    if (val) {
        document.location = '/search/' + encodeURIComponent(val);
    }
}
function summaryActive(navLinks, sections) {
    let currentSectionId = "";
    sections.forEach(section => {
        const sectionTop = section.getBoundingClientRect().top;
        if (sectionTop >= 0 && sectionTop <= 100) {
            currentSectionId = section.getAttribute("id");
        }
    });
    if (!currentSectionId) {
        let sectionLen = sections.length;
        for (var i = 0; i < sectionLen; i++) {
            var section = sections[i];
            const sectionTop = section.getBoundingClientRect().top;
            if (sectionTop < -30) {
                var curr = i + 1;
                if (curr >= sectionLen) {
                    curr = i;
                }
                currentSectionId = sections[curr].getAttribute("id");
            }
        }
    }
    navLinks.forEach(link => {
        if (currentSectionId) {
            link.classList.remove("summary-active");
            if (link.getAttribute("href") === `#${currentSectionId}`) {
                link.classList.add("summary-active");
            }
        }
    });
}