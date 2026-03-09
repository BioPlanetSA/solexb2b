$(document).one('ready', function () {

    zaznaczAktywneMenu();

    var tl = PobierzTlumaczenia("Tak;Nie;Czy na pewno chcesz opuścić stronę? Wpisane ilości nie zostaną zapamiętane.;Wybierz plik");

    $(document).ready(function () {
        $('.submenu').on('mouseleave', function () {
            $('.opis-parent', this).show();
            $('div[data-opis]', this).hide();
        });
        $(' a[data-opis]').on({
            mouseover: function () {
                var el = $(this);
                $('div[data-opis]').hide();
                var submenu = el.closest('.submenu');
                $('.opis-parent', submenu).hide();
                var id = el.data('opis');
                $('div[data-opis="' + id + '"]').show();
            }
        });

    });

    $('body').delegate('a', 'click', UstawLinki);
    function UstawLinki(e) {
        var modaln = $(this).hasClass('okno-modalne');
        var lin = $(this).prop('href');
        // ~~~~~~~~~~~~~~~~~~~~~~~~
        //tylko linki z /m otwieramy w modalu - bron boze mi tu dopisaywac inne kwaity np. modal == true!! albo ,m !!!
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        if (modaln || lin.endsWith('/m') || lin.indexOf('/m?') > 0) {    //lin.indexOf(',m,') > 0 || lin.endsWith(',m') || lin.endsWith('%2cm') ||
            e.preventDefault();
            PokazModal(this);
        }
    }

    //page haeder jest fixex od góry, wiec calosc strony musi byc obnizona o wysokosc naglowka ktory jest doczepiony do góry
    //jesli jest banner topHeaderBanner - to kasujemy obnizanie - to sie zrobi dynamicznie przy ustawieniu dop bannera
    var wysokoscPageHeadera = $('#page-header').height();
    $('#page-header').css('height', wysokoscPageHeadera );
    $('#page-content').css('padding-top', wysokoscPageHeadera );

    InicjacjaJs(this);

    $('.slide-slick').slick({
        adaptiveHeight: true,
        dots: false,
        arrows: true,
        infinite: true,
        mobileFirst: true,
        pauseOnHover: true,
        pauseOnDotsHover: false,
        pauseOnFocus: false
    });

     $('#toTop').click(function () {
            $('body,html').animate({ scrollTop: 0 }, 800);
     });

    window.onbeforeunload = function() {
        if (SaWpisaneIlosci()) {           
            return tl["Czy na pewno chcesz opuścić stronę? Wpisane ilości nie zostaną zapamiętane."];
        }
    }

    $('img.podmieniajObrazkiPoNajechaniu').PodmieniajObrazkiPoNajechaniu(this);
    
    //blogi ladowanie dynamicznie aktualnosci
    $('.blog-lista-ladujaktuanosci-przycisk').click(function () {
        var el = $(this);
        var par = el.data('parametry');
        $.ajax({
            url: '/Blog/WpisyBlogaPobierzWpisy',
            type: 'POST',
            data: JSON.stringify(par),

            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                if (data === null || data === "" || data.pokazywacPrzyciskLadowania === false) {
                    el.hide();
                }
                el.closest(".blog-lista").find(".row").append(data.data);
                par.IloscJuzPokazanaKlientowi = data.ileJuzPokazaneKlientowi;   //mega wazna jest nazwa properosi jak sie nie zgodza to lipa
                el.data('parametry', par);
            },
            error: function () {
                console.log('blad wczytywania blogów');
            }
        });
    });


    //zmiana fileinputow na ładniejsze
      $('input[type="file"]').inputfile({
          uploadText: tl["Wybierz plik"],   //'<span class="fa fa-upload"></span> ' + 
          removeText: '<span class="fa fa-remove close"></span>',

          uploadButtonClass: 'btn btn-secondary',
          removeButtonClass: 'clickable'
      });   
});

var lastScrollTop = 0;

$(window).scroll(function () {
    //sticky ustawienie
    var navbarHeight = $('#page-header').height();
    var delta = 5;

    var st = $(this).scrollTop();

    // Make sure they scroll more than delta
    if (Math.abs(lastScrollTop - st) <= delta) {
        return;
    }

    // If they scrolled down and are past the navbar, add class .nav-up.
    // This is necessary so you never see what is "behind" the navbar.

    if ($(this).scrollTop() <= $('#page-header').height() ) {
        //kasowanie w ogole belki
        $('#page-header').removeClass('belka-plywajaca-aktywna-dol').removeClass('belka-plywajaca-aktywna-gora');
        return;
    }


    if (st > lastScrollTop && st > navbarHeight) {
        // Scroll Down
        $('#page-header').addClass("belka-plywajaca-aktywna-gora").removeClass('belka-plywajaca-aktywna-dol');
    } else {
        // Scroll Up
        if (st + $(window).height() < $(document).height()) {
            $('#page-header').addClass("belka-plywajaca-aktywna-dol").removeClass('belka-plywajaca-aktywna-gora');
        }
    }

    lastScrollTop = st;
});
