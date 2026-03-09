function Sortuj(sender) {
    var kolumna = $(sender).attr('data-pole-sort');
    var parent = $(sender).parents("[data-type='lista']");
    var poprzednie = $(parent).attr('data-sort');
    var sort = $(parent).attr('data-sort-kierunek');
    var zmianakiernku = poprzednie == kolumna;
    if (zmianakiernku && sort == 'desc') {
        sort = 'asc';
    } else {
        sort = 'desc';
    }
    $(parent).attr('data-sort-kierunek', sort);
    $(parent).attr('data-sort', kolumna);
    UstawWartoscWProfiluKlienta('KolumnaSortowaniaDokumentow', kolumna+" "+sort);
}
function DodajSzukanie(parent, place) {
    var szuabue = KontrolkaSzukanie(parent);
    if (place == undefined) {
        place = $(parent).attr('data-szukanie-placeholder');
    }
    var szukanie = $('<div class="wyszukiwanie-wewnetrzne"><input data-szukanie="wartosc" type="text" class="input-xs form-control-sm" autocomplete="off" placeholder="' + place + '"> <i class="fa fa-search"></i> </div>');
 
    if (szuabue.is(":hidden")) {
        szuabue.append(szukanie);
        szuabue.show();
    }
    return szuabue;
}

function KontrolkaSzukanie(parent) {
    var szuabue = $(parent).attr('data-szukanie-pole');
    if (szuabue == undefined) {
        szuabue = '[data-szukanie="pole"]';
    }
    return $(szuabue, parent);
}
function PoZaladowaniuListy(target) {
    InicjacjaJs(target);
    var parent = target.parents('[data-type="lista"]');
    var sort = $(parent).attr('data-sort');
    var sortkierunek = $(parent).attr('data-sort-kierunek');
    var trybPlat = $(parent).attr('data-odkiedyplatnosc');
    var trybPrzeg = $(parent).attr('data-odkiedyprzeglad');
    var trybDoKiedy = $(parent).attr('data-now');
    var szuabue = KontrolkaSzukanie(parent);
    var sortikona;
    if (sortkierunek == 'asc') {
        sortikona = $(parent).attr('data-sort-icon-asc');
    } else {
        sortikona = $(parent).attr('data-sort-icon-desc');
    }
    $("th[data-pole-sort]", parent).each(function () {
        var pole = $(this).attr('data-pole-sort');
        var szerokosc = $(this).width();
        if (pole == sort) {
            var sortelem = $(('<span />')).addClass(sortikona).html('&nbsp;');
            $(this).append(sortelem);
            szerokosc += $(sortelem).width();
            $(this).width(szerokosc);
        }
        var content = $(this).html();
        var span = $('<span />').append(content).css('cursor', 'pointer');
        $(this).empty();
        $(this).append(span);
    });
    var metodaurl = window[$(parent).attr('data-metoda-url')];
    $("th[data-pole-sort]", parent).children().off();
    $("th[data-pole-sort]", parent).children().on('click', function () {
        var p = $(this).parent();
        Sortuj(p);
        ZaladujListe(parent, metodaurl);
    });
    $("th[data-zaznacz-wszysztkie]", parent).off();
    $("input[data-zaznacz-wszysztkie]", parent).on('change', function () {
        ZaznaczWszystkie(this, parent);
    });

    //szukanie wewnetrzne powinno dziala tylko w nazwach dokumentow span.numer-dokumentu
    var szukane = $('[data-szukanie="wartosc"]', szuabue).val();
    if (szukane != undefined) {
        target.highlight(szukane.split(' '));
    }

    $('.tylkoNiezaplacone', parent).on('click', function () {
        var tylkoNiezaplacone = $(this).is(':checked') ? 'true' : 'false';
        WylaczTaby();
        UstawWartoscWProfiluKlienta('DokumentyTylkoNiezaplacone', tylkoNiezaplacone);
        ZaladujListe(parent);
    });

    $('.tylkoNiezrealizowane', parent).on('click', function () {
        var tylkoNiezrealizowane = $(this).is(':checked') ? 'true' : 'false';
        UstawWartoscWProfiluKlienta('DokumentyTylkoNiezrealizowane', tylkoNiezrealizowane);
        ZaladujListe(parent);
    });

    $('.tylkoPrzeterminowane', parent).on('click', function () {
        var tylkoPrzeterminowane = $(this).is(':checked') ? 'true' : 'false';
        WylaczTaby();
        UstawWartoscWProfiluKlienta('DokumentyTylkoPrzeterminowane', tylkoPrzeterminowane);
        ZaladujListe(parent);
    });
    
    $('a[href="#TrybPlatnosci"]').on('click', function () {
        UstawWartoscWProfiluKlienta('DokumentyTylkoNiezaplacone', 'true');
        UstawWartoscWProfiluKlienta('DokumentyTylkoPrzeterminowane', 'false');
        var kolumna = "TerminPlatnosci";
        var sort = "asc";
        UstawWartoscWProfiluKlienta('KolumnaSortowaniaDokumentow', kolumna + " " + sort);
        $('.dokumenty-lista').attr('data-sort-kierunek', sort);
        $('.dokumenty-lista').attr('data-sort', kolumna);
        $('.tylkoNiezaplacone').prop("checked", true);
        $('.tylkoPrzeterminowane').prop("checked", false);
        $('.dokumenty-lista').attr('data-odkiedy', trybPlat);
        $('.dokumenty-lista').attr('data-dokiedy', trybDoKiedy);
        ZaladujWyborDat(parent);
        ZaladujListe(parent);
    });

    $('a[href="#TrybPrzegladania"]').on('click', function () {
        UstawWartoscWProfiluKlienta('DokumentyTylkoNiezaplacone', 'false');
        UstawWartoscWProfiluKlienta('DokumentyTylkoPrzeterminowane', 'false');
        var kolumna = "DataUtworzenia";
        var sort = "desc";
        UstawWartoscWProfiluKlienta('KolumnaSortowaniaDokumentow', kolumna + " " + sort);
        $('.dokumenty-lista').attr('data-sort-kierunek', sort);
        $('.dokumenty-lista').attr('data-sort', kolumna);
        $('.tylkoNiezaplacone').prop("checked", false);
        $('.tylkoPrzeterminowane').prop("checked", false);
        $('.dokumenty-lista').attr('data-odkiedy', trybPrzeg);
        $('.dokumenty-lista').attr('data-dokiedy', trybDoKiedy);
        ZaladujWyborDat(parent);
        ZaladujListe(parent);
    });

}


