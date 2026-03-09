/// <reference path="../typings/jquery/jquery.d.ts"/>
/// <reference path="../typings/bootstrap/index.d.ts"/>
/// <reference path="../typings/perfect-scrollbar.d.ts"/>
/// <reference path="_ajax.ts"/>
/// <reference path="_helpers.ts"/>
/// <reference path="_helpers.ts"/>

///zaznacza aktywne menu na podstawie tagów data-
function zaznaczAktywneMenu() {
    //zaznaczanie wg. aktywnego ID treści
    let aktywneMenuId: number = $('body').data('aktywna-tresc-id');

    let elementyDoZaznaczenia;

    if (aktywneMenuId !== null && aktywneMenuId !== 0) {
        elementyDoZaznaczenia = $('[data-zaznacz-aktywne-menu] [data-menu-opcja-id=' + aktywneMenuId + ']');
    } else {
        console.debug('nie mozna znalezc elementu data-aktywna-tresc-id w body. blad skryptu');
    }

    //zaznaczenie wg. linku aktualnego - bo np. byl link alternatywny dla menu - i inny jest id tresci aktualny, ale musimy zaznaczyc pozycje w menu z ktorej wszedl user

    let obecnyLink = window.location.pathname;
    elementyDoZaznaczenia.add($('[data-zaznacz-aktywne-menu] [href="' + obecnyLink + '"]').parent());   //nadajemy klase aktynowny na parenta - czyli LI a nie samego a hrefa

    //zaznaczeni aktywnosci na elemencie i kazdym menu wyzej
    elementyDoZaznaczenia.each((index, element) => {
        let e = $(element);
        e.addClass('aktywny');

        //czy aktualne element jest w akordionie - jesli tak to trzeba inaczej szukac parenta i rozwinac akordiona parenta
        let parent = e.parent();
        if (parent.hasClass('akordion-tresc')) {
            parent.collapse('show').removeClass('collapsing').addClass('collapse in aktywny');  //po to zeby nie robic animacji takie klasy sa nadawane natychmiast
            //zaznaczanie wzywalacza  aktualengo menu akordionowego - nadkategoria
            $("a[data-target='#" + parent.attr('id') + "'").addClass('aktywny');
        } else {
            e.parentsUntil('.menu', '[data-menu-opcja-id]').addClass('aktywny');
        }
    });
}

class MenuHamburger {
    ///funkcja inicjalizuje menu hamburgerowe
    public menuHamburger_inicjalizuj() {
        if (helpers.BlokadaUruchomieniaDwaRazy('menuHamburger_inicjalizuj')) {
            return true;
        }

        let menuHamburger = $('div.menu-hamburger');

        //klikanie
        menuHamburger.each((index, element) => {
            let menu = $(element);

            //czy juz bylo inicjalizowany to menu - moze byc kilka hamburgerow na stronie
            if (menu.hasClass('hamburger-initialized')) {
                return;
            }

            menu.find('button.hamburger')
                .click(e => {
                    //pierwsze klikniecie doladowanie menu ajaxem
                    if (!menu.hasClass('hamburger-loaded-ajax')) {

                        //czy jestesmy na liscie produktow aktualnie
                        let kontrolkaListaProduktow = $('div.kontrolka-ListaProduktow  .lista-produktow-zawartosc');                    
                        let linkajax: string = menu.data('menu-ajax');

                        //czy szukanie globalne - jak tak to dodać trzeba do linka ajaxowego
                        if (kontrolkaListaProduktow !== null && kontrolkaListaProduktow.length > 0) {
                            let dataSzukanieGlobalne = kontrolkaListaProduktow.data('szukane');
                            if (dataSzukanieGlobalne.length >= 1) {
                                linkajax = linkajax + '/' + kontrolkaListaProduktow.data('szukane');
                            }
                        }

                        let menuKontenerAjax = menu.find('.navbar');
                        WczytajAjax(menuKontenerAjax, linkajax, x => {
                            this.toogleMenu($(e.currentTarget), menu);

                            //podpiecie pod zdarzenie przeladowania kategorii produktow - jesli sa kategorie produktow pokazywane w menu hamburger
                            if (!menu.hasClass('hamburger-odswierzanie-kategorii')) {
                                //czy kategori produktow sa w aktualnym menu
                                if (menu.find('div.kategorie-produktow') !==  null) {
                                    $('div.kontrolka-DrzewoKategorii .drzewo-kategorii').on('drzewo-kategorii-przeladowano',
                                        () => {
                                            menu.removeClass('hamburger-loaded-ajax');
                                            menu.find('div.navbar').removeClass('hamburger-initialized');
                                            console.debug('przeladowanie menu hamburger - z powodu przeładowania kategorii produktów');
                                        });
                                }
                                menu.addClass('hamburger-odswierzanie-kategorii');
                            }
                        }, 'GET', null, menuKontenerAjax);
                        menu.addClass('hamburger-loaded-ajax');
                    } else {
                        //klikanie w MENU - togle
                        this.toogleMenu($(e.currentTarget), menu);
                    }
                });

            menu.addClass('hamburger-initialized');
        });
    }

