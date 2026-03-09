/// <reference path="../typings/jquery/jquery.d.ts"/>
/// <reference path="_listaProduktow.ts"/>
/// <reference path="_ajax.ts"/>


let ustawieniaProfilKlienta: UstawieniaProfilKlienta;

function UstawieniaProfilKlienta_InicjalizujDlaListyProduktow(listaProduktow: JQuery) {
    if (ustawieniaProfilKlienta === undefined) {
        ustawieniaProfilKlienta = new UstawieniaProfilKlienta();
    }

    ustawieniaProfilKlienta.ZaladujUstawieniaProfilowePoZaladowaniuListyProduktow(listaProduktow);
}

class UstawieniaProfilKlienta {
    private ProfileKlientaBool;
    private RozmiarStrony;
    private SortowanieListaProduktow;
    private SzablonyListy;

    public ZaladujUstawieniaProfilowePoZaladowaniuListyProduktow(listaProduktow: JQuery) {
        this.ProfileKlientaBool = listaProduktow.find('.zmiana-profilu-bool');
        this.RozmiarStrony = listaProduktow.find('.produkty-dol');
        this.SortowanieListaProduktow = listaProduktow.find('.lista-produktow-sortowanie');
        this.SortowanieListaProduktow = listaProduktow.find('.lista-produktow-sortowanie');
        this.SzablonyListy = listaProduktow.find('.SzablonyListy');

        this.PodepnijZdarzenia_ProfileKlientaSelect();
    }

    private PodepnijZdarzenia_ProfileKlientaSelect() {
        let profileBoolSelect = this.ProfileKlientaBool.find('a.ustawienie-klienta');
        let filtryRozmiarStrony = this.RozmiarStrony.find('a.dropdown-item[data-rozmiarstrony]');
        let sortowanieProduktowSelect = this.SortowanieListaProduktow.find('a.dropdown-item');
        let szablonListySelect = this.SzablonyListy.find('a.szablon-listy');

        //obsługa klikniecia elementu który odpowiada za ustawienie profilu klienta (bool)
        profileBoolSelect.click((element) =>
            this.ZmienUstawienieBool(element)
        );
        //obsluga klikniecia buttona zmiany wielkosci strony
        filtryRozmiarStrony.click((element) =>
            this.RozmiarStronySelect(element)
        );
        //obsluga sortowania nad listą produktów
        sortowanieProduktowSelect.click((element) =>
            this.UstawSortowanieSelect($(element.currentTarget))
        );
        //obsługa wyboru szablonu listy produktów
        szablonListySelect.click((element) =>
            this.UstawSzablonListy($(element.currentTarget))
        );
    }


    private ZmienUstawienieBool(filtr) {
        let obiektKlikniety = $(filtr.currentTarget);
        let typUstawienia = obiektKlikniety.data('ustawienie');
        let wartosc = obiektKlikniety.data("wartosc");
        let obiektListyProduktow = $('.lista-produktow').filter(function () {
            return ($(this).parents().is('.kontrolka-ListaProduktow'));
        });
        let elementListy = obiektListyProduktow.find('.lista-produktow-zawartosc ');
        let numerStrony: number = 1;
        if (elementListy != null && elementListy) {
            numerStrony = elementListy.data('strona');
        }
        let wartoscUStawienia: boolean = wartosc as boolean;
            //typeof (wartosc) != "boolean" ? wartosc.toLowerCase() === "true" : wartosc;
        //pobieramy parametry kliknietego elementu (potrzebny jest nazwa ustawienia oraz wartość) i przesyłamy do uniwersalnej metody zapisującej ustawienie w profilu klienta
        this.WysljNowaWartosc(typUstawienia, wartoscUStawienia,true,numerStrony);

    }

    private RozmiarStronySelect(element) {
        let obiektKlikniety = $(element.currentTarget);
        let rozmiarStrony: number = obiektKlikniety.data('rozmiarstrony');
        //pobieramy parametry kliknietego elementu (potrzebny jest tylko rozmiar strony) i przesyłamy do uniwersalnej metody zapisującej ustawienie w profilu klienta
        this.WysljNowaWartosc("RozmiarStronyListaProduktow", rozmiarStrony,false);
    }

    private UstawSortowanieSelect(element) {
        let sortowanie = element.data('sortowanie');
        //pobieramy parametry kliknietego elementu (potrzebny jest tylko nazwa parametru według którego bedziemy sortować) i przesyłamy do uniwersalnej metody zapisującej ustawienie w profilu klienta
        this.WysljNowaWartosc("KolumnaSortowaniaListyProduktow", sortowanie, true);
    }

    private UstawSzablonListy(element) {
        let szablon = element.data('szablonlisty');
        this.WysljNowaWartosc("SzablonListy", szablon, true);
    }

    //metoda która wysyła request na serwer z typem ustawienia oraz wartości po czym przeładowywuje listę produktów
    private WysljNowaWartosc(typ, wartosc, idzNaGoreStrony, strona=1) {
        $.ajax(
            {
                url: "/ProfilKlienta/UstawWartoscUstawienia",
                type: "POST",
                data: { typ: typ, wartosc: wartosc }
            })
            .done(data => {
                listaProduktow.ListaProduktow_Przeladuj(null, strona, null, null, null, idzNaGoreStrony);
            });
    }

}



//$('#sortowanie-produkty div a')
//    .on('click',
//    function () {
//        var value = $(this).attr('data-sortowanie');
//        UstawWartoscWProfiluKlienta('KolumnaSortowaniaListyProduktow', value);
//    });