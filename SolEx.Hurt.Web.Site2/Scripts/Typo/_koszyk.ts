///<reference path="../typings/Ajax.d.ts"/>
/// <reference path="../typings/jquery/jquery.d.ts"/>
/// <reference path="_ajax.ts"/>
///<reference path="_alerty.ts"/>
///<reference path="_ustawieniaProfilKlienta.ts"/>

let koszyk: Koszyk;
function koszyk_ZaladujSkrypty() {
    koszyk = new Koszyk();

    //usuwamy widoczne tooltipy
    helpers.UsunTooltip();

    //musi byc uruchomione 
    koszyk.ZdarzeniaPoZaladowaniuKoszyka();
    koszyk.ZmienRozmiarPolaZUwagiami();
}

class PozycjaKoszyka {
    public Id;
    public Ilosc;
    public JednostkaId;
    public Indywidualizacja;
   
    constructor(idPozycji: number, ilosc: number, idJednostki: number, indywidualizacja: IndywidualizacjePozycji[] ) {
        this.Id = idPozycji;
        this.Ilosc = ilosc;
        this.JednostkaId = idJednostki;
        this.Indywidualizacja = indywidualizacja;
    }
}

class IndywidualizacjePozycji {
    public IndywidualizacjaID:number;
    public PozycjaId:number;
    public Wartosc;

    constructor(indywidualizacjaId: number, pozycjaId: number, wartosc: any) {
        this.IndywidualizacjaID = indywidualizacjaId;
        this.PozycjaId = pozycjaId;
        this.Wartosc = wartosc;
    }
}

class ParametryDoDodatkowychPolWKoszyku {
    public IdKontrolki;
    public Wartosc;
    public CzyElementJestDodatkowymParametrem;
    public Modul;
    public Symbol;

    constructor(idKontrolki: number, wartosc: string, czyElementJestPolemDodatkowym: boolean, modul: string, symbol:string) {
        this.IdKontrolki = idKontrolki;
        this.Wartosc = wartosc;
        this.CzyElementJestDodatkowymParametrem = czyElementJestPolemDodatkowym;
        this.Modul = modul;
        this.Symbol = symbol;
    }

}


class Koszyk {

    private KoszykCalosc = $('#koszyk-calosc-holder');
    private PrzyciskiPrzelicz;
    private UsunPozycje;
    private WyczyscKoszyk;
    private Uwagi;
    private ZmianaCeny;
    private ParametryKoszyka;
    private idKontrolki: number = this.KoszykCalosc.data('id-kontrolki');

    public ZmienRozmiarPolaZUwagiami() {
        this.Uwagi = this.KoszykCalosc.find('.koszyk-opis textarea');

        this.Uwagi.autogrow().maxlength({
            threshold: 50,
            warningClass: "label label-success",
            limitReachedClass: "label label-danger",
            validate: true,
            placement: "top"
        });

    }

