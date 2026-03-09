jQuery.fn.ustawJakoTopBanner = function () {
    return this.each(function () {
        ustawJakoTopBanner(this);
    });
}

function ustawJakoTopBanner(sender) {
    //& $(sender).text().trim().length > 0
    if (sender != null) {
        //usuwanie obecnego headera - musi byc usuwanie zeby nie doklejac w nieskaczonosc nowych elementow
        $('#topHeaderBanner').html($(sender));

        //jesli jest jakis banner to usuwamy obnizanie contentu z powodu menu fixed
        if ($(sender).length) {
            $('#page-content').css('padding-top', 0);
        }

        //$(sender).appendTo('#topHeaderBanner');
        if (!$('.page-header').hasClass('centrujAbsolute')) {
            $('.page-header').addClass('centrujAbsolute');
        }
    }
}
