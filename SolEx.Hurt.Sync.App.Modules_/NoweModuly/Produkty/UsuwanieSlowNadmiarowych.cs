using System.Reflection;
using System.Text.RegularExpressions;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.Interfaces.Sync;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{

    [ModulStandardowy]
    [FriendlyName("Usuń wybrane słowa z pola produktu",FriendlyOpis = "Usuwa wybrane słowa z pola produktu. Moduł przydatny żeby np. usuwać rozmiar z nazwy rodziny")]
    public class UsuwanieSlowNadmiarowych : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UsuwanieSlowNadmiarowych()
        {
            DoUsuniecia = "xxs,xs,s,m,l,xl,xxl,xxxl";
            Pola = new List<string>() {"Rodzina"};
        }

        public override string uwagi
        {
            get { return ""; }
        }

        [FriendlyName("Słowa pomijane przy pobieraniu wybranego pola", FriendlyOpis = "Słowa ktore będą pomijane przy pobieraniu wybranego pola. Wielkość liter nie ma znaczenia, tylko pełne słowa będą pomijane.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string DoUsuniecia { get; set; }


        [FriendlyName("Lista pól, które mają zostać oczyszczone")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Pola { get; set; }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki,
            ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp,
            ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            PrzygotujDane();
            //filtrujemy i grupujemy tłumaczenia dla pól produktów 
            var tlumaczenia = produktyTlumaczenia.Where(x=>Pola.Contains(x.Pole) && x.JezykId!=SyncManager.PobierzInstancje.Konfiguracja.JezykIDPolski);
            Dictionary<long,HashSet<Tlumaczenie>> slownikTlumaczen = new Dictionary<long, HashSet<Tlumaczenie>>();
            foreach (Tlumaczenie tlumaczenie in tlumaczenia)
            {
                HashSet<Tlumaczenie> jezykTlumaczenia;
                if (slownikTlumaczen.TryGetValue(tlumaczenie.ObiektId, out jezykTlumaczenia))
                {
                   jezykTlumaczenia.Add(tlumaczenie);
                }
                else
                {
                    slownikTlumaczen.Add(tlumaczenie.ObiektId, new HashSet<Tlumaczenie>(){ { tlumaczenie} });
                }
            }

            List<PropertyInfo> propertisy = typeof(Produkt).GetProperties().Where(x => Pola.Contains(x.Name)).ToList();
            foreach (var produkty in listaWejsciowa)
            {
                HashSet<Tlumaczenie> tlumaczeniaProduktu;
                slownikTlumaczen.TryGetValue(produkty.Id, out tlumaczeniaProduktu);
                foreach (var p in propertisy)
                {
                    try
                    {
                        var wartosc = p.GetValue(produkty);
                        if(wartosc == null) continue;
                        if(string.IsNullOrEmpty(wartosc.ToString())) continue;
                        p.SetValue(produkty, Oczysc(wartosc.ToString()));
                        
                        //usuwamy słowa z tłumaczeń dla pola
                        if (tlumaczeniaProduktu == null || !tlumaczeniaProduktu.Any()) continue;
                        foreach (var tlumaczenie in tlumaczeniaProduktu)
                        {
                            if (tlumaczenie == null) continue;
                            string stary = tlumaczenie.Wpis;
                            tlumaczenie.Wpis = Oczysc(stary);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(new Exception($"Wystąpił błąd przy przetwarzaniu pola {p.Name} dla towaru: {produkty.Kod}: {ex.Message}{ex} "));
                    }
                }
            }
        }

        private Dictionary<Regex,string> _regex= new Dictionary<Regex, string>();
        private void PrzygotujDane()
        {
            string[] doUsuniecia = DoUsuniecia.Split(",;".ToCharArray());
            foreach (string s in doUsuniecia)
            {
                _regex.Add(new Regex(@"(\s" + s + @")$", RegexOptions.IgnoreCase), "");
                _regex.Add(new Regex(@"^(" + s + @"\s)", RegexOptions.IgnoreCase), "");
                _regex.Add(new Regex(@"(\s" + s + @"\s)", RegexOptions.IgnoreCase)," ");
            }
        }
        private string Oczysc(string tekst)
        {
           // string old = tekst;
            foreach (var reg in _regex)
            {
               tekst = reg.Key.Replace(tekst, reg.Value);
            }
            //if(!old.Equals(tekst) && Log.IsDebugEnabled) Log.DebugFormat("UsuwanieSlowNadmiarowych: Przed: {0}, po: {1}", old, tekst);
            return tekst;
        }
    }
}