    public ZdarzeniaPoZaladowaniuKoszyka() {
        InicjacjaJs(this.KoszykCalosc);
        this.PrzyciskiPrzelicz = this.KoszykCalosc.find('a.przelicz-koszyk');
        this.UsunPozycje = this.KoszykCalosc.find('td > a.usun-pozycje');
        this.WyczyscKoszyk = this.KoszykCalosc.find('.belka-nad-produktami a.wyczysc-koszyk');
        this.Uwagi = this.KoszykCalosc.find('.koszyk-opis textarea');
        this.ZmianaCeny = this.KoszykCalosc.find('.cena-zmian-przedstawiciel input.zmiana-ceny-input');
        this.ParametryKoszyka = this.KoszykCalosc.find('.parametry-koszyk-wybor');

        var self = this;

        //Obsluga klikniecia przeładowania koszyka
        this.PrzyciskiPrzelicz.click(() =>
            this.PrzeliczKoszyk()
        );
        //Akcja usuniecia pojedynczej pozycji z koszyka
        this.UsunPozycje.click((element) =>
            this.UsunWybranaPozycje(element)
        );
        //Akcja czyszczenia calego koszyka z confirmem
        this.WyczyscKoszyk.click(() => 
            this.UsunWszystkieElementyKoszyka(this.idKontrolki)
        );

        //podpiecie zdarzenia focusout na uwagi
        this.Uwagi.focusout((element) => {
            let elemetnKlikniety = $(element.currentTarget);
            let uwagi = elemetnKlikniety.val();
            WczytajAjax(elemetnKlikniety, "/Koszyk/AktualizacjaUwag", null, "Post", { uwagi: uwagi });
        });

        //podpiecie zdarzenia focusout na zmiane ceny przedstawiciela ciezkej sie wlamac
        this.ZmianaCeny.focusout((element) => {
            let elemetnKlikniety = $(element.currentTarget);
            let idPozycji = elemetnKlikniety.closest('tr[data-id-pozycji]').data('id-pozycji');
            let wartosc: number = elemetnKlikniety.val();
            WczytajAjax(elemetnKlikniety, "/Koszyk/AktualizujWymuszonaCenePrzedstawiciela", null, "Post", { idPozycji: idPozycji, cena: wartosc });
        });
        
        //podpiecie zdarzenia gdy ktos wybierze wartosc z dropdowna w koszyku
        $('.przeladowanie select', this.ParametryKoszyka).on('change', function (e) {
            self.WyslijZmianyPolaWlasnego($(e.currentTarget));
        });

        //podpiecie zdarzenia gdy ktos wybierze wartosc zdropdowna na liscie produktow
        $('.przeladowanie input[type="text"]', this.ParametryKoszyka).on('change', function (e) {
            self.WyslijZmianyPolaWlasnego($(e.currentTarget));
        });
        //podpiecie zdarzenia zmiany na impucie ktory ma opcje true/false
        $('.przeladowanie input[type="radio"]', this.ParametryKoszyka).on('change', function (e) {
            self.WyslijZmianyPolaWlasnego($(e.currentTarget));
        });
        //podpiecie przycisku finalizacji koszyka
        $('#koszyk-calosc-holder .finalizuj').on('click', function (e) {
            self.FinalizujKoszyk(self.idKontrolki);
        });
         //podpiecie przycisku odrzucenia koszyka
        $('#koszyk-calosc-holder .odrzuc').on('click', function (e) {
            e.preventDefault();
            self.OdrzucKoszyk(self.idKontrolki);

        });
        //podpiecie zdarzenia sortowania
        $('#sortowanie-koszyk div a').on('click', function (e) {
            var value = $(e.currentTarget).attr('data-sortowanie');

            $.ajax(
                {
                    url: "/ProfilKlienta/UstawWartoscUstawienia",
                    type: "POST",
                    data: { typ: "KolumnaSortowaniaKoszykLista", wartosc: value }
                })
                .done(data => {
                    WczytajAjax(self.KoszykCalosc, "/Koszyk/CzescDynamiczna", () => {
                        koszyk_ZaladujSkrypty();
                    }, 'POST', { Id: self.idKontrolki }, null, false);
                });
           
        });

        $('body').delegate('.modal.szablon-Adresy-edycja form', 'submit', function (e) {
            e.preventDefault();
            var forma = $(this);
            self.AktualizujAdres(forma);
        });
    };

