using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.SzczegolyProduktu;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu;
using SolEx.Hurt.Web.Site2.PageBases;
using Klient = SolEx.Hurt.Model.Klient;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("SzczegolyProduktu")]
    public class SzczegolyProduktuController : SolexControler
    {
        [System.Web.Mvc.Route("Cena")]
        public PartialViewResult Cena(long produktid, JakieCenyPokazywac typceny, RodzajCeny rodzajceny, bool niepokazujjeslibrakceny, int? poziomcenowy, string nazwaceny, bool cenazawszewidoczna)
        {
            ProduktKlienta pk = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produktid, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
            if (!cenazawszewidoczna)
            {
                string info;
                bool pokazujCene;
                SolexBllCalosc.PobierzInstancje.ProduktyBazowe.MoznaDodacDoKoszyka(pk, out info, out pokazujCene);
                if (!pokazujCene)
                {
                    return null;
                }
            }

            //jak sa gradacje da prodktu to nie pokazujemy nic w tej kontrolce - uzyj kontrolki gradacji
            if (pk.GradacjePosortowane != null)
            {
                return null;
            }

            string waluta = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Waluta>(pk.FlatCeny.WalutaId).WalutaB2b;

            CenaDoPokazania parametryCeny = new CenaDoPokazania();
            parametryCeny.CoPokazywac = typceny;
            parametryCeny.JednostkaMiaryOpcjonalna = pk.JednostkaPodstawowa.Nazwa;

            if (rodzajceny == RodzajCeny.CenaPrzedRabatem)
            {
                parametryCeny.CenaNetto = pk.FlatCeny.CenaHurtowaNetto;
                parametryCeny.CenaBrutto =pk.FlatCeny.CenaHurtowaBrutto;
                parametryCeny.OpisNetto = "Cena przed rabatem netto";
                parametryCeny.OpisBrutto = "Cena przed rabatem brutto";
                parametryCeny.CssCalosc = "ceny-przed-rabatem row karta";
                parametryCeny.CssOpis = parametryCeny.CssWartosc = "col-xs-6";
            }
            else if (rodzajceny == RodzajCeny.CenaDetaliczna)
            {
                if (pk.FlatCeny.CenaDetalicznaNetto.Waluta == null || pk.FlatCeny.CenaDetalicznaNetto.Waluta.Equals(waluta, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
                parametryCeny.CenaNetto = pk.FlatCeny.CenaDetalicznaNetto;
                parametryCeny.CenaBrutto = pk.FlatCeny.CenaDetalicznaBrutto;
                parametryCeny.OpisNetto = "Cena detaliczna netto";
                parametryCeny.OpisBrutto = "Cena detaliczna brutto";
                parametryCeny.CssCalosc = "ceny-detaliczna row karta";
                parametryCeny.CssOpis = parametryCeny.CssWartosc = "col-xs-6";
            }
            else if (rodzajceny == RodzajCeny.CenaPoRabacie)
            {
                parametryCeny.CenaNetto = pk.FlatCeny.CenaNetto;
                parametryCeny.CenaBrutto = pk.FlatCeny.CenaBrutto;
                parametryCeny.OpisNetto = "Twoja cena netto";
                parametryCeny.OpisBrutto = "Twoja cena brutto";
                parametryCeny.CssCalosc = "ceny-twoja row karta";
                parametryCeny.CssOpis = parametryCeny.CssWartosc = "col-xs-6";

                if (pk.FlatCeny.CenaNettoPrzedPromocja != null)
                {
                    parametryCeny.CenaNettoPrzekreslona = pk.FlatCeny.CenaNettoPrzedPromocja;
                    parametryCeny.CenaBruttoPrzekreslona =pk.FlatCeny.CenaBruttoPrzedPromocja;
                }
            }
            else if (rodzajceny == RodzajCeny.CenaWWybranymPoziomieCenowym)
            {
                if(poziomcenowy == null) throw new NullReferenceException("Brak wybranego poziomu cenowego dla kontrolki");

                if (!pk.CenyPoziomy.ContainsKey(poziomcenowy.Value)) { return null;}

                CenaPoziomu poziom =pk.CenyPoziomy[poziomcenowy.Value];

                if (poziom.WalutaId != pk.FlatCeny.WalutaId){return null;}
                if (string.IsNullOrEmpty(nazwaceny)) nazwaceny = "Cena";
                string walutaPoz = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Waluta>(poziom.WalutaId).WalutaB2b;
                parametryCeny.CenaNetto = new WartoscLiczbowa(pk.CenyPoziomy[poziomcenowy.Value].Netto, walutaPoz);
                parametryCeny.CenaBrutto = new WartoscLiczbowa(Kwoty.WyliczBrutto(parametryCeny.CenaNetto.Wartosc, pk.Vat), walutaPoz);
                parametryCeny.OpisNetto = nazwaceny+" netto";
                parametryCeny.OpisBrutto = nazwaceny + " brutto";
                parametryCeny.CssCalosc = "ceny-w-poziomie row karta";
                parametryCeny.CssOpis = parametryCeny.CssWartosc = "col-xs-6";
            } else if (rodzajceny == RodzajCeny.CenaPoRabacieBezPrzeliczeniaWaluty)
            {
                if (pk.FlatCeny.PrzeliczenieWaluty_Kurs == 0)
                {
                    // nie bylo przeliczanie walut
                    return null;
                }
                parametryCeny.CenaBrutto = pk.FlatCeny.CenaBruttoPrzedPrzewalutowaniem;
                parametryCeny.CenaNetto = new WartoscLiczbowa(pk.FlatCeny.PrzeliczenieWaluty_CenaNettoBazowa, parametryCeny.CenaBrutto.Waluta);


                parametryCeny.OpisNetto = "Twoja cena netto przed przewalutowaniem";
                parametryCeny.OpisBrutto = "Twoja cena brutto przed przewalutowaniem";
                parametryCeny.CssCalosc = "ceny-twoja row karta";
                parametryCeny.CssOpis = parametryCeny.CssWartosc = "col-xs-6";
            }

            if (!cenazawszewidoczna && niepokazujjeslibrakceny && parametryCeny.CenaNetto.Wartosc == 0)
            {
                return null;
            }

            parametryCeny.CssSeparatorPoziomow = "col-xs-12";

            return PartialView("_Cena", parametryCeny);
        }
        [System.Web.Mvc.Route("Rabat")]
        public PartialViewResult Rabat(long produktid, string uklad, bool pokaznazwe, string naglowek, string stopka, string textzastepczy, string nazwazastepcza)
        {
            ProduktKlienta pk = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produktid, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
            string info;
            bool pokazujCene;
            SolexBllCalosc.PobierzInstancje.ProduktyBazowe.MoznaDodacDoKoszyka(pk, out info, out pokazujCene);
            if (!pokazujCene) return null;
            if (pk.GradacjePosortowane != null)
            {
                return null;
            }

            IList<PoleSzczegolyDane> listaPol = new List<PoleSzczegolyDane>();
            string rabat = pk.FlatCeny.Rabat.DoRabatString();
            if (string.IsNullOrEmpty(rabat) && !string.IsNullOrEmpty(textzastepczy)) rabat = textzastepczy;
            string nazwa = (pokaznazwe) ? "Rabat" : "";
            nazwa = (string.IsNullOrEmpty(nazwazastepcza)) ? nazwa : nazwazastepcza;
            listaPol.Add(new PoleSzczegolyDane(SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(SolexHelper.AktualnyJezyk.Id, nazwa), rabat));
            return PartialView("PolaProduktu/" + uklad,
                new ParametryPrzekazywaneDoSzczegolow(pk, naglowek, listaPol, null, "cena cena-bazowa rabat", pokaznazwe,
                    stopka));
        }

        [System.Web.Mvc.Route("NaglowekNazwa")]
        public PartialViewResult NaglowekNazwa(long produktid)
        {
            var pk = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produktid,
                SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
            return PartialView("PolaProduktu/Nazwa",
                new ParametryPrzekazywaneDoNazwy(pk, RodzajMetki.SzczegolyProduku, null, null, false, false));
        }

        [System.Web.Mvc.Route("Metka")]
        public PartialViewResult Metka(long produktid, MetkaPozycjaSzczegoly pozycja)
        {
            var pk = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produktid,
                SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
            var listaCech = pk.PobierzMetkiSzczegoly(pozycja);
            return PartialView("Produkty/_MetkiProduktu",
                new MetkaParametry(listaCech, string.Format("metki {0}", pozycja)));
            // return PartialView("Metka", listaCech);
        }

        [System.Web.Mvc.Route("Sciezka")]
        public PartialViewResult Sciezka(long produktid, string symbolstronapoprzednia)
        {
            var pk = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produktid, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
            return WspolnaCzescSciezka(Url.ZbudujLink(pk, SolexHelper.AktualnyJezyk), pk.Nazwa, symbolstronapoprzednia);
        }

        //todo:TEST SYlwester zrob do tego co zrobiles!
        [ChildActionOnly]
        public PartialViewResult ZglosBledneDane(long produktid, string mailgdybrakopiekuna, string naglowek, string stopka)
        {
            string email = "";
            SolEx.Hurt.Core.Klient k;
            var pk = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produktid,
                SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);

            if (pk.MenagerId != null)
            {
                k = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<SolEx.Hurt.Core.Klient>(pk.MenagerId);
                if (k != null) email = k.Email;
            }
            if (pk.PrzedstawicielId != null)
            {
                k = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<SolEx.Hurt.Core.Klient>(pk.PrzedstawicielId);
                if (k != null) email = k.Email;
            }
            if (string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(mailgdybrakopiekuna))
            {
                email = mailgdybrakopiekuna;
            }
            if (string.IsNullOrEmpty(email))
            {
                if (SolexHelper.AktualnyKlient != null && SolexHelper.AktualnyKlient.Opiekun != null)
                {
                    email = SolexHelper.AktualnyKlient.Opiekun.Email;
                }
            }

            if (string.IsNullOrEmpty(email)) return null;
            return PartialView("ZglosBlad", new ParametryDoZglosBlad(email, "Błędne dane produktu: ", pk.Nazwa, "Zgłoś błędne dane produktu",naglowek,stopka));
        }

        [ChildActionOnly]
        public PartialViewResult WszystkieAtrybuty(int id)
        {
            WszystkieAtrybutyProduktu kontrolka = this.PobierzKontrolke<WszystkieAtrybutyProduktu>(id);

            var pk = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(kontrolka.ProduktId, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
            List<CechyBll> cechyProduktu = pk.Cechy.Values.Where(x => x.Widoczna).ToList();
            if (!cechyProduktu.Any())
            {
                return null;
            }

            HashSet<int> listaIdAtrybutow = new HashSet<int>( cechyProduktu.Where(x => x.AtrybutId.HasValue).Select(x => x.AtrybutId.Value) );

            //pomijanie wskazanych atrybutow
            if (kontrolka.AtrybutyDoPominiecia != null && kontrolka.AtrybutyDoPominiecia.Any())
            {
                listaIdAtrybutow.ExceptWith(kontrolka.AtrybutyDoPominiecia);
            }

            var listaAtrybutow = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<AtrybutBll>(SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, x => listaIdAtrybutow.Contains(x.Id) && x.Widoczny).OrderBy(x => x.Kolejnosc).ThenBy(x=>x.Nazwa);

            Dictionary<string, List<KeyValuePair<CechyBll, bool>>> slownikAtrybutowICech = new Dictionary<string, List<KeyValuePair<CechyBll, bool>>>();
            foreach (var i in listaAtrybutow)
            {
                var tmp = cechyProduktu.Where(x => x.AtrybutId == i.Id && x.Widoczna)
                        .OrderBy(x => x.Kolejnosc)
                        .Select(x => new KeyValuePair<CechyBll, bool>(x, i.PokazujNaLiscieProduktow))
                        .ToList();
                if (slownikAtrybutowICech.ContainsKey(i.Nazwa))
                {
                    slownikAtrybutowICech[i.Nazwa].AddRange(tmp);
                }
                else
                {
                    slownikAtrybutowICech.Add(i.Nazwa, tmp);
                }

            }
            if (!string.IsNullOrEmpty(kontrolka.TextZastepczy) && !slownikAtrybutowICech.Any())
                return PartialView("Text", kontrolka.TextZastepczy);

            return PartialView("WszystkieAtrybuty", slownikAtrybutowICech);
        }

        [ChildActionOnly]
        public PartialViewResult PoleProduktu(long produktid, string naglowek, string[] pola, int[] atrybuty, bool pokaznazwe, string uklad, string stopka, string textzastepczy, string nazwazastepcza, string formatowanie = "", bool linkowaniecech = false)
        {
            ProduktKlienta pk = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produktid,SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
            Type type = pk.GetType();
            IList<PoleSzczegolyDane> listaPol = new List<PoleSzczegolyDane>();
            IList<PoleSzczegolyDane> listaAtrybutow = new List<PoleSzczegolyDane>();
            IKlient klient = pk.Klient;
            HashSet<long> dostepneDlaKlietnaProdukty = SolexBllCalosc.PobierzInstancje.ProduktyKlienta.PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(klient);

            if (pola != null && pola.Any())
            {
                foreach (string np in pola)
                {
                    PropertyInfo property = type.GetProperty(np);
                    FriendlyNameAttribute atr = property.GetCustomAttribute<FriendlyNameAttribute>();
                    string p = atr != null ? atr.FriendlyName: np;
                    if (!string.IsNullOrEmpty(nazwazastepcza) && pola.Length == 1) p = nazwazastepcza;
                    p = SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(SolexHelper.AktualnyJezyk.Id, p);
                    Type typ = property.PropertyType;

                    string sofix = "";

                    var valueTmp = property.GetValue(pk);
                    if (valueTmp == null) continue;

                    if (property.Name == "Vat") sofix = "%";
                    if (property.Name == "Waga") sofix = " kg";
                    if (property.Name == "IloscMinimalna") sofix = " "+pk.JednostkaPodstawowa.Nazwa;
                    if (property.Name == "IloscWOpakowaniu") sofix = " " + pk.JednostkaPodstawowa.Nazwa;
                    if (property.Name == "JednostkaPodstawowa")
                        listaPol.Add(!string.IsNullOrEmpty(formatowanie)
                            ? new PoleSzczegolyDane(p, string.Format(formatowanie, pk.JednostkaPodstawowa.Nazwa), sofix)
                            : new PoleSzczegolyDane(p, pk.JednostkaPodstawowa.Nazwa, sofix));
                    if (property.Name == "Marka")
                    {
                        string value;
                        bool link = SolexBllCalosc.PobierzInstancje.KategorieDostep.JestWidocznaDlaKlienta(pk.Marka, klient, dostepneDlaKlietnaProdukty);
                        string linkkat = Url.ZbudujLink(pk.Marka, SolexHelper.AktualnyJezyk);

                        if (link)
                        {
                            if (pk.Marka.Obrazek != null)
                            {
                                value = "<a href=" + linkkat + "><img src=" + pk.Marka.Obrazek.LinkWWersji("producent") +
                                        " alt=" + pk.Marka.Nazwa + " /></a>";
                            }
                            else
                            {
                                value = "<a href = " + linkkat + " > " +
                                        (!string.IsNullOrEmpty(formatowanie)
                                            ? string.Format(formatowanie, pk.Marka.Nazwa)
                                            : pk.Marka.Nazwa) + " </a>";
                            }
                        }
                        else
                        {
                            if (pk.Marka.Obrazek != null)
                            {
                                value = "<img src=" + pk.Marka.Obrazek.LinkWWersji("producent") + " alt=" +
                                        pk.Marka.Nazwa + " />";
                            }
                            else
                            {
                                value = !string.IsNullOrEmpty(formatowanie)
                                    ? string.Format(formatowanie, pk.Marka.Nazwa)
                                    : pk.Marka.Nazwa;
                            }
                        }

                        listaPol.Add(new PoleSzczegolyDane(p, value, sofix));
                    }

                    if (typ == typeof(string))
                    {
                        string value = valueTmp.ToString();
                        if (string.IsNullOrEmpty(value)) continue;
                        listaPol.Add(!string.IsNullOrEmpty(formatowanie)
                            ? new PoleSzczegolyDane(p, string.Format(formatowanie, value), sofix)
                            : new PoleSzczegolyDane(p, value, sofix));
                    }
                    if (typ == typeof(decimal) || typ == typeof(decimal?))
                    {
                        var value = (decimal) valueTmp;
                        if (value == 0) continue;
                        listaPol.Add(!string.IsNullOrEmpty(formatowanie)
                            ? new PoleSzczegolyDane(p, string.Format(formatowanie, value.ToString("0.##")), sofix)
                            : new PoleSzczegolyDane(p, value, sofix));
                    }
                    if (typ == typeof(int) || typ == typeof(int?))
                    {
                        var value = (int) valueTmp;
                        if (value == 0) continue;
                        listaPol.Add(!string.IsNullOrEmpty(formatowanie)
                            ? new PoleSzczegolyDane(p, string.Format(formatowanie, value), sofix)
                            : new PoleSzczegolyDane(p, value, sofix));
                    }
                }
            }

            if (atrybuty != null && atrybuty.Any())
            {
                var atrybutyLista = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<AtrybutBll>(SolexHelper.AktualnyJezyk.Id,SolexHelper.AktualnyKlient, x => Sql.In(x.Id, atrybuty)).OrderBy(x=>x.Kolejnosc).ThenBy(x=>x.Nazwa).ToList();
                foreach (var atrybutid in atrybutyLista)
                {
                    //AtrybutBll atryb = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<AtrybutBll>(atrybutid, SolexHelper.AktualnyJezyk.Id);
                    List<CechyBll> cechy = pk.CechyDlaAtrybutu(atrybutid.Id);
                    //jak nie ma cech dla danego produktu z tego atrybutu to pomijamy
                    if (cechy.Any())
                    {
                        var wart = new Dictionary<string, string>();
                        foreach (CechyBll cecha in cechy)
                        {
                            string url = "";
                            if (linkowaniecech)
                            {
                                KategorieBLL kat =
                                    SolexBllCalosc.PobierzInstancje.KategorieDostep.CzyNaPodstwieCechy(cecha);
                                if (kat != null && SolexBllCalosc.PobierzInstancje.KategorieDostep.JestWidocznaDlaKlienta(kat,SolexHelper.AktualnyKlient, dostepneDlaKlietnaProdukty))
                                {
                                    url = Url.ZbudujLink(kat, SolexHelper.AktualnyJezyk);
                                        //new GeneratorLinkowController().LinkKategoria(kat).Replace(" ", "%20");
                                }
                                else
                                {
                                    url = Url.ZbudujLink(cecha, SolexHelper.AktualnyJezyk); //cecha.LinkDoProduktow;
                                }
                            }
                            wart.Add(
                                !string.IsNullOrEmpty(formatowanie)
                                    ? string.Format(formatowanie, cecha.Nazwa)
                                    : cecha.Nazwa, url);
                        }
                        listaAtrybutow.Add(new PoleSzczegolyDane((atrybutid != null) ? SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(SolexHelper.AktualnyJezyk.Id, atrybutid.Nazwa) : "", wart));
                    }
                }
            }

            if (!listaAtrybutow.Any() && !listaPol.Any())
            {
                if (string.IsNullOrEmpty(textzastepczy))
                {
                    return null;
                }
                listaPol.Add(new PoleSzczegolyDane(nazwazastepcza, textzastepczy));
            }

            return PartialView("PolaProduktu/" + uklad, new ParametryPrzekazywaneDoSzczegolow(pk, naglowek, listaPol, listaAtrybutow, null, pokaznazwe, stopka));
        }

        [System.Web.Mvc.Route("GaleriaZdjecMiniaturki")]
        public PartialViewResult GaleriaZdjecMiniaturki(long produktid, string rozmiarzdjecia, string rozmiarminiatur,
            string textzastepczy, string naglowek, string stopka, bool duzypodglad)
        {
            var pk = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produktid,
                SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
            if ((pk.Zdjecia==null || pk.Zdjecia.Count == 1) && string.IsNullOrEmpty(textzastepczy))
            {
                return null;
            }
            return PartialView("GaleriaZdjecMiniaturki",
                new ParametryGaleriiZdjecProduktu(pk, rozmiarzdjecia, naglowek, stopka, rozmiarminiatur, textzastepczy)
                {
                    DuzyPodglad = duzypodglad
                });

        }
        [System.Web.Mvc.Route("GaleriaZdjecZdjecieGlowne")]
        public PartialViewResult GaleriaZdjecZdjecieGlowne(long produktid, bool pobierzpelny, bool duzypodglad,
            string rozmiarzdjecia, string naglowek, string stopka)
        {
            var pk = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produktid,SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
            return PartialView("GaleriaZdjecGlowne", new ParametryGaleriiZdjecProduktu(pk, rozmiarzdjecia, naglowek, stopka, pobierzpelny, duzypodglad));

        }

        [System.Web.Mvc.Route("Nawigacja")]
        public PartialViewResult Nawigacja(long produktid, int[] grupaprod)
        {
            var pk = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produktid, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
           
            //List<long> listaKategorii = new List<long>();
            //if (grupaprod != null && grupaprod.Length > 0)
            //{
            //    foreach (int grupa in grupaprod)
            //    {
            //        List<long> kategorie = SolexBllCalosc.PobierzInstancje.KategorieDostep.PobierzDrzewkoKategorii(grupa, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient).Select(x => x.Id).ToList();
            //        listaKategorii.AddRange(kategorie);
            //    }
            //}
            //else
            //{
            //    GrupaBLL prod = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<GrupaBLL>(x => x.Producencka, SolexHelper.AktualnyKlient);
            //    listaKategorii = prod.PobierzKategorie(SolexHelper.AktualnyKlient).Select(x => x.Id).ToList();
            //}

            //if (!listaKategorii.Any()) return null;

         IList<ProduktKlienta> produkty =
             SolexBllCalosc.PobierzInstancje.ProduktyKlienta.ProduktySpelniajaceKryteria(null, null,
                 SolexHelper.AktualnyKlient, SolexHelper.AktualnyJezyk.Id,
                 null, new Dictionary<int, HashSet<long>>(), null, null);

            int pozycjaAktualna = produkty.ToList().IndexOf(pk);
            if (pozycjaAktualna == -1) return null;
            IProduktKlienta next = null;
            if (pozycjaAktualna < produkty.Count - 1) next = produkty.ToList()[pozycjaAktualna + 1];
            IProduktKlienta prev = null;
            if (pozycjaAktualna > 0) prev = produkty.ToList()[pozycjaAktualna - 1];

            return PartialView("Nawigacja", new ParametryDoNawigacjiProduktu(next, prev, produkty.Count, pozycjaAktualna));
        }

        [System.Web.Mvc.Route("KategorieProduktu")]
        public PartialViewResult KategorieProduktu(long produktid, string naglowek, string stopka)
        {
            ProduktKlienta produkt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produktid,SolexHelper.AktualnyKlient);

            List<KategorieBLL> kategorie = produkt.Kategorie.Where(x => x.Widoczna && x.Grupa.Widoczna).ToList();
            ParametryDoKategoriProduktuSzczegoly parametry = new ParametryDoKategoriProduktuSzczegoly(new List<KategorieBLL>(4), naglowek, stopka);

            //odfiltrowanie kategorii jesli sie zawieraja w sobie albo powtarzaja
            foreach (KategorieBLL kat in kategorie)
            {
                if (kategorie.Any(x =>x.ParentId != null && x.PobierzIdWszystkichNadrzednych().Contains(kat.Id)))
                {
                    continue;
                }
                parametry.Kategorie.Add(kat);
            }

            return PartialView("KategorieProduktu", parametry);
        }

        [System.Web.Mvc.Route("KoncesjeProduktu")]
        public PartialViewResult KoncesjeProduktu(long produktid, string naglowek, string stopka)
        {
            ProduktKlienta produkt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produktid,SolexHelper.AktualnyKlient);
            if (produkt.WymaganaKoncesja == null || !produkt.WymaganaKoncesja.Any()) return null;

            HashSet<CechyBll> cechy = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzCechyOId(produkt.WymaganaKoncesja, SolexHelper.AktualnyJezyk.Id);

            List<string> koncesje = cechy.Select(x => x.Nazwa).ToList();
            return PartialView("KoncesjeProduktu", new ParametryDoKoncesjiProduktu(koncesje, naglowek, stopka));
        }
        [System.Web.Mvc.Route("StanyProduktu")]
        public PartialViewResult StanyProduktu(long produktid, string naglowek, string stopka, List<long> listasposobow)
        {
            var produkt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produktid, SolexHelper.AktualnyKlient);
            List<StanNaMagazynie> listaStanowDlaMagazynow = new List<StanNaMagazynie>();
            Dictionary<PozycjaLista, Dictionary<long, SposobPokazywaniaStanow>> sposoby = SolexBllCalosc.PobierzInstancje.SposobyPokazywaniaStanowBll.WszystkieSposobyKlienta(SolexHelper.AktualnyKlient, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyPrzedstawiciel,listasposobow,false);
            if (sposoby == null)
            {
                return null;
            }
            foreach (var sposob in sposoby.Values)
            {
                foreach (var sposobPokazywaniaStanow in sposob.Values)
                {
                    var stany = SolexBllCalosc.PobierzInstancje.SposobyPokazywaniaStanowBll.PobierzStanProduktuWgSposobu(sposobPokazywaniaStanow, produkt, produkt.JezykId, SolexHelper.AktualnyKlient);
                    foreach (var s in stany)
                    {
                        listaStanowDlaMagazynow.AddRange(s.Value);
                    }
                }
            }
            if (!sposoby.Any() || !listaStanowDlaMagazynow.Any())
            {
                return null;
            }
            return PartialView("StanyProduktow", new KeyValuePair<ProduktKlienta, List<StanNaMagazynie>>(produkt,listaStanowDlaMagazynow));
        }
        [System.Web.Mvc.Route("WyswietlWymiaryProduktu")]
        public PartialViewResult WyswietlWymiaryProduktu(long produktid, string naglowek, string stopka, string jednostkagabarytow, string jednostkaobjetosci, string jednostkawagi, int? obrazekpudelka, int? obrazekopzbiorczego, int? obrazekpalety, bool pokazujjednostkepodstawowa, int zaokragleniewymirow, int zaokragleniewagi)
        {
            ProduktKlienta produkt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produktid,SolexHelper.AktualnyKlient);

            WymiaryProduktu wymiary = new WymiaryProduktu();
            wymiary.JednostkaGabarytow = jednostkagabarytow;
            wymiary.JednostkaObjetosci = jednostkaobjetosci;
            wymiary.JednostkaWagi = jednostkawagi;
            IObrazek pudelko = null;
            if (obrazekpudelka != null)
            {
                pudelko = SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek((int) obrazekpudelka);
            }

            IObrazek zbiorcze = null;
            if (obrazekopzbiorczego != null)
            {
                zbiorcze = SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek((int) obrazekopzbiorczego);
            }
            IObrazek paleta = null;
            if (obrazekpalety != null)
            {
                paleta = SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek((int) obrazekpalety);
            }
            
            if (!produkt.JednostkaPodstawowa.Calkowitoliczowa) produkt.Waga = null;

            wymiary.OpJednostkowe = new WymiaryOpakowania(pudelko, 
                produkt.OpJednostkoweGlebokosc != null ? decimal.Round((decimal)produkt.OpJednostkoweGlebokosc, zaokragleniewymirow) : produkt.OpJednostkoweGlebokosc,
                produkt.OpJednostkoweWysokosc != null ? decimal.Round((decimal)produkt.OpJednostkoweWysokosc, zaokragleniewymirow) : produkt.OpJednostkoweWysokosc,
                produkt.OpJednostkoweSzerokosc != null ? decimal.Round((decimal)produkt.OpJednostkoweSzerokosc, zaokragleniewymirow) : produkt.OpJednostkoweSzerokosc,
                produkt.Waga != null ? decimal.Round((decimal)produkt.Waga, zaokragleniewagi) : produkt.Waga,
                produkt.Objetosc != null ? decimal.Round((decimal)produkt.Objetosc, zaokragleniewagi) : produkt.Objetosc
                );

            if (produkt.OpZbiorczeIloscWOpakowaniu == null && produkt.IloscWOpakowaniu != 1)
            {
                produkt.OpZbiorczeIloscWOpakowaniu = produkt.IloscWOpakowaniu;
            }

            wymiary.OpZbiorcze = new WymiaryOpakowania(zbiorcze, 
                produkt.OpZbiorczeGlebokosc != null ? decimal.Round((decimal)produkt.OpZbiorczeGlebokosc, zaokragleniewymirow) : produkt.OpZbiorczeGlebokosc,
                produkt.OpZbiorczeWysokosc != null ? decimal.Round((decimal)produkt.OpZbiorczeWysokosc, zaokragleniewymirow) : produkt.OpZbiorczeWysokosc,
                produkt.OpZbiorczeSzerokosc != null ? decimal.Round((decimal)produkt.OpZbiorczeSzerokosc, zaokragleniewymirow) : produkt.OpZbiorczeSzerokosc,
                produkt.OpZbiorczeWaga != null ? decimal.Round((decimal)produkt.OpZbiorczeWaga, zaokragleniewagi) : produkt.OpZbiorczeWaga,
                produkt.OpZbiorczeObjetosc != null ? decimal.Round((decimal)produkt.OpZbiorczeObjetosc, zaokragleniewagi) : produkt.OpZbiorczeObjetosc,
                produkt.OpZbiorczeIloscWOpakowaniu);

            wymiary.Paleta = new WymiaryOpakowania(paleta,
                produkt.OpPaletaGlebokosc!=null?decimal.Round((decimal)produkt.OpPaletaGlebokosc,zaokragleniewymirow): produkt.OpPaletaGlebokosc,
                produkt.OpPaletaWysokosc != null ? decimal.Round((decimal)produkt.OpPaletaWysokosc, zaokragleniewymirow) : produkt.OpPaletaWysokosc,
                produkt.OpPaletaSzerokosc != null ? decimal.Round((decimal)produkt.OpPaletaSzerokosc, zaokragleniewymirow) : produkt.OpPaletaSzerokosc,
                produkt.OpPaletaWaga!=null? decimal.Round((decimal)produkt.OpPaletaWaga,zaokragleniewagi):produkt.OpPaletaWaga,
                produkt.OpPaletaObjetosc != null? decimal.Round((decimal)produkt.OpPaletaObjetosc, zaokragleniewagi):produkt.OpPaletaObjetosc,
                produkt.OpPaletaIloscWOpakowaniu, 
                produkt.OpPaletaIloscNaWarstwie);

            return PartialView("WymiaryProduktu", new ParametryDoWymiarowProduktu(wymiary, obrazekpudelka, obrazekopzbiorczego, obrazekpalety, (pokazujjednostkepodstawowa) ? produkt.JednostkaPodstawowa.Nazwa : "", new DaneNaglowekStopka(naglowek, stopka)));
        }
    }
}