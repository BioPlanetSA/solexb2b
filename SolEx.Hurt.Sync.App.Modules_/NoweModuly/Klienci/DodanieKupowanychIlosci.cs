using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CsvHelper;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    [Obsolete]
    [FriendlyName("Dodanie kupowanych ilosci", FriendlyOpis = "Dodawane kupowane ilości klientów, jeśli wybrano konkretną datę należy wprowadzić w formacie dd.MM.yyyy")]
    public class DodanieKupowanychIlosci : PrzeliczenieIlosci
    {
        [FriendlyName("Nazwa kolumny zawierająca identfikator towaru")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaKolumnyIdentyfikator { get; set; }
        
        [FriendlyName("Nazwa kolumny zawierająca identfikator klienta")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaKolumnyKlient { get; set; }
        
        [FriendlyName("Nazwa kolumny zawierająca dodatkową ilość")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaKolumnyIlosc { get; set; }
        
        [FriendlyName("Pole po którym mapujemy produkty")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PolaProdukt { get; set; }
        
        [FriendlyName("Pole po którym mapujemy klientów")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PolaKlient { get; set; }

        protected override void DodajDane(DateTime? odkiedy, DateTime? dokiedy, Dictionary<long, KupowaneIlosci> ilosciKlientow, Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B)
        {
            Wykonaj(odkiedy, dokiedy, ilosciKlientow, listaWejsciowa, produktyB2B);
        }

        public void Wykonaj(DateTime? odkiedy, DateTime? dokiedy, Dictionary<long, KupowaneIlosci> ilosciKlientow, Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B)
        {
            //wywalamy tabele kupowane ilosci - trzeba jesli juz robic zamówienia sztuczne
            throw new NotImplementedException();
            //PropertyInfo[] propertisy = typeof(Produkt).GetProperties();
            //PropertyInfo piprodukt = propertisy.First(x => x.Name == PolaProdukt);
            //PropertyInfo[] propertisykl = typeof(Klient).GetProperties();
            //PropertyInfo piklienci = propertisykl.First(x => x.Name == PolaKlient);
            //var streamrd = StrumienDanych();
            //if (streamrd == null)
            //{
            //    LogiFormatki.PobierzInstancje.LogujInfo("Brak pliku lub odmowa dostępu do {0}, moduł kończy działanie", SciezkaDoCSVZDodatkowymiIlosciami);
            //    return;
            //}
            //CsvReader r = new CsvReader(streamrd);
            //r.Configuration.Delimiter = ";";
            //r.Configuration.Encoding = Encoding.Default;
            //r.Configuration.TrimFields = true;
            //r.Configuration.HasHeaderRecord = true;
            //int i = 1;
            //while (r.Read())
            //{
            //    i++;
            //    string produkt = r[NazwaKolumnyIdentyfikator];
            //    string klient = r[NazwaKolumnyKlient];
            //    string iloscstr = r[NazwaKolumnyIlosc];
            //    if (string.IsNullOrEmpty(produkt))
            //    {
            //        LogiFormatki.PobierzInstancje.LogujInfo("Pomijam wiersz {0}, kolumna z produktem jest pusta", i);
            //        continue;
            //    }
            //    var produkkobj = produktyB2B.Values.FirstOrDefault(x => ((string)piprodukt.Get(x)).Equals(produkt,StringComparison.InvariantCultureIgnoreCase));
            //    if (produkkobj == null)
            //    {
            //        LogiFormatki.PobierzInstancje.LogujInfo("Pomijam wiersz {0}, nie znaleziono produktu identyfikowanego przez {1} w polu {2}", i, produkt, PolaProdukt);
            //        continue;
            //    }
            //    if (string.IsNullOrEmpty(klient))
            //    {
            //        LogiFormatki.PobierzInstancje.LogujInfo("Pomijam wiersz {0}, kolumna z klientem jest pusta", i);
            //        continue;
            //    }
            //    var klientobj = listaWejsciowa.Values.FirstOrDefault(x => ((string)piklienci.Get(x)).Equals(klient, StringComparison.InvariantCultureIgnoreCase));
            //    if (klientobj == null)
            //    {
            //        LogiFormatki.PobierzInstancje.LogujInfo("Pomijam wiersz {0}, nie znaleziono klienta identyfikowanego przez {1} w polu {2}", i, klient, PolaKlient);
            //        continue;
            //    }
            //    decimal ilosc;
            //    if (!TextHelper.PobierzInstancje.SprobojSparsowac(iloscstr, out ilosc))
            //    {
            //        LogiFormatki.PobierzInstancje.LogujInfo("Pomijam wiersz {0}, ilość jest w niepoprawnym formacie", i);
            //        continue;
            //    }
            //    DodajWpis(odkiedy, dokiedy, ilosc, produkkobj.Id, klientobj.Id, ilosciKlientow);
               
            //}  
        }
        public virtual StreamReader StrumienDanych()
        {
            if (!File.Exists(SciezkaDoCSVZDodatkowymiIlosciami))
            {

                return null;
            }
            var fsstr = new FileStream(SciezkaDoCSVZDodatkowymiIlosciami, FileMode.Open, FileAccess.Read, FileShare.Read);
            return new StreamReader(fsstr, Encoding.Default);
        }
    }
}
