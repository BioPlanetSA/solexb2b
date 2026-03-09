using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [FriendlyName("Konfigurowalne uwagi")]
    public class KonfigurowalneUwagi : KonfigurowalnePolaBaza
    {
       
        [WidoczneListaAdmin(false, false, true, false)]
        [FriendlyName("Uwagi wg wzoru", FriendlyOpis = "wg wzoru można dowolnie skonfigurować uwagi, które będą dodane do zamówienia. Dostępne parametry:" +
            "<table>" +
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
            "<tr><td>{KategoriaKlienta[Nazwa grupy]} - Kategoria na której należy klient w wybranej grupie - proszę uważań na zbędne spacje w znaczniku</td></tr>" +
            "<tr><td>{ProduktyOferta} - Produkty z oferty</td></tr>" +
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
        public string Wzor { get; set; }

        private Regex myRegex;

        [FriendlyName("Filtr regex do oczyszczania wartości dla kategorii klienta")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string Filtr { get; set; }

        public KonfigurowalneUwagi()
        {
            Filtr = " [A-Z]+ ";
        }

        [FriendlyName("Czy doklejać opis parametru do wartości np. Miasto dostawy: Wrocław")]
        [WidoczneListaAdmin(false, false, true, false)]
        public bool DoklejajOpis { get; set; }

     public override bool Wykonaj(IKoszykiBLL koszyk)
        {
           Dictionary<string, string> tmp = PobierzSlownikParametrow(koszyk, Wzor);

            string uwagitmp = Wzor;
            if (string.IsNullOrEmpty(uwagitmp))
            {
                throw new Exception("Pole wzór w module konfigurowalnch uwag nie jest wypełniony");
            }
            foreach (var v in tmp)
            {
                string nowa = v.Value;
                if (DoklejajOpis && !string.IsNullOrEmpty(nowa))
                {
                    string opis = DostepneParametry[v.Key];
                    if (v.Key == "{dodatkowe_parametry}")
                    {
                        opis = "";
                    }
                    nowa = opis + ": " + v.Value;
                }
                uwagitmp = uwagitmp.Replace(v.Key, nowa);
            }
            int idxp;
            IKlient klient = koszyk.Klient;
            do
            {
                string fp = "{KategoriaKlienta[";
                string fk = "]}";
                idxp = uwagitmp.IndexOf(fp, StringComparison.InvariantCulture);

                if (idxp != -1)
                {
                    int koniec = uwagitmp.IndexOf(fk, idxp, StringComparison.InvariantCulture);
                    string grupa = uwagitmp.Substring(idxp + fp.Length, koniec - idxp - fp.Length);
                    string fraza = uwagitmp.Substring(idxp, koniec - idxp + fk.Length);
                    var kats = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategoriaKlienta>(null, x =>  Sql.In(x.Id,klient.Kategorie) && x.Grupa==grupa);

                    var nazwy = Serializacje.PobierzInstancje.SerializeList(kats.Select(x => x.Nazwa));
                    if (!string.IsNullOrEmpty(Filtr))
                    {
                        nazwy = Oczysc(nazwy);
                    }
                    uwagitmp = uwagitmp.Replace(fraza, nazwy);
                }
            }
            while (idxp != -1);
            koszyk.Uwagi = uwagitmp;
            return true;
        }
        /// <summary>
        /// Czyszczenie kategorii klienta wg regexa
        /// </summary>
        /// <param name="wartosc"></param>
        /// <returns></returns>
        public string Oczysc(string wartosc)
        {
            if (myRegex == null)
            {
                myRegex = new Regex(Filtr, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            }
            string wynik=wartosc;
            foreach (Match o in myRegex.Matches(wartosc))
            {
                wynik = o.Value.Trim();
            }
            return wynik;
        }
    }
}