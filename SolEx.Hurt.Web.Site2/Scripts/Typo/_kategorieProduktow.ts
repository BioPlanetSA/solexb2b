/// <reference path="../typings/jquery/jquery.d.ts"/>
/// <reference path="_listaProduktow.ts"/>
/// <reference path="_helpers.ts"/>
/// <reference path="_cookies.ts"/>

/* ------ Customowe eventy-----------

 - $('.drzewo-kategorii').on('drzewo-kategorii-przeladowano',()=> {} ); - zdarzenie po przeładowaniu drzewa kategorii

*/


function DrzewoKategorii_ZaladujSkrypty() {
    $(document)
        .ready(function () {
            drzewo = new DrzewoKategorii();
            drzewo.ZaznaczAktualnieWybranaKategorie();
            drzewo.WlaczAjaxDlaDrzewaKategorii();
        });
}

class DrzewoKategorii {
    private drzewoKategorii: JQuery = $('.drzewo-kategorii');
    private czyDrzewoRozwinieteZawsze: boolean = this.drzewoKategorii.hasClass("rozwiniete-wszystko");

    private grupyTabyKontener: JQuery = this.drzewoKategorii.find('.tree-tab');
    private grupyTabyElementy = this.grupyTabyKontener.find('.tree-tab-item');
    private kategorieDanejGrupyDrzewoElementy = this.drzewoKategorii.find('.tree-elements');

    private nazwaCookieWybranaGrupa = "wybrana-grupa";

    private aktualnieWybranaKategoria(nowaWartosc?: number): number {
        if (nowaWartosc) {
            //jak jest podana nowa wartosc to zapis - jak nie to odczyt
            this.drzewoKategorii.data("kategorie-wybrane", nowaWartosc);
            return nowaWartosc;
        } else {
            return $(this.drzewoKategorii).data("kategorie-wybrane"); 
        }
    }

    //zaznacza aktualnie wybrana kategorie w drzewie kategorii
    public ZaznaczAktualnieWybranaKategorie(usunZaznaczenieZamiastDodaj?:boolean) {
        if (this.czyDrzewoRozwinieteZawsze) {
            return;
        }
        //wyszukanie kategorii ktora ma byc aktywna
        let aktualnieWybranaKategoria = this.aktualnieWybranaKategoria();
        let kategoriaElement = this.drzewoKategorii.find(".tree-element.kat-" + aktualnieWybranaKategoria);

        if (usunZaznaczenieZamiastDodaj) {
            kategoriaElement.removeClass("kategoria-wybrana");
            kategoriaElement.find(" > a").removeClass("kategoria-wybrana");
            //wszystkie parenty wyzej trzeba zaznaczyć
            kategoriaElement.parentsUntil(this.drzewoKategorii, ".tree-element").removeClass("kategoria-wybrana");
        } else {

            if (kategoriaElement.length === 0) {
                //brak kategorii - zaznaczenie pierwszego taba
                let grupaId = this.drzewoKategorii.find('div[data-grupa-id]').data('grupa-id');
                //Odczytywanie cookiesa
                let idGrupy = cookies.pobierzCookie(this.nazwaCookieWybranaGrupa);
                if (idGrupy !== null) {
                    grupaId = idGrupy;
                }
                
                this.WybranieGrupyKategorii(grupaId);
            } else {
                kategoriaElement.addClass("kategoria-wybrana");
                kategoriaElement.find(" > a").addClass("kategoria-wybrana");
                //wszystkie parenty wyzej trzeba zaznaczyć
                kategoriaElement.parentsUntil(this.drzewoKategorii, ".tree-element").addClass("kategoria-wybrana");

                //wybranie wlasciwej GRUPY kategorii - wyszukiwanie parenta dla kategorii o klasie .tree-elements

                let grupaTab = kategoriaElement.closest('div.tree-elements', this.drzewoKategorii[0]);

                if (grupaTab === null || grupaTab === undefined) {
                    console.error('brak grupy dla kategorii id');
                    return;
                }

                let grupaId = grupaTab.data('grupa-id');
                //Odczytywanie cookiesa
                let idGrupy = cookies.pobierzCookie(this.nazwaCookieWybranaGrupa);
                if (idGrupy !== null) {
                    grupaId = idGrupy;
                }
                this.WybranieGrupyKategorii(grupaId);
            }
        }
    }
    
