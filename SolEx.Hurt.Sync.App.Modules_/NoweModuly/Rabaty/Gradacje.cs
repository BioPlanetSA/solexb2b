 using System;
using System.Collections.Generic;
 using System.Globalization;
 using System.Linq;
 using System.Reflection;
 using ServiceStack.Common;
 using ServiceStack.Text;
 using SolEx.Hurt.Core.BLL;
 using SolEx.Hurt.Core.Helper;
 using SolEx.Hurt.Core.ModelBLL;
 using SolEx.Hurt.Core.Sync;
 using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
 using SolEx.Hurt.Model.Helpers;
 using SolEx.Hurt.Model.Interfaces;
 using SolEx.Hurt.Model.Interfaces.SyncModuly;
 

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty
{
    [FriendlyName("Gradacje",FriendlyOpis = "Gradacja na podstawie cech produktu. Działa dla poszczególnych klientów (po symbolu klienta), po grupie klientów (wg nazwy grupy) oraz globalnie dla każdego klienta. Kwota gradacji musi być podana liczbowo lub procentowo. <br/>" +
                       "Przykładowa cecha dla gradacji: <br /> hurt_32=35;54=666;detal_55=23 gdzie średnik to separator kolejnych gradacji, hurt i detal to nazwy grup klienta lub symbole klienów (w zależności od ustawienia TypDlaGradacji), następnie ilość produktów oraz rabat (w przykładzie separatorem jest =). Inne przykładowe cechy dla gradacji: <br/>" +
                       "10=15%;14=50 <br/> 12=5 <br/> test_55=23% <br/> Wartość rabatu można podać z znakiem %, co oznacza zniżkę procentową.")]
    public class Gradacje : SyncModul, IModulRabaty, ITestowalna
    {
        public Gradacje()
        {
            SeparatorKolejnychGradacji = ";";
            TrybDzialania = Tryb.Automatyczny;
            TypDlaGradacji = GradacjaDlaTypu.Wszyscy;
            SeparatorWartosci = "=";
            SeparatorGrupy = "_";
        }

        [FriendlyName("Atrybuty gradacji")]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<int> AtrybutGradacji { get; set; }

        [FriendlyName("Jeśli będziesz podawał kilka gradacji równocześnie, podaj separator oddzielający kolejne gradacje np.: 12=33;23=56;999=30 gdzie separatorem jest znak ;")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string SeparatorKolejnychGradacji { get; set; }

        [Niewymagane]
        [FriendlyName("Separator oddzielający grupę klienta lub symbol klienta od wartości rabatu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string SeparatorGrupy { get; set; }

        [FriendlyName("Separator ilości produktów od wartości rabatu (np. dla 12=33;23=56;999=30 takim separatorem jest znak =)")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string SeparatorWartosci { get; set; }

        [FriendlyName("Czy cecha produktu ma wartość, procent czy automatycznie rozpoznaje typ po znaku %")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public Tryb TrybDzialania { get; set; }

        [FriendlyName("Ustawia typ dla którego jest tworzona gradacja. Dostępne są gradacje dla klientów (po symbolu), grupy klientów i wszystkich klientów")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public GradacjaDlaTypu TypDlaGradacji { get; set; }

        //[Niewymagane]
        //[FriendlyName("Symbol waluty dla danej gradacji. To NIE jest nazwa poziomu cenowego - należy wpisać walutę w postaci np. PLN, EUR itd.")]
        //[Obsolete("Pole wycofane - skorzystaj z pola ListaRozwijanaWalut")]
        //public string SymbolWaluty { get; set; }

        //[Niewymagane]
        //[FriendlyName("Opcjonalny parametr, który określa grupę klientów, dla której będą dodane wszystkie gradacje.")]
        //[Obsolete("Pole wycofane - skorzystaj z pola ListaRozwijanaGrupKlienta")]
        //public string GrupaKlientow { get; set; }

        [Niewymagane]
        [FriendlyName("Symbol waluty dla danej gradacji (jeśli brak wyboru - gradacja działa dla wszystkich walut, co ma sens tylko przy gradacjach procentowych)")]
        [PobieranieSlownika(typeof(SlownikWalut))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string ListaRozwijanaWalut { get; set; }
        
        [Niewymagane]
        [FriendlyName("Lista kategorii klientów których gradacja będzie dotyczyć - na sztywno (jeśli nie ma podanej grupy dynamicznie w definicji gradacji)")]
        [WidoczneListaAdmin(false, false, true, false)]
        [PobieranieSlownika(typeof(SlownikKategoriiKlienta))]
        public List<string> ListaRozwijanaGrupKlienta { get; set; }

        private List<int> ListaIdGrupKlienta
        {
            get
            {
                return ListaRozwijanaGrupKlienta != null && ListaRozwijanaGrupKlienta.Any()
                    ? ListaRozwijanaGrupKlienta.Select(int.Parse).ToList()
                    : null;
            }
        } 

        public enum Tryb
        {
            Wartosc, Procent, Automatyczny
        }

        public enum GradacjaDlaTypu
        {
            Klienci, GrupyKlientow, Wszyscy
        }
        
        public override string uwagi => "";

        public void Przetworz(ref List<Rabat> rabatyNaB2B, ref List<ProduktUkryty> produktyUkryteNaB2B, ref Dictionary<long,Konfekcje> konfekcjaNaB2B, IDictionary<long, Klient> kliencib2B, 
            Dictionary<long, Produkt> produkty, List<PoziomCenowy> c, List<Cecha> cechy, Dictionary<long, ProduktCecha> cp, Dictionary<long, KategoriaProduktu> kategorie, 
            List<ProduktKategoria> produktyKategorie, ref IDictionary<int, KategoriaKlienta> kategorieKlientow, ref IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            string separator = SyncManager.PobierzInstancje.Konfiguracja.SeparatorAtrybutowWCechach[0].ToString(CultureInfo.InvariantCulture);

            List<Atrybut> atrybutyB2B = ApiWywolanie.PobierzAtrybuty().Values.ToList();

            foreach (KategoriaKlienta katkl in kategorieKlientow.Values)
            {
                katkl.Nazwa = katkl.Nazwa.ToLower();
            }
            foreach (Atrybut atrybuty1 in atrybutyB2B)
            {
                atrybuty1.Nazwa = atrybuty1.Nazwa.ToLower();
            }
            
            //int? idKategoriiKlientow = null;
            if (ListaIdGrupKlienta != null && ListaIdGrupKlienta.Any() && ListaIdGrupKlienta[0] != 0)
            {
                foreach (var idKategorii in ListaIdGrupKlienta)
                {
                    KategoriaKlienta kk = new KategoriaKlienta();
                    kk = kategorieKlientow.FirstOrDefault(x => x.Key == idKategorii).Value;
                    if (kk == null)
                    {
                        LogiFormatki.PobierzInstancje.LogujError(new Exception(string.Format("Nie znaleziono kategorii klientów o id: {0}. Moduł o id: {1} zakończy działanie.", idKategorii, this.Id)));
                        return;
                    }
                }
            }
            
            foreach (int atrybutgradacji in AtrybutGradacji)
            {
                var atrybut = atrybutyB2B.FirstOrDefault(a => a.Id == atrybutgradacji);
                if (atrybut == null)
                    continue;

                Cecha ostatniaCecha = null;
                var cechyGradacji = cechy.Where(a => a.AtrybutId == atrybut.Id);
                try
                {
                    foreach (Cecha cecha in cechyGradacji)
                    {
                        ostatniaCecha = cecha;
                        List<Konfekcje> konfekcja = PrzetworzCeche(cecha, separator, ref kategorieKlientow, atrybut.Nazwa, kliencib2B);
                        if (konfekcja != null && konfekcja.Count > 0)
                        {
                            foreach (Konfekcje konfekcje in konfekcja)
                            {
                                if (ListaIdGrupKlienta != null && ListaIdGrupKlienta.Any())
                                {
                                    foreach (var idGrupy in ListaIdGrupKlienta)
                                    {
                                        if (idGrupy != 0)
                                            konfekcje.KategoriaKlientowId = idGrupy;
                                        if (!konfekcjaNaB2B.ContainsKey(konfekcje.Id))
                                        {
                                            konfekcjaNaB2B.Add(konfekcje.Id, konfekcje);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!konfekcjaNaB2B.ContainsKey(konfekcje.Id))
                                    {
                                        konfekcjaNaB2B.Add(konfekcje.Id, konfekcje);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ostatniaCecha == null)
                    {
                        throw new Exception(ex.Message);
                    }
                    throw new Exception($"Wystąpił problem z gradacją z cechy: {ostatniaCecha.Nazwa} o id {ostatniaCecha.Id}, sprawdz czy cecha jest poprawna.Błąd: {ex.Message}");
                }
            }
        }

        //private void SprawdzenieKonfekcji(Konfekcje konfekcje, ref Dictionary<long, Konfekcje> konfekcjeklucprop,ref List<Konfekcje> konfekcjaNaB2B)
        //{
        //    var waluty = ApiWywolanie.PobierzWaluty();
        //    if (!konfekcjeklucprop.ContainsKey(konfekcje.Id))
        //    {
        //        konfekcjeklucprop.Add(konfekcje.Id, konfekcje);
        //        konfekcjaNaB2B.Add(konfekcje);
        //    }
        //    else
        //    {
        //        LogiFormatki.PobierzInstancje.LogujDebug(string.Format("Znaleziono zdublowaną konfekcje o id:{0}.", konfekcje.Id));
        //        LogiFormatki.PobierzInstancje.LogujDebug( string.Format(
        //            "dublowana id: {0}, kategoria klientów: {1}, produkt: {2}, klient: {3}, waluta: {4}, ilość: {5}, rabat: {6}, kwota rabatu: {7}",
        //                konfekcjeklucprop[konfekcje.Id].Id,
        //                konfekcjeklucprop[konfekcje.Id].KategoriaKlientowId,
        //                konfekcjeklucprop[konfekcje.Id].ProduktId,
        //                konfekcjeklucprop[konfekcje.Id].KlientId, waluty[konfekcjeklucprop[konfekcje.Id].WalutaId].WalutaB2b,
        //                konfekcjeklucprop[konfekcje.Id].Ilosc, konfekcjeklucprop[konfekcje.Id].Rabat,
        //                konfekcjeklucprop[konfekcje.Id].RabatKwota));
        //        LogiFormatki.PobierzInstancje.LogujDebug(
        //            string.Format(
        //                "Dublucjaca id: {0}, kategoria klientów: {1}, produkt: {2}, klient: {3}, waluta: {4}, ilość: {5}, rabat: {6}, kwota rabatu: {7}",
        //                konfekcje.Id, konfekcje.KategoriaKlientowId, konfekcje.ProduktId,
        //                konfekcje.KlientId, waluty[konfekcje.WalutaId].WalutaB2b, konfekcje.Ilosc,
        //                konfekcje.Rabat, konfekcje.RabatKwota));
        //    }
        //}

       
        public KategoriaKlienta ZnajdzKategorie(string nazwa, ref IDictionary<int, KategoriaKlienta> listakategoriiklientow)
        {
            string grupa = "";
            string cecha = "";
            
            string[] poSplitowaniu = nazwa.ToUpper().Split(new[] {":"}, StringSplitOptions.RemoveEmptyEntries);

            
            
            if (poSplitowaniu.Length == 2)
            {
                grupa = poSplitowaniu.First();
                cecha = poSplitowaniu.Last();
            }

            else if (poSplitowaniu.Length == 1)
            {
                cecha = poSplitowaniu.First();
            }

            return listakategoriiklientow.Values.FirstOrDefault(a => ((!string.IsNullOrEmpty(grupa) && !string.IsNullOrEmpty(a.Grupa) && a.Grupa.ToUpper() == grupa) || string.IsNullOrEmpty(grupa)) && ((!string.IsNullOrEmpty(cecha) && a.Nazwa.ToUpper() == cecha) || string.IsNullOrEmpty(cecha)));
        }

        public List<Konfekcje> PrzetworzCeche(Cecha cecha, string separatorcechy, ref IDictionary<int, KategoriaKlienta> kategorie, string atrybutgradacji, IDictionary<long, Klient> klienci)
        {
            List<Konfekcje> listaKonfekcji = new List<Konfekcje>();

            string[] gradacje = cecha.Symbol.ToLower().Replace(atrybutgradacji + separatorcechy, "").Split(new[] {SeparatorKolejnychGradacji}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string pojedynczaGradacja in gradacje)
            {
                //0 - grupa klientów, 1 - wartości
                string[] dane = {pojedynczaGradacja};
                if (!string.IsNullOrEmpty(SeparatorGrupy))
                    dane = pojedynczaGradacja.Split(new[] {SeparatorGrupy}, StringSplitOptions.RemoveEmptyEntries);


                List<string> gradacja;

                if (dane.Length >= 2)
                    gradacja = dane[1].Split(new[] {SeparatorWartosci}, StringSplitOptions.RemoveEmptyEntries).ToList();
                else
                    gradacja = dane[0].Split(new[] {SeparatorWartosci}, StringSplitOptions.RemoveEmptyEntries).ToList();
                Tryb wykrytyTryb;
                if (CzyCechaPasujeDoTrybu(gradacja[1], out wykrytyTryb))
                {
                    decimal ilosc;
                    if (TextHelper.PobierzInstancje.SprobojSparsowac(gradacja[0], out ilosc))
                    {
                        Konfekcje k = new Konfekcje {WyliczonePrzezModul = this.Id};
                        if (dane.Length >= 2)
                        {
                            switch (TypDlaGradacji)
                            {
                                case GradacjaDlaTypu.GrupyKlientow:
                                {
                                    KategoriaKlienta kk = ZnajdzKategorie(dane[0], ref kategorie);
                                    if (kk != null)
                                    {
                                        k.KategoriaKlientowId = kk.Id;
                                    }
                                }
                                    break;

                                case GradacjaDlaTypu.Klienci:
                                {
                                    Klient klient = klienci.Values.FirstOrDefault(a => string.Equals(a.Symbol, dane[0], StringComparison.CurrentCultureIgnoreCase));
                                    if (klient != null)
                                    {
                                        k.KlientId = klient.Id;
                                    }
                                }
                                    break;
                            }
                        }

                        k.CechaId = cecha.Id;
                        Log.InfoFormat($"id: {ListaRozwijanaWalut}");
                        k.WalutaId = long.Parse(ListaRozwijanaWalut);
                        decimal wartosc;
                        switch (wykrytyTryb)
                        {
                            case Tryb.Procent:
                            {
                                if (TextHelper.PobierzInstancje.SprobojSparsowac(gradacja[1].Replace("%", ""), out wartosc))
                                {
                                    k.Rabat = wartosc;
                                }
                            }
                                break;

                            case Tryb.Wartosc:
                            {
                                if (TextHelper.PobierzInstancje.SprobojSparsowac(gradacja[1].Replace("%", ""), out wartosc))
                                {
                                    k.RabatKwota = wartosc;
                                }
                            }
                                break;
                        }

                        k.Ilosc = ilosc;

                        if (k.Rabat == null && k.RabatKwota == null)
                            LogiFormatki.PobierzInstancje.LogujError(new Exception($" Powód: Nie udało się pobrać rabatu z cechy o symbolu {cecha.Symbol}, gradacja: {pojedynczaGradacja} ."));

                        else listaKonfekcji.Add(k);
                    }
                }
            }

            return listaKonfekcji;
        }

        private bool CzyCechaPasujeDoTrybu(string dane, out Tryb tryb)
        {
            if (dane.EndsWith("%"))
                tryb = Tryb.Procent;
            else tryb = Tryb.Wartosc;

            return TrybDzialania == tryb || TrybDzialania == Tryb.Automatyczny;
        }

        public List<string> TestPoprawnosci()
        {
            List<string>bledy = new List<string>();
            var kategorieKlientow = new HashSet<int>( SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategoriaKlienta>(null).Select(x => x.Id) );
            if (ListaIdGrupKlienta == null || !ListaIdGrupKlienta.Any())
            {
                bledy.Add("Nie wybrane kategorie klientów");
            }
            else
            {
                var listaZbednych = ListaIdGrupKlienta.Except(kategorieKlientow).ToList();
                if (listaZbednych.Any())
                {
                    bledy.Add($"Brak kategori klientów o id: {listaZbednych.ToCsv()}");
                }
            }
            return bledy;
        }
    }
}
