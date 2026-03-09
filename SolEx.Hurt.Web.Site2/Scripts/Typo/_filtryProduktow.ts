/// <reference path="../typings/jquery/jquery.d.ts"/>
/// <reference path="_listaProduktow.ts"/>
/// <reference path="_ajax.ts"/>
/// <reference path="../typings/perfect-scrollbar.d.ts"/>

let filtryProduktow: FiltryProduktow;

function FiltryProduktow_InicjalizujDlaListyProduktow(listaProduktow: JQuery) {
    if (filtryProduktow === undefined) {
        filtryProduktow = new FiltryProduktow();
    }
    
    filtryProduktow.ZaladujFiltryPoZaladowaniuListyProduktow(listaProduktow);
}



class FiltryProduktow {
    private FiltryWLiscieProduktow;
    private StaleFiltry;
    private BelkaNadProduktami;

    //laduje filtry z listy produktow
    public ZaladujFiltryPoZaladowaniuListyProduktow(listaProduktow: JQuery) {
        this.FiltryWLiscieProduktow = listaProduktow.find('.filtry');
        this.StaleFiltry = $('#page-content').find('.filtry-stale');
        this.BelkaNadProduktami = listaProduktow.find('.belka-nad-produktami');

        //podpinamy zdarzenia dla filtrów
        this.PodepnijZdarzenia_filtrySelecty(listaProduktow);
    }

    private ZaznaczWybraneFiltry() {
        //Znajdujemy te filtry które maja juz cos wybrane
        let wybraneFiltry = this.FiltryWLiscieProduktow.find('div.fil[data-wybrane != "null"]');

        //sprawdzamy czy coś zostało znalezione
        if (wybraneFiltry == null || wybraneFiltry.length === 0) {
            return;
        }

        //przechodzimy po atrybutach wybranych
        for (let i = 0; i < wybraneFiltry.length; i++) {
            //pobieramy konkretny filtr oraz wyciagamy jakie cechy był wybrane
            let elemtntWybrany = $(wybraneFiltry[i]);
            let wybraneWAtrybucie = elemtntWybrany.data('wybrane');
            //sprawdzamy czy coś zostało znalezione
            if (wybraneWAtrybucie == null || wybraneWAtrybucie.length === 0) {
                continue;
            }
            //przechodzimy po cechach wybranych w danym atrybucie
            for (let j = 0; j < wybraneWAtrybucie.length; j++) {
                //Znajdujemy ceche która była wybrana oraz dodajemy ptaszek iż był on juz zaznaczony
                let elementWybrany = elemtntWybrany.find('a.dropdown-item[data-id = "' + wybraneWAtrybucie[j] + '"]');
                if (elementWybrany.hasClass('filtr-wybrany')) {
                    continue;
                }
                elementWybrany.addClass('filtr-wybrany');
                $(elementWybrany).append('<i class="fa fa-check"></i>');
            }
        }
    }