    public AktualizujAdres(forma) {
        let postData = forma.serializeArray();
        let formUrl = forma.attr("action");
        postData.push({ name: 'czyZKoszyka', value: true });
        $.ajax({
            url: formUrl,
            type: "POST",
            data: postData
        }).done(data => {
            forma.closest('.modal').modal('hide');
            this.PrzeliczKoszyk();
        });
    }
    private OdrzucKoszyk(idKontrolki: number) {
        let pozycje = this.PobierzPozycjeKoszyka();
        let koszykCalosc = this.KoszykCalosc;
        let komunikat: string;
        let fraza: string;
        fraza = "Czy na pewno chcesz odrzucić zamówienie?";
        komunikat = PobierzTlumaczenia("Czy na pewno chcesz odrzucić zamówienie?;Tak;Nie");
        //wrzucamy komunikat w odpowiednie znaczniki htmlowe
        let komunikatKoncowy = "<span style=\"color:red;\">" + komunikat[fraza] + "</span>";

        PotwierdzenieAlert(koszykCalosc, '', komunikatKoncowy, komunikat["Tak"], komunikat["Nie"],
            function () {
                WczytajAjax(koszykCalosc, "/Koszyk/Odrzuc",
                    () => {
                        koszyk_ZaladujSkrypty();
                    }, "POST", { Pozycje: pozycje, IdKontrolki: idKontrolki });
            });


    }
    private FinalizujKoszyk(idKontrolki:number) {
        let koszykCalosc = this.KoszykCalosc;
         //sprawdzamy była zmiana cen
        let czyJestZmianaCen = koszykCalosc.find('.koszyk-lista-pozycji').data('utratacen') as boolean;

        let komunikat: string;
        let fraza: string;
        //ustawiamy odpowiedni komunikat zależny od tego czy jest wymuszona cena czy nie
        if (!czyJestZmianaCen) {
            komunikat = PobierzTlumaczenia("Czy na pewno chcesz zakończyć i przesłać zamówienie?;Tak;Nie");
            fraza = "Czy na pewno chcesz zakończyć i przesłać zamówienie?";
        } else {
            komunikat = PobierzTlumaczenia("Uwaga! ceny były zmieniane prze przedstawiciela - tylko on może zatwierdzić z zmienionymi cenami. Jeśli zatwierdzisz zamówienie samodzielnie to utracisz ceny wpisane przez przedstawiciela;Tak;Nie");
            fraza = "Uwaga! ceny były zmieniane prze przedstawiciela - tylko on może zatwierdzić z zmienionymi cenami. Jeśli zatwierdzisz zamówienie samodzielnie to utracisz ceny wpisane przez przedstawiciela";
        }
        //wrzucamy komunikat w odpowiednie znaczniki htmlowe
        let komunikatKoncowy = "<span style=\"color:red;\">" + komunikat[fraza] + "</span>";
        //pobieramy prametry pozycji z koszyka (idpozcyji, ilosc, jednostka)
        let pozycje = this.PobierzPozycjeKoszyka();
        //Tworzymy potwierdzenia po czym wysyłamy żądanie na serwer aby sfinalizować koszyk
        PotwierdzenieAlert(koszykCalosc, '', komunikatKoncowy, komunikat["Tak"], komunikat["Nie"],
            function () {
                WczytajAjax(koszykCalosc, "/Koszyk/Finalizuj",
                    () => {
                        koszyk_ZaladujSkrypty();
                    }, "POST", { Pozycje: pozycje, IdKontrolki: idKontrolki });
            });

    }

    private PrzeliczKoszyk() {
        //pobieramy prametry pozycji z koszyka (idpozcyji, ilosc, jednostka)
        let pozycje: PozycjaKoszyka[] = this.PobierzPozycjeKoszyka();

        //wysylamy na serwer zadanie przeliczenia koszyka
        WczytajAjax(this.KoszykCalosc, "/Koszyk/Przelicz", (data) => {
            koszyk_ZaladujSkrypty();
        }, 'POST', { Pozycje: pozycje, IdKontrolki: this.idKontrolki }, null, false);

      
    }

    //pobieramy id pozycji oraz usuwamy go z koszyka
    private UsunWybranaPozycje(element) {
        //pobieramy id pozycji oraz id kontrolki 
        let idPozycji: number[] = $(element.currentTarget).data('id-pozycji');
        WczytajAjax(this.KoszykCalosc, "/Koszyk/UsunPozycjeZKoszyka", () => {
            koszyk_ZaladujSkrypty();
        }, "POST", { pozycje: idPozycji, idKontrolki: this.idKontrolki } , null, false  );


        //UsunPozycjeKoszyka_Ajax(idKontrolki,idPozycji);

    }
    //metoda wyświetlająca confirm czy na pewno chcemy usunąć wszystkie pozycje z koszyka
    private UsunWszystkieElementyKoszyka(idKontrolki: number) {
        
        var tl = PobierzTlumaczenia("Czy na pewno chcesz usunąć wszystkie pozycje z koszyka?;Tak;Nie");
        PotwierdzenieAlert(this.KoszykCalosc, '', tl["Czy na pewno chcesz usunąć wszystkie pozycje z koszyka?"], tl["Tak"], tl["Nie"],
            function () {
                WczytajAjax($('#koszyk-calosc-holder'), "/Koszyk/UsunPozycjeZKoszyka",
                () => {
                    koszyk_ZaladujSkrypty();
                    }, "POST", { pozycje: [], idKontrolki: idKontrolki}, null, false );
            });
    }


