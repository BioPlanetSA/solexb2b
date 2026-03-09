/// <reference path="_listaProduktow.ts"/>



//function DrukowanieKatalogu_InicjalizujDlaListyProduktow(listaProduktow: JQuery) {
//    if (drukowanieKatalogu === undefined) {
//        drukowanieKatalogu = new DrukowanieKatalogu();
//    }

//    drukowanieKatalogu.ZaladujDrukowanieKataloguPoZaladowaniuListyProduktow(listaProduktow);
//}



class DrukowanieKatalogu {
    private BelkaNadProduktami;
   

    public DrukujSzablon(sender) {
        let forma = $('#katalog-drukowanie');

        //ustalenie formatu
        let formatInput: JQuery = forma.find('#format');

        if (formatInput === undefined || formatInput.length === 0) {
            var inputFormy = $("<input>").attr("type", "hidden").attr("name", "format").attr("id", "format").val(sender.getAttribute('data-format'));
            forma.append($(inputFormy));
        }else {
            formatInput.val(sender.getAttribute('data-format'));
        }

        //czy forma ma juz dodane dane produktow
        if (forma.hasClass('dodane-dane-produktow')) {
            return;
        }

        forma.addClass('dodane-dane-produktow');

         //jak jestemsy na liscie produktow to trzeba jeszcze przekopiowac pole daty z listy produktow zeby drukowac dla aktualnych produktow
        if (listaProduktow != null || listaProduktow != undefined) {
            let parametryListy = listaProduktow.PobierzParametryListyProduktow();
            if (parametryListy !== null) {
                for (var data in parametryListy) {
                    let wartosc = parametryListy[data];
                    if (typeof (parametryListy[data]) === 'object') {
                        wartosc = JSON.stringify(parametryListy[data]);
                    }

                    let formatInput: JQuery = forma.find('#' + data);

                    if (formatInput === undefined || formatInput.length === 0) {
                        var input = $("<input>").attr("type", "hidden").attr("name", data).attr("id", data).val(wartosc);
                        forma.append($(input));
                    } else {
                        formatInput.val(wartosc);
                    }
                }
            }
        }
        //Jesteœmy jesteœmy w koszyku nie podajemy tych parametrów


        

        //niech leci dalej submit normalny
        return;
    }

     public PokazOdpowiednieFormaty() {
         var select: HTMLSelectElement = <HTMLSelectElement>$('#katalog-drukowanie div.szablony-wydruku select')[0];
         let dostepneFormaty: string = select.options[select.selectedIndex].getAttribute('data-formaty');

             //if (dostepneFormaty === undefined) {
             //    dostepneFormaty = oknoDrukowania.find('div.szablony-wydruku > select option:first').data('formaty');
             //}

             if (dostepneFormaty === undefined) {
                 console.error('wygl¹da na to ¿e nie ma w ogóle szablonów do druku - b³¹d.');
                 return;
             }

            var formatyPrzyciski = $('#katalog-drukowanie .szablon-przycisk');

            for (let i = 0; i < formatyPrzyciski.length; i++) {
                var format = formatyPrzyciski[i].id;
                if (dostepneFormaty.indexOf(format) !== -1) {
                    formatyPrzyciski[i].style.display = "inline-block";
                } else {
                    formatyPrzyciski[i].style.display = "none";
                }
            }
        }



//      function PokazOdpowiednieFormaty(sender) {
//    var dostepneFormaty = sender.options[sender.selectedIndex].getAttribute('data-formaty');;

//    var formatyPrzyciski = document.getElementsByClassName('szablon-przycisk');

//    for (i = 0; i < formatyPrzyciski.length; i++) {
//        var format = formatyPrzyciski[i].id;
//        if (dostepneFormaty.indexOf(format) !== -1) {
//            formatyPrzyciski[i].style.display = "inline-block";
//        } else {
//            formatyPrzyciski[i].style.display = "none";
//        }
//    }
//}


    //laduje filtry z listy produktow
    //public ZaladujDrukowanieKataloguPoZaladowaniuListyProduktow(listaProduktow: JQuery) {
    //    this.BelkaNadProduktami = listaProduktow.find('.belka-nad-produktami');
    //    let drukujKatalogDiv = this.BelkaNadProduktami.find('.drukuj-katalog');
    //    let modal: JQuery = drukujKatalogDiv.find('.modal-okno');
    //    let przycisk = drukujKatalogDiv.find('button.drukarka');
    //    //podpinamy zdarzenie klikniêcia na button drukowania katalogu
    //    przycisk.click((element) => {
    //        if (!drukujKatalogDiv.hasClass('drukowanie-katalogu-loaded-ajax')) {
    //            let linkajax: string = drukujKatalogDiv.data('drukowanie-ajax');
    //            WczytajAjax(modal, linkajax, x => { this.toogleModal($(element.currentTarget), drukujKatalogDiv); }, 'GET', null, modal);
    //            drukujKatalogDiv.addClass('drukowanie-katalogu-loaded-ajax');
    //        }
    //    });
    //}



    //private ZaladujSkryptyDlaModala() {
    //    let szablonDrukowania: JQuery = $('.szablony-wydruku-okno-wyboru');
    //    let szablony: JQuery = szablonDrukowania.find('szablony-wydruku ');

    //    szablony.find('a.dropdown-item[data-id]') 
    //        .click((element) => {
    //            let wybranySzablon = szablony.find('span.wybrany-szablon');
    //            $(wybranySzablon).text("test");

    //        });
    //}

    //public PokazOknoDrukowania(obiektKlikniety: JQuery) {

    //    let linkajax: string = obiektKlikniety.data('url');
    //    WczytajAjax(,
    //        linkajax,
    //        x => { $($this).modal({ show: true }); }, 'GET', null);
    //}

}


let drukowanieKatalogu: DrukowanieKatalogu = new DrukowanieKatalogu();