    private PodepnijZdarzenia_filtrySelecty(listaProd: JQuery) {
        let filtrySelecty = this.FiltryWLiscieProduktow.find('.fil.fil-select[data-atrybut]  button');
        let filtryCheckBox = this.FiltryWLiscieProduktow.find('a.a-checkbox[data-id]');
        let staleFiltryCheckBox = this.StaleFiltry.find('a.a-checkbox[data-id]');
        let staleFiltryBtn = this.StaleFiltry.find('a.btn[data-id]');
        let wyczyscFiltry = listaProd.find('a.wyczysc-filtry');
        let wyczyscSzukanie = listaProd.find('a.wyczysc-wyszukiwanie');
        let usunKategorie = listaProd.find('a.wyczysc-katogorie');
        let usunPojedynczyFiltr = listaProd.find('a.usunPojedynczyFiltr');
        let usunPojedynczyStalyFiltr = listaProd.find('a.usun-staly-filtr');

        //Klikniecia - rozwiniecie dropdowna
        filtrySelecty.click((element) =>
            this.RozsuwanieFiltraSelecta(element)
          );

        //obsługa kliknięć checkbox dla aktywnych filtrów
        filtryCheckBox.click((element) =>
            this.ChechBoxFiltrSelect(element)
        );

        //sprawdzamy czy zostało dodane zdarzenie kliknięcia na stałe filtry (sprawdzenie jest potrzebne gdyż nie przeladowywujemy kontrolki stałych filtrów, bez if-a dodaje się wiele kliknięć na jeden element)
        if (!staleFiltryCheckBox.hasClass('staly-filtr-initialized')) {
            staleFiltryCheckBox.click((element) =>
                this.StaleFiltryChecBoxSelect(element,false)
            );
            staleFiltryCheckBox.addClass('staly-filtr-initialized');
        }

        if (!staleFiltryBtn.hasClass('staly-filtr-initialized')) {
            staleFiltryBtn.click((element) =>
                this.StaleFiltryChecBoxSelect(element, true)
            );
            staleFiltryBtn.addClass('staly-filtr-initialized');
        }
        //obsluga przycisku do czyszczenia aktywnych filtrów (nie czyści stałych filtrów)
        wyczyscFiltry.click(() => {
            listaProduktow.ListaProduktow_Przeladuj(0, 1, "", "", null,true, true,true);
        });

        //podpiecie zdarzenia usuniecia wybranej kategorii
        usunKategorie.click(() => {
            //Do momentu az nie bedzie wierowybieralnosci czyścimy kategorie
            listaProduktow.ListaProduktow_Przeladuj(0, 1, "", "", null, true, false, true);
        });

        //Podpinamy usuwanie pojedynczego stałego filtra z listy wybranych filtrów - nie jest to klikniecie w stały filtr
        usunPojedynczyStalyFiltr.click((element) => {
            //wyciagamy id stałego filtru który chcemy usunąć
            let idKlieknietegoStalegoFiltra: number = $(element.currentTarget).data('cecha');
            //Znajdujemy element który jest wyświetlany jako stały filtr (w kontrolce stałych filtrów)`
            let obiekt = this.StaleFiltry.find('a[data-id = "' + idKlieknietegoStalegoFiltra + '"]');
            let checkBox: JQuery = obiekt;
            //sprawdzamy czy obiekt stałego filtra jest btn jeżeli tak to musimy pobrać checkBox ktory jest w divie niżej 
            if (obiekt.hasClass('btn')) {
                checkBox = obiekt.find('div.a-checkbox');
            }
            //Wysyłamy zmiane stałego filtra na serwer, usuwamy klase zaznaczenia z checkboxa oraz klasa wybrania na stałym filtrze
            this.WyslijStaleFiltryNaSerwer("/Filtry/UsunStalyFiltr", idKlieknietegoStalegoFiltra, obiekt, checkBox, false);
        });

        //czyszczenie wyszukiwarki globalnej oraz wewnetrznej
        wyczyscSzukanie.click((element) => {
            let szukanie = $(element.currentTarget).data('szukanie');
            if (szukanie === 'SzukanaFrazaGlobalnie') {
                listaProduktow.ListaProduktow_Przeladuj(null, 1, "", null, null, false, false,true);
            } else if (szukanie === 'SzukanaFrazaWewnatrzKategorii') {
                listaProduktow.ListaProduktow_Przeladuj(null, 1, null, "", null, false, false);
            }
        });



        usunPojedynczyFiltr.click((element) => {
            this.UsunPojedynczyFiltr(element);
        });

    }

    private UsunPojedynczyFiltr(element) {
        let idCechy = $(element.currentTarget).data('cecha');
        let idAtrybutu = $(element.currentTarget).data('atrybut');
        listaProduktow.ListaProduktow_Przeladuj(0, 1, null, null, new WybranaCecha(idCechy, idAtrybutu,true), true, false);

    }

    public UstawFiltry(tablicaParametrow: { [index: number]: string[] }, wybierzCeche: WybranaCecha, czyWymuszonaSciezkaFiltrow) {

        let aktualnieWybraneFiltry: { [index: number]: string[] } = tablicaParametrow;
        //czy sa jakies inne cechy wybrane - jak nie to prosty temat
        if (aktualnieWybraneFiltry == null || Object.keys(aktualnieWybraneFiltry).length === 0) {

            aktualnieWybraneFiltry = {};
            aktualnieWybraneFiltry[wybierzCeche.IdAtrybutu] = [wybierzCeche.IdCecha.toString()];

        } else {
            //czy dany atrybut nie jest wybrany
            if (aktualnieWybraneFiltry[wybierzCeche.IdAtrybutu] == null) {
                //dodajemy atrybut i ceche
                aktualnieWybraneFiltry[wybierzCeche.IdAtrybutu] = [wybierzCeche.IdCecha.toString()];
            }
            //Atrybut jest wybrany
            else {
                //Sprawdzamy czy juz byl taka cecha wybrana
                let index: number = aktualnieWybraneFiltry[wybierzCeche.IdAtrybutu].indexOf(wybierzCeche.IdCecha.toString());

                if (index > -1) {

                    aktualnieWybraneFiltry[wybierzCeche.IdAtrybutu].splice(index, 1);
                    //Sprawdzamy czy jest cos jeszcze wybrane z danego atrybutu
                    if (aktualnieWybraneFiltry[wybierzCeche.IdAtrybutu].length === 0) {
                        if (czyWymuszonaSciezkaFiltrow) {
                            //pobieramy filtr na którym była zmana oraz pobieramy wszystkie fitry po nim które miały jakąś wybrana wartość
                            let elementyPoWybranym = $('div.fil[data-atrybut = "' + wybierzCeche.IdAtrybutu + '"]').nextAll('div.fil[data-wybrane != "null"]');
                            //przechodzimy po poszczegolnych filtrach (tych po naszym zmienianym) i usuwamy je z wybranych filtrow
                            elementyPoWybranym.each((element, value) => {
                                let idAtrybutu = $(value).data('atrybut');
                                delete aktualnieWybraneFiltry[idAtrybutu];
                            });
                            //usuwamy filtr na który był odznaczany
                            delete aktualnieWybraneFiltry[wybierzCeche.IdAtrybutu];
                        }
                    }
                } else {
                    //Jest multiwybor
                    if (wybierzCeche.AtrybutZezwalaNaMultiwybor) {
                        aktualnieWybraneFiltry[wybierzCeche.IdAtrybutu].push(wybierzCeche.IdCecha.toString());
                    }
                    //Brak multiwyboru
                    else {
                        aktualnieWybraneFiltry[wybierzCeche.IdAtrybutu] = [wybierzCeche.IdCecha.toString()];
                    }
                }
            }
        }
        return JSON.stringify(aktualnieWybraneFiltry);
    }