    public testowaMetodaZwracaZawsze1() {
        return 1;
    }

    protected zamknijMenuJesliKliknietaKategoriaProduktowNieMaDzieci(kliknietaKategoria: JQuery, menu: JQuery) {
        if (!kliknietaKategoria.parent().hasClass('tree-has-childs')) {
            console.debug('Kliknieta kategoria nie ma lisci wiec zamykam okno kategorii');
            this.przyciskZamknieciaMenu(menu).click();
        }
    }

    private przyciskZamknieciaMenu(menu: JQuery) {
        return menu.find('button.close-button');
    }

    private toogleMenu(kliknietyPrzycisk: JQuery, menu: JQuery) {
        kliknietyPrzycisk.toggleClass('open');
        menu.toggleClass('open');

        //pokazujemy czy ukrywamy
        if (menu.hasClass('open')) {
            //otwieramy

            //pokazania przykrycia tla
            $('<div class="modal-backdrop fade"></div>').appendTo(menu);
            //musi być jakakolwiek przerwa do nadania klasy IN zeby animacja CSS zadzialala - dom sie musi przeladowac
            window.setTimeout(function () { $('div.modal-backdrop.fade').addClass('in'); }, 1);

            //podeponij suwaki scroole ladniejsze niz standardowe o ile juz ich nie ma podpietych i przycisku zamykania
            let navbar = menu.find('div.navbar');

            if (!navbar.hasClass('hamburger-initialized')) {

                //BARTEK -  deaktywuje perfect scroola bo robi bardzo duzo problemow, a i tak na wersji mobilnej nie pokazuje sie (nie jest potrzebny)

                //navbar.perfectScrollbar({ suppressScrollX: true });
                ////dodatkowo odswierzanie pojemnika jesli jest zmiana wysokosci - czyli klikniety jakikolwiek element ktory moze sie rozszerzac
                ////zdarzenie wykonac PO rozszerzeniu
                //menu.find('ul.collapse').on('shown.bs.collapse hidden.bs.collapse', () => {
                //    navbar.perfectScrollbar('update');
                //});

                //podpiecie zamykania menu mobilnego
                let przyciskClose = this.przyciskZamknieciaMenu(menu);
                przyciskClose.addClass('hamburger-initialized').click(e => { this.toogleMenu(kliknietyPrzycisk, menu) });

                //przesuniecie na pozycje fixed do konca do prawej strony
                przyciskClose.css('left', navbar.width() - 70);

                //dodanie do menu akordiona stylow - zeby byly takie samo jak kategorie produktow sa
                navbar.find('div.menu-rozsuwane a').addClass('ramka-wykropkowana-dol');

                //podpiecie pod klikanie po kategoriach dodatkowego zdarzenia do sprawdzania czy to ostatni lisc jak tak to zamykanie menu
                navbar.find('.drzewo-kategorii a.link')
                    .click(klikniete => {
                        this.zamknijMenuJesliKliknietaKategoriaProduktowNieMaDzieci($(klikniete.currentTarget), menu);
                    });

                //dodanie inicjalizacji zeby juz nie robic 2 raz tego samego
                navbar.addClass('hamburger-initialized');
            }

            //rozwiniecie grupy aktywnej - o ile sa produkty pokazywane aktualnie - jesli nie ma produktow to nie bedzie kontrolki kategorii produktow
            navbar.find('div.tree-elements.grupa-aktywna').collapse('show');

        } else {
            //zamykamy
            //usuwanie przykrycia tla
            $(".modal-backdrop").remove();
        }
    }

}



let menuHamburger: MenuHamburger = new MenuHamburger();