    public WlaczAjaxDlaDrzewaKategorii() {
        //klikanie kategorii - wybieranie kategorii
        this.drzewoKategorii.find('a:not(.akordion-naglowek)')
            .click(link => {
                link.preventDefault();

                let obiektKlikniety = $(link.currentTarget);
                let idKategoriiNowej: number = obiektKlikniety.data('katid');

                //todo:jesli nowe id == stare id to schowanie kategorii (zwijanie)

                this.WybranieKategorii(idKategoriiNowej);

                //ustawienia id kategorii wybranej i grupy i przechodzimy na 1 strone zmieniac kategorie
                listaProduktow.ListaProduktow_Przeladuj(idKategoriiNowej, 1, null, "", null,true, true);
            });

        //klikanie tabów grupy - wybieranie grup
        this.grupyTabyElementy.click(link => {
            let obiektKlikniety = $(link.currentTarget);
            let idGrupyKliknietej = obiektKlikniety.data('grupa-id');
            cookies.ustawCookie(this.nazwaCookieWybranaGrupa, idGrupyKliknietej,null);
            this.WybranieGrupyKategorii(idGrupyKliknietej);
            });
    }

    private WybranieKategorii(idKategoriiNowej: number) {
        this.ZaznaczAktualnieWybranaKategorie(true);
        this.aktualnieWybranaKategoria(idKategoriiNowej);
        this.ZaznaczAktualnieWybranaKategorie();
   }

    public DrzewoKategorii_Przeladuj(szukanieGlobalne?: string) {
        let elementDoPrzeladowania: JQuery = $('.kontrolka-DrzewoKategorii .drzewo-kategorii');

        let kategoriaId: number = this.drzewoKategorii.data('kategorie-wybrane');
        let idKontrolki: number = elementDoPrzeladowania.data('id-kontrolki');

        WczytajAjax(elementDoPrzeladowania, '/KategorieProduktowe/DrzewkoKategorie', () => {
                this.PoZaladowaniuDrzewaKategoriiUstawienieWidocznoscGrup(elementDoPrzeladowania);
            },
            'POST', { wybranaKategoria: kategoriaId, idKontrolki: idKontrolki, szukanieGlobalne: szukanieGlobalne });

        this.drzewoKategorii.trigger("drzewo-kategorii-przeladowano");
    }

    private PoZaladowaniuDrzewaKategoriiUstawienieWidocznoscGrup(drzewoZaladowane: JQuery) {
        //zaznaczamy aktualna kategorie
        this.ZaznaczAktualnieWybranaKategorie();
    }

    public WyczyscAktualnieWybraneKategorieWDrzewku() {
        if (this.drzewoKategorii != undefined && this.drzewoKategorii.length !== 0) {
            this.drzewoKategorii.data('kategorie-wybrane', 0);   
        }
    }


   private WybranieGrupyKategorii(idWybranejGrupy: number) {
       if (idWybranejGrupy === null || idWybranejGrupy === 0 || idWybranejGrupy === undefined ) {
           console.error('Błąd! Id grupy nie moze byc 0 ani puste');
           return;
       }

       //odznaczenie wszystkich
       this.grupyTabyElementy.removeClass('grupa-aktywna');

       //zaznaczenie tylko tej kliknietej jednej
       this.grupyTabyElementy.find('.tree-grupa-' + idWybranejGrupy).addClass('grupa-aktywna');

       //ustawienie grupa kategorii widocznosci
       this.kategorieDanejGrupyDrzewoElementy.removeClass('grupa-aktywna');
       this.drzewoKategorii.find('.tree-grupa-' + idWybranejGrupy).addClass('grupa-aktywna');
   }

}



let drzewo: DrzewoKategorii = null;


