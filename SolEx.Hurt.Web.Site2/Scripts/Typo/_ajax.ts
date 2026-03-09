/// <reference path="../typings/jquery/jquery.d.ts"/>

function WczytajAjax(target, url, callback, metoda, parametry, elementWKtorymPokazacAnimacjeLadowaniaAjax?, przewinNaGore: boolean=false) {
    if (metoda == undefined) {
        metoda = "GET";
    }
    var referer = document.referrer;

    if (document.URL != referer && referer !== '') {
        var data = { name: 'referer', value: referer };
        if (parametry == undefined) {
            parametry = [];
        }
        parametry.referer = referer;
    }

    if (url != undefined && url != '') {

        //sprawdzamy czy dal targetu jest juz wykonywane jakies zapytanie AJAXowe - jak tak to killujemy go
        var klucz = getPageFromUrl(url);
        var zadanie = target.data(klucz);
        if (zadanie != undefined) {
            zadanie.abort();    //killowanie starego zadania
        }

        if (elementWKtorymPokazacAnimacjeLadowaniaAjax == undefined) {
            elementWKtorymPokazacAnimacjeLadowaniaAjax = target;
        }

        var pozycjascroll = $(window).scrollTop();
        var ladowanie = PobierzHtmlLadowanie();
        elementWKtorymPokazacAnimacjeLadowaniaAjax.prepend(ladowanie);

        zadanie = $.ajax(
            {
                url: url,
                type: metoda,
                dataType: 'html',
                data: parametry,
                cache: false,
                success: data => {
                    target.html(data);  //podmiana html, zadanie sie samo skasuje po podmienie

                    elementWKtorymPokazacAnimacjeLadowaniaAjax.find('.ajax-oczekiwanie').remove(); //usuwanie animacji

                    //jak byl callback to wywołać
                    if (callback != undefined) {
                        callback(target);
                    }

                    //scrool
                    let rozmiar = Math.max($(target).height(), $(window).height());
                    //Sprawdzamy czy bedzie pokazywany modal - w tej sytuacji nie bedziemy przechodzić na góre strony
                    let czyModal = ($(target).data('containter') === "modal") as boolean;
                    if (przewinNaGore) {
                        $(window).scrollTop(0);
                    } else {
                        if (rozmiar < pozycjascroll && !czyModal) {
                            $(window).scrollTop(rozmiar);
                        } else {
                            $(window).scrollTop(pozycjascroll);
                        }
                    }
                },
                error: data => {
                    if (status != "abort") {
                        var fraza = 'Nie udało się wczytać danych, skontaktuj się z opiekunem';
                        target.html(fraza);
                    }
                }
            });
        //wstawienie zadania
        target.data(klucz, zadanie);
    }
}


function PobierzHtmlLadowanie() {
    var czekanie = $('<div class="c ajax-oczekiwanie" data-type="czekanie">   <img src="/Layout/images/gif-load.gif"/></div>');
    return czekanie;
}

//funkcja wycaiga z URL tylko utl bez QS
function getPageFromUrl(url) {
    var idx = url.indexOf('?');
    if (idx == -1) {
        return url;
    }
    return url.substring(0, idx);
}


function PobierzTlumaczenia(parameters) {
    var d;
    $.ajax({
        url: '/PobierzTlumaczenia',
        type: 'POST',
        data: { frazy: parameters },
        async: false
    }).done(function (data) {
        d = data;
    });
    return d;
}





//Array.prototype.RemoveElement = function (comparer) {
//    for (var i = 0; i < this.length; i++) {
//        if (comparer(this[i]))
//            this.splice(i, 1);
//    }
//};
//Array.prototype.inArray = function (comparer) {
//    for (var i = 0; i < this.length; i++) {
//        if (comparer(this[i])) return true;
//    }
//    return false;
//};
//Array.prototype.pushOrReplace = function (element, comparer) {
//    if (this.inArray(comparer)) {
//        this.RemoveElement(comparer);
//    }
//    this.push(element);
//};

