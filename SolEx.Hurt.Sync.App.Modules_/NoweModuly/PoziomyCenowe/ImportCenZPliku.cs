using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CsvHelper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.SyncModuly;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.PoziomyCenowe
{
    public enum RodzajCeny
    {
        Netto, Brutto
    }
    public class ImportCenZPliku : SyncModul, IModulPoziomyCen
    {
        private List<Produkt> _Cechy;

        public virtual List<Produkt> Produkty
        {
            get
            {
                if (_Cechy == null)
                {
                    _Cechy = ApiWywolanie.PobierzProdukty().Values.ToList();
                }
                return _Cechy;
            }
        }
        public void Przetworz(ref Dictionary<int, PoziomCenowy> listaPoziomowCen, ref List<CenaPoziomu> ceny, Dictionary<int, PoziomCenowy> poziomyNaB2B, Dictionary<long, CenaPoziomu> cenyPoziomyB2B)
        {
            PoziomCenowy pc = poziomyNaB2B.Values.FirstOrDefault(x => x.Nazwa == NazwaPoziomuCenowego);
            if (pc == null)
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Dodawanie poziomu cenowego o nazwie: {0} z pliku", NazwaPoziomuCenowego);
                pc=new PoziomCenowy();
                pc.Nazwa = NazwaPoziomuCenowego;
                pc.WalutaId = WalutaPoziomuCenowego.ToLower().WygenerujIDObiektuSHAWersjaLong();
                pc.Id = pc.WygenerujIDObiektu();
            }
            listaPoziomowCen.Add(pc.Id, pc);

            PropertyInfo[] propertisy = typeof(Produkt).GetProperties();
            PropertyInfo pi = propertisy.First(x => x.Name == Pola);
            var streamrd = StrumienDanych();
            if (streamrd == null)
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Brak pliku lub odmowa dostępu do {0}, moduł kończy działanie", Sciezka);
                return;
            }
            LogiFormatki.PobierzInstancje.LogujInfo("Odczytano poprawne plik: {0}", Sciezka);
            CsvReader r = new CsvReader(streamrd);
            r.Configuration.Delimiter = ";";
            r.Configuration.Encoding = Encoding.Default;
            r.Configuration.TrimFields = true;
            r.Configuration.HasHeaderRecord = true;
            r.Read();
            r.ReadHeader();
            int i = 1;
            while (r.Read())
            {
                string produkt = r[NazwaKulumnyIdentyfikator];
                string cenastr = r[NazwaKulumnyCena];
                if (string.IsNullOrEmpty(produkt))
                {
                    LogiFormatki.PobierzInstancje.LogujInfo("Pomijam wiersz {0}, kolumna z produktem jest pusta",i);
                    continue;
                }
                var produkkobj = Produkty.FirstOrDefault(x => (string) pi.GetValue(x) == produkt && x.Widoczny);  //todo: TU trzeb zrobić słownik z produktemai juz wyciagnieteymi a nie na biezaco wyciagac caly czas
                if (produkkobj == null)
                {
                    LogiFormatki.PobierzInstancje.LogujInfo("Pomijam wiersz {0}, nie znaleziono produktu identyfikowanego przez {1} w polu {2}", i, produkt, Pola);
                    continue;
                }
                decimal cena;

                if (!TextHelper.PobierzInstancje.SprobojSparsowac(cenastr, out cena))
                {
                    LogiFormatki.PobierzInstancje.LogujInfo("Pomijam wiersz {0}, cena jest w niepoprawnym formacie", i);
                    continue;
                }
                decimal cenanetto;
                if (RodzajCeny == RodzajCeny.Netto || produkkobj.Vat == 0)
                {
                    cenanetto = cena;
                }
                else
                {
                    cenanetto = decimal.Round(cena*100M/(produkkobj.Vat + 100M), 2);
                }
                CenaPoziomu cp = ceny.FirstOrDefault(x => x.ProduktId == produkkobj.Id && x.PoziomId == pc.Id);
                if (cp == null)
                {
                    cp=new CenaPoziomu();
                    cp.ProduktId = produkkobj.Id;
                    cp.PoziomId = pc.Id;
                    ceny.Add(cp);
                }
                cp.WalutaId = WalutaPoziomuCenowego.ToLower().WygenerujIDObiektuSHAWersjaLong();;
                cp.Netto = cenanetto;
                i++;
            }
        }
        [FriendlyName("Nazwa poziomu cenowego, jako jakie mają być zaimportowane ceny, jeśli nie istnieje to zostane stworzony")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaPoziomuCenowego { get; set; }
        
        [FriendlyName("Waluta poziomu cenowego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string WalutaPoziomuCenowego { get; set; }
        
        [FriendlyName("Pole po którym mapujemy produkty")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Pola { get; set; }
        
        [FriendlyName("Nazwa kolumny zawierająca identfikator towaru")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaKulumnyIdentyfikator { get; set; }
        
        [FriendlyName("Nazwa kolumny zawierająca cenę towaru")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaKulumnyCena { get; set; }
        
        [FriendlyName("Rodzaj ceny")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public RodzajCeny RodzajCeny { get; set; }
        
        [FriendlyName("Sciezka do pliku z danymi")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Sciezka { get; set; }
        
        public override string uwagi
        {
            get { return ""; }
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
    }
}
