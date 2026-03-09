/// <reference path="../typings/jquery/jquery.d.ts"/>
/// <reference path="../typings/google.maps.d.ts"/>
/// <reference path="../typings/bootstrap-select/index.d.ts"/>

class Mapa {
    private chekedClass = 'a-checkbox-checked';

    public inicjalizujMape(elementMapa: JQuery) {
        let plikKlm = elementMapa.data('sciezkaklm');
        if (plikKlm === '') {
            this.LadujMape(elementMapa);
        } else {
            this.LadujMapeKlm(elementMapa, plikKlm);
        }
    }

    private LadujMapeKlm(mapaElement: JQuery, plik: string) {
        let map = this.CreateMap(mapaElement);
        var ctaLayer = new google.maps.KmlLayer(plik, { map: map, preserveViewport: true });

        google.maps.event.addListener(ctaLayer, 'defaultviewport_changed', () => {
            var getCenter = ctaLayer.getDefaultViewport().getCenter();
            map.setCenter(getCenter);
        });
    }

    //Ładujemy mapę googla z elementami
    private LadujMape(elementMapa: JQuery) {
        let wyborMiastaDropDown = elementMapa.find('.wybor-miasta');
        if (wyborMiastaDropDown != null) {
            wyborMiastaDropDown.on('change',
                (event: JQueryEventObject) => {
                    //po zmianie miasta zaczytujemy mapę
                    this.OdswierzMape(elementMapa);
                });
        };

        let kategorieCheckboxes = elementMapa.find('.wybor-kategorii .kategoria');
        if (kategorieCheckboxes != null) {
            kategorieCheckboxes.each((index, elem: Element) => $(elem)
                .on('click',
                () => {
                    //obsługujemy checkboxa (zaznaczanie/odznaczanie)
                    this.PrzeladujKategorie($(elem));
                    //pobieramy miasta dostępne lda wybranych kategorii
                    this.PobierzMiastaDlaKategorii(elementMapa);
                    //odświerzamy mapę
                    this.OdswierzMape(elementMapa);
                }));
        }
        this.OdswierzMape(elementMapa);
    }

    //zaznaczanie/odznaczania chekboxów 
    private PrzeladujKategorie(element: JQuery) {
        let czyWybrane = element.hasClass(this.chekedClass);
        if (czyWybrane) {
            element.removeClass(this.chekedClass);
        } else {
            element.addClass(this.chekedClass);
        }
        return;
    }
    
    //pobiera miasta dostępne dla wybranych kategorii
    private PobierzMiastaDlaKategorii(mapa: JQuery) {
        //poobieramy adres do zapytania o miasta
        let url = mapa.find('.wybor-kategorii').data('url');
        //pobieramy wybrane kategorie
        let param = {
            kid: this.PobierzWybraneKategorie(mapa)
        }
        //wysyłamy zapytanie o miasta dotępne dla wybranych kategorii
        $.post(url,
            param,
            (json) => {
                this.PrzeladujMiasta(mapa, json);
            });
    }

    //ładuje nowe miasta do kontrolki
    private PrzeladujMiasta(mapaElement: JQuery, data) {
        let selectMiasta = mapaElement.find('.selectpicker.miasta-dropdown');
        //usuwamy wszystkie stare wartości
        selectMiasta.find('option').remove();
        //ładujemy wartość domyślną jesli jest więcej niż jedno miasto
        if (data.length > 1) {
            selectMiasta.append('<option value="">-</option>');
        }
        //ładujemy miasta które dostaliśmy z serwera
        for (let i of data) {
            selectMiasta.append('<option value="' + i + '">' + i + '</option>');
        }

        if (data.length > 1) {
            //wybieramy wartość domyslną 
            selectMiasta.val("");
        }
        if (data.length === 0) {
            mapaElement.find('.wybor-miasta').hide();
            mapaElement.find('#brak-elementow').show();
        } else {
            mapaElement.find('.wybor-miasta').show();
            mapaElement.find('#brak-elementow').hide();
        }
        //odświerzamy kontrolką 
        selectMiasta.selectpicker('refresh');
    }

