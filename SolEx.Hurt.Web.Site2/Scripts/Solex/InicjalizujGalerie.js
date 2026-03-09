
//duze jak i male zdjecia po kliknieciu pokazuja sie w colorbox
function InicjalizujGalerie() {
 //   var frazy = PobierzTlumaczenia("Następny;Poprzedni;Zamknij;Zdjęcie {current} z {total}");
    $('.galeria-produktu-miniaturki a.obrazki')
        .mouseenter(function() {
            var preset = $('.galeria-produktu-miniaturki').data().rozmiarDuzy;
            var zrodlo = $(this).attr('href');
            var numer = $(this).attr('numer');
            var galeria = zrodlo + '?preset=' + preset;
            var pobieranie = zrodlo + '?preset=full';
            $("#zdjecie-glowne-produktu img.zdjecie-produktu").attr('src', galeria);
            $("#pobieranie").attr('href', pobieranie);
            $("#zdjecie-glowne-produktu a.zdjecie-produktu").attr('href', pobieranie);
            $("#zdjecie-glowne-produktu img.zdjecie-produktu, #zdjecie-glowne-produktu a.zdjecie-produktu").attr('numer', numer);

            //info dodajmey na aktualnym elemencie ze to on jest teraz glownym
          //  $(this).addClass('aktualnieGlowne')
        });

    $('.galeria-produktu-miniaturki.galeria-podglad-po-kliknieciu a.obrazki, #zdjecie-glowne-produktu.galeria-podglad-po-kliknieciu a.zdjecie-produktu')
        .colorbox({
            rel: function() {
                if ($(this).hasClass('zdjecie-glowne')) {
                    return ':not([numer=' + $(this).attr('numer') + '].obrazki)';
                } else {
                    return ':not(.zdjecie-glowne)';
                }
            },
        maxHeight: '80%',
        maxWidth: '80%',
        next: '',
        previous: '',
        close: '',
        current: "{current}/{total}"
        //scrolling: false
});
    
    $('.wylacz-hrefy').click(function (e) {
        e.preventDefault();
    });

    //$('#zdjecie-glowne-produktu img.zdjecie-produktu').click(function () {
    //    var numer = $(this).attr('numer');L

    //    if (numer == null) {
    //        numer = 0;
    //    }

    //    $("a[numer='" + numer + "']").click();
    //});
 
}