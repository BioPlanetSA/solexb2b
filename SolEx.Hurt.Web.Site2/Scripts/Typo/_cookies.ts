/// <reference path="../typings/jquery/jquery.d.ts"/>
/// <reference path="../typings/bootstrap/index.d.ts"/>


class Cookies {
    public pobierzCookie(nazwaCookie) {
        let pelnaNazwa: string = nazwaCookie + "=";

        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) === ' ') c = c.substring(1, c.length);
            if (c.indexOf(pelnaNazwa) === 0) return c.substring(pelnaNazwa.length, c.length);
        }
        return null;
    }

    //Ustawiamy na okreslony czas cookies - w minutach
    public ustawCookie(nazwaCookie, wartosc, minuty) {
        let wygasniecie: string = "";
        if (minuty) {
            var date = new Date();
            date.setTime(date.getTime() + (minuty * 60000));
            wygasniecie = "; expires=" + date.toUTCString();
        }
        document.cookie = nazwaCookie + "=" + (wartosc || "") + wygasniecie;
    }
}

let cookies: Cookies = new Cookies();