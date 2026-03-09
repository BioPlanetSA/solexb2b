/// <reference path="../typings/jquery/jquery.d.ts"/>
/// <reference path="../typings/Ajax.d.ts"/>
/// <reference path="_ajax.ts"/>
/// <reference path="../typings/bootstrap/index.d.ts"/>

class helpersKlasa {

    ///funkcja sprawdza czy juz byla uruchomiona dana funkcja - jak tak to zwraca TRUE
    public BlokadaUruchomieniaDwaRazy(klucz: string ) {
        klucz = 'solex-' + klucz;
        if ($('body').hasClass(klucz)) {
            console.debug('funkcja ' + klucz + ' juz była uruchomiona na stronie');
            return true;
        }

        $('body').addClass(klucz);
        return false;
    }

    public UsunTooltip() {
        $('body').find('.tooltip div').remove();
    }
}




function PokazModal(sender, url, celfiltr) {
    try {
        var evt = window.event || arguments.callee.caller.arguments[0];
        evt.preventDefault();
    }
    catch (ex) { }
    if (url == undefined) {
        url = $(sender).prop('href');
        if (url === undefined) {
            url = $(sender).data('url');
        }
    }
    if (celfiltr == undefined) {
        celfiltr = '[data-containter="modal"]';
    }
    var cel = $(celfiltr);
    WczytajAjax(cel, url, InitModal, undefined, undefined, $('body'));
    return false;
}

//funkcja która po przeładowaniu listy produktów/ koszyka bedzie usuwała rozwinięte tooltipy


function InitModal(target, callback?, callbackzamkniecie?) {
    $('.modal', target).modal();
    $('.modal', target).on('shown.bs.modal', function () {
        var tla = $('.modal-backdrop');
        var popupy = $('.modal', target);
        var i = 0;
        if (tla.length > popupy.length) {
            $(tla).each(function () {
                if (i < (tla.length - popupy.length)) {
                    $(this).remove();
                }
                i++;
            });
        }
    });
    InicjacjaJs(target);
    if (callback != undefined) {

        callback(target);
    }
    $('.modal', target).on('hide.bs.modal', function (e) {
        if ($(this).hasClass('nie-zamykac')) {
            e.preventDefault();
            e.stopImmediatePropagation();
            $(this).removeClass('nie-zamykac');

            return false;
        }
    }).on('.modal', target).on('hidden.bs.modal', function () {
        if ($('.modal:visible').length > 0) {
            $('body').addClass('modal-open');
        }
        if (callbackzamkniecie != undefined) {

            callbackzamkniecie(target);
        }
    });
}


function WyswietlModal(data, targetsciezka) {
    var cel = '[data-containter="modal"]';
    if (targetsciezka != undefined) {
        cel = targetsciezka;
    }
    data = $.trim(data);
    if (data != '') {
        let target = $(cel)[0];
        $(target).html(data);
        InitModal(target);
    }
}




let helpers: helpersKlasa = new helpersKlasa();