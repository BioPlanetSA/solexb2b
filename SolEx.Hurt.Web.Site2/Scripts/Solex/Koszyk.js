function InitDodawanieDoKoszyka(sender) {
    $('.dodawanie-panel', sender).each(function (i, el) {
        var tt = $(el);
        var zr = tt.attr('data-zrobiony');
        if (zr === "1") {
            return;
        }
        tt.attr('data-zrobiony', '1');
        var produkt = tt.find('input.idproduktu', tt).val();
        var typ = tt.find('input.TypPozycji', tt).val();

        var select = $('select', tt);
        var button = $('span.bsk', tt);
        var input = $('input.input-ilosc', tt);
        //kliekanie dodaj do koszyka
        button.on('click', function (e) {
            e.preventDefault();
            var lista = [];
            var indywialne = [];
            var val = input.val();
            var jednostka = $('.jednostki', tt).val();
            if (val == '') {
                var domyslnaIlosc = tt.attr('data-domyslna-ilosc');
                if (domyslnaIlosc != undefined) {
                    val = domyslnaIlosc.replace(",", ".");
                }
            }
            DodajPozycje(tt, produkt, val, jednostka, lista, indywialne, typ);
            DodajPozycjeWypelnione(sender, produkt, lista, indywialne);
        });

        //kliekanie dodaj do koszyka koniec
        //zmiana jednostki
        select.on('change ', function () {
            var przelicznikz = parseFloat(select.find(':selected').attr('data-przelicznik').replace(',', '.'));
            var przelicznik = tt.attr('data-przelicznik-aktualny');
            var aktualny = 1;
            if (przelicznik != undefined) {
                aktualny = parseFloat(przelicznik.replace(',', '.'));
            }

            tt.attr('data-przelicznik-aktualny', przelicznikz);
            var val = input.val();
            if (val != '') {
                var ilosc = parseFloat(val);
                if (ilosc == 0) {
                    input.val('');
                } else {
                    input.val((aktualny / przelicznikz * ilosc).toString());
                }
            }
        });
        //zmiana jednostki koniec

        //chmura pokazywanie
        tt.children().on('change mouseover click', function (evt) {
            //var step = tt.attr('data-step');
            //if (step != undefined) {
            //    var stepz = parseFloat(step.replace(",", "."));
            //} else {
            //    stepz = 1;
            //}
            //var wart = select.find(':selected').attr('data-step');
            //if (wart != undefined) {
            //    stepz = parseFloat(wart.replace(',', '.'));
            //}
            //SprawdzKomunikatNowaStrona(tt);
            var jednostka = $('.jednostki', tt).val();
            var ilosc = parseFloat(input.val().replace(',', '.'));
            if (ilosc == '' || isNaN(ilosc)) {
                ilosc = 0;
            }
            if (evt.type == 'click') {
                var target = $(evt.target);


                if (target.hasClass('input-ilosc')) {
                    return;
                }
                if (target.hasClass('wgore')) {
                   // ilosc += stepz;
                    PopupDymek(tt, produkt, 0, jednostka, input, 1);
                    return;
                } else if (target.hasClass('wdol')) {
                  //  ilosc -= stepz;
                    PopupDymek(tt, produkt, 0, jednostka, input, -1);
                    return;
                } else if (target.hasClass('zbiorczododaj')) {
                    PopupDymek(tt, produkt, Number(input.data('iloscwopakowaniu')), jednostka, input, 9);
                    return;
                }


                ilosc = ilosc.toFixed(4); //max 4 cyfry po przecinku - jak trzeba wiecej to zmienic

                tt.attr('data-poprzednio', ilosc);
            }
            if (ilosc < 0) {
                ilosc = 0;
            }

            PopupDymek(tt, produkt, ilosc, jednostka, input);
        });
        input.on('keyup', function () {
            KliknijJesliWpisanoEnter(button);
        });

        input.on('mouseenter', function () {
            input.focus();
        });
    });
}
function PopupDymek(tt, produkt, ilosc, jednostka, input, tryb) {
    if (jednostka == undefined || jednostka == '') {
        jednostka = null;
    }

    var pars = produkt + '-' + ilosc + '-' + jednostka;
    var kom = $(tt).data('data-popup');
    var typPozycji = $(tt).attr('data-typ-pozycji');

    if (typPozycji == undefined) {
        typPozycji = "Zwykly";
    }

    var parsist = $(tt).attr('data-pars');
    var poprzednio = tt.attr('data-poprzednio');
    if (poprzednio == '' || poprzednio == undefined) {
        poprzednio = 0;
    }
    var zmieniony = false;
    if (pars != parsist) {
        $(tt).data('parametry-dymek', pars);
        var url = '/Produkty/Dymek';
        var req;
        if (jednostka == null) {
            req = { produkt: produkt, ilosc: ilosc, typPozycji: typPozycji, poprzednio: poprzednio, tryb: tryb };
        } else {
            req = { produkt: produkt, ilosc: ilosc, jednostka: jednostka, typPozycji: typPozycji, poprzednio: poprzednio, tryb: tryb };
        }
        $.ajax({ url: url, data: req, async: false }).done(function (data) {
            var nowy = $.trim(data.Item1);
            var zaokraglenie = $.trim(data.Item3);
            if (nowy != kom) {
                kom = nowy;
                zmieniony = true;
                //tt.tooltip('hide');
            }
            $(tt).data('data-popup', nowy);
            if (input != undefined) {
                input.val(data.Item2 == 0 ? '' : data.Item2);
            }
            tt.attr('data-poprzednio', data.Item2);
            if (zaokraglenie !== '') {
                ShowMessage('', zaokraglenie, 'info', 3500, true);
            }
        });

    }
    if (zmieniony && kom != '') {
        tt.tooltip({ title: kom, html: true, trigger: 'hover' });
        $(tt).attr('data-original-title', kom);
        //$(tt).tooltip("option", "content", kom);
        tt.tooltip('show');
    }
}

