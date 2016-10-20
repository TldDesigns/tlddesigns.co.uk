//var isIE = false;

$(document).ready(setupPageIndex);

function setupPageIndex() {
    //isIE = detectIE();
    //setupPage();
    //enableSmoothScroll();
    enableFromValidation();
    setEventListeners();

    //cookieConsent.init();
};

function setEventListeners() {
    var submitBtn;
    

    submitBtn = document.getElementById('submitBtn');

    submitBtn.addEventListener('click', submitClick);

    //cookieConsentBtn = document.getElementById('cookieConsent-Btn');

    //cookieConsentBtn.addEventListener('click', cookieConsent.onClick);
};

function submitClick(e) {
    var formData;
    var isValid;

    $('#contact_us').data('bootstrapValidator').validate();

    isValid = $('#contact_us').data('bootstrapValidator').isValid();

    //console.log(validateResult);

    //alert();

    isValid = true;

    if (isValid == true) {

        formData = getContactFromData();

        //alert(JSON.stringify(formData));

        //console.log(formData);

        submitFormData(formData);
    }
};

function getContactFromData() {
    var formData;

    var frmName, frmEmail, frmNumber, frmMessage, frmSubject;

    frmName = document.getElementById('frmName');

    frmSubject = document.getElementById('frmSubject');

    frmEmail = document.getElementById('frmEmail');

    frmPhone = document.getElementById('frmPhone');

    //frmMessage = document.getElementById('frmMessage');

    frmMessage = encodeURIComponent( document.getElementById('frmMessage').value);

    formData = { name: frmName.value, subject: frmSubject.value, from: frmEmail.value, number: frmPhone.value, message: frmMessage };

    return formData;
}

function submitFormData(data) {
    
    $.ajax({
        type: "POST",
        url: './sendMail.aspx',
        data: data,
        
    }).done(function (msg) {
    
    });;
}

//function enableSmoothScroll() {

//    $('a[href*="#"]:not([href="#"])').click(function () {
//        if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
//            var target = $(this.hash);
//            target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
//            if (target.length) {
//                $('html, body').animate({
//                    scrollTop: target.offset().top
//                }, 1000);
//                return false;
//            }
//        }
//    });

//};

function enableFromValidation() {

    $('#contact_us').bootstrapValidator({
        // To use feedback icons, ensure that you use Bootstrap v3.1.0 or later
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            name: {
                validators: {
                    stringLength: {
                        min: 2,
                    },
                    notEmpty: {
                        message: 'Please supply your name'
                    }
                }
            },

            email: {
                validators: {
                    notEmpty: {
                        message: 'Please supply your email address'
                    },
                    emailAddress: {
                        message: 'Please supply a valid email address'
                    }
                }
            },
            subject: {
                validators: {
                    notEmpty: {
                        message: 'Please supply a subject'
                    }
                }
            },

            phone: {
                validators: {
                    notEmpty: {
                        message: 'Please supply your phone number'
                    },
                    //phone: {
                    //    country: 'UK',
                    //    message: 'Please supply a vaild phone number with area code'
                    //}
                }
            },

            comment: {
                validators: {
                    stringLength: {
                        min: 10,
                        max: 200,
                        message: 'Please enter at least 10 characters and no more than 200'
                    },
                    notEmpty: {
                        message: 'Please supply a description of your project'
                    }
                }
            }
        }
    })
.on('success.form.bv', function (e) {
    $('#success_message').slideDown({ opacity: "show" }, "slow") // Do something ...
    $('#contact_us').data('bootstrapValidator').resetForm();

    // Prevent form submission
    e.preventDefault();

    // Get the form instance
    var $form = $(e.target);

    // Get the BootstrapValidator instance
    var bv = $form.data('bootstrapValidator');

    // Use Ajax to submit form data
    $.post($form.attr('action'), $form.serialize(), function (result) {
        console.log(result);
    }, 'json');
});
};