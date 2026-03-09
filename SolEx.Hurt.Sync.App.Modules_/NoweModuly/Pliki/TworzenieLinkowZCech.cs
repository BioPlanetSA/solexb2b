using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Common;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki
{
    [FriendlyName("Tworzenie linków z cech",FriendlyOpis = "Wczytywanie dodatkowych materiałów multimedialnych do produktów z atrybutów")]
    public class TworzenieLinkowZCech: SyncModul, IModulPliki
    {
        [FriendlyName("Lista atrybutów określajace cechy zawierające linki do materiałów dodatkowych")]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> ListaAtrybutow { get; set; }

        [FriendlyName("Sposób otwierania linku z materiałami dodatkowymi")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public SposobOtwierania SposobOtwieraniaLinku { get; set; }

        [FriendlyName("Rozmiar okna - wysokość")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int Wysokosc { get; set; }

        [FriendlyName("Rozmiar okna - szerokość")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int Szerokosc { get; set; }

        [FriendlyName("Klasa css")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Klasa { get; set; }
        
        [Niewymagane]
        [FriendlyName("Obrazek reprezentujący zawartość ")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Obrazek { get; set; }

        [FriendlyName("<b>Formatuj wartość z atrybutu wg. wzorca</b><br />(wzór {0} odpowiada wartości z cechy)")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Wzor { get; set; }

        public TworzenieLinkowZCech()
        {
            Wysokosc = 600;
            Szerokosc = 800;
            SposobOtwieraniaLinku = SposobOtwierania.NoweOkno;
        }

        public List<int> ListaIdAtrybutow
        {
            get { return ListaAtrybutow.Select(int.Parse).ToList(); }
        }

        public void Przetworz(IDictionary<long, Produkt> produktyNaB2B, ref List<ProduktPlik> plikiLokalnePowiazania, ref List<Plik> plikiLokalne, Model.Interfaces.Sync.ISyncProvider provider, ref List<Cecha> cechyB2B, ref List<KategoriaProduktu> kategorieB2B, ref List<Klient> klienciB2B)
        {
            int licznikMinimalny = -10000;
            if (plikiLokalne.Count > 0)
            {
                licznikMinimalny = plikiLokalne.Min(b => b.Id) - 1;
                if (licznikMinimalny >= 0)
                {
                    licznikMinimalny = -10000;
                }
            }
            var cechyZAtrybutem = cechyB2B.Where(x=>x.AtrybutId.HasValue && ListaIdAtrybutow.Contains(x.AtrybutId.Value));
            List<Cecha> cechyZPoprawnymiLinkami = new List<Cecha>();
            if (string.IsNullOrEmpty(Wzor))
            {
                cechyZPoprawnymiLinkami = cechyZAtrybutem.Where(x => PoprawnyLink(x.Nazwa)).ToList();
            }
            else
            {
                cechyZPoprawnymiLinkami = cechyZAtrybutem.ToList();
            }
             
            if (!cechyZPoprawnymiLinkami.Any())
            {
                throw new Exception("Brak cechy z poprawnym formatem linków");
            }
            HashSet<long> idCechZPoprawnymiLinkami = new HashSet<long>( cechyZPoprawnymiLinkami.Select(x => x.Id) );
            var lacznikiCech = ApiWywolanie.PobierzCechyProdukty(idCechZPoprawnymiLinkami).Values.ToList();

            foreach (var cecha in cechyZPoprawnymiLinkami)
            {
                string nazwa = string.IsNullOrEmpty(Wzor) ? Path.GetFileName(cecha.Nazwa) ?? "" : Path.GetFileName(string.Format(Wzor,cecha.Nazwa));
                var plik = new Plik
                {
                    Nazwa = nazwa,
                    Sciezka = string.IsNullOrEmpty(Wzor) ? cecha.Nazwa.Replace(nazwa, "") : string.Format(Wzor, cecha.Nazwa).Replace(nazwa, ""),//cecha.nazwa.Replace(nazwa,""),
                    Data = new DateTime(1900,1,1),
                    RodzajPliku = RodzajPliku.Link,
                    HtmlPrzycisku = Obrazek,
                    SposobOtwierania = SposobOtwieraniaLinku,
                    SzerokoscOkna = Szerokosc,
                    WysokoscOkna = Wysokosc,
                    KlasaCss = Klasa
                };
                plik.Id = licznikMinimalny;
                licznikMinimalny--;

                plikiLokalne.Add(plik);

                var idProduktowZCecha = lacznikiCech.Where(x => x.CechaId == cecha.Id).Select(x => x.ProduktId).ToList();
                foreach (var produkt in idProduktowZCecha)
                {
                    ProduktPlik pp = new ProduktPlik {PlikId = plik.Id, ProduktId = produkt};
                   
                    plikiLokalnePowiazania.Add(pp);
                }
            }
        }

        private bool PoprawnyLink(string link)
        {
            return link.StartsWith("http:",StringComparison.InvariantCultureIgnoreCase) || link.StartsWith("https:",StringComparison.InvariantCultureIgnoreCase) || link.StartsWith("ftp:",StringComparison.InvariantCultureIgnoreCase);
        }

        public override string uwagi
        {
            get { return ""; }
        }
        public override string Opis
        {
            get { return "Wczytywanie dodatkowych materiałów multimedialnych do produktów z atrybutów"; }
        }
    }
    
}

