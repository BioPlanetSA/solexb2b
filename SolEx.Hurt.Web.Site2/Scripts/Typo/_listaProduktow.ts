/// <reference path="../typings/jquery/jquery.d.ts"/>
/// <reference path="../typings/jquery.highlight.d.ts"/>
/// <reference path="../typings/Ajax.d.ts"/>
/// <reference path="_ajax.ts"/>
/// <reference path="_wyszukiwarkaProduktow.ts"/>
/// <reference path="_kategorieProduktow.ts"/>
/// <reference path="_filtryProduktow.ts"/>
/// <reference path="_ustawieniaProfilKlienta.ts"/>
///<reference path="_drukowanieKatalogu.ts"/>
///<reference path="_helpers.ts"/>


/* ------ Customowe eventy-----------

 - this.ListaProduktowElement.trigger("lista-produktow.przeladawano"); - zdarzenie po zalogowaniu listy produktów, ma się wykonać event o nazwie lista-produktow.przeladawano

*/


let listaProduktow: ListaProduktow = null;

function listaProduktow_ZaladujSkrypty() {
    //bedzie wiele wywolywan tej metody - np. za kazdym przeladowaniem listy produktow ajaxowym

    listaProduktow = new ListaProduktow();
    //musi byc uruchomione 
    listaProduktow.ZdarzeniaPoZaladowaniuProduktow();
}


class WybranaCecha {
    public IdCecha: number;
    public IdAtrybutu: number;
    public AtrybutZezwalaNaMultiwybor: boolean;

    constructor(idCechy: number, idAtrybutu: number, atrybutZezwalaNaMultiwybor: boolean) {
        this.IdCecha = idCechy;
        this.IdAtrybutu = idAtrybutu;
        this.AtrybutZezwalaNaMultiwybor = atrybutZezwalaNaMultiwybor;
    }

}


class ListaProduktow {
    private ListaProduktowElement = $('.lista-produktow').filter(function() {
        return ($(this).parents().is('.kontrolka-ListaProduktow'));
    });
    private ListaProduktowPrzeladowywanyKontener = this.ListaProduktowElement.find('.lista-produktow-zawartosc');
    private StronnicowanieKontenery = $('.stronicowanie');

    public PobierzParametryListyProduktow(): Object {
        if (this.ListaProduktowPrzeladowywanyKontener === undefined) {
            return null;
        }

        return this.ListaProduktowPrzeladowywanyKontener.data();
    }

    ///Metoda przeladowywująca listę produktów - wartosc null w parametrze oznacza iż nie uległy one zmianie
    ///kategoria - kategoria która została wybrana
    ///numer strony na której mamy być
    ///szukanieGlobalne - fraza szukana w wyszukiwarce globalnej
    ///szukanieWewnetrzne - fraza szukana w wyszukiwarce wewnetrznej
    ///wybierzCeche - wybrana cecha - wybrany aktywny filtr
    ///przewinNaGore - określa czy po przeładowaniu chcemy zostać w tym samym miejscu co jesteśmy czy chcemy wrócić na góre stron
    ///usunFiltry - określa czy dla wybranego przeładowania chcemy usunąć filtry
    ///przeladujDrzewkoKategorii określa czy chcemy przeładować drzewko kategorii - na ten moment wykorzystywany parametr dla stałych filtrów
    public ListaProduktow_Przeladuj(kategoria?: number, strona?: number, szukanieGlobalne?: string, szukanieWewnetrzne?: string, wybierzCeche?: WybranaCecha, przewinNaGore:boolean=false, usunFiltry: boolean=false, przeladujDrzewkoKategorii: boolean=false ) {
        //zebranie parametrow
        let tablicaParametrow = this.PobierzParametryListyProduktow();

        let przeladujkategorie: boolean = false;

        if (kategoria != null) {
            if (!tablicaParametrow.hasOwnProperty('kategoria')) {
                alert('Brak wymaganego parametru kategoria dla listy produktow. Nie można załadować listy produktów.');
                return;
            }
            if (kategoria === 0) {
                tablicaParametrow['kategoria'] = null;
            } else {
                tablicaParametrow['kategoria'] = kategoria;
            }
        }

        if (strona != null) {
            if (!tablicaParametrow.hasOwnProperty('strona')) {
                alert('Brak wymaganego parametru strona dla listy produktow. Nie można załadować listy produktów.');
                return;
            }
            tablicaParametrow['strona'] = strona;
        }     

        if (szukanieGlobalne != null) {
            if (!tablicaParametrow.hasOwnProperty('szukane')) {
                alert('Brak wymaganego parametru szukanie dla listy produktow. Nie można załadować listy produktów.');
                return;
            }
            tablicaParametrow['szukane'] = szukanieGlobalne;
            przeladujkategorie = true;
            wyszukiwarkaProduktow.UstawFrazeWSzukarceGlobalnej(szukanieGlobalne);
        }  

        if (szukanieWewnetrzne != null) {
            if (!tablicaParametrow.hasOwnProperty('szukanawewnetrzne')) {
                alert('Brak wymaganego parametru szukanawewnetrzne dla listy produktow. Nie można załadować listy produktów.');
                return;
            }
            tablicaParametrow['szukanawewnetrzne'] = szukanieWewnetrzne;
            wyszukiwarkaProduktow.UstawFrazeWSzukarceWewnetrznej(szukanieWewnetrzne);
        }    

        if (usunFiltry) {
            tablicaParametrow['filtry'] = null;
        } else if (wybierzCeche != null) {
            tablicaParametrow['strona'] = 1;
            if (!tablicaParametrow.hasOwnProperty('filtry')) {
                alert('Brak wymaganego parametru filtry dla listy produktow. Nie można załadować listy produktów.');
                return;
            }
            let czyWymuszonaSciezkaFiltrow = this.ListaProduktowPrzeladowywanyKontener.data('wymuszona-sciezka');
            tablicaParametrow['filtry'] = filtryProduktow.UstawFiltry(tablicaParametrow['filtry'], wybierzCeche, czyWymuszonaSciezkaFiltrow);

            //tablicaParametrow['filtry'] = aktualnieWybraneFiltry;
        } else {
            tablicaParametrow['filtry'] = JSON.stringify(tablicaParametrow['filtry']);
        }

        WczytajAjax(this.ListaProduktowElement, '/Produkty/ListaWczytaj', () => {
            listaProduktow_ZaladujSkrypty();

            this.ZamienLink(tablicaParametrow, this.ListaProduktowElement);
            //kursor ma wrocic do pola ktorego wywolalo - JESLI bylo szukanie
            if (szukanieGlobalne != null && szukanieGlobalne !== "") {
                wyszukiwarkaProduktow.WrocKursorDoSzukarkiGlobalnej();
            }

            if (szukanieWewnetrzne != null && szukanieWewnetrzne !== "") {
                wyszukiwarkaProduktow.WrocKursorDoSzukarkiWewnetrznej();
            }
            InicjacjaJs(this.ListaProduktowElement);
        }, 'POST', tablicaParametrow, null,przewinNaGore );

        if (przeladujkategorie || przeladujDrzewkoKategorii) {
            //tworzenie nowej instacji drzewa, sytuacja gdy jest wybrany staly filtr ale nie ma produktów i nie ma rowniez drzewa
            if (drzewo == null) {
                drzewo = new DrzewoKategorii();
            }
            if (kategoria === 0) {
                drzewo.WyczyscAktualnieWybraneKategorieWDrzewku();
            }
            
            drzewo.DrzewoKategorii_Przeladuj(tablicaParametrow['szukane']);
        }
    }

