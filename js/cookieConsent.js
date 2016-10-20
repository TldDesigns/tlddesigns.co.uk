var cookieConsent = {
    options: {
        messageText: 'We use cookies to help us improve your browsing experience. By clicking ok you agree to us using cookies. ',
        buttonText: 'Ok',
        linkText: 'Learn More',
        linkDestination: './privacy',
        cookieName: 'cookiesOk',
        cookieExpiresDay: 1
    },
    cookieConsentBanner: null,
    cookieConsentBtn: null,
    supportsLS: false,
    supportsClasslist: false,
    init: function () {
        this.createDiv();
        this.supportsLS = this.checkLS();
        this.supportsClasslist = this.checkClasslist();
        this.showDiv();
    },

    createDiv: function () {
        var             ccFooter = document.createElement('footer'),
          ccContainer = document.createElement('div');
        
        ccRow = document.createElement('div'),
        ccMsg = document.createElement('div'),
        ccLink = document.createElement('a'),
        ccBtnContainer = document.createElement('div'),
        ccBtn = document.createElement('button');

        ccFooter.id = 'cookieConsent';

        ccFooter.className = 'footer fade';

                ccContainer.className = 'container';

        ccRow.className = 'row';

        ccMsg.className = 'col-xs-10';
        ccMsg.className += ' col-sm-11';

        ccBtnContainer.className = 'col-xs-2';
        ccBtnContainer.className += ' col-sm-1';

        ccBtnContainer.className += ' text-center';

        ccBtn.className += ' btn btn-sm btn-info';

        ccLink.href = this.options.linkDestination;

        ccLink.innerText = this.options.linkText;

        ccMsg.id = 'cookieConsent-Msg';

        ccMsg.innerText = this.options.messageText;

        ccBtn.id = 'cookieConsent-Btn';

        ccBtn.innerText = this.options.buttonText;

        ccBtn.setAttribute('type', 'button');

        ccBtn.addEventListener('click', this.onClick);

        ccBtnContainer.appendChild(ccBtn);

        ccMsg.appendChild(ccLink);

        ccRow.appendChild(ccMsg);

        ccRow.appendChild(ccBtnContainer);

        ccContainer.appendChild(ccRow);

        ccFooter.appendChild(ccContainer);

        document.body.appendChild(ccFooter);

    },

    showDiv: function () {
        var cookiesOk = this.getCookie();

                if (!cookiesOk) {
            var cookieConsentBanner = document.getElementById('cookieConsent');

            if (this.supportsClasslist) {

                cookieConsentBanner.classList.add('in');
            }
            else {
                cookieConsentBanner.className += ' in';
            }
        }
    },

    onClick: function () {
        {
            var cookieConsentBanner = document.getElementById('cookieConsent');

            cookieConsent.setCookie();

            if (this.supportsClasslist) {

                cookieConsentBanner.classList.remove('in');
            }
            else {
                cookieConsentBanner.className += ' cookieConsent-Closed';
            }
        }
    },

    checkLS: function () {
        if (localStorage) {
            return true;
        }
        else {
            return false;
        }
    },
    checkClasslist: function () {
        if ("classList" in document.createElement("_")) {
            return true;
        }
        else {
            return false;
        }
    },
    setCookie: function () {
        var d = new Date();
        d.setTime(d.getTime() + (this.options.cookieExpiresDay * 24 * 60 * 60 * 1000));
        var expires = "expires=" + d.toUTCString();

        if (this.supportsLS) {
            localStorage.setItem(this.options.cookieName, true);
        }
        else {
            document.cookie = this.options.cookieName + '=true; ' + expires;
        }
    },
    getCookie: function () {
        var name = this.options.cookieName + '='
        var ca = document.cookie.split(';');
        var cookieValue = new String;
        var cookieBool = false;

        if (this.supportsLS) {
            cookieValue = localStorage.getItem(this.options.cookieName);
            if (!cookieValue) {
                cookieValue = 'false';
            }
        }
        else {
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) == ' ') {
                    c = c.substring(1);
                }
                if (c.indexOf(name) == 0) {
                    cookieValue = c.substring(name.length, c.length);
                }


            }
        }

        if (cookieValue.toUpperCase() == 'TRUE') {
            cookieBool = true;
        }
        else {
            cookieBool = false;
        }
        return cookieBool;
    }
}

