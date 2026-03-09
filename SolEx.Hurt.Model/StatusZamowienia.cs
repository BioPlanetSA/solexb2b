using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    [FriendlyName(friendlyName: "Statusy dokumentów")]
    public class StatusZamowienia : IHasIntId, IPoleJezyk
    {
        public StatusZamowienia()
        {
            Widoczny = true;
        }


        public StatusZamowienia(StatusImportuZamowieniaDoErp status, bool importwacZamowieniaZeStatusem, int jezykId)
        {
            Id = (int) status;
            Symbol = status.ToString();
            Nazwa = status.ToString();
            Importowac = importwacZamowieniaZeStatusem;
            Widoczny = true;
            JezykId = jezykId;
        }


        [PrimaryKey]
        [WidoczneListaAdmin(true, true, false, false)]
        public int Id { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [Lokalizowane]
        public virtual string Nazwa {get;set;}

        [WidoczneListaAdmin(true, true, true, false)]
        [FriendlyName("Pokazywać dokumenty z tym statusem klientowi")]
        public bool Widoczny { get; set; }

        [WidoczneListaAdmin(true, true, true, false)]
        [FriendlyName("Czy importować zamówienia z tym statusem do systemu księgowego")]
        public bool Importowac { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        public bool PobranoErp { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Symbol")]
        public virtual string Symbol { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Kolor tła statusu na liście dokumentów")]
        [WymuszonyTypEdytora(TypEdytora.PoleKolor)]
        public string Kolor { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Kolor czcionki statusu na liście dokumentów")]
        [WymuszonyTypEdytora(TypEdytora.PoleKolor)]
        public string KolorCzcionki { get; set; }
        
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Czy wysyłać mail do klienta o zmianie statusu, gdy dokument otrzyma ten status")]
        public bool PowiadomienieZmianaStatusu { get; set; }

        [FriendlyName("Traktować dokumenty z tym statusem jako ofertę")]
        [WidoczneListaAdmin(true, false, true, false)]
        public bool TraktujJakoOferte { get; set; }
        
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Traktować dokumenty z tym statusem jako dokumenty faktoringowe")]
        public virtual bool TraktujJakoFaktoring { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Dokumenty z tym statusem mogą być płacone online")]
        public bool PlatnoscOnline { get; set; }

        [Ignore]
        public int JezykId { get; set; }
    }
}
