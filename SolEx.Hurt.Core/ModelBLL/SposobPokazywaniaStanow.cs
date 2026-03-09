using System.Collections.Generic;
using System.Linq;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL
{
    public class SposobPokazywaniaStanow : IObiektWidocznyDlaOkreslonychGrupKlientow, IPolaIDentyfikujaceRecznieDodanyObiekt
    {
        public SposobPokazywaniaStanow()
        {
            PozycjaLista = PozycjaLista.Kolumna;
        }

        [PrimaryKey]
        [UpdateColumnKey]
        [AutoIncrement]
        [WidoczneListaAdmin(true, true, false, false)]
        public long Id { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        public string Nazwa { get; set; }

        [FriendlyName("Magazyn z jakiego pobrać stan",FriendlyOpis = "Zawsze najpierw pobieramy dostępne magazyny klienta (pole na kliencie). " +
                "Następnie z tego ustawienia wskazany magazyn. " +
                "Jeśli klient nie ma magazynu przydzielonego, pobieramy wszystkie magazyny i renderujemy z nich wszystkich stany. Stany magazynów można sumować tworząc stany wirtualne MAG + MAg2 + MAG3 itd.")]
        [WidoczneListaAdmin(false, false, true, false)]
        [PobieranieSlownika(typeof(SlownikMagazynow))]
        public int? DomyslnyMagazynId { get; set; }

        //[Ignore]
        //public List<SposobPokazywaniaStanowRegula> Reguly { get; set; }


        [WidoczneListaAdmin(true, true, true, true)]
        public AccesLevel Dostep { get; set; }

        [FriendlyName("Pozycja na liście produktów")]
        [Niewymagane]
        [WidoczneListaAdmin(false, false, true, false)]
        public PozycjaLista PozycjaLista { get; set; }

        [WidoczneListaAdmin(false, false, true, false)]
        [Niewymagane]
        public List<RoleType> DozwolonaRolaKlienta { get; set; }

        [FriendlyName("Widoczność")]
        [WymuszonyTypEdytora(TypEdytora.WidocznoscDlaKlientow)]
        [Ignore]
        [WidoczneListaAdmin(false, false, true, false)]
        public WidocznosciTypow Widocznosc { get; set; }

        public bool RecznieDodany()
        {
            return true;
        }

        [Ignore]
        [WidoczneListaAdmin(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.ListaWarunkow, typeof(SposobPokazywaniaStanowRegula))]
        [FriendlyName("Reguły sposobu pokazywania stanów")]
        public virtual List<SposobPokazywaniaStanowRegula> Reguly { get; set; }
    }
}
