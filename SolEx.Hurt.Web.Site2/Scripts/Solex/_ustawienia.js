function WczytajUstawnia() {
    WczytajAjax($('#ustawieniaZawartosc'), '/Admin/UstawieniaLista', PoWczytaniuUstawien);
}

function PoWczytaniuUstawien() {
    $('body').scrollspy({ target: '#przewijanie' });
    $('.table-ustawienia a').on('click', PokazUstawienie);
    $('#search')
        .keyup(function() {
            SzukanieUstawien(this);
        });
    $('#search').keyup();
}

function PokazUstawienie(e) {
    e.preventDefault();
    var url = $(this).prop('href');
    $.ajax({ url: url, method: 'Post' })
        .done(function (data) {
            var pojemnik = $('[data-containter="modal"]');
            pojemnik.html(data);
            InitModal(pojemnik, null, WczytajUstawnia);
        });
}

//szukamy po ustawieniach 
function SzukanieUstawien(element) {
    var $rows = $('#ustawienia-szukanie-tabela tr');
    //pobieramy wartośc z pola i usuwamy podwujne spacje i trimujemy
    var value = $.trim($(element).val().replace(/\s+/g, ' '));
    if (value.length === 0) {
        //jeśli warość jest pusta czyli nie mamy czego szukać i pokazujemy wszystkie ustawienia 
        $rows.show();
    } else {
        //ukrywanie linijek które nie zawierają szukanych wartości 
        var val = value.toLowerCase();
        var elem = val.split(" ");
        $rows.show()
            .filter(function () {
                var text = $(this).text().replace(/\s+/g, ' ').toLowerCase();
                return !SzukajWystapien(text,elem);
            })
            .hide();
    }

    //pokazywanie/ukrywanie nagłówków do grup i podgrup    
    $('tr[id*="grupa"]')
        .show()
        .filter(
            function () {
                var ilosc = $("tr[data-grupa='" + $(this).attr('id') + "']:visible").size();
                var iloscpg = $("tr[data-podgrupa='" + $(this).attr('data-podgr') + "']:visible").size();
                return (ilosc + iloscpg) === 0;
            }
        )
        .hide();

    //pokazywanie/ukrywanie nemu z grupami
    $('.ustawienia-kategorie>nav>ul>li')
        .show()
        .filter(
            function () {
                var grupa = $('tr[id*="' + $(this).attr('data-gr') + '"]:visible').size();
                return grupa === 0;
            }
        )
        .hide();

    //pokazywanie/ukrywanie nemu z podgrupami
    $('.ustawienia-kategorie>nav>ul>li>ul>li')
      .show()
      .filter(
          function () {
              var grupa = $('tr[data-podgr*="' + $(this).attr('data-podgr') + '"]:visible').size();
              return grupa === 0;
          }
      )
      .hide();
}

//sprawdzamy czy w text znajdują się wartości podane w searchArray
//searchArray - tablica szukanych elementów
function SzukajWystapien(text, searchArray) {
    var czyJest = true;
    for (var i = 0; i < searchArray.length; i++) {
        if (!czyJest) return false;
        czyJest = SzukajWystapienia(text,searchArray[i]);
    }
    return czyJest;
}

//sprawdzamy czy w text zawiera wartość search
function SzukajWystapienia(text, search) {
    return text.indexOf(search) > -1;
}