function DodajPozycjeWypelnione(sender, produkt, lista, indywidualne) {
    if (lista == undefined) {
        lista = [];
    }
    if (indywidualne == undefined) {
        indywidualne = [];
    }
    $(".dodawanie-panel", sender).each(function () {
        var p = $('input.idproduktu', this).val();

        if (p != produkt) {
            var typ = $('input.TypPozycji', this).val();
            var i = $('input.input-ilosc', this).val();
            var j = $('.jednostki', this).val();
            DodajPozycje(this, p, i, j, lista, indywidualne, typ);
        }
    });

    //dodajemy produkty z listy
    DodajDoKoszyka(sender, lista);

    //dodajemy z indywidualne
    if (indywidualne.length > 0) {
        PobierzOknoIndywidualizacje(indywidualne);
    }
}

function PobierzOknoIndywidualizacje(lista) {
    if (lista.length > 0) {
        $.ajax({
            url: '/Koszyk/Indywidualizacja',
            data: JSON.stringify(lista),
            type: 'POST',
            contentType: 'application/json',
            success: function (data) {
                WyswietlModal(data, '[data-containter="indywidualny"]');
            }
        });
    }
}

function ZaladujKoszyk(id, metoda, parametry) {
    var target = $(id);
    if (parametry == undefined) {
        parametry = [];
    }
    var par = ParametryPobierz(target, null);
    for (var i = 0; i < par.length; i++) {
        parametry.push({ name: par[i].name, value: par[i].value });
    }
    var szukanie = $('[data-szukanie="wartosc" ]').val();
    if (szukanie != undefined) {
        szukanie = OczyscFraze(szukanie);
    } else {
        szukanie = '';
    }
    parametry.push({ name: "SzukanaFraza", value: szukanie });
    parametry.push({ name: "WybraneSortowanie", value: $('#sortowanie-koszyk').attr('data-sortowanie-pole') });
    WczytajAjax($('#koszyk-lista-produktow'), metoda, LadowanieKoszykaCallback, "POST", parametry);
}

//function LadowanieKoszykaCallback(target) {
//    InicjacjaJs(target);
//    PobierzIloscPozycjiWKoszyku();
//    $('#koszyk-uwagi', target).autogrow().maxlength({
//        threshold: 50,
//        warningClass: "label label-success",
//        limitReachedClass: "label label-danger",
//        validate: true,
//        placement: "top"
//    });
//    var fraza = $(target).attr('data-szukanie-placeholder');
//    var szukarka = DodajSzukanie('#belka-zarzadzanie-koszyk', fraza);
//    $('input[data-szukanie="wartosc"]', szukarka).on('keyup', function () {
//        delay(function () {
//            WyslijKoszykNaSerwer(target, '/Koszyk/Przelicz', true);
//        }, 500);
//    });
//    $('.przeladowanie select', target).on('change', function (e) {
//        PrzeliczKoszyk(e, '#koszyk-calosc-holder', true);
//    });
//    $('.przeladowanie input[type="radio"]', target).on('change', function (e) {
//        PrzeliczKoszyk(e, '#koszyk-calosc-holder', true);
//    });

