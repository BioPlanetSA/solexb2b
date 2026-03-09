using ServiceStack.Text.WP;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Koszyk
{
    public class KoszykCalosc :KontrolkaTresciBaza
    {
        public override string Nazwa => "Koszyk cały";

        public override string Kontroler => "Koszyk";

        public override string Akcja => "Calosc";

        public override string Grupa => "Koszyk";

        public override string Ikona => "fa fa-shopping-cart";

        [FriendlyName("Klient może dodać opis pozycji")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Globalne", 5)]
        public bool PokazujOpisPozycji { get; set; }


        [FriendlyName("Jaką cene po rabacie oraz wartość pokazywać w koszyku")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Globalne", 5)]
        public JakieCenyPokazywac CenaPoRabaciePokazuj { get; set; }

        [FriendlyName("Które pole z produktów pokazać w kolumnie 20 koszyka")]
        [WidoczneListaAdmin]
        [PobieranieSlownika(typeof(SlownikPolProduktow))]
        [GrupaAtttribute("Globalne", 5)]
        [Niewymagane]
        public string PoleProduktuKoszykKolumna20 { get; set; }


        [FriendlyName("Które pole z produktów pokazać w kolumnie 80 koszyka")]
        [WidoczneListaAdmin]
        [PobieranieSlownika(typeof(SlownikPolProduktow))]
        [GrupaAtttribute("Globalne", 5)]
        [Niewymagane]
        public string PoleProduktuKoszykKolumna80 { get; set; }

        [FriendlyName("Format kolumny 80", FriendlyOpis = "Wzorzec formatowania wyświetlnaje frazy w kolumnie 80. Jako fraza do pokazania użyj symbolu: {0} - czyli np. aby pokazać wartość z znakiem % na końcu należy wpisać tutaj '{0}%'. " +
                    "Format może zawierać HTML - czyli dozwolona jest składnia np. '<b>{0}</b>'")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Globalne", 5)]
        [Niewymagane]
        public string PoleProduktuKoszykKolumna80Format { get; set; }


        [FriendlyName("Rozmiar zdjęć w koszyku")]
        [WidoczneListaAdmin]
        [PobieranieSlownika(typeof(SlownikRozmiarZdjec))]
        [GrupaAtttribute("Globalne", 5)]
        public string RozmiarZdjeciaWKoszyku { get; set; }


        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Pokazywać belkę dostępności dla produktów w koszyku")]
        [GrupaAtttribute("Globalne",5)]
        public bool PokazywacBelkeDostepnosci { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy w koszyku ma być dostępna opcja dodawania nowego adresu")]
        [GrupaAtttribute("Globalne", 5)]
        public bool DodawanieNowegoAdresu { get; set; }

        public KoszykCalosc()
        {
            PokazywacBelkeDostepnosci = true;
            PokazywacDateDodaniaDoKoszyka=PokazywacDateDodaniaDoKoszykaProduktyAutomatyczne=PokazywacDateDodaniaDoKoszykaProduktyGratisy=PokazywacDateDodaniaDoKoszykaProduktyZaPkt = false;
            PokazywacMetkeRodzinowaKoszykProdukty = PokazywacMetkeRodzinowaKoszykProduktyAutomatyczne=PokazywacMetkeRodzinowaKoszykProduktyAutomatyczne=PokazywacMetkeRodzinowaKoszykProduktyZaPkt = true;
            PokazywacZdjecieProduktuKoszykProdukty=PokazywacZdjecieProduktuKoszykProduktyAutomatyczne=PokazywacZdjecieProduktuKoszykProduktyGratisy=PokazywacZdjecieProduktuKoszykProduktyZaPkt = true;
            PokazywacNazweProduktuKoszykProdukty =PokazywacNazweProduktuKoszykProduktyAutomatyczne =PokazywacNazweProduktuKoszykProduktyGratisy = PokazywacNazweProduktuKoszykProduktyZaPkt = true;
            PokazywacKodKreskowyProduktuKoszykProdukty = PokazywacKodKreskowyProduktuKoszykProduktyAutomatyczne =PokazywacKodKreskowyProduktuKoszykProduktyGratisy = PokazywacKodKreskowyProduktuKoszykProduktyZaPkt = true;
            PokazywacVatKoszykProduktyGratisy = PokazywacVatKoszykProdukty = PokazywacVatKoszykProduktyAutomatyczne = true;
            PokazywacWartoscVatKoszykProdukty = PokazywacWartoscVatKoszykProduktyGratisy = PokazywacWartoscVatKoszykProduktyAutomatyczne = true;
            PokazywacWageProdukty = PokazywacWageProduktyAutomatyczne = PokazywacWageProduktyGratisy = PokazywacWageProduktyZaPkt = true;
            PokazywacCenaKatalogowaProdukty = PokazywacCenaKatalogowaProduktyAutomatyczne = PokazywacCenaKatalogowaProduktyGratisy = JakieCenyPokazywac.NettoBrutto;
            PokazywacInformacjeORoznicyPrzyZaokraglaniu = true;
            FormatPokazywanejWagi = "0.##";
            CenaPoRabaciePokazuj = JakieCenyPokazywac.NettoBrutto;
            RozmiarZdjeciaWKoszyku = "ico50x50wp";
            KoszykPokazujMetkeProduktyGratisowe = true;
            PokazywacRabatKoszyk = true;
            PokazywacWartoscKatalogowaKoszyk = true;
            PokazywacWartoscPrzedRabatemKoszyk = true;
            CzyPokazywacCeneNettoWPodsumowaiu = true;
            CzyPokazywacCeneBruttoWPodsumowaiu = true;
            CzyPokazywacVatWPodsumowaiu = true;
            CzyPokazywacProcentWPodsumowaiu = true;
            PokazywacPrzesylkePodsumowanie = true;
            PokazywacWagePodsumowanie = true;
            PokazywacObjetoscPodsumowanie = true;
            PokazywacPodsumowanieWPodsumowaniu = true;
            PokazywacWartoscPoRabacieKoszyk = true;
            PokazywacZyskKlientaKoszyk = true;
            PokazywacMetkeRodzinowaKoszykProduktyGratisyPopUp = true;
            PokazywacZdjecieProduktuKoszykProduktyGratisyPopUp = true;
            PokazywacNazweProduktuKoszykProduktyGratisyPopUp = true;
            PokazywacSymbolProduktuKoszykProduktyGratisyPopUp = true;
            PokazywacKodKreskowyProduktuKoszykProduktyGratisyPopUp = true;
            PokazywacCenaKatalogowaProduktyGratisyPopUp = JakieCenyPokazywac.NettoBrutto;
            DopuszczalnaDlugoscUwag = 300;
        }

        //Ustawienia dla produktow dodanych przez klienta
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać kolumne data dodania")]
        [GrupaAtttribute("Dodane przez klienta", 10)]
        public bool PokazywacDateDodaniaDoKoszyka { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać metkę rodzinową")]
        [GrupaAtttribute("Dodane przez klienta", 10)]
        public bool PokazywacMetkeRodzinowaKoszykProdukty { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać zdjęcie produktu")]
        [GrupaAtttribute("Dodane przez klienta", 10)]
        public bool PokazywacZdjecieProduktuKoszykProdukty { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać nazwę produktu")]
        [GrupaAtttribute("Dodane przez klienta", 10)]
        public bool PokazywacNazweProduktuKoszykProdukty { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać symbol produktu")]
        [GrupaAtttribute("Dodane przez klienta", 10)]
        public bool PokazywacSymbolProduktuKoszykProdukty { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać kod kreskowy produktu")]
        [GrupaAtttribute("Dodane przez klienta", 10)]
        public bool PokazywacKodKreskowyProduktuKoszykProdukty { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać vat produktu")]
        [GrupaAtttribute("Dodane przez klienta", 10)]
        public bool PokazywacVatKoszykProdukty { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać wartość vat")]
        [GrupaAtttribute("Dodane przez klienta", 10)]
        public bool PokazywacWartoscVatKoszykProdukty { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Jakie ceny detaliczne pokazywać")]
        [GrupaAtttribute("Dodane przez klienta", 10)]
        public JakieCenyPokazywac PokazywacCenaKatalogowaProdukty { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywac wagę")]
        [GrupaAtttribute("Dodane przez klienta", 10)]
        public bool PokazywacWageProdukty { get; set; }

        

        //Ustawienia dla produktow dodanych automatycznie
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać kolumne data dodania")]
        [GrupaAtttribute("Dodane automatycznie", 11)]
        public bool PokazywacDateDodaniaDoKoszykaProduktyAutomatyczne { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać metkę rodzinową")]
        [GrupaAtttribute("Dodane automatycznie", 11)]
        public bool PokazywacMetkeRodzinowaKoszykProduktyAutomatyczne { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać zdjęcie produktu")]
        [GrupaAtttribute("Dodane automatycznie", 11)]
        public bool PokazywacZdjecieProduktuKoszykProduktyAutomatyczne { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać nazwę produktu")]
        [GrupaAtttribute("Dodane automatycznie", 11)]
        public bool PokazywacNazweProduktuKoszykProduktyAutomatyczne { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać symbol produktu")]
        [GrupaAtttribute("Dodane automatycznie", 11)]
        public bool PokazywacSymbolProduktuKoszykProduktyAutomatyczne { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać kod kreskowy produktu")]
        [GrupaAtttribute("Dodane automatycznie", 11)]
        public bool PokazywacKodKreskowyProduktuKoszykProduktyAutomatyczne { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać vat produktu")]
        [GrupaAtttribute("Dodane automatycznie", 11)]
        public bool PokazywacVatKoszykProduktyAutomatyczne { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać wartość vat")]
        [GrupaAtttribute("Dodane automatycznie", 11)]
        public bool PokazywacWartoscVatKoszykProduktyAutomatyczne { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Jakie ceny detaliczne pokazywać")]
        [GrupaAtttribute("Dodane automatycznie", 11)]
        public JakieCenyPokazywac PokazywacCenaKatalogowaProduktyAutomatyczne { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywac wagę")]
        [GrupaAtttribute("Dodane automatycznie", 11)]
        public bool PokazywacWageProduktyAutomatyczne { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazyć metkę na produktach automatycznych w koszyku")]
        [GrupaAtttribute("Dodane automatycznie", 11)]
        public bool KoszykPokazujMetkeProduktyAutomatyczne { get; set; }

        //Ustawienia dla produktow gratisowych
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać kolumne data dodania")]
        [GrupaAtttribute("Gratisy", 12)]
        public bool PokazywacDateDodaniaDoKoszykaProduktyGratisy { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać metkę rodzinową")]
        [GrupaAtttribute("Gratisy", 12)]
        public bool PokazywacMetkeRodzinowaKoszykProduktyGratisy { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać zdjęcie produktu")]
        [GrupaAtttribute("Gratisy", 12)]
        public bool PokazywacZdjecieProduktuKoszykProduktyGratisy { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać nazwę produktu")]
        [GrupaAtttribute("Gratisy", 12)]
        public bool PokazywacNazweProduktuKoszykProduktyGratisy { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać symbol produktu")]
        [GrupaAtttribute("Gratisy", 12)]
        public bool PokazywacSymbolProduktuKoszykProduktyGratisy { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać kod kreskowy produktu")]
        [GrupaAtttribute("Gratisy", 12)]
        public bool PokazywacKodKreskowyProduktuKoszykProduktyGratisy { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać vat produktu")]
        [GrupaAtttribute("Gratisy", 12)]
        public bool PokazywacVatKoszykProduktyGratisy { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać wartość vat")]
        [GrupaAtttribute("Gratisy", 12)]
        public bool PokazywacWartoscVatKoszykProduktyGratisy { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Jakie ceny detaliczne pokazywać")]
        [GrupaAtttribute("Gratisy", 12)]
        public JakieCenyPokazywac PokazywacCenaKatalogowaProduktyGratisy { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywac wagę")]
        [GrupaAtttribute("Gratisy", 12)]
        public bool PokazywacWageProduktyGratisy { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywac metkę produktów gratisowych")]
        [GrupaAtttribute("Gratisy", 12)]
        public bool KoszykPokazujMetkeProduktyGratisowe { get; set; }

        //Ustawienia dla produktow za pkt
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać kolumne data dodania")]
        [GrupaAtttribute("Produkty za punkty", 13)]
        public bool PokazywacDateDodaniaDoKoszykaProduktyZaPkt { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać metkę rodzinową")]
        [GrupaAtttribute("Produkty za punkty", 13)]
        public bool PokazywacMetkeRodzinowaKoszykProduktyZaPkt { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać zdjęcie produktu")]
        [GrupaAtttribute("Produkty za punkty", 13)]
        public bool PokazywacZdjecieProduktuKoszykProduktyZaPkt { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać nazwę produktu")]
        [GrupaAtttribute("Produkty za punkty", 13)]
        public bool PokazywacNazweProduktuKoszykProduktyZaPkt { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać symbol produktu")]
        [GrupaAtttribute("Produkty za punkty", 13)]
        public bool PokazywacSymbolProduktuKoszykProduktyZaPkt { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać kod kreskowy produktu")]
        [GrupaAtttribute("Produkty za punkty", 13)]
        public bool PokazywacKodKreskowyProduktuKoszykProduktyZaPkt { get; set; }

        [FriendlyName("Czy pokazyć metkę na produktach automatycznych w koszyku")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Produkty za punkty", 13)]
        public bool KoszykPokazujMetkeProduktyZaPkt { get; set; }
        
        //globalne
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywac wagę")]
        [GrupaAtttribute("Produkty za punkty", 13)]
        public bool PokazywacWageProduktyZaPkt { get; set; }
        [FriendlyName("Czy pokazywać informacje o różnicy cen spowodowaje zaokrągleniem")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Globalne", 5)]
        public bool PokazywacInformacjeORoznicyPrzyZaokraglaniu { get; set; }

        [FriendlyName("Format wyświetlanej wagi")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Globalne", 5)]
        public string FormatPokazywanejWagi { get; set; }

        [FriendlyName("Maxymalna dłudość uwag.", FriendlyOpis = "Maxymalna długość uwag które może wprowadzić klient. Domyślnie 300 znaków.")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Globalne", 5)]
        public int DopuszczalnaDlugoscUwag { get; set; }


        //podsumowanie
        [FriendlyName("Czy w podsumowaniuu pokazywać wartość przed rabacie")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Podsumowanie", 14)]
        public bool PokazywacWartoscPrzedRabatemKoszyk { get; set; }

        [FriendlyName("Czy w podsumowaniuu pokazywać wartość katologowe produktów")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Podsumowanie", 14)]
        public bool PokazywacWartoscKatalogowaKoszyk { get; set; }

        [FriendlyName("Czy w podsumowaniuu pokazywać rabat")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Podsumowanie", 14)]
        public bool PokazywacRabatKoszyk { get; set; }

        [FriendlyName("Czy w podsumowaniuu pokazywać cene netto")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Podsumowanie", 14)]
        public bool CzyPokazywacCeneNettoWPodsumowaiu { get; set; }

        [FriendlyName("Czy w podsumowaniuu pokazywać cene brutto")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Podsumowanie", 14)]
        public bool CzyPokazywacCeneBruttoWPodsumowaiu { get; set; }

        [FriendlyName("Czy w podsumowaniuu pokazywać wartość vat")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Podsumowanie", 14)]
        public bool CzyPokazywacVatWPodsumowaiu { get; set; }

        [FriendlyName("Czy w podsumowaniuu pokazywać % zysku klienta / rabatu")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Podsumowanie", 14)]
        public bool CzyPokazywacProcentWPodsumowaiu { get; set; }

        [FriendlyName("Czy w podsumowaniuu pokazywać przesyłke")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Podsumowanie", 14)]
        public bool PokazywacPrzesylkePodsumowanie { get; set; }

        [FriendlyName("Czy w podsumowaniuu pokazywać wage")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Podsumowanie", 14)]
        public bool PokazywacWagePodsumowanie { get; set; }

        [FriendlyName("Czy w podsumowaniuu pokazywać objętość")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Podsumowanie", 14)]
        public bool PokazywacObjetoscPodsumowanie { get; set; }

        [FriendlyName("Czy w podsumowaniu pokazywać podsumowanie")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Podsumowanie", 14)]
        public bool PokazywacPodsumowanieWPodsumowaniu { get; set; }

        [FriendlyName("Czy w podsumowaniu pokazywać wartość po rabacie")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Podsumowanie", 14)]
        public bool PokazywacWartoscPoRabacieKoszyk { get; set; }

        [FriendlyName("Czy w podsumowaniu pokazywać zysk klienta")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Podsumowanie", 14)]
        public bool PokazywacZyskKlientaKoszyk { get; set; }

        //Gratisy
        [FriendlyName("Czy w koszyku pokazywać kolumne z metką rodzinową")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Gratisy Pop-Up", 15)]
        public bool PokazywacMetkeRodzinowaKoszykProduktyGratisyPopUp { get; set; }

        [FriendlyName("Czy w koszyku pokazywać kolumne ze zdjęciem produktu")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Gratisy Pop-Up", 15)]
        public bool PokazywacZdjecieProduktuKoszykProduktyGratisyPopUp { get; set; }
        [FriendlyName("Czy w koszyku pokazywać nazwę produktu")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Gratisy Pop-Up", 15)]
        public bool PokazywacNazweProduktuKoszykProduktyGratisyPopUp { get; set; }

        [FriendlyName("Czy w koszyku pokazywać symbol produktu")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Gratisy Pop-Up", 15)]
        public bool PokazywacSymbolProduktuKoszykProduktyGratisyPopUp { get; set; }

        [FriendlyName("Czy w koszyku pokazywać kod kreskowy produktu")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Gratisy Pop-Up", 15)]
        public bool PokazywacKodKreskowyProduktuKoszykProduktyGratisyPopUp { get; set; }

        [FriendlyName("Czy w koszyku pokazywać vat ceny po rabacie produktu")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Gratisy Pop-Up", 15)]
        public bool PokazywacVatCenyKoszykowejGratisPopUp { get; set; }

        [FriendlyName("Które ceny detaliczne pokazywać w PopUpie gratisów")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Gratisy Pop-Up", 15)]
        public JakieCenyPokazywac PokazywacCenaKatalogowaProduktyGratisyPopUp { get; set; }

        [FriendlyName("Które dane klienta nadrzędnego mają wyświetlać się na przycisku finalizacji zamówienia (tylko jeżeli zamówienie może być akceptowane przez jedno konto)")]
        [WidoczneListaAdmin]
        [GrupaAtttribute("Subkonta", 15)]
        [PobieranieSlownika(typeof(SlownikPolKlienta))]
        public string PoleKontaAkceptujacego { get; set; }
    }
}