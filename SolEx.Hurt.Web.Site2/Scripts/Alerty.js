function PotwierdzenieAlert(sender, tytul, msg, przyciskTak, przyciskNie, callbackok, callbackanuluj) {
    bootbox.confirm({
        title: tytul,
        message: msg,
        buttons:
        {
            'cancel': {
                label: przyciskNie
            },
            'confirm': {
                label: przyciskTak
            }
        },
        callback: function(result) {
            if (result) {
                if (callbackok != undefined) {
                    callbackok(sender);
                }
            } else {
                if (callbackanuluj != undefined) {
                    callbackanuluj(sender);
                }
            }
        }
    });
}

function Informacja(sender, tytul, msg, callback) {
    bootbox.dialog({
        title: tytul,
        message: msg,
        buttons:
        {
            'przyciskOK': {
                label: "OK",
                className: "btn-warning btn-lg"
            }
        },
        callback: function() {
            if (callback != undefined) {
                callback(sender);
            }
        }
    });
}


function ShowMessage(title, text, type) {
    ShowMessage(title, text, type, true, 'auto', 'auto', '');
}

function ShowMessage(title, text, type, opoznienie, hide, cssStyle) {
    var a = new PNotify({
        title: title,
        text: text,
        history: false,
        type: type,
        addclass: cssStyle,
        icon: false,
        hide: hide,
        delay: opoznienie,
        buttons: {
            sticker: false
        }
    });

}


function ShowModal(title, text, type, hide, cssStyle, width, heigth) {
    var modal_overlay;
    var a = new PNotify({
        title: title,
        text: text,
        type: type,
        history: false,
        stack: false,
        width: width,
        min_height: heigth,
        hide: hide,
        icon: false,
        addclass: 'modal-pnotify ' + cssStyle,
        pnotify_insert_brs: false,
        buttons: {
            sticker: false
        },
        after_open: function (pnotify) {

            var elment = pnotify.get();
            elment.css({
                "top": ($(window).height() / 2) - $(elment).height() / 2,
                "left": ($(window).width() / 2) - $(elment).width() / 2
            });
            // Make a modal screen overlay.
            modal_overlay = jQuery("<div />", {
                "class": "ui-widget-overlay",
                "css": {
                    "display": "none",
                    "position": "fixed",
                    "top": "0",
                    "bottom": "0",
                    "right": "0",
                    "left": "0"
                }
            }).appendTo("body").fadeIn("fast");
        },
        before_close: function () {
            modal_overlay.fadeOut("fast");
        }
    });
}


function ZamknijOkna() {
    $('.modal-pnotify .glyphicon-remove').click();
}