function WylaczTaby() {
    $('a[href="#TrybPrzegladania"]').removeClass('active');
    $('a[href="#TrybPlatnosci"]').removeClass('active');
}

function ZaladujListe(parent) {
    var szuabue = KontrolkaSzukanie(parent);
    var metodaurl =window[$(parent).attr('data-metoda-url')];
    DodajSzukanie(parent);
    var szukanie = $(parent).attr('data-szukanie-fraza');
    var url = metodaurl(parent,szukanie);
    $('[data-szukanie="wartosc"]', szuabue).off();
    $('[data-szukanie="wartosc"]', szuabue).bind("input propertychange", function () {
        var kontrolka = this;
        if (window.event && event.type == "propertychange" && event.propertyName != "value")
            return;
        window.clearTimeout($(kontrolka).data("timeout"));
        $(kontrolka).data("timeout", setTimeout(function () {
            var input = $(kontrolka);
            var fraza = $(input).val();
            var podmieniaj = $(parent).attr('data-podmieniaj-backslash') == "1";
            fraza = OczyscFraze(fraza, podmieniaj);
            $(parent).attr('data-szukanie-fraza', fraza);
            ZaladujListe(parent, metodaurl);
        }, 500));
    });

    var elementdata = $('#data', parent);
    WczytajAjax(elementdata, url, PoZaladowaniuListy);
}
function ZaznaczWszystkie(sender, parent) {
    var grupa = $(sender).attr('data-zaznacz-wszysztkie');
    var zaz = $(sender).is(':checked');
    $('input[data-zaznaczanie='+grupa+']', parent).prop('checked', zaz);
}
function PobierzWybrane(parent) {
    var ids = [];
    $('[data-zaznaczanie]:checked', parent).each(function () {
        ids.push($(this).val());
    });
    return ids;
}


function SprawdzWartosc(sender) {
    var el = $(sender);
    var tekst = el.attr('data-niepoprawna');
    var wart = parseFloat(el.val());

    if (wart < 0) {
        el[0].setCustomValidity(tekst);
        el.tooltip({ title: tekst });
        el.tooltip('show');
    } else {
        el[0].setCustomValidity('');
        el.tooltip('destroy');
    }
}
function ZaznaczWszystkieZaplaty(el) {
    el = $(el);
    var els = el.closest('#data').find('[data-dokument]').siblings('input[type="checkbox"]');
    if (el.is(':checked')) {
        els.prop('checked', true);
    } else {
        els.prop('checked', false);
    }
    els.trigger("change");

}
function ZaplacacDokument(el) {
    el = $(el);
    var wartosc = el.siblings('input[type="text"]');
    if (el.is(':checked')) {
        wartosc.show();
    } else {
        wartosc.hide();
    }
}