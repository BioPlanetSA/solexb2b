///<reference path="../typings/jquery/jquery.d.ts"/>
///<reference path="_ajax.ts"/>

//obsługa przycisku wyślij do importu z pliku csv
function importujNaSerwerPlik(e) {
    e.preventDefault();
    let obiektKlikniety: JQuery = $(e.currentTarget);
    var formName = obiektKlikniety.closest('form').attr('id');
    if (document.forms[formName].checkValidity()) {
        var formaKliknieta = $(document.forms[formName]);
        var file = (<HTMLInputElement>formaKliknieta.find("#FileUpload")[0]).files[0];
        var typ = formaKliknieta.find("#Typ").val();

        var formData = new FormData();
        formData.append("FileUpload", file);
        formData.append("Typ", typ);

        var formURL = formaKliknieta.attr("action");
      //  obiektKlikniety.prop("disabled", true);

        let elementWKtorymPokazacAnimacjeLadowaniaAjax = $("#manager-importow-rezultat-upload");

        var ladowanie = PobierzHtmlLadowanie();
        elementWKtorymPokazacAnimacjeLadowaniaAjax.prepend(ladowanie);

        $.ajax(
        {
            url: formURL,
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success(response) {
                elementWKtorymPokazacAnimacjeLadowaniaAjax.find('.ajax-oczekiwanie').remove(); //usuwanie animacji
                obiektKlikniety.prop("disabled", false);
                // var old = $("#manager-importow-rezultat-upload").html();  - nie chemy starych danych, chcemy tylko nowe dane
                $("#manager-importow-rezultat-upload").html(response);
            },
            error(xhr, ajaxOptions, thrownError) {
                obiektKlikniety.prop("disabled", false);
                $("#manager-importow-rezultat-upload").html("ERROR: " + thrownError);
            }
        });
    }
}

