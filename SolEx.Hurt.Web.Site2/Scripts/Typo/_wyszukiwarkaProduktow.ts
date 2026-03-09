/// <reference path="../typings/jquery/jquery.d.ts"/>
/// <reference path="_listaProduktow.ts"/>

let wyszukiwarkaProduktow: WyszukiwarkaProduktow;

function wyszukiwarkiProduktow_ZaladujSkrypty() {
    if (wyszukiwarkaProduktow === undefined) {
        wyszukiwarkaProduktow = new WyszukiwarkaProduktow();
        wyszukiwarkaProduktow.InicjalizujSzukarkeProduktowGlobalna();   //globalne sa tylko raz inicjalizowane na poczatku dlatego ze sa jedne na stronie i sa raczej stale - nie przeladowuja sie ajaxem
    }
}


function wyszukiwarkaProduktow_ZaladujSkryptyPoZaladowaniuProduktow() {
    wyszukiwarkaProduktow.InicjalizujSzukarkeProduktowWewnetrzna();
    wyszukiwarkaProduktow.InicjalizujCzyszczenieSzukania();
}

class WyszukiwarkaProduktow {
    private wyszukiwarkaGlobalna = $('.wyszukiwanie-globalne');
    private wyszukiwarkaGlobalnaInput = this.wyszukiwarkaGlobalna.find('input');
    private czyszczenieSzukania;

    private wyszukiwarkaWewnetrzna;

    public InicjalizujSzukarkeProduktowGlobalna() {
        this.wyszukiwarkaGlobalnaInput.keypress(klawisz => this.Szukaj(klawisz));
        this.wyszukiwarkaGlobalna.find('i.fa-search').click(x =>{
            var e = jQuery.Event("keypress");
            e.which = 13; //ENTER
            e.keyCode = 13;//ENTER
            this.wyszukiwarkaGlobalnaInput.trigger(e);
        } );
    }


    public UstawFrazeWSzukarceGlobalnej(fraza:string) {
        this.wyszukiwarkaGlobalnaInput.data('szukane', fraza);
        this.wyszukiwarkaGlobalnaInput.val(fraza);
    }

    public UstawFrazeWSzukarceWewnetrznej(fraza: string) {
        this.wyszukiwarkaWewnetrzna.data('szukane', fraza);
        this.wyszukiwarkaWewnetrzna.val(fraza);
    }

    public InicjalizujCzyszczenieSzukania() {
        this.czyszczenieSzukania = $('.czyszczenie-szukania');
        this.czyszczenieSzukania.click(() => {
            listaProduktow.ListaProduktow_Przeladuj(0, 1, "", "",null,true,true);
        });
    }

    public InicjalizujSzukarkeProduktowWewnetrzna() {
        this.wyszukiwarkaWewnetrzna = $('.wyszukiwanie-wewnetrzne > input');
        //wyszukiwarki wewnetrzne musza byc zawsze odszukane od nowa bo po kazdym przladowaniu ajaxa jest inna
        this.wyszukiwarkaWewnetrzna.keypress(klawisz => this.Szukaj(klawisz));
    }

    public WrocKursorDoSzukarkiGlobalnej() {
        this.wyszukiwarkaGlobalnaInput.focus().val(this.wyszukiwarkaGlobalnaInput.val());
    }

    public WrocKursorDoSzukarkiWewnetrznej() {
        this.wyszukiwarkaWewnetrzna.focus().val( $('.wyszukiwanie-wewnetrzne > input').val()  );
    }

    private Szukaj(klawisz) {
        //jak enter to submit jak nie to olewamy
        if (klawisz.which === 13)  {
            klawisz.preventDefault();

            //obiekt klikniety
            let obiektKlikniety = $(klawisz.target);
            let szukanaWartosc: string = obiektKlikniety.val();

            //czy ta sama wartosc co poprzednio - jak tak to przerywamy i nie robimy nic
            let szukanaWartoscStara: string = obiektKlikniety.data('szukane');
            if (szukanaWartoscStara === szukanaWartosc) {
                return;
            }

            //podmiana satrego szukania na nowe
            obiektKlikniety.data('szukane', szukanaWartosc);

            //jesli jestemy na produktach (istnieje klasa JS ListaProduktow) to ajax, jak nie to przeladowanie
            if (listaProduktow != null) {
                //jestesmy na liscie produktow 0 AJAXy beda
                if (obiektKlikniety.parent().hasClass('wyszukiwanie-globalne') ) {
                    //szukanie globalne
                    listaProduktow.ListaProduktow_Przeladuj(0, 1, szukanaWartosc, "", null,true, true);
                } else {
                     //szukanie wewnetrzne
                    listaProduktow.ListaProduktow_Przeladuj(null, 1, null, szukanaWartosc);
                }               
            } else {
                //redirect do strony produktów
                    var url = obiektKlikniety.parent().data('adres');
                    url += '?szukane=' + szukanaWartosc;
                     window.location.href = url;
            }


        }
    }

}



    //<script language="javascript" type= "text/javascript" >
    //    $(document).ready(function () {
    //        $('.wyszukiwanie .szukanie-fraza').on('keyup', function (e) {
    //            var el = $(this);
    //            var timer = el.data('timer');
    //            if (timer == undefined) {
    //                timer = 0;
    //            }
    //            clearTimeout(timer);
    //            var naProduktachAktualnie = el.attr('data-produkty') === "True";
    //            var czas = parseInt(el.attr('data-szukanie'));
    //            var code = e.keyCode || e.which;
    //            if (code === 13) { //Enter keycode
    //                if (naProduktachAktualnie) {
    //                    SzukajAkcja(el, SzukajAjax);
    //                } else {
    //                    SzukajAkcja(el, SzukajLadowanie);
    //                }
    //            }
    //            else if (czas > 0) {
    //                timer = setTimeout(function () {
    //                    if (naProduktachAktualnie) {
    //                        SzukajAkcja(el, SzukajAjax);
    //                    } else {
    //                        SzukajAkcja(el, SzukajLadowanie);
    //                    }
    //                }, czas);
    //                el.data('timer', timer);
    //            }
    //        });
    //    });

    //function SzukajAkcja(sender, callback) {
    //    var fraza = OczyscFraze(sender.val());
    //    var esc = encodeURI(fraza);
    //    var poprzednia = sender.attr('data-poprzednia');
    //    if (fraza != poprzednia) {
    //        sender.attr('data-poprzednia', fraza);
    //        callback(sender, esc);
    //    }
    //}
    //function SzukajLadowanie(sender, esc) {
    //    var url = $(sender).data('adres');
    //    url += '?szukane=' + esc;
    //    window.location = url;
    //}
    //function SzukajAjax(el, esc) {
    //    el = $(el);
    //    el.data('szukanafrazaglobalnie', el.val());
    //    el.data('kategorie', null);
    //    el.data('szukanafrazawewnatrzkategorii', null);
    //    var target = $('.lista-produktow-ajax').parent();
    //    ZaladujProdukty(target, true, el, '/Produkty/ListaWczytaj');
    //}
    //</script>