using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using ServiceStack.Common;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class PrzypisanieCechyNapodstaiwieCechyPlikBioplanet : SyncModul, IModulProdukty, IModulCechyIAtrybuty
    {
        public override string uwagi
        {
            get { return ""; }
        }

        [FriendlyName("Nazwa atrybutu, do którego będą dodawana nowe cechy, jeśli nie istniej to zostanie stworzony")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Atrybut { get; set; }
       
        [FriendlyName("Nazwa kolumny zawierająca nazwę nowej cechy")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NowaCechaKolumna { get; set; }
        
        [FriendlyName("Nazwa kolumny zawierająca id istniejącej cechy")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string IstniejacaCechaKolumna { get; set; }

        [FriendlyName("Sciezka do pliku z danymi")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Sciezka { get; set; }
        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
       {
           var atrybut = ZrobAtrybut();
           var streamrd = StrumienDanych();

           var lacznikipoprodukcie = lacznikiCech.Values.GroupBy(x => x.ProduktId).ToDictionary(x => x.Key, x => new HashSet<long>( x.Select(y=>y.CechaId) ));
           if (streamrd == null)
           {
               LogiFormatki.PobierzInstancje.LogujInfo("Brak pliku lub odmowa dostępu do {0}, moduł kończy działanie", Sciezka);
               return;
           }
           CsvReader r = new CsvReader(streamrd);
           r.Configuration.Delimiter = ";";
           r.Configuration.Encoding = Encoding.Default;
           r.Configuration.TrimFields = true;
           r.Configuration.HasHeaderRecord = true;
            r.Read();
            r.ReadHeader();
           while (r.Read())
           {
               string nowa = r[NowaCechaKolumna];
               string istniejaca = r[IstniejacaCechaKolumna];
               int idist;
               if (!int.TryParse(istniejaca, out idist) || idist == 0)
               {
                   LogiFormatki.PobierzInstancje.LogujInfo("Nie udało się przetrozyc {0} na id cechy, wiersz jest pomijany", istniejaca);
                   continue;
               }
               if (!string.IsNullOrEmpty(nowa))
               {
                   var cecha = ZrobCeche(nowa, atrybut.Id);
                   foreach (Produkt p in listaWejsciowa)
                   {
                       if (!lacznikipoprodukcie.ContainsKey(p.Id))
                       {
                           continue;//brak łaczników dla tego produktu, czyli na pewno nie będzie miał cech
                       }
                       if (lacznikipoprodukcie[p.Id].Contains(idist))//dany produkt ma cechę wymaganą
                       {
                           if (!lacznikipoprodukcie[p.Id].Contains(cecha.Id))//ale nie ma nowej cechy
                           {
                               var cp = new ProduktCecha {CechaId = cecha.Id, ProduktId = p.Id};
                               lacznikiCech.Add(cp.Id, cp);
                               lacznikipoprodukcie[p.Id].Add(cecha.Id);
                           }
                       }
                   }
               }
           }
       }

        //private List<cechy> _Cechy;

        //public virtual List<cechy> CechyB2B
        //{
        //    get
        //    {
        //        if (_Cechy == null)
        //        {
       //            _Cechy = ApiWywolanie.PobierzCechy(new CechySearchCriteria()).Values.ToList();
        //        }
        //        return _Cechy;
        //    }
        //}

        private Atrybut ZrobAtrybut()
        {
            Atrybut atrybut = new Atrybut(Atrybut);
            atrybut.Id = atrybut.WygenerujIDObiektu();
            return atrybut;
        }

        private Cecha ZrobCeche(string nazwa, int atrybutid)
        {
            string symbol = Atrybut + ":" + nazwa;
            var cecha = new Cecha(nazwa, symbol);
            cecha.AtrybutId = atrybutid;
            cecha.Id = cecha.WygenerujIDObiektu();
            return cecha;
        }

        public virtual StreamReader StrumienDanych()
        {
            if (!File.Exists(Sciezka))
            {
             
                return null;
            }
            var fsstr = new FileStream(Sciezka, FileMode.Open, FileAccess.Read, FileShare.Read);
            return new StreamReader(fsstr, Encoding.Default);
        }
      
       public void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
       {
           var atrybut = ZrobAtrybut();
           if (atrybuty.All(x => x.Id != atrybut.Id))
           {
               atrybuty.Add(atrybut);
           }
           var streamrd = StrumienDanych();
           if (streamrd == null)
           {
               LogiFormatki.PobierzInstancje.LogujInfo("Brak pliku lub odmowa dostępu do {0}, moduł kończy działanie", Sciezka);
               return;
           }
           CsvReader r = new CsvReader(streamrd);
           r.Configuration.Delimiter = ";";
           r.Configuration.Encoding = Encoding.Default;
           r.Configuration.TrimFields = true;
           r.Configuration.HasHeaderRecord = true;
           r.Read();
           r.ReadHeader();
            Log.DebugFormat("nagłówki csv: {0}", string.Join(", ", r.FieldHeaders));
            while (r.Read())
           {
               string nowa = r[NowaCechaKolumna];
               if (!string.IsNullOrEmpty(nowa))
               {
                   var cecha = ZrobCeche(nowa, atrybut.Id);
                   if (cechy.All(x => x.Id != cecha.Id))
                   {
                       cechy.Add(cecha);
                   }
               }
           }
       }
    }
}
