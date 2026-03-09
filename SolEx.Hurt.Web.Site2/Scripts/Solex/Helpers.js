

function getQsFromUrl(url) {
    var idx = url.indexOf('?');
    if (idx < 0) {
        return '';
    }
    return url.substring(idx, url.length);
}
function setUrlEncodedKey(key, value, query) {
    query = query || window.location.search;
    var q = query + "&";
    var re = new RegExp("[?|&]" + key + "=.*?&");
    if (!re.test(q))
        q += key + "=" + encodeURI(value);
    else
        q = q.replace(re, "&" + key + "=" + encodeURIComponent(value) + "&");
    q = q.trimStart_("&").trimEnd_("&");
    var w = q[0] == "?" ? q : q = "?" + q;

    return w.replace("??", "?");
}

function isEndOf(origin, target) {
    return (origin.substr(target.length * -1, target.length) === target);
}

function OczyscFraze(fraza, pomijajbackslaSH) {
    var f = fraza.replace(/[-[\]{}()*+?,\\^$|#\s]/g, ' ').trim();

    //usuwanie ostatnich kropek na koncu
    f = f.replace(/\.+$/, "");

    if (pomijajbackslaSH) {
        return f.replace(new RegExp('/', 'g'), "%2F");
    }
    return f.replace(new RegExp('/', 'g'), " ").trim();
}

function WalidujPole() {
    var elem = $(this);
    var walidator = elem.attr('data-walidator');
    var klucz = elem.attr('data-identyfikatorObiektu');
    if (walidator == '' || klucz == '') {
        return;
    }
    var testowane = elem.val();
    var kluczelem = 'walidacja';
    var zadanie = elem.data(kluczelem);
    if (zadanie != undefined) {
        zadanie.abort();
    }

    zadanie = $.ajax(
    {
        url: '/Walidacja/Waliduj',
        type: 'POST',
        dataType: 'json',
        async: false,
        data: { walidator: walidator, klucz: klucz, testowane: testowane },
        success: function (data) {
            if (!data.Wynik) {
                elem[0].setCustomValidity(data.KomunikatBledu);
                //elem.prop('disabled', true);
            } else {
                elem[0].setCustomValidity('');
            }
            elem.prop('disabled', false);
            elem.focus();
        },
        error: function (xhr) {
            if (!userAborted(xhr)) {
                elem[0].setCustomValidity('Błąd walidacji');
            } else {
                elem[0].setCustomValidity('');
            }
            elem.prop('disabled', false);
            elem.focus();
        }
    });

    elem.add(kluczelem, zadanie);
}
function KliknijJesliWpisanoEnter(sender) {
    var evt = window.event || arguments.callee.caller.arguments[0];
    evt = (evt) ? evt : window.event;
    evt.stopPropagation();
    evt.preventDefault();
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode == 13) {

        $(sender).click();
    }
}
String.prototype.trimEnd_ = function (c) {
    if (c)
        return this.replace(new RegExp(c.escapeRegExp() + "*$"), '');
    return this.replace(/\s+$/, '');
}
String.prototype.trimStart_ = function (c) {
    if (c)
        return this.replace(new RegExp("^" + c.escapeRegExp() + "*"), '');
    return this.replace(/^\s+/, '');
}

String.prototype.escapeRegExp = function () {
    return this.replace(/[.*+?^${}()|[\]\/\\]/g, "\\$0");
};

(function ($) {

    jQuery.fn.observe_field = function (frequency, callback) {



        return this.each(function () {
            var $this = $(this);
            var prev = $this.val();
            var prevChecked = $this.prop('checked');

            var check = function () {
                if (removed()) { // if removed clear the interval and don't fire the callback
                    if (ti) clearInterval(ti);
                    return;
                }

                var val = $this.val();
                var checked = $this.prop('checked');
                if (prev != val || checked != prevChecked) {
                    prev = val;
                    prevChecked = checked;
                    $this.map(callback); // invokes the callback on $this
                }
            };

            var removed = function () {
                return $this.closest('html').length == 0
            };

            var reset = function () {
                if (ti) {
                    clearInterval(ti);
                    ti = setInterval(check, frequency);
                }
            };

            check();
            var ti = setInterval(check, frequency); // invoke check periodically

            // reset counter after user interaction
            $this.bind('keyup click mousemove', reset); //mousemove is for selects
        });

    };

})(jQuery);