    //Pobieranie parametrow indywidualizacji
    private PobierzIndywidualizacjePozycji(element: JQuery, idPozycji: number): IndywidualizacjePozycji[] {

        let parametryIndywidualizacji: IndywidualizacjePozycji[] = [];
        //Wyciagamy diva w ktorym jest indywidualizacja
        let indywidualizacja = element.find('.indywidualizacje-pozycji');
        if (indywidualizacja.length === 0) {
            return parametryIndywidualizacji;
        }
        //znajdujemy wszystkie indywidualizacje dla produktu
        let indywidualizacje = indywidualizacja.find('div[data-indywidualizacja-parametr]');
        //przechodzimy po indywidualizacjach i budujemy tablice indywidualizacji ktore będą pozniej wysylane na serwer
        for (var i = 0; i < indywidualizacje.length; i++) {
            let element = $(indywidualizacje[i]);
             //pobieramy id indywidualizacji
            let indywidualizacjaId: number = element.data('indywidualizacja-parametr');
             //pobieramy wartosc indywidualizacji
            let wartosc = element.find('span.wartosc-parametru input').val();
            parametryIndywidualizacji.push(new IndywidualizacjePozycji(indywidualizacjaId, idPozycji, wartosc));
        }
        return parametryIndywidualizacji;
    }
    //pobieramy prametry pozycji z koszyka (idpozcyji, ilosc, jednostka)
    private PobierzPozcjeKoszyka(element: JQuery, idPozycji, indywidualizacja: IndywidualizacjePozycji[]): PozycjaKoszyka {

        let dodawnieDoKoszyka = $(element).find('.kolumna-dodawanie-do-koszyka');
        let ilosc = dodawnieDoKoszyka.find('input.input-ilosc').val();
        let jednostka = dodawnieDoKoszyka.find('input.jednostki, select.jednostki option:selected').val();
        return new PozycjaKoszyka(idPozycji, ilosc, jednostka, indywidualizacja);
    }

    //pobieramy wymuszoną cene przedstawiciela
        private PobierzWymuszonaCene(element: JQuery): number {
        //Sprawdzamy czy jest zmiana cen przedstawiciela
        let zmianaCeny = $(element).find('.cena-zmian-przedstawiciel');
        if (zmianaCeny.length === 0) {
            //brak zwracamy nulla
            return null;
        }
        let cena: number = zmianaCeny.find('input.zmiana-ceny-input').val();
        return cena;
    }

    //funkcja majaca na celu pobranie parametrow dla zmienionego pola wlasnego koszyka i wyslanie ich na serwer
    private  WyslijZmianyPolaWlasnego(zmienionyElement) {
        //pobieramy jaka jest wartosc zmienianego pola
        let wybrany: string = zmienionyElement.val();
        //pobieramy albo id modulu albo nazwe pola wlasnego koszyka
        let element = zmienionyElement.data('modul');
        let elegmentGlowny = $('#koszyk-calosc-holder');
        //pobieramy symbol modulu
        let symbol: string = zmienionyElement.data('symbol');
        //sprawdzamy jest jest zmieniane pole to pola dodatkowe
        let czyElementJestDodatkowymParametrem: boolean = zmienionyElement.closest('.parametry-dodatkowe').length != 0;
        let idKontrolki: number = elegmentGlowny.data('id-kontrolki');

        let par: ParametryDoDodatkowychPolWKoszyku = new ParametryDoDodatkowychPolWKoszyku(idKontrolki, wybrany, czyElementJestDodatkowymParametrem, element, symbol);

        WczytajAjax(elegmentGlowny, "/Koszyk/UstawDodatkoweParametryKoszyka", () => {
            koszyk_ZaladujSkrypty();
            InicjacjaJs(elegmentGlowny);
        }, 'POST', par, null,false);
    }

    private PobierzPozycjeKoszyka(){
        let pozycje: PozycjaKoszyka[] = [];

        //pobieramy wiersz koszyka - pozycja koszyka
        let listaProduktow: JQuery = $('#koszyk-calosc-holder').find('#koszyk-lista-pozycji .koszyk-lista-produktow tbody>tr[data-id-pozycji]');
        //przechodzimy po poszczegolnych pozycjach i pobieramy parametry - id pozycji, ilosc produktu oraz jednostka
        for (var i = 0; i < listaProduktow.length; i++) {
            let element = $(listaProduktow[i]);

            //pobieramy idPozycji
            let idPozycji: number = element.data('id-pozycji');

            //pobieramy indywidualizacje dla pozycji
            let indywidualizacje: IndywidualizacjePozycji[] = this.PobierzIndywidualizacjePozycji(element, idPozycji);

            //wrzucamy parametry pozycji (idPozycji, ilość oraz jednostka)
            pozycje.push(this.PobierzPozcjeKoszyka(element, idPozycji, indywidualizacje));

        }
        return pozycje;
    }
   
}