//    $('.przeladowanie input[type="text"]', target).on('change', function (e) {
//        PrzeliczKoszyk(e, '#koszyk-calosc-holder', true);
//    });

    
//    $('[data-containter="modal"]').one('hidden.bs.modal', function (e) {
//        ZaladujKoszyk('#koszyk-calosc-holder', '/Koszyk/CzescDynamiczna');

//    });

//    $('#sortowanie-koszyk div a').on('click', function (e) {
//        var value = $(this).attr('data-sortowanie');
//        UstawWartoscWProfiluKlienta('KolumnaSortowaniaKoszykLista', value);
//        PrzeliczKoszyk(e, '#koszyk-calosc-holder', true);
//    });
//}

//function WybierzAdres(forma,data) {
//    var evt = window.event || arguments.callee.caller.arguments[0];
//    $('#SlownikParametrow_AdresId').append('<option value="' + data + '">My option</option>');
//    $('#SlownikParametrow_AdresId').val(data);
//    PrzeliczKoszyk(evt, '#koszyk-calosc-holder', true);
//    $('[data-dismiss="modal"]').click();
//}

function InicjacjaGratisowCallback(target) {
    $(target).one('hidden.bs.modal', function (e) {

        PrzeliczKoszyk(e, '#koszyk-calosc-holder', true);
    });
}
//function UsunPozycje(e, id, pozycjaid) {
//    $('[data-pozycjaid="' + pozycjaid + '"]').val('0');
//    PrzeliczKoszyk(e, id, true);
//}

//function PrzeliczKoszyk(e, id, pominwalidacje) {
//    try {
//        e.preventDefault();
//    } catch (ex) {
//    }
//    WyslijKoszykNaSerwer(id, '/Koszyk/Przelicz', pominwalidacje);
//}

//function WyczyscKoszyk(e, id) {
//    var tl = PobierzTlumaczenia("Czy na pewno chcesz usunąć wszystkie pozycje z koszyka?;Tak;Nie");
//        PotwierdzenieAlert(id, '', tl["Czy na pewno chcesz usunąć wszystkie pozycje z koszyka?"], tl["Tak"], tl["Nie"], function () {
//        $('[data-pozycjaid]').val('0');
//        PrzeliczKoszyk(e, id, true);
//    });

//}

function WyslijKoszykNaSerwer(id, metoda, pominwalidacje) {
    var walidacja = document.forms["koszyk-lista-pozycji"].checkValidity();
    if (walidacja || pominwalidacje == true) {
        var target = $('#koszyk-lista-pozycji', id);
        var postData = target.serializeArray();
        ZaladujKoszyk(id, metoda, postData);
    }
}

function DodajZbiorczo(sender) {
    DodajPozycjeWypelnione(0);
    return false;
}

function DodajPozycje(sender, produkt, il, jednostka, lista, indywidualne, typ) {
    var ind = $(sender).attr('data-indywidualizowany');
    var idBazowe = $(sender).find('#ProduktBazowyId').val();
    if (idBazowe == undefined) {
        idBazowe = $(sender).find('.ProduktBazowyId').val();
    }
    var ob = StworzObiekt(produkt, jednostka, il, typ,ind, idBazowe);
    if (ob != null) {
        if (ind != undefined && ind === "1") {
            indywidualne.push(ob);
        } else {
            lista.push(ob);
        }
        $(".przycisk-id-" + produkt + " .bsk").addClass('dodano-do-koszyka');
    }
}

function DodajIndywidualne(sender) {
    var lista = [];
    var parent = $(sender).parents('[data-indywidualizacja="panel"]');
    $('[data-indywidualizacja]', parent).each(function () {
        var produkt = $(this).attr('data-indywidualizacja');
        var produktBazowy = $(this).attr('data-indywidualizacja-bazowy');
        var jednostka = $(this).attr('data-indywidualizacja-jednostka');
        var typ = $(this).attr('data-typpozycji');
        var ilosc = $(this).attr('data-iloscpozycji');
        var ind = PobierzIndywidualizacje(this, produkt);
        var ob = StworzObiekt(produkt, jednostka, ilosc, typ, ind,produktBazowy);
        if (ob != null) {
            lista.push(ob);
        }
    });
    DodajDoKoszyka("#tabela-lista-produktow", lista);
    parent.find('[data-dismiss="modal"]').click();
    return false;
}


