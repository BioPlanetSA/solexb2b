using System.Text.RegularExpressions;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [FriendlyName("Utwórz nazwę - Gedeon", FriendlyOpis = "Moduł, który tworzy nazwy produktów wg wzoru (Gedeon). Nazwy są zmieniane dla produktów w wybranej kategorii. Atrybuty należy podawać w nawiasach {}, zwykły lokalizowany tekst w nawiasach []. np [Albumy tradycyjne] {typ albumu}. Dodatkowe parametry, które można użyć to pola z produktu: [symbol]")]
    public class UworzNazweGedeon : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {
        
        public UworzNazweGedeon()
        {
            WzorNazwy = "";
            Kategoria = "";
        }

        public override string uwagi
        {
            get { return ""; }
        }

        //public override string Opis
        //{
        //    get { return "Moduł, który tworzy nazwy produktów wg wzoru (Gedeon). Nazwy są zmieniane dla produktów w wybranej kategorii. Atrybuty należy podawać w nawiasach {}, zwykły lokalizowany tekst w nawiasach []. np [Albumy tradycyjne] {typ albumu}. Dodatkowe parametry, które można użyć to pola z produktu: [symbol]"; }
        //}

        [FriendlyName("Wzór nazwy")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public  string WzorNazwy { get; set; }

        [FriendlyName("Nazwa kategorii produktu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public  string Kategoria { get; set; }

        private  List<string> _listaPol;
        private  List<string> ListaPol
        {
            get
            {
                if (_listaPol == null)
                {
                    _listaPol = new List<string>();
                    _listaPol.Add("[symbol]");
                }

                return _listaPol;
            }
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> slowniki, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            //List<Cecha> cechy = ApiWywolanie.PobierzCechy().Values.ToList();
          
            //var atrybuty = ApiWywolanie.PobierzAtrybuty().Values.ToList();
            var slownikiapi = ApiWywolanie.GetSlowniki().Values.ToList();
           // var pola = ApiWywolanie.GetSystemTypes();

            //const string albumTradycyjny = "Album tradycyjny";
            //const string iloscKart = "Ilosc kart";
            //const string rozmiarStron = "Rozmiar stron";
            //const string kolorStron = "kolor stron";
            //Log.Debug("przetwarzanie dla kategorii " + Kategoria);
            //Log.Debug("wzór: " + WzorNazwy);
            var listagrup = ListaGrup(WzorNazwy);
        //    Log.Debug("liczba grup: " + listagrup.Length);
            foreach (var p in listaWejsciowa) 
            {
                try
                {
                    //var sl = pobierzcechezsymbolu("Sposób łączenia", CechyProduktyNaPlatfromie, cechy, slowniki, atrybuty, p);
                    //var rs = pobierzcechezsymbolu("Rozmiar stron", CechyProduktyNaPlatfromie, cechy, slowniki, atrybuty, p);
                    //var iis = pobierzcechezsymbolu("Ilosc stron", CechyProduktyNaPlatfromie, cechy, slowniki, atrybuty, p);
                    //var ks = pobierzcechezsymbolu("kolor stron", CechyProduktyNaPlatfromie, cechy, slowniki, atrybuty, p);
                    //var rz = pobierzcechezsymbolu("Rozmiar zdjec", CechyProduktyNaPlatfromie, cechy, slowniki, atrybuty, p);
                    //var iz = pobierzcechezsymbolu("Ilosc zdjec", CechyProduktyNaPlatfromie, cechy, slowniki, atrybuty, p);
                    //var izns = pobierzcechezsymbolu("Ilość zdjęć na stronie", CechyProduktyNaPlatfromie, cechy, slowniki, atrybuty, p);
                    //var mr = pobierzcechezsymbolu("materiał ramki", CechyProduktyNaPlatfromie, cechy, slowniki, atrybuty, p);
                    //var rr = pobierzcechezsymbolu("Rozmiar ramka", CechyProduktyNaPlatfromie, cechy, slowniki, atrybuty, p);
                 //   Log.Error("NOWY TOWAR " + p.kod + " " + p.nazwa);
                    var kategoria = kategorie.Values.FirstOrDefault(a => a.Nazwa == Kategoria);
                    
                    if (kategoria != null)
                    {
                        var podrzedne = kategorie.Values.Where(a => a.ParentId == kategoria.Id).ToList();
                     //   Log.Debug("kategorii podrzędnych: " + podrzedne.Count);
                        var kategorieProduktu =
                            lacznikiKategorii.FirstOrDefault(
                                a => a.ProduktId == p.Id && (a.KategoriaId == kategoria.Id || podrzedne.Any(b => b.Id == a.KategoriaId)));


                        if (kategorieProduktu != null)
                        {
                       //     Log.Debug("kategoria produktu " + kategorieProduktu.kategoria_id);

                            foreach (KeyValuePair<int, Jezyk> jezyk in SyncManager.PobierzInstancje.Konfiguracja.JezykiWSystemie)
                            {
                                //if (jezyk.Key == Config.JezykIDDomyslny)
                                //    continue;

                                string nazwaTowaru = WzorNazwy;

                                foreach (string grupa in listagrup)
                                {
                                    string nazwaGrupy = grupa;
                                 //   Log.Debug("nazwa grupy " + grupa);
                                    var atrybutyzgrupy = Atrybut(nazwaGrupy);

                                    foreach (var atrybut in atrybutyzgrupy)
                                    {
                                  //      Log.Debug("atrybut " + atrybut);
                                        var tlumaczenie = Tlumaczenie(nazwaGrupy);
                                   //     Log.Debug("tłumaczenie " + tlumaczenie);

                                        //foreach (string atrybut in listaAtrybutow)
                                        //{
                                        string atr = "";
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(atrybut))
                                            {
                                                atr = pobierzcechezsymbolu(atrybut.Replace("{", "").Replace("}", ""), lacznikiCech.Values, cechy, slownikiapi, atrybuty, p, jezyk.Key);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.Error(new Exception(ex.Message, ex));
                                        }

                                   //     Log.Debug("atrybut: " + atr);

                                        if (string.IsNullOrEmpty(atr) && !string.IsNullOrEmpty(atrybut))
                                        {
                                            //jeśli w grupie jest więcej atrybutów to nie wywalamy całości
                                            if (atrybutyzgrupy.Count > 1)
                                            {
                                                nazwaGrupy = nazwaGrupy.Replace(atrybut, "");
                                            }
                                            else
                                                //jeśli nie ma danego atrybutu to ukrywamy całość
                                                nazwaGrupy = "";
                                        }
                                        else
                                        {

                                            if (!string.IsNullOrEmpty(atrybut))
                                                nazwaGrupy = nazwaGrupy.Replace(atrybut, atr);
                                            //}
                                        //    Log.Debug("nazwa grupy " + nazwaGrupy);
                                            //foreach (string tlumaczenie in listaTlumaczen)
                                            //{

                                            if (!string.IsNullOrEmpty(tlumaczenie))
                                            {
                                                string tlumaczeniebezZbednychZnakow = tlumaczenie.Replace("[", "").Replace("]", "");
                                                string przetlumaczonepole = SyncManager.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(jezyk.Key, tlumaczeniebezZbednychZnakow);

                                                nazwaGrupy = nazwaGrupy.Replace(tlumaczenie, przetlumaczonepole);
                                            }
                                      //      Log.Debug("nazwa grupy " + nazwaGrupy);
                                            //}
                                        }
                                    }
                                    nazwaTowaru = nazwaTowaru.Replace(grupa, nazwaGrupy);

                                }

                                nazwaTowaru = nazwaTowaru.Replace("[symbol]", p.Kod);
                                nazwaTowaru = nazwaTowaru.Replace("(", "").Replace(")", "");
                                nazwaTowaru = nazwaTowaru.Trim();
                                if (!string.IsNullOrEmpty(nazwaTowaru) && nazwaTowaru != WzorNazwy)
                                {
                                    if (jezyk.Key == SyncManager.PobierzInstancje.Konfiguracja.JezykIDPolski)
                                    {
                                        p.Nazwa = nazwaTowaru;
                                    }
                                    else
                                    {

                                        Tlumaczenie tlumaczenienazwy = slowniki.FirstOrDefault(a =>a.ObiektId == p.Id && a.JezykId == jezyk.Key && a.Pole == "Nazwa");

                                        if (tlumaczenienazwy != null)
                                        {
                                            tlumaczenienazwy.Wpis = nazwaTowaru;
                                        }
                                        else
                                        {
                                            Tlumaczenie noweTlumaczenie = new Tlumaczenie();
                                            noweTlumaczenie.JezykId = jezyk.Key;
                                            noweTlumaczenie.ObiektId = p.Id;
                                            noweTlumaczenie.Pole = "Nazwa";
                                            noweTlumaczenie.Typ = (typeof (ProduktBazowy)).PobierzOpisTypu();
                                              //  pola.FirstOrDefault(a => a.Nazwa == typeof (Produkt).ToString()).Id;
                                            noweTlumaczenie.Wpis = nazwaTowaru;

                                            slowniki.Add(noweTlumaczenie);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Log.Error(string.Format("Wystąpił błąd przy przetwarzaniu nazwy. Błą: {0}", ex.Message), ex);
                }
            }
        }

        private  List<string> Atrybut(string zrodlo)
        {
            List<string> listaAtrybutow = new Regex(@"\{(.*?)\}").Matches(zrodlo).Cast<Match>()
                                                             .Select(m => m.Groups[0].Value)
                                                             .ToList();
            return listaAtrybutow;
        }

        private  string[] ListaGrup(string zrodlo)
        {
            string[] listaGrup = new Regex(@"\((.*?)\)").Matches(zrodlo).Cast<Match>()
                                                             .Select(m => m.Groups[0].Value)
                                                             .ToArray();
            return listaGrup;
        }

        private  string Tlumaczenie(string zrodlo)
        {
            string listaTlumaczen = new Regex(@"\[(.*?)\]").Matches(zrodlo).Cast<Match>()
                                                           .Select(m => m.Groups[0].Value).FirstOrDefault(a => ListaPol.All(b => b != a));
            return listaTlumaczen;
        }

        private string pobierzTlumaczenieAtrybutu(string nazwaAtrybutu, List<Tlumaczenie> slowniki, List<Atrybut> atrybuty)
        {
            string ostatecznetlumaczenie = nazwaAtrybutu;
            Atrybut atrybut = atrybuty.FirstOrDefault(a => a.Nazwa == nazwaAtrybutu);
            if (atrybut != null)
            {
                Tlumaczenie tlumaczenie =
                    slowniki.FirstOrDefault(a => a.ObiektId == atrybut.Id && a.Pole == "Nazwa");

                if (tlumaczenie != null)
                    ostatecznetlumaczenie = tlumaczenie.Wpis;
            }

            return ostatecznetlumaczenie;
        }

        private string pobierzcechezsymbolu(string symbol, IEnumerable<ProduktCecha> cechyprodukty, IEnumerable<Cecha> cechy, List<Tlumaczenie> slowniki, List<Atrybut> atrybuty, Produkt p, int jezykid)
        {
       //     Log.Debug("szukanie atrybutu "  + symbol);
            var atrybut = atrybuty.FirstOrDefault(a => a.Nazwa == symbol);
            if (atrybut != null)
            {
         //       Log.Debug("znaleziono atrybut " + atrybut.nazwa);

                var cechyproduktu = cechyprodukty.Where(a => a.ProduktId == p.Id).ToList();
           //     Log.Debug("cechy produktu " + cechyproduktu.Count);
                var cecha =
                    cechy.FirstOrDefault(a => cechyproduktu.Any(b => b.CechaId == a.Id) && a.AtrybutId == atrybut.Id);


                if (cecha != null)
                {
                 //   Log.Debug("cecha produktu " + cecha.symbol);

                    var val = string.IsNullOrEmpty(cecha.Nazwa) ? cecha.Symbol : cecha.Nazwa;
                    var val2 = 
                        slowniki.FirstOrDefault(
                            a => a.JezykId == jezykid && a.Pole == "Nazwa" && a.ObiektId == cecha.Id && a.Typ == (typeof(Cecha)).PobierzOpisTypu());//.typy.FirstOrDefault(b => b.Nazwa == typeof(Cecha).ToString()).Id);

                    //Log.Debug("jezykid=" + jezykid);
                    //Log.Debug("obiekt_id=" + cecha.cecha_id);
                    //Log.Debug("typ_id=" + typy.FirstOrDefault(b => b.nazwa == typeof(cechy).ToString()).id);
                    //Log.Debug("znalezione val2: " + (val2 != null ? "tak" : "nie"));

                    if (val2 != null && !string.IsNullOrEmpty(val2.Wpis))
                    {
                        val = val2.Wpis;
                     //   Log.Debug("podmiana na tłumaczenie: " + val2.wpis);
                    }

                    return val;
                }
                
            }

            return "";
        }
    }
}
