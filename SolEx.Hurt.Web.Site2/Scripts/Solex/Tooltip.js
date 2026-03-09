function ZaladujTooltipDwustanowy(sender) {
    var jest = $(sender).hasClass('dwustanowy-jest');
    var tytul = $(sender).attr(jest ? 'data-dwustanowy-title-jest' : 'data-dwustanowy-title-brak');

    $(sender).attr('title', tytul);
    $(sender).tooltip();
}
