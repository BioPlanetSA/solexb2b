function PoprawUrlOJezyk (url, symboljezyka) {
    if (url.substring(0, 4) === "http" || url.charAt(0) === "[") {
        return url;
    }

    if (url.charAt(0) !== '/') {
        alert('Zły link ajax - url musi zaczynać się od /. Zgłoś programiście. Link: ' + url);
        return url;
        // url = '/' + url;
    }

    if (url.indexOf(symboljezyka) < 0) {
        return (symboljezyka + url);
    }

    return url;
}



function PodmienTlumaczenie(text) {
    $('[data-dismiss="modal"]').click();
    $(".tlumaczenie-w-locie").val(text);
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

function UstawWartoscWProfiluKlienta(typ,wartosc) {
    var d;
    $.ajax({ url: '/ProfilKlienta/UstawWartoscUstawienia', data: { 'uniq_param': (new Date()).getTime(), Typ: typ, wartosc: wartosc }, async: false, cache: false }).done(function (data) {
        d = data;
    });
    return d;
}

function InicjacjaJs(parent) {
    $(".UstawMinimalnaSzerokoscZObrazka img", parent).UstawMinimalnaSzerokoscZObrazka();
    $(".ustawJakoTopBanner", parent).ustawJakoTopBanner();

    $('[data-original-title]:not([data-original-title=""]), [title]:not([title=""])', parent)
        .tooltip({ html: true, trigger: 'hover' });

    if (!('ontouchstart' in window)) {

        $("img.pokaz-popover", parent)
            .popover({
                html: true,
                trigger: 'hover',
                constraints: [
                    {
                        to: 'scrollParent',
                        attachment: 'together'
                    }
                ],
                delay: { "show": 0, "hide": 50 }
            });
    }

    InitDodawanieDoKoszyka(parent);

    $(".selectpicker", parent).selectpicker();

    InicjalizujGalerie(); //.galeria, .galeria-produktu-miniaturki

    $(".galeria-zdjec", parent)
        .unitegallery({
                theme_enable_text_panel: false,
                thumb_border_effect: false,
                thumb_color_overlay_effect: true,
                thumb_overlay_color: "#ffffff",
                thumb_overlay_reverse: false,
                thumb_overlay_opacity: 0.5,
                strippanel_enable_buttons: false,
                strippanel_enable_handle: false,
                theme_enable_hidepanel_button: false,
                slider_enable_zoom_panel: false,
                thumb_width: 120,
                thumb_height: 80,
                slider_enable_fullscreen_button: true,
                strip_thumbs_align: "center",
                theme_enable_fullscreen_button: false,
                strippanel_background_color: "rgb(239, 239, 239)"
            }
        );

    //obsługa przycisku wyślij z odpowiedzią zwrotną 
    $("button:submit[data-postback-ajax]")
        .on("click",
            function() {
                var sender = this;
                var formName = $(this).closest('form').attr('id');
                if (document.forms[formName].checkValidity()) {
                    var postData = $('#' + formName).serializeArray();
                    var formURL = $('#' + formName).attr("action");
                    sender.disabled = true;
                    $.ajax(
                    {
                        url: formURL,
                        type: "POST",
                        data: postData,
                        success: function(data) {
                            sender.disabled = false;
                            $("#result").html("<i class=\"fa fa-floppy-o\"></i>");
                        },
                        error: function(data) {
                            sender.disabled = false;
                            $("#result").html("<i class=\"fa fa-remove\"></i>");
                        }
                    });
                }
            });

    //ustawienie przepelnienia - trzeba minusowac ofset od gory - nie da sie trgo CSS zrobic niestety
    $(".przepelnienie")
        .each(function() {
            var top = $(this).offset().top;
            $(this).css('height', 'calc(100vh - ' + top + 'px');
        });

    //tłumacznie w locie 
    $(".tlumaczenie-w-locie")
        .on('click',
            function() {
                var hash = $(this).attr("data-hash");
                var url = "/Admin/TlumaczenieWLocie?hash=" + hash;
                PokazModal(this, url);
            });
}


function InicjujListe(pojemnik,selektomodal, akcja) {
    WczytajListeInternal(pojemnik, akcja);
    $('body').delegate(selektomodal, 'hidden.bs.modal', function () { WczytajListeInternal(pojemnik, akcja); }).delegate(selektomodal + ' form', 'submit', function (e) {
        e.preventDefault();
        var forma = $(this);
        WyslanieDanychNaSerwer(forma);
    }).delegate(pojemnik+ ' .btn-usun','click',function(e) {
        e.preventDefault();
        var tl = PobierzTlumaczenia("Tak;Nie;Czy usunąć wybrany element?");
        PotwierdzenieAlert($(this), '', tl["Czy usunąć wybrany element?"], tl["Tak"], tl["Nie"], function (el) {
            var url = el.prop('href');
            $.ajax({
                url: url,
                type: "POST"
            }).done(function () {
                WczytajListeInternal(pojemnik, akcja);
            });
        });

      
    });

}
function WyslanieDanychNaSerwer(forma, callback) {
    if (forma[0].checkValidity()) {
        var postData = forma.serializeArray();
        var formUrl = forma.attr("action");
        $.ajax({
            url: formUrl,
            type: "POST",
            data: postData
        }).done(function (data) {
            forma.closest('.modal').modal('hide');
            if (callback != undefined) {
                callback(forma, data);
            }
        });
    } else {
        forma.find(':submit').click();
    }
}
function WczytajListeInternal(pojemnik,akcja) {
    var elment = $(pojemnik);
    WczytajAjax(elment, akcja,PrzerobNaGridLadny, 'Post');
}
function PrzerobNaGridLadny(pojemnik) {
    var elem = pojemnik.find('table');
    
    var tl = PobierzTlumaczenia("Przetwarzanie...;Szukaj:;Pokaż _MENU_ pozycji;Pozycje od _START_ do _END_ z _TOTAL_ łącznie;Pozycji 0 z 0 dostępnych;(filtrowanie spośród _MAX_ dostępnych pozycji);Wczytywanie...;Nie znaleziono pasujących pozycji;Brak danych;Pierwsza;Poprzednia;Następna;Ostatnia;: sortuj rosnąco;: sortuj malejąco");

    var table = elem.DataTable({
        "paging": false,
        //"pagingType": "simple",
        "info":     false,
        "responsive": true,
        "language": {
            "processing":     tl["Przetwarzanie..."],
            "search":         tl["Szukaj:"],
            "lengthMenu":     tl["Pokaż _MENU_ pozycji"],
                "info":           tl["Pozycje od _START_ do _END_ z _TOTAL_ łącznie"],
                "infoEmpty":      tl["Pozycji 0 z 0 dostępnych"],
                "infoFiltered":   tl["(filtrowanie spośród _MAX_ dostępnych pozycji)"],
                "infoPostFix":    "",
                "loadingRecords": tl["Wczytywanie..."],
                "zeroRecords":    tl["Nie znaleziono pasujących pozycji"],
                "emptyTable":     tl["Brak danych"],
                "paginate": {
                    "first":      tl["Pierwsza"],
                    "previous":   tl["Poprzednia"],
                    "next":       tl["Następna"],
                    "last":       tl["Ostatnia"]
                },
                "aria": {
                    "sortAscending": tl[": sortuj rosnąco"],
                    "sortDescending": tl[": sortuj malejąco"]
                }
        }
    });

    pojemnik.find('.dataTables_filter').remove();
    pojemnik.find(".dataTables_wrapper").removeClass('form-inline');
    //// Apply the search
    elem.find('tfoot input').on('keyup change', function () {
        var pozycja = elem.find('tfoot input').index(this);
        table.columns(pozycja).search(this.value).draw();
        });
}

function ZaladujInfoOPliku(sender) {
    sender.disabled = false;
    var plik = $(sender).find(":selected").attr('data-plik-wzorcowy');
    if (plik != null && plik !== "") {

        $(".koszyk-import-przyklad a").attr("href", plik);
        $(".koszyk-import-przyklad").show();
    } else $(".koszyk-import-przyklad").hide();
}

function Close(sender) {
    $('#'+sender).remove();
}

function GenerowanieKluczaAPi(e, element, t) {
    e.preventDefault();
    var target = $(t);
    var tl = PobierzTlumaczenia("Czy napewno chcesz wygenerować nowy klucz do API? Wszystkie linki do plików zostaną zmienione. Stare linki przestaną działać!;Tak;Nie");
    PotwierdzenieAlert(element, '', tl["Czy napewno chcesz wygenerować nowy klucz do API? Wszystkie linki do plików zostaną zmienione. Stare linki przestaną działać!"],
            tl["Tak"], tl["Nie"], function () {
        var url = $(element).prop('href');
        WczytajAjax(target, url);
    });
}

//definicja funkcji endsWith dla IE 
if (!String.prototype.endsWith) {
    String.prototype.endsWith = function (searchString, position) {
        var subjectString = this.toString();
        if (typeof position !== 'number' || !isFinite(position) || Math.floor(position) !== position || position > subjectString.length) {
            position = subjectString.length;
        }
        position -= searchString.length;
        var lastIndex = subjectString.indexOf(searchString, position);
        return lastIndex !== -1 && lastIndex === position;
    };
}
