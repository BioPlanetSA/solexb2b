using System.Collections.Generic;
using System.Linq;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core
{
    public class IndywidualizacjaWartosc
    {
        public IndywidualizacjaWartosc() { }

        public IndywidualizacjaWartosc(Indywidualizacja indywidualizacja, long produktID, object wartosc)
        {
            Indywidualizacja = indywidualizacja;
            this.ProduktID = produktID;
            this.Wartosc = wartosc;
            this.IndywidualizacjaID = indywidualizacja.Id;
        }

        public object Wartosc { get; set; }
        public Indywidualizacja Indywidualizacja { get; set; }

        public long ProduktID { get; set; }
        public long IndywidualizacjaID { get; set; }
        public int PozycjaId { get; set; }
    }

    public class Indywidualizacja : IHasLongId, IPolaIDentyfikujaceRecznieDodanyObiekt
    {
        public Indywidualizacja()
        {
            CenyIndywidualizacja = WygenerujCeny();
        }

        public IndywidualizacjaCena[] WygenerujCeny()
        {
            var waluty = SolexBllCalosc.PobierzInstancje.Konfiguracja.SlownikWalut;
            List<IndywidualizacjaCena> ceny = new List<IndywidualizacjaCena>(waluty.Count);
            foreach(var waluta in waluty.Values)
            {
                ceny.Add( new IndywidualizacjaCena(waluta.Id) );
            }
            return ceny.ToArray();
        }

        [AutoIncrement]
        [PrimaryKey]
        public long Id { get; set; }

        [Niewymagane]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikCech,SolEx.Hurt.Core")]
        [FriendlyName("Cechy które muszą mieć produkty, aby zostały przetowrzone przez ten moduł. Jeśli pusta to przetwarza wszystkie produkty")]
        [WidoczneListaAdmin(true, false, true, false)]
        public HashSet<long> ListaCechWmaganych { get; set; }

        [FriendlyName("Typ wartości pola indiwidualizacji")]
        [WidoczneListaAdmin(true, true, true, false)]
        public RodzajKontrolki RodzajKontrolki { get; set; }

        [Niewymagane]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikAtrybutow,SolEx.Hurt.Core")]
        [FriendlyName("Pole wymagane jeżeli typ pola indywidualizacji to Atrybut")]
        [WidoczneListaAdmin(true, false, true, false)]
        public int? Atrybut { get; set; }


        public AtrybutBll PobierzAtrybut()
        {
            return null;
        }

        [FriendlyName("Nazwa pola indywidualizacji")]
        [WidoczneListaAdmin(true, true, true, false)]
        public string Nazwa { get; set; }

        [FriendlyName("Sposób indywidualizacji", FriendlyOpis = "Parametr określa czy w momencie dodawania do koszyka kilku produktów " +
                                                                "pokazać opcje indywidualizacji dla każdej sztuki niezależne, czy też dla wszystkcih zbiorczo.")]
        [WidoczneListaAdmin(false, false, true, false)]
        public SposobIndywidualizacji SposobIndywidualizacji { get; set; }

        [FriendlyName("Czy indywidualizacja ma być wymagana")]
        [WidoczneListaAdmin(true, true, true, false)]
        public bool Wymagane { get; set; }

        /// <summary>
        /// Czy dodawać nazwę indywidualizacji do poisu pozycji zamówienia
        /// </summary>
        [FriendlyName("Czy tworząc opis indywidualizacji doklejać jej nazwę")]
        [WidoczneListaAdmin(false, false, true, false)]
        public bool DodajNazweDoOpisu { get; set; }

        /// <summary>
        /// autouzupelnianie
        /// </summary>
        [Ignore]
        [FriendlyName("Ceny indywidualizacji")]
        [WidoczneListaAdmin(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.IndywidualizacjaCenaTablica)]
        public IndywidualizacjaCena[] CenyIndywidualizacja { get; set; }


        public IndywidualizacjaCena PobierzCeneDlaWaluty(long walutaID)
        {
            return CenyIndywidualizacja.FirstOrDefault(x => x.WalutaId == walutaID);
        }

        public bool RecznieDodany()
        {
            return true;
        }
    }
}