function PobierzIndywidualizacje(element,produkt) {
    var parametry = [];
    element = $(element);
    element.find(".parametry-indywidualizacji div").each(function (nr, el) {
            el = $(el);
            var idIndywidualizacji = el.attr('data-indywidualizacja-parametr');
            el.find(".wartosc-parametru input").each(function(nr2, el2) {
                el2 = $(el2);
                var wartosc = el2.val();
                parametry.push({ IndywidualizacjaID: idIndywidualizacji, Wartosc: wartosc, ProduktID: produkt });
            });
        });
    return parametry;
}


function StworzObiekt(produkt, jednostka, ilosc, typ, indywidualizacja, produktBazowy) {
    var ind = null;
    if (indywidualizacja != undefined) {
        ind = indywidualizacja;
    }
    //jednostka = jednostka);
    if (jednostka == '' || jednostka == undefined || isNaN(jednostka)) {
        jednostka = null;
    }
    //jednostka = Number(jednostka);
    var il = parseFloat(ilosc);
    if (produkt == 0 || il <= 0 || isNaN(il)) {
        return null;
    }
    var ob = { ProduktId: produkt, Ilosc: il, JednostkaId: jednostka, Indywidualizacja: ind, TypPozycji: typ, ProduktBazowyId: produktBazowy };
    return ob;
}
function DodajDoKoszyka(sender, lista) {
    if (lista.length > 0) {
        $.ajax({
            url: '/Koszyk/DodajPozycje',
            data: JSON.stringify(lista),
            type: 'POST',
            contentType: 'application/json',
            dataType: 'json'
        }).done(function (data) {
            var modal = data.CzyModal;
            var msg = data.Odpowiedzi;
            if (modal && msg != undefined) {
                //jesli ma się pokazać modal i jest wiadomość to pokazujemy modal 
                for (var i = 0; i < msg.length; i++) {
                    if (msg[i].Tekst != '') {
                        Informacja(sender, "Komunikat", msg[i].Tekst);
                    }
                }
            } else {
                DodajDoKoszykaKomunikaty(data);
            }
            
            try {
                $(".dodawanie-panel").tooltip('destroy');
            }catch (e) {
                console.log('Błąd niszczenia tooltipa.');
            }
            $(".dodawanie-panel").data("original-title", "");
            var zamykac = $('.dodawanie-panel').attr('data-zamykajModal');
            if (zamykac != undefined) {
                $('.dodawanie-panel').closest('.modal').modal('hide');

            }
            $(".dodawanie-panel input.input-ilosc").val("");
            //$(".dodawanie-panel input.input-ilosc", sender).val("");
            $(".dodawanie-panel").data('parametry-dymek', "");

            //odswiezamy pogląd koszyka
            OdswiezPodgladKoszyka(data);

            //Czy jestesmy w koszyku jak tak to przeladowywujemy

            var koszyk = $('#page-content').find('#koszyk-calosc-holder');
            if (koszyk!=undefined && koszyk.length!==0) {
                var idKontrolki = koszyk.data('id-kontrolki');
                WczytajAjax(koszyk, '/Koszyk/Calosc', null, 'POST', { Id: idKontrolki });
            }



            $(".dodawanie-panel", sender).each(function () {
                $('input.input-ilosc', this).val("");
            });
        });
    }
}
function OdswiezPodgladKoszyka(data) {
    var pozycje = data.Pozycje;
    if (data != null && pozycje.length !== 0) {
        for (var i = 0; i < pozycje.length; i++) {
            var klucz = pozycje[i].ProduktId;
            var ilosc = pozycje[i].Ilosc;

            var kluczObiektu = ".przycisk-id-" + klucz + " .bsk.bsk_icon";
            var spanIlosc = $(kluczObiektu + ' > span.ilosc');

            if (spanIlosc.length !== 0) {
                spanIlosc.html(ilosc);
            } else {
                spanIlosc = $(kluczObiektu);
                if (spanIlosc !== 0) {
                    var html = "<span class='ilosc'>" + ilosc + "</span>";
                    spanIlosc.append(html);
                }
            }
        }
        OdswiezWartosc(data);
    }
}

