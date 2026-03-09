using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CsvHelper;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki
{
    public class ImportPlikowNaPodstawieCsv : TrecZdjecia
    {
        public override string uwagi
        {
            get { return "Pobiera zdjęcia na podstawie pliku csv"; }
        }
        public virtual StreamReader StrumienDanych()
        {
            if (!File.Exists(SciezkaPliku))
            {

                return null;
            }
            var fsstr = new FileStream(SciezkaPliku, FileMode.Open, FileAccess.Read, FileShare.Read);
            return new StreamReader(fsstr, Encoding.Default);
        }
        [FriendlyName("Scieza do pliku z danymi")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string SciezkaPliku { get; set; }
        
        [FriendlyName("Kolumna z identyfikatorem produktu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string IdentyfikatorProdukt{ get; set; }
        
        [FriendlyName("Kolumna z url do pliku")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string IdentyfikatorPlik { get; set; }
        
        [FriendlyName("Pole produktu po którmy szukamy produkt")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoleProduktu { get; set; }

        public Dictionary<string, List<string>> ProduktSymbol()
        {
            var streamrd = StrumienDanych();
            Dictionary<string, List<string>> wynik = new Dictionary<string, List<string>>();
                  CsvReader r = new CsvReader(streamrd);
           r.Configuration.Delimiter = ";";
           r.Configuration.Encoding = Encoding.Default;
           r.Configuration.TrimFields = true;
           r.Configuration.HasHeaderRecord = true;
            while (r.Read())
            {
                string produkt = r[IdentyfikatorProdukt];
                string sciezka = r[IdentyfikatorPlik];
                if (!string.IsNullOrEmpty(produkt) && !string.IsNullOrEmpty(sciezka))
                {
                    var wiersze = sciezka.Split(new[] {separator}, StringSplitOptions.RemoveEmptyEntries);
                    if (!wynik.ContainsKey(produkt))
                    {
                        wynik.Add(produkt,new List<string>());
                    }
                    wynik[produkt].AddRange( wiersze);
                }
            }
            return wynik;
        }
        string separator =", ";
        public override void Przetworz(IDictionary<long, Produkt> produktyNaB2B, ref List<ProduktPlik> plikiLokalnePowiazania, ref List<Plik> plikiLokalne, ISyncProvider provider, ref List<Cecha> cechyB2B, ref  List<KategoriaProduktu> kategorieB2B, ref List<Klient> klienciB2B)
        {
            List<string> plikiDoKtorychSaZdjecia = new List<string>();
            Dictionary<string,List<Produkt>> mapowania=new Dictionary<string, List<Produkt>>();
            foreach (var produkt in produktyNaB2B.Values)
            {
                object nazwa = produkt.PobierzWartoscPolaObiektu(PoleProduktu);
                if (nazwa != null)
                {
                    string klucz = nazwa.ToString();
                    if (!string.IsNullOrEmpty(klucz))
                    {
                        if (!mapowania.ContainsKey(klucz))
                        {
                            mapowania.Add(klucz, new List<Produkt>());
                        }
                        mapowania[klucz].Add(produkt);
                    }
                }
            }
            var plik = ProduktSymbol();
            foreach (var p in mapowania)
            {
                if (plik.ContainsKey(p.Key))
                {
                    var fotki = plik[p.Key];
                    foreach (var produkt in p.Value)
                    {
                        PrzeworzZdjecie(produkt, fotki, plikiDoKtorychSaZdjecia);
                    }
                }
            }
            Usun(plikiDoKtorychSaZdjecia);
        }
    }
}
