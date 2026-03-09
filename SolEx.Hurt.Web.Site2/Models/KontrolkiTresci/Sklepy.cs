using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class Sklepy : KontrolkaTresciBaza
    {
        public Sklepy()
        {
            SklepyWysokosc = "600";
            SklepyKategoria = null;
            Powiekszenie = 10;
            PokazFiltrKategorii = true;
            MinimalnePowiekszenie = 5;
            MaksymalnePowiekszenie = 15;
            SklepyListaMiast = true;
            PokazujSklepy = RodzajSklepu.SklepyMapa;
            SklepySciezkaKLMId = null;
            RozmiarZdjecia="ico175x175wp";
            PokazujWszystkieSklepyNarazGdyBrakWyboruMiasta = true; //test bartek
        }
        [Lokalizowane]
        public override string Nazwa
        {
            get { return "Sklepy"; }
        }

        public override string Kontroler
        {
            get { return "Sklepy"; }
        }

        public override string Akcja
        {
            get { return "Mapa"; }
        }

        public override string Opis
        {
            get { return "Kontrolka wyświetlająca sklepy na mapie"; }
        }

        [FriendlyName("Wysokość")]
        [WidoczneListaAdmin(true,true,true,true)]
        [Wymagane]
        public string SklepyWysokosc { get; set; }
        
        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [PobieranieSlownika(typeof(SlownikKategoriiSklepow))]
        [FriendlyName("Kategorie sklepów z których pokazywać sklepy",FriendlyOpis = "Jeśli nic nie wybrane - pokazujemy sklepy z wszystkich kategorii")]
        public HashSet<long> SklepyKategoria { get; set; }

        [FriendlyName("Powiększenie startowe",FriendlyOpis = "Wartość domyślna: 10")]
        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Mapa", 5)]
        public int Powiekszenie { get; set; }

        [FriendlyName("Pokazuj filtr kategorii sklepów", FriendlyOpis = "Pokazywane są kategorie sklepów tylko te które mają zaznaczone 'Widoczna na mapie' - opcja jest aktywna tylko jeśli jest więcej niż jedna kategoria do pokazania")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool PokazFiltrKategorii { get; set; }

        [FriendlyName("Minimalne powiększenie",FriendlyOpis = "Wartość domyślna:5")]
        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Mapa", 5)]
        public int MinimalnePowiekszenie { get; set; }

        [FriendlyName("Maksymalne powiększenie",FriendlyOpis = "Wartość domyślna: 15")]
        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Mapa", 5)]
        public int MaksymalnePowiekszenie { get; set; }

        //[FriendlyName("Pokazuj tylko sklepy z poprawnymi koordynatami")]
        //[WidoczneListaAdmin(true, true, true, true)]
        //public bool SklepyZDobrymiKoordynatami { get; set; }

        [FriendlyName("Pokazuj listę miast do wyboru dla klienta")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool SklepyListaMiast { get; set; }

        [FriendlyName("Ścieżka do pliku KLM")]
        [WidoczneListaAdmin(true, true, true, true)]
        [Niewymagane]
        [WymuszonyTypEdytora(TypEdytora.PolePlik)]
        [WymuszoneRoszerzeniePliku("'kml'")]
        [GrupaAtttribute("Mapa", 5)]
        public int? SklepySciezkaKLMId { get; set; }

        [Ignore]
        public Plik SklepySciezkaKLM
        {
            get
            {
                if (this.SklepySciezkaKLMId != null)
                {
                    return SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Plik>(this.SklepySciezkaKLMId);
                }
                return null;
            }
        }
        [FriendlyName("Widok dla sklepów")]
        [WidoczneListaAdmin(true, true, true, true)]
        public RodzajSklepu PokazujSklepy { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Dodatkowe klasy css dla poszczególnych sklepów")]
        [Niewymagane]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKlassCss,SolEx.Hurt.Core")]
        public string[] DodatkoweKlasyCssElementyKontrolki { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Rozmiar kafelka")]
        [Niewymagane]
        [PobieranieSlownika(typeof(SlownikRozmiarZdjec))]
        [GrupaAtttribute("Kafle", 5)]
        public string RozmiarZdjecia { get; set; }

        //, FriendlyOpis = "Opcja jest niebezpieczne dlatego że można szybko ukraść liste sklepów z przeglądarki. Zaleca się ustawiać opcje na NIE"
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Pokazuj wszystkie sklepy naraz, gdy brak wyboru konkretnego miasta")]
        [GrupaAtttribute("Mapa", 5)]
        public bool PokazujWszystkieSklepyNarazGdyBrakWyboruMiasta { get; set; }

        [FriendlyName("Czy pokazywać wszystkie sklepy mimo wybranego miasta")]
        [GrupaAtttribute("Mapa", 5)]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool PokazujWszystkieSklepyWybraneMiasto { get; set; } 
    }
}



        
        
        
        