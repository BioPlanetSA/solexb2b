jQuery.fn.UstawMinimalnaSzerokoscZObrazka = function () {
    return this.each(function() {
        UstawMinimalnaSzerokosc(this);
    });
}

function UstawMinimalnaSzerokosc(sender) {
    var t = new Image();
    var img_element = $(sender);

    t.onload = function () {
        if (t.width == 0) {
            console.info('Błąd wyliczenia szerokości obrazka ' + t.src + ' wyliczona szerokość=' + t.width);
        } else {
            img_element.css('min-width', t.width);
        }
    };

    t.src = img_element.attr('src');



}
