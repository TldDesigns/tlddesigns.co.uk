//functions shared by all pages

$(document).ready(setupPage);

function setupPage() {
    setEventHandlersOnStyles();
    enableSmoothScroll();

    cookieConsent.init();
}

function enableSmoothScroll() {

    $('a[href*="#"]:not([href="#"])').click(function () {
        if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
            var target = $(this.hash);
            target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
            if (target.length) {
                $('html, body').animate({
                    scrollTop: target.offset().top
                }, 1000);
                return false;
            }
        }
    });

};

function setEventHandlersOnStyles() {
    var styles;

    styles = document.querySelectorAll("link[as='style']");

    if (styles.forEach) {

        styles.forEach(function (link, key, listObj, argument) {

            styleOnLoadHandler(link);

        });
    }
    else {
        for (i = 0; i < styles.length; ++i) {

            styleOnLoadHandler(link[i]);

        }
    }
}

function styleOnLoadHandler(link) {
    
    link.rel = 'stylesheet';
    
}