    private ChechBoxFiltrSelect(filtr) {
        //pobieramy prametry data ktore sa nam niezbedne do przeladowania listy
         let obiektKlikniety = $(filtr.currentTarget);
         let idAtrybutu: number = obiektKlikniety.data('atrybut');
         let idCechy: number = obiektKlikniety.data('id');
        //przeładowanie listy produktów z przekazaniem aktualnie kliknietego filtra (checkboxa)
         listaProduktow.ListaProduktow_Przeladuj(null, null, null, null, new WybranaCecha(idCechy, idAtrybutu, false),true);
    }

    private StaleFiltryChecBoxSelect(element, czyBtn:boolean) {
        element.preventDefault();
        let obiektKlikniety = $(element.currentTarget);
        let wyb = obiektKlikniety.data("wybrany");
        let wybrany: boolean = typeof (wyb) != "boolean" ? wyb.toLowerCase() === "true" : wyb ;
        let idCechy: number = obiektKlikniety.data("id");
        let checkBox: JQuery = obiektKlikniety;

        //Jeżeli jest stały filtr jako button to musimy dodatkow znaleść obiekt z klasa checkbox gdyz w sposobie pokazywania filtrów jako button checkbox jest opakowany w dodatkowego diva
        if (czyBtn) {
            checkBox = obiektKlikniety.find('div.a-checkbox');
        }

        //wysylamy request na serwer aby w profilu klienta zapisać dodanie bądź usuniecie klikniętego filtra, po skonczonym requescie dodajemy odpowiednie klasy gdyż nie przeladowywujemy stałych filtrów po ich wybraniu
        if (wybrany) {
            this.WyslijStaleFiltryNaSerwer("/Filtry/UsunStalyFiltr", idCechy, obiektKlikniety, checkBox, false);
        } else {

            this.WyslijStaleFiltryNaSerwer("/Filtry/DodajStalyFiltr", idCechy, obiektKlikniety,checkBox,true);
        }
    }


    private WyslijStaleFiltryNaSerwer(adres: string, idCechy: number, obiektKlikniety: JQuery, checkBox: JQuery, dodawanie: boolean) {
        $.ajax(
            {
                url: adres,
                type: "POST",
                data: { cecha: idCechy }
            }).done(data => {
                if (dodawanie) {
                    obiektKlikniety.data("wybrany", true);
                    checkBox.addClass("a-checkbox-checked");
                    obiektKlikniety.addClass("active");
                } else {
                    obiektKlikniety.data("wybrany", false);
                    checkBox.removeClass("a-checkbox-checked");
                    obiektKlikniety.removeClass("active");

                }
                listaProduktow.ListaProduktow_Przeladuj(null, 1, null, null, null, true, false, true);
            });
    }

    private RozsuwanieFiltraSelecta(filtr) {

        //$('.dropdown-menu').perfectScrollbar();

        //Ustawiamy zaznaczenie dla juz wybranych filtrow
        this.ZaznaczWybraneFiltry();

        let obiektKlikniety = $(filtr.target);
        let czyJuzAktywowanyWczesniej: boolean = obiektKlikniety.data('juz-aktywny');

        if (czyJuzAktywowanyWczesniej) {
            return;
        }
        let filtrParent = obiektKlikniety.closest('.fil-select', this.FiltryWLiscieProduktow);

        //podlaczenie klikania na linka
        filtrParent.find('a.dropdown-item[data-id]').click((element) => {
            let idAtrybutu: number = filtrParent.data('atrybut');
            let multiwybor: boolean = typeof (filtrParent.data('multiwybor')) != "boolean" ? filtrParent.data('multiwybor').toLowerCase() === "true" : filtrParent.data('multiwybor');
            let idCechy: number = $(element.currentTarget).data('id');
            listaProduktow.ListaProduktow_Przeladuj(null, null, null, null, new WybranaCecha(idCechy, idAtrybutu, multiwybor),true);
        });
                //danie info ze juz aktywowany filtr, zeby przy kolenymrozsuwanie nie robic go
        obiektKlikniety.data('juz-aktywny', true);

        //podpinamy perfect-scrollbar pod dropdowny w filtrach nad lista
        let menu = filtrParent.find('.dropdown-menu');
        if (menu.hasClass('dropdown-menu-rozmiar')) {
            return;
        }
        menu.addClass('dropdown-menu-rozmiar');
        menu.perfectScrollbar();
    }

    //akcja dla filtrow ktore wybiera sie klikajac np. selec, albo checkbox
    //private WybranieFiltra_klikanie(klikniety) {
    //    alert('klik fitra')
    //}
}