    private PobierzWybraneMiasto(mapaElement: JQuery) {
        let selectMiasta = mapaElement.find('.miasta-dropdown');

        if (selectMiasta == null) {
            return null;
        }

        return selectMiasta.find("option:selected").val();
    }

    private PobierzWybraneKategorie(mapaElement: JQuery) {
        let boxyKategorii = mapaElement.find('.wybor-kategorii .kategoria');

        let id = '';
        if (boxyKategorii == null || boxyKategorii.length===0 ) {
            id = mapaElement.data('kategoria');
            return id;
        }

        boxyKategorii.each((index, element) => {
            let e = $(element);
            if (e.hasClass(this.chekedClass)) {
                id += e.data('kategoria-id') + ',';
            }
        });
        return id;
    }

    private PobierzLokalizacjeUzytkownika(mapaElement: JQuery,map: google.maps.Map) {
        navigator.geolocation.getCurrentPosition(
            //po poprawnym pobraniu pozycji
            function (position) {
                let location = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                map.setCenter(location);
            },
        //w przypadku błędu
            function() {
                let brakZgody = mapaElement.find("#brak-zgody");
                if (brakZgody.length === 0 || brakZgody === null || brakZgody === undefined ) {
                    console.error("Brak elementu brak-zgody w elemencie Mapa");
                } else {
                    brakZgody.show();
                }
            }
        );
    }

    private OdswierzMape(mapaElement: JQuery) {
        let map = this.CreateMap(mapaElement);
        
        let punkty = mapaElement.data('punkty');
        if (punkty === null || punkty === undefined || punkty.length === 0) {
            let url = mapaElement.data('url');
            let wybraneMiasto = this.PobierzWybraneMiasto(mapaElement);
            let opcjaSklepow = {
                idKontrolki: mapaElement.data('idkontrolki'),
                miasto: wybraneMiasto,
                kid: this.PobierzWybraneKategorie(mapaElement)
            }

            //pobieranie punktow
            $.post(url, opcjaSklepow,
                (json) => {
                    this.ZaladujPunkty(json, map, wybraneMiasto !== "", mapaElement);
                });
            if (wybraneMiasto === "") {
                this.PobierzLokalizacjeUzytkownika(mapaElement, map);
            }
        } else {
            this.ZaladujPunkty(punkty, map, true, mapaElement);
            mapaElement.removeAttr('data-punkty');
        }
    }

    //wprowadza punkty na mapę 
    private ZaladujPunkty(json, map: google.maps.Map, centrujNaPierwszy: boolean, mapaElement: JQuery) {
        var infowindow;
        $.each(json,
            function (index, data) {
                var latLng = new google.maps.LatLng(data.Lon, data.Lat);
                // Creating a marker and putting it on the map
                var marker = new google.maps.Marker({
                    position: latLng,
                    map: map,
                    title: data.Title,
                    icon: data.ObrazekPineskaId
                });
                //dodawanie info o sklepie po kliknięciu
                let url = mapaElement.data('url-info');
                google.maps.event.addListener(marker,
                    'click',
                    function () {
                        if (infowindow != null) {
                            infowindow.close();
                        }
                        let sklep = {
                            idSklepu:data.Id
                        }
                        $.post(url,sklep, (json) => {
                            infowindow = new google.maps.InfoWindow({ disableAutoPan: true, maxWidth: 300, content: json });
                            infowindow.open(map, this);
                        });
                    });
                //centrowanie mapy na pierwszy punkt
                if (index === 0 && centrujNaPierwszy) {
                    map.setCenter(latLng);
                }
            });
    }

    //Tworzenie obiektu mapy
    private CreateMap(mapaElement: JQuery) {
        let pojemnikMapy = mapaElement.find('.mapa-kontener');

        //parametry środka polski
        let lat = 52.03;
        let lng = 19.27;

        var mapOptions = {
            zoom: parseInt(mapaElement.data('zoom')),
            scrollwheel: false,
            maxZoom: parseInt(mapaElement.data('maxZoom')),
            minZoom: parseInt(mapaElement.data('minzoom')),
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            center: new google.maps.LatLng(lat, lng)
        };

        // Creating a new map
        let map = new google.maps.Map(pojemnikMapy[0], mapOptions);
        return map;
    }
}

let mapa: Mapa = new Mapa();