    public ZdarzeniaPoZaladowaniuProduktow() {
        
        this.PodlaczStronnicowanie();
        wyszukiwarkaProduktow_ZaladujSkryptyPoZaladowaniuProduktow();

        //podswietlenie szukania o ile jest cos szukane
        this.PodswietlSzukaneFrazy();

        //usuwamy widoczne tooltipy
        helpers.UsunTooltip();

        //ladowanie filtrow
        FiltryProduktow_InicjalizujDlaListyProduktow(this.ListaProduktowPrzeladowywanyKontener);

        //ladowanie drukowania katalogu
    //    DrukowanieKatalogu_InicjalizujDlaListyProduktow(this.ListaProduktowPrzeladowywanyKontener);

        //ladowanie profilow klienta
        UstawieniaProfilKlienta_InicjalizujDlaListyProduktow(this.ListaProduktowPrzeladowywanyKontener);

        //podpinamy eventy
        this.ListaProduktowElement.trigger("lista-produktow.przeladawano");
    }

    

    private PodswietlSzukaneFrazy() {
        let szukanaFrazaGlobalnie: string = this.ListaProduktowPrzeladowywanyKontener.data('szukane');
        let szukanaFrazaWewnetrznie: string = this.ListaProduktowPrzeladowywanyKontener.data('szukanawewnetrzne');

        let sumaSzukan = szukanaFrazaGlobalnie + " " + szukanaFrazaWewnetrznie;

        if (sumaSzukan.length > 0) {
            this.ListaProduktowPrzeladowywanyKontener.highlight(sumaSzukan.split(" ")); //wypadlo by zeby szukac tylko w td.name - ale ten skrypt obecnie nie oblsuguje tego
        }
    }


    private PodlaczStronnicowanie() {
        //klikanie w numery stron
        this.StronnicowanieKontenery.find('a')
            .click((klikniety) => {
                klikniety.preventDefault();
                //obiekt klikniety
                let obiektKlikniety = $(klikniety.target);

                //strona aktualna
                let aktualnaStrona: number = this.ListaProduktowPrzeladowywanyKontener.data('strona');

                let numerWybranejStrony: number = obiektKlikniety.data('strona');

                if (aktualnaStrona != numerWybranejStrony) {
                    this.ListaProduktow_Przeladuj(null, numerWybranejStrony, null, null, null, true);
                }
            });

        //wpisanie w inputa
        this.StronnicowanieKontenery.find('input')
            .keypress((klawisz) => {
                //jak enter to submit jak nie to olewamy
                if (klawisz.which === 13) {
                    klawisz.preventDefault();
                    //obiekt klikniety
                    let obiektKlikniety = $(klawisz.target);
                    let numerWybranejStrony: number = obiektKlikniety.val();

                    //strona aktualna
                    let aktualnaStrona: number = this.ListaProduktowPrzeladowywanyKontener.data('strona');

                    //czy w przedziale
                    let maxStrona:number = obiektKlikniety.data('max');

                    if (numerWybranejStrony <= maxStrona && numerWybranejStrony > 0 && aktualnaStrona != numerWybranejStrony) {
                        this.ListaProduktow_Przeladuj(null, numerWybranejStrony, null, null, null, true);
                    } else {
                        obiektKlikniety.blur();
                    }
                }
            });

    }

    private ZamienLink(parametry, element) {
        let linkUrl = element.find('.lista-produktow-zawartosc').data('urllink');

        console.debug('url:' + window.location.href);

        if (linkUrl != undefined &&
            !(unescape(window.location.href).substr(linkUrl.length * -1, linkUrl.length) === linkUrl)) {
            window.history.pushState({ data: parametry }, null, linkUrl);
        }
    }

}

