///<reference path="_alerty.ts"/>
///<reference path="../typings/jquery/jquery.d.ts"/>
///<reference path="../typings/bootstrap/index.d.ts"/>>

function Ulubione(sender, produkt) {
    ZmienStatusProduktu(sender, produkt, '/Koszyk/UlubioneZmien/' + produkt);
}
function ZmienStatusProduktu(sender, produkt, url) {
    $.get(url, data => {
        var czyIstnieje = data.CzyModal;
        var odpowiedz = data.Odpowiedzi;
        var ids = $(sender).attr('data-identyfikator');
        var wybrane = $('[data-identyfikator="' + ids + '"]');
        $(wybrane).each(function (index, value) {
            if (czyIstnieje) {
                $(value).addClass('dodano-info dwustanowy-jest');
            } else {
                $(value).removeClass('dodano-info dwustanowy-jest');
            }
            ZmienTytul(value, sender);
        });
        ShowMessage('', odpowiedz[0].Tekst, odpowiedz[0].Typ, odpowiedz[0].CzasWyswietlania, true);
    });
}

function ZmienTytul(target, sender) {
    var jest = $(target).hasClass('dwustanowy-jest');
    var tytul = $(target).attr(jest ? 'data-dwustanowy-title-jest' : 'data-dwustanowy-title-brak');
    $(target).attr('data-original-title', tytul);

    if (target == sender) {
        $(target).tooltip('show');
    }
}