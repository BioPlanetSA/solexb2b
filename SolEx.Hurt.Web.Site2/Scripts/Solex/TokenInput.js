function PoprawInputytokenInput() {
    var el = $(this);
    var parent = el.closest('div');
    var name = parent.find('.nazwa-poczatek').prop('name');
    parent.find('.wartosc-ukryta').remove();
    var akt = el.tokenInput("get");
    for (var i = 0; i < akt.length; i++) {
        var input = $('<input>').prop('type', 'hidden').prop('name', name + 'artosc').addClass('wartosc-ukryta').val(akt[i].Value);
        parent.append(input);
    }
}