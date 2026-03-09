function DodajOkruszkiElementy(elementy) {
    var sciezka = $('.breadcrumb');
    for (var i = elementy.length - 1; i >= 0; i--) {
        var elnowy = $('<li class="dynamiczny"><a href="' + elementy[i].link + '">' + elementy[i].nazwa + '</a></li>');
        sciezka.append(elnowy);
    }
    sciezka.find('li:last').addClass('active');
}

function DodajOkruszekKategorii(el) {
    var elementy = [];
    var element = el.parent();
    var sciezka = $('.breadcrumb');
    sciezka.find('li.dynamiczny').remove();
    sciezka.find('li').removeClass('active');
    while (element.length > 0 && !element.hasClass('tree_tab_elem')) {
        var link = element.find('a:first').prop('href');
        var nazwa = element.find('a:first').clone().children().remove().end().text();
        elementy.push({ link: link, nazwa: nazwa });
        element = element.parent();
    }
    DodajOkruszkiElementy(elementy);
}


