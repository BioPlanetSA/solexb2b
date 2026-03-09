function ZmianaSzablonuMaila(sender) {
    var pole = $(sender).attr('data-pole');
    var wartosc;
    if ($(sender).is("input")) {
        if ($(sender).is('input[type="checkbox"]')) {
            wartosc = $(sender).prop('checked');
        } else {
            wartosc = $(sender).val();
        }
    } else {
        wartosc = $(sender).attr('data-wartosc');
    }
    var idZdarzenia = $(sender).closest('.row').attr('data-id');
    var doKogo = $(sender).attr('data-doKogo');

    var div = '#SzablonMaila-' + idZdarzenia;
    var parametry = [];
    //$.ajax({ url: '/SzablonyMaili/AktualizujListe', type: 'GET', data: { idZdarzenia: idZdarzenia, pole: pole, wartosc: wartosc, doKogo: doKogo } });
    parametry.push({ name: 'idZdarzenia', value: idZdarzenia });
    parametry.push({ name: 'pole', value: pole });
    parametry.push({ name: 'wartosc', value: wartosc });
    if (doKogo != undefined) {
        parametry.push({ name: 'doKogo', value: doKogo });
    }
    WczytajAjax($(div), "/SzablonyMaili/AktualizujListe", null, "GET", parametry);
}