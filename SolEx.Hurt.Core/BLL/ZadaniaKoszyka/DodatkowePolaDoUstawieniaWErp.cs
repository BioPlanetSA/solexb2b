using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [FriendlyName("Dodatkowe pola/flagi w erp przy zapisie zamówienia - sztywno podana wartość do wpisania do pola",
       FriendlyOpis = "Ustawia dodatkowe pola/flagi w ERP przy zapisie zamówienia - uwaga! moduł nadpisuje dodatkowe pola, któe mogą być ustawiane przez inne moduły (np. definicja dokumentu ERP). W tym module podajemy na sztywno jaka wartość ma być wpisana do pola.")]
    internal class DodatkowePolaDoUstawieniaWErp : KonfigurowalnePolaBaza
    {
        [FriendlyName("Nazwa pola z ERP do którego zostania wpisana wartość.", FriendlyOpis = "W przypadku nadania FLAGI NA DOKUMENCIE w nazwę prosze wpisać FLAGA, a w wartości wpisać nazwę flagi")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string DodatkowePola { get; set; }

        [FriendlyName("Wartość która bęzie ustawiona dla pola z ERP-a", FriendlyOpis = "W przypadku nadania FLAGI NA DOKUMENCIE wpisać nazwę flagi. <br/>" +
                                                                                       "Dostępne parametry:" +
            "<table>" +
             "<tr><td>Wartość wpisana nie pasująca do żadnej opcji zostanie wpisana bez zmian</td></tr>" +
            "<tr><td> {uwagi_klienta} - Uwagi klienta</td></tr>" +
            "<tr><td>{adres_dostawy} - Adres dostawy z telefonem i e-mailem</td></tr>" +
            "<tr><td>{adres_telefon} - Telefon z adresu dostawy</td></tr>" +
            "<tr><td>{adres_dostawy_nazwa} - Nazwa adresu dostawy</td></tr>" +
            "<tr><td{adres_dostawy_kod_pocztowy} - Kod pocztowy dostawy</td></tr>" +
            "<tr><td>{adres_dostawy_miasto - Miasto dostawy</td></tr>" +
            "<tr><td>{adres_dostawy_ulica} - Ulica dostawy</td></tr>" +
            "<tr><td>{adres_dostawy_kraj} - Kraj dostawy</td></tr>" +
            "<tr><td>{adres_dostawy_email} - Adres email dostawy</td></tr>" +
            "<tr><td>{adres_dostawy_typ} - Typ adresu</td></tr>" +
            "<tr><td>{liczba_produktow} - Liczba produktów na zamówieniu</td></tr>" +

            "<tr><td>{najtanszy_produkt} - Najtańszy produkt na zamówieniu</td></tr>" +
            "<tr><td>{najdrozszy_produkt} - Najdroższy produkt na zamówieniu</td></tr>" +
            "<tr><td>{termin_dostawy} - Termin dostawy</td></tr>" +
            "<tr><td>{dodatkowe_parametry} - Dodatkowe parametry</td></tr>" +
            "<tr><td>{sposob_platnosci} - Sposób płatności</td></tr>" +
            "<tr><td>{czas} - Data zamówienia</td></tr>" +
            "<tr><td>{laczna_waga} - Łaczna waga zamówienia</td></tr>" +
            "<tr><td>{laczna_objetosc} - Łączna objetość zamówienia</td></tr>" +
            "<tr><td>{klient_email} - WiadomoscEmail klienta</td></tr>" +
            "<tr><td>{klient_nazwa} - Nazwa klienta</td></tr>" +
            "<tr><td>{Inicjaly_osoby_zamawiajacej_w_imieniu_klienta} - Inicjały osoby składającej zamówienia w imieniu klienta</td></tr>" +

            "<tr><td>{Nazwa_osoby_zamawiajacej_w_imieniu_klienta} - Nazwa osoby składającej zamówienia w imieniu klienta</td></tr>" +
            "<tr><td>{produkty_automatycznie_dodane} - Automatycznie dodane produkty</td></tr>" +
            "<tr><td>{numer_zamowienia_klienta} - Numer zamówienia klienta</td></tr>" +
            "<tr><td>{dokumenty_niezaplacone_liczba} - Liczba niezapłaconych dokumentów</td></tr>" +
            "<tr><td>{dokumenty_niezaplacone_wartosc} - Wartość niezapłaconych dokumentów</td></tr>" +
            "<tr><td>{dokumenty_przeterminowane_liczba} - Liczba przeterminowanych dokumentów</td></tr>" +
            "<tr><td>{dokumenty_przeterminowane_wartosc} - Wartość przeterminowanych dokumentów</td></tr>" +
            "<tr><td>{dostawa_nazwa} - Wybrany sposób dostawy</td></tr>" +
            "<tr><td>{dostawa_nazwa_opis} - Opis wybranego sposobu dostawy</td></tr>" +
            //"<tr><td>{MateriałyReklamowe} - Materiały reklamowe</td></tr>" +
            "<tr><td>{ProduktyOferta - Produkty z oferty</td></tr>" +
            "<tr><td>{sumaryczneIlosciPozcji - sumaryczne ilosc pozcji</td></tr>" +

            "<tr><td>{adres_dostawy_platforma} - Adres dostawy z telefonem i e-mailem - tylko adres spoza ERP-a</td></tr>" +
            "<tr><td>{adres_dostawy_nazwa_platforma} - Nazwa adresu dostawy - tylko adres spoza ERP-a</td></tr>" +
            "<tr><td>{adres_telefon_platforma} - Telefon z adresu dostawy - tylko adres spoza ERP-a</td></tr>" +
            "<tr><td>{adres_dostawy_kod_pocztowy_platforma} - Kod pocztowy dostawy - tylko adres spoza ERP-a</td></tr>" +
            "<tr><td>{adres_dostawy_miasto_platforma} - Miasto dostawy - tylko adres spoza ERP-a</td></tr>" +
            "<tr><td>{adres_dostawy_kraj_platforma} - Kraj dostawy - tylko adres spoza ERP-a</td></tr>" +
            "<tr><td>{adres_dostawy_ulica_platforma} - Ulica dostawy - tylko adres spoza ERP-a</td></tr>" +
            "<tr><td>{adres_dostawy_email_platforma} - Adres email dostawy - tylko adres spoza ERP-a</td></tr>" +
            "<tr><td>{adres_dostawy_typ_platforma} - Typ adresu - tylko adres spoza ERP-a</td></tr>" +
             
            "</table>")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string Wzor { get; set; }


        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            Dictionary<string, string> tmp = PobierzSlownikParametrow(koszyk,Wzor);

            string dodatkowePola = Wzor;
            foreach (var v in tmp)
            {
                dodatkowePola = dodatkowePola.Replace(v.Key, v.Value);
            }
            if (string.IsNullOrEmpty(koszyk.DodatkowePolaErp))
            {
                koszyk.DodatkowePolaErp = $"{DodatkowePola}:{dodatkowePola}";
            }
            else
            {
                if (!koszyk.DodatkowePolaErp.Contains(DodatkowePola))
                {
                    koszyk.DodatkowePolaErp += $";{DodatkowePola}:{dodatkowePola}";
                }
            }
            
            return true;
        }
    }
}