function PokazKoszykPodglad(sender, ile) {
    $.get('/koszyk/podglad/' + ile, function (data) {
        data = $.trim(data);
        if (data != '') {
            $('.koszyk-podglad-wysuwany').html(data);
            $('.koszyk-podglad-wysuwany').css("display", "");
            $(sender).dropdownHover();
        } else {
            $('.koszyk-podglad-wysuwany').hide();
        }
    });
}