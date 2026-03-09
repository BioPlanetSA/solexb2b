using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public abstract class KonfigurowalnePolaBaza : ZadanieCalegoKoszyka, IZadaniePoFinalizacji
    {
        private Dictionary<string, string> _dostepneparametry;

        protected Dictionary<string, string> DostepneParametry
        {
            get
            {
                return _dostepneparametry ?? (_dostepneparametry = new Dictionary<string, string>
                {
                    {"{uwagi_klienta}", "Uwagi klienta"},
                    {"{adres_dostawy}", "Adres dostawy z telefonem i e-mailem"},
                    {"{adres_telefon}", "Telefon z adresu dostawy"},
                    {"{adres_dostawy_nazwa}", "Nazwa adresu dostawy"},
                    {"{adres_dostawy_kod_pocztowy}", "Kod pocztowy dostawy"},
                    {"{adres_dostawy_miasto}", "Miasto dostawy"},
                    {"{adres_dostawy_ulica}", "Ulica dostawy"},
                    {"{adres_dostawy_kraj}", "Kraj dostawy"},
                    {"{adres_dostawy_email}", "Adres email dostawy"},
                    {"{adres_dostawy_typ}","Typ adresu" },
                    {"{liczba_produktow}", "Liczba produktów na zamówieniu"},
                    {"{najtanszy_produkt}", "Najtańszy produkt na zamówieniu"},
                    {"{najdrozszy_produkt}", "Najdroższy produkt na zamówieniu"},
                    {"{termin_dostawy}", "Termin dostawy"},
                    {"{dodatkowe_parametry}", "Dodatkowe parametry"},
                    {"{sposob_platnosci}", "Sposób płatności"},
                    {"{czas}", "Data zamówienia"},
                    {"{laczna_waga}", "Łaczna waga zamówienia"},
                    {"{laczna_objetosc}","Łączna objetość zamówienia" },
                    {"{klient_email}", "WiadomoscEmail klienta"},
                    {"{klient_nazwa}", "Nazwa klienta"},
                    {"{Inicjaly_osoby_zamawiajacej_w_imieniu_klienta}", "Inicjały osoby składającej zamówienia w imieniu klienta"},
                    {"{Nazwa_osoby_zamawiajacej_w_imieniu_klienta}", "Nazwa osoby składającej zamówienia w imieniu klienta"},
                    {"{produkty_automatycznie_dodane}", "Automatycznie dodane produkty"},
                    {"{numer_zamowienia_klienta}", "Numer zamówienia klienta"},
                    {"{dokumenty_niezaplacone_liczba}", "Liczba niezapłaconych dokumentów"},
                    {"{dokumenty_niezaplacone_wartosc}", "Wartość niezapłaconych dokumentów"},
                    {"{dokumenty_przeterminowane_liczba}", "Liczba przeterminowanych dokumentów"},
                    {"{dokumenty_przeterminowane_wartosc}", "Wartość przeterminowanych dokumentów"},
                    {"{dostawa_nazwa}", "Wybrany sposób dostawy"},
                    {"{dostawa_nazwa_opis}", "Opis wybranego sposobu dostawy"},
                    {"{MateriałyReklamowe}", "Materiały reklamowe"},
                    {"{KategoriaKlienta[Nazwa grupy]}", "Kategoria na której należy klient w wybranej grupie - proszę uważań na zbędne spacje w znaczniku"},
                    {"{ProduktyOferta}", "Produkty z oferty"},
                    {"{sumaryczneIlosciPozcji}", "sumaryczne ilosc pozcji"},

                    //Pola do adresów które sa dodawane na pltfromie ale nie ma ich w erpie
                    {"{adres_dostawy_platforma}", "Adres dostawy z telefonem i e-mailem - tylko adres spoza ERP-a"},
                    {"{adres_dostawy_nazwa_platforma}","Nazwa adresu dostawy - tylko adres spoza ERP-a"},
                    {"{adres_telefon_platforma}","Telefon z adresu dostawy - tylko adres spoza ERP-a" },
                    { "{adres_dostawy_kod_pocztowy_platforma}","Kod pocztowy dostawy - tylko adres spoza ERP-a" },
                    {"{adres_dostawy_miasto_platforma}","Miasto dostawy - tylko adres spoza ERP-a" },
                    {"{adres_dostawy_kraj_platforma}","Kraj dostawy - tylko adres spoza ERP-a" },
                    {"{adres_dostawy_ulica_platforma}","Ulica dostawy - tylko adres spoza ERP-a" },
                    {"{adres_dostawy_email_platforma}","Adres email dostawy - tylko adres spoza ERP-a" },
                    {"{adres_dostawy_typ_platforma}","Typ adresu - tylko adres spoza ERP-a" }

                });
            }
        }
        public override string Opis
        {
            get
            {
                string parametry = DostepneParametry.Aggregate("", (current, pair) => current + (pair.Key + " -" + pair.Value + "<br/>"));
                return "wg wzoru można dowolnie skonfigurować uwagi, które będą dodane do zamówienia. Dostępne parametry: <br/>" + parametry;
            }
        }



        public void ZwrocDaneOferty(IKoszykiBLL koszyk, Dictionary<string, string> tmp)
        {
            StringBuilder ofertaOpis = new StringBuilder();
            var dokumentyZOfertaDlaKlienta = Calosc.DostepDane.Pobierz<DokumentyBll>(koszyk.Klient, x => x.Rodzaj == RodzajDokumentu.Oferta);
            if (dokumentyZOfertaDlaKlienta == null || !dokumentyZOfertaDlaKlienta.Any())
            {
                tmp.Add("{ProduktyOferta}", "");
                return;
            }
            var ofertyprodukty = new HashSet<long>(dokumentyZOfertaDlaKlienta.SelectMany(x => x.PobierzPozycjeDokumentu()).Select(x => x.ProduktId));

            foreach (var p in koszyk.PobierzPozycje)
            {
                if (ofertyprodukty.Contains(p.ProduktId))
                {
                    ofertaOpis.Append($"{p.Produkt.Kod}: {p.Ilosc}{p.Jednostka().Nazwa}");
                }
            }
            tmp.Add("{ProduktyOferta}", ofertaOpis.ToString().Trim().Trim(','));
        }

        public void ZwrocKlienta(IKoszykiBLL koszyk, Dictionary<string, string> tmp)
        {
            tmp.Add("{klient_email}", koszyk.Klient.Email);
            tmp.Add("{klient_nazwa}", koszyk.Klient.Nazwa);
            tmp.Add("{numer_zamowienia_klienta}", koszyk.NumerZamowienia);
        }

        public void ZwrocInfoOKoszyku(IKoszykiBLL koszyk, Dictionary<string, string> tmp)
        {
            List<KoszykPozycje> pozycje = koszyk.PobierzPozycje;
            tmp.Add("{uwagi_klienta}", koszyk.Uwagi??"");
            tmp.Add("{laczna_waga}", koszyk.WagaCalokowita()?.ToString()??"");
            tmp.Add("{laczna_objetosc}", koszyk.CalkowitaObjetoscKoszyka().ToString("0.###"));
            tmp.Add("{czas}", DateTime.Now.ToShortTimeString());
            tmp.Add("{liczba_produktow}", pozycje.Count.ToString(CultureInfo.InvariantCulture));
            tmp.Add("{najtanszy_produkt}", pozycje.Any()?pozycje.Min(a => a.CenaNetto).Wartosc.ToString("F2"):"");
            tmp.Add("{najdrozszy_produkt}", pozycje.Any() ? pozycje.Max(a => a.CenaNetto).Wartosc.ToString("F2"):"");
            tmp.Add("{termin_dostawy}", koszyk.TerminDostawy?.ToShortDateString() ?? "");
            tmp.Add("{sposob_platnosci}", koszyk.Platnosc ?? "");
        }

        public void ZwrocDostawe(IKoszykiBLL koszyk, Dictionary<string, string> tmp)
        {
            if (koszyk.KosztDostawy() != null && koszyk.KosztDostawy().ProduktDostawy != null)
            {
                tmp.Add("{dostawa_nazwa}", koszyk.KosztDostawy().ProduktDostawy.Kod);
                tmp.Add("{dostawa_nazwa_opis}", koszyk.KosztDostawy().OpisDostawy);
            }
            else
            {
                tmp.Add("{dostawa_nazwa}", "");
                tmp.Add("{dostawa_nazwa_opis}", "");
            }
        }

        public void ZwrocSumIloscPozycji(IKoszykiBLL koszyk, Dictionary<string, string> tmp)
        {
            List<KoszykPozycje> pozycje = koszyk.PobierzPozycje;
            if (pozycje == null || !pozycje.Any())
            {
                tmp.Add("{sumaryczneIlosciPozcji}", "");
                return;
            }
            IEnumerable<IGrouping<long?, IKoszykPozycja>> jednostki = pozycje.GroupBy(x => x.JednostkaId);

            StringBuilder iloscWgJednostki = new StringBuilder();
            foreach (var jednostkaProduktu in jednostki)
            {
                decimal wynik;
                wynik = jednostkaProduktu.Sum(x => x.Ilosc);
                IKoszykPozycja elementAtOrDefault = jednostkaProduktu.ElementAtOrDefault(0);
                if (elementAtOrDefault != null)
                {
                    iloscWgJednostki.AppendFormat(" {0} {1},", wynik, elementAtOrDefault.Jednostka().Nazwa);
                }
            }
            tmp.Add("{sumaryczneIlosciPozcji}", iloscWgJednostki.ToString().TrimEnd(','));
        }

        
        public void ZwrocAdres(IKoszykiBLL koszyk, Dictionary<string, string> tmp)
        {
            tmp.Add("{adres_dostawy}", $"{(string.IsNullOrEmpty(koszyk.Adres?.Nazwa) ? "" : koszyk.Adres?.Nazwa + ", ")} {koszyk.Adres?.UlicaNr}, {koszyk.Adres?.KodPocztowy} {koszyk.Adres?.Miasto}, {koszyk.Adres?.Kraj}, {koszyk.Adres?.Telefon}, {koszyk.Adres?.Email}");
            tmp.Add("{adres_dostawy_nazwa}", koszyk.Adres?.Nazwa);
            tmp.Add("{adres_telefon}",koszyk.Adres?.Telefon);
            tmp.Add("{adres_dostawy_kod_pocztowy}", koszyk.Adres?.KodPocztowy);
            tmp.Add("{adres_dostawy_miasto}", koszyk.Adres?.Miasto);
            tmp.Add("{adres_dostawy_kraj}", koszyk.Adres?.Kraj);
            tmp.Add("{adres_dostawy_ulica}", koszyk.Adres?.UlicaNr);
            tmp.Add("{adres_dostawy_email}",koszyk.Adres?.Email);
            tmp.Add("{adres_dostawy_typ}", koszyk.Adres?.TypAdresu.ToString());

            bool jestAdresINieMaWErp = koszyk.Adres != null && koszyk.Adres.Id < 0;
            tmp.Add("{adres_dostawy_platforma}", jestAdresINieMaWErp ? $"{(string.IsNullOrEmpty(koszyk.Adres.Nazwa) ? "" : koszyk.Adres.Nazwa + ", ")} {koszyk.Adres.UlicaNr}, {koszyk.Adres.KodPocztowy} {koszyk.Adres.Miasto}, {koszyk.Adres.Kraj}, {koszyk.Adres.Telefon}, {koszyk.Adres.Email}" : "");
            tmp.Add("{adres_dostawy_nazwa_platforma}", jestAdresINieMaWErp ? koszyk.Adres.Nazwa : "");
            tmp.Add("{adres_telefon_platforma}", jestAdresINieMaWErp ? koszyk.Adres.Telefon : "");
            tmp.Add("{adres_dostawy_kod_pocztowy_platforma}", jestAdresINieMaWErp ? koszyk.Adres.KodPocztowy : "");
            tmp.Add("{adres_dostawy_miasto_platforma}", jestAdresINieMaWErp ? koszyk.Adres.Miasto : "");
            tmp.Add("{adres_dostawy_kraj_platforma}", jestAdresINieMaWErp ? koszyk.Adres.Kraj : "");
            tmp.Add("{adres_dostawy_ulica_platforma}", jestAdresINieMaWErp ? koszyk.Adres.UlicaNr : "");
            tmp.Add("{adres_dostawy_email_platforma}", jestAdresINieMaWErp ? koszyk.Adres.Email : "");
            tmp.Add("{adres_dostawy_typ_platforma}", jestAdresINieMaWErp ? koszyk.Adres.TypAdresu.ToString() : "");
        }

        public void ZwrocDokumenty(IKoszykiBLL koszyk, Dictionary<string, string> tmp)
        {
            DocumentSummary posumadok = Calosc.DokumentyDostep.PobierzPodsumowanieFakturKlient(koszyk.Klient);
            tmp.Add("{dokumenty_niezaplacone_liczba}", posumadok?.Niezaplacone?.IloscPozycji.ToString(CultureInfo.InvariantCulture) ?? "");
            tmp.Add("{dokumenty_niezaplacone_wartosc}", posumadok?.Niezaplacone?.Cena.ToString() ?? "");
            tmp.Add("{dokumenty_przeterminowane_liczba}", posumadok?.Przeterminowane?.IloscPozycji.ToString(CultureInfo.InvariantCulture) ?? "");
            tmp.Add("{dokumenty_przeterminowane_wartosc}", posumadok?.Przeterminowane?.Cena.ToString() ?? "");
        }

        public void ZwrocDodatkoweParametry(IKoszykiBLL koszyk, Dictionary<string, string> tmp)
        {
            if (koszyk.DodatkoweParametry == null)
            {
                tmp.Add("{dodatkowe_parametry}", "");
                return;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var s in koszyk.DodatkoweParametry)
            {
                sb.Append($"{s.Value.Symbol}:{s.Value.WybraneWartosciString}, ");

                if (s.Value.Symbol == "MateriałyReklamowe")
                    tmp.Add("{MateriałyReklamowe}", s.Value.WybraneWartosciString);
            }
            tmp.Add("{dodatkowe_parametry}", sb.ToString().Trim().Trim(','));
        }

        public void ZwrocProduktyAutomatyczne(IKoszykiBLL koszyk, Dictionary<string, string> tmp)
        {
            StringBuilder sbautomatyczne = new StringBuilder();
            var produktyAutomatyczne = koszyk.PobierzPozycje.Where(x => x.TypPozycji == TypPozycjiKoszyka.Automatyczny).ToList();
            if (!produktyAutomatyczne.Any())
            {
                tmp.Add("{produkty_automatycznie_dodane}", "");
                return;
            }
            foreach (var s in produktyAutomatyczne)
            {
                sbautomatyczne.Append($"{s.Produkt.Nazwa}:{ s.Ilosc} {s.Jednostka().Nazwa}");
            }
            tmp.Add("{produkty_automatycznie_dodane}", sbautomatyczne.ToString());
        }

        public void ZwrocOsobeZamawiajacaWImieniuKlienta(IKoszykiBLL koszyk, Dictionary<string, string> tmp)
        {
            string iniclaly = "";
            string nazwaosoby = "";
            if (koszyk.PrzedstawicielKtoryEdytujeKoszykWImieniuKlienta != null)
            {
                string[] slowa = koszyk.PrzedstawicielKtoryEdytujeKoszykWImieniuKlienta.Nazwa.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                iniclaly = slowa.Aggregate("", (current, s) => current + s[0]);
                nazwaosoby = koszyk.PrzedstawicielKtoryEdytujeKoszykWImieniuKlienta.Nazwa;
            }
            tmp.Add("{Nazwa_osoby_zamawiajacej_w_imieniu_klienta}", nazwaosoby);
            tmp.Add("{Inicjaly_osoby_zamawiajacej_w_imieniu_klienta}", iniclaly);
        }
        /// <summary>
        /// Pobieranie parametrów z koszyka które bedizemy dopisywać do uwag. 
        /// </summary>
        /// <param name="koszyk"></param>
        /// <param name="wzor">Wzór z parametrami które chcemy użyć</param>
        /// <returns></returns>
        public Dictionary<string, string> PobierzSlownikParametrow(IKoszykiBLL koszyk, string wzor)
        {
            Dictionary<string, string> tmp = new Dictionary<string, string> { };
            ZwrocKlienta(koszyk, tmp);
            ZwrocInfoOKoszyku(koszyk, tmp);
            //sprawdzanie czy we wroze chcemy dostawe
            if (wzor.IndexOf("{dostawa_", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                //czy w koszyku jest w ogóle dostala
                if (koszyk.KosztDostawy() == null || koszyk.KosztDostawy().ProduktDostawy == null)
                {
                    throw new Exception("Brak dostawy");
                }
                ZwrocDostawe(koszyk, tmp);
            }
            //czy we wzorze jest w ogóle adres
            if (wzor.IndexOf("{adres_", StringComparison.InvariantCultureIgnoreCase)>-1)
            {
                //czy jest adres w koszyku
                if (koszyk.Adres == null)
                {
                    throw new Exception("Brak adresu");
                }
                ZwrocAdres(koszyk, tmp);
            }
            //czy we wzorze chcemy dokumenty
            if (wzor.IndexOf("{dokumenty_", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                ZwrocDokumenty(koszyk, tmp);
            }
            ZwrocDodatkoweParametry(koszyk, tmp);
            ZwrocProduktyAutomatyczne(koszyk, tmp);
            //czy we wzorze chcemy dane osoby zamawiącej w imieniu klienta
            if (wzor.IndexOf("osoby_zamawiajacej_", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                ZwrocOsobeZamawiajacaWImieniuKlienta(koszyk, tmp);
            }
            //czy we wzorze chcemy produkty oferty
            if (wzor.IndexOf("{ProduktyOferta", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                ZwrocDaneOferty(koszyk, tmp);
            } 
            //czy we wzorze chcemy ilość pozycji
            if (wzor.IndexOf("{sumaryczneIlosciPozcji", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                ZwrocSumIloscPozycji(koszyk, tmp);
            }
            return tmp;
        }
    }
}