function DodajDoKoszykaKomunikaty(data) {
    for (var i = 0; i < data.Odpowiedzi.length; i++) {
        OdswiezWartosc(data);
        if (data.Odpowiedzi[i].Tekst != '') {
            ShowMessage('', data.Odpowiedzi[i].Tekst, data.Odpowiedzi[i].Typ, data.Odpowiedzi[i].CzasWyswietlania, true);
        }
    }
}
function KomunikatyModal(data) {
    var tresc = "";
    for (var i = 0; i < data.length; i++) {
        if (data[i].Tekst != '') {
            tresc += data[i].Tekst;
        }
    }
    WyswietlModal(tresc);
}
function OdswiezWartosc(data) {
    if (data != undefined) {
        if (data.Netto != null && data.Brutto != null && data.Netto.Wartosc != undefined && data.Netto.Wartosc != undefined)
        {
            if (data.Netto.Wartosc > 0) {
                $('#cena_netto', '.koszyk-podglad a')
                    .html(data.Netto.Wartosc.toString().replace('.', ',') + ' ' + data.Waluta);
            }
            if (data.Netto.Wartosc > 0) {
                $('#cena_brutto', '.koszyk-podglad a')
                    .html(data.Brutto.Wartosc.toString().replace('.', ',') + ' ' + data.Waluta);
            };
        }
        if (data.IloscPozycji != undefined) {
            $('.koszyk-podglad a #ilosc_pozycji').html(data.IloscPozycji);
        }
        if (data.IloscPozycji != undefined && data.IloscPozycji > 0) {
            $('.koszyk-podglad a').show();
            $('.koszyk-podglad a').removeClass('koszyk-podglad-pusty');
        } 
        if (data.IloscPozycji != undefined && data.IloscPozycji <= 0) {
            $('.koszyk-podglad a').addClass('koszyk-podglad-pusty');
        }
    }
}

function PobierzIloscPozycjiWKoszyku() {
    $.ajax({
        url: '/Koszyk/PobierzIloscPozycjiWKoszyku',
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json'
    }).done(function (data) {
        OdswiezWartosc(data);
    });
}

function DodajDoKoszykaDokument(id) {
    $.post('/Koszyk/DodajDokument', { id: id }, DodajDoKoszykaKomunikaty);

}
function InfoDostepnosc(sender, produkt) {
    ZmienStatusProduktu(sender, produkt, '/Koszyk/InfoDostepnoscZmien/' + produkt);
}


//------------------------
//wychodzenie ze strony
//------------------------

function SaWpisaneIlosci() {
    var saIlosci = false;
    $('.lista-produktow:not(.koszyk-lista-produktow) .dodawanie-panel input.input-ilosc').each(function () {
        if ($(this).val() != '') {
            saIlosci = true;
            return false;
        }
    });
    return saIlosci;
}

function LadnyKomunikatOpuscStrone(el, callbackTak, callbackNie) {
    var tl = PobierzTlumaczenia("Tak;Nie;Czy na pewno chcesz opuścić stronę? Wpisane ilości nie zostaną zapamiętane.");
    PotwierdzenieAlert(el, '', tl["Czy na pewno chcesz opuścić stronę? Wpisane ilości nie zostaną zapamiętane."], tl["Tak"], tl["Nie"], callbackTak, callbackNie);
}

function SprawdzIlosc(el, przeladujCallback) {
    el = $(el);
    if (SaWpisaneIlosci()) {
        LadnyKomunikatOpuscStrone(el, przeladujCallback);
    } else {
        przeladujCallback(el);
    }
}

function LadowanieGratisowCallback(target) {
    InitModal(target, InicjacjaGratisowCallback);

}



function ImportujPozycjeKoszyka(sender) {
    var tryb = sender.id;
    sender.disabled = true;
    var formData = new FormData();
    var url = '/Koszyk/Upload';
    if (tryb === "upload") {
        var file = document.getElementById("FileUpload").files[0];
        var dropDownVal = $("select[name=selectItem]").val();
        UstawWartoscWProfiluKlienta("KoszykOstatnioWybranyImportPozycji", dropDownVal);
        if (file == null || dropDownVal == null) {
            return null;
        }
        formData.append("FileUpload", file);
        formData.append("DropDownSelect", dropDownVal);
    }

    if (tryb === "send") {
        url = '/Koszyk/Import';
        var dataText = $("#importDane").val();
        formData.append("DataContext", dataText);
        var provider = $("#provider").val();
        formData.append("Provider", provider);
        if (dataText.trim() === "") return null;
    }
    $.ajax({
        type: "POST",
        url: url,
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            var wynik = response.data;
            sender.disabled = false;
            // DodajDoKoszykaKomunikaty(response.pozycje);
            var old = $("#manager-importow-rezultat-" + tryb).html();
            $("#manager-importow-rezultat-" + tryb).html(wynik + old);
            $("#komunikatPoImporcie-" + tryb).css("display", "");
            OdswiezPodgladKoszyka(response.pozycje);
            ZaladujKoszyk('#koszyk-calosc-holder', '/Koszyk/CzescDynamiczna');
        }
    });
}