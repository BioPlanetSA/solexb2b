using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class Slajd : IHasLongId,IPolaIDentyfikujaceRecznieDodanyObiekt,IPoleJezyk
    {
        public Slajd()
        {
            CzyPokazywacTytul = false;
        }

        public Slajd(string name)
        {
            Nazwa = name;
        }

        public Slajd(Slajd slajd)
        {
            Id = slajd.Id;
            Nazwa = slajd.Nazwa;
            Opis = slajd.Opis;
            PlikTlaId = slajd.PlikTlaId;
            KolorTla = slajd.KolorTla;
            WysokoscTla = slajd.WysokoscTla;
            LinkUrl = slajd.LinkUrl;
            CzyPokazywacTytul = slajd.CzyPokazywacTytul;
        }

        [AutoIncrement]
        [PrimaryKey]
        [WidoczneListaAdmin(true, true, false, false,false)]
        public long Id { get; set; }

        [WidoczneListaAdmin(true, true, true, false)]
        [FriendlyName("Tytuł")]
        [Lokalizowane]
        public string Nazwa { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywac Tytuł?")]
        public bool CzyPokazywacTytul { get; set; }

        [WidoczneListaAdmin(true, true, true, false)]
        [Niewymagane]
        [FriendlyName("Link Url", FriendlyOpis = "skrót linka będzie kierował pod ten link, aby kierować do zewnętrznej strony, link musi zaczynać się od http://")]
        public AdresUrl LinkUrl { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Tło")]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? PlikTlaId { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleKolor)]
        [FriendlyName("Kolor tła")]
        public string KolorTla { get; set; }

        [FriendlyName("Wysokość tła")]
        [WidoczneListaAdmin(true, false, true, false)]
        public int? WysokoscTla { get; set; }

        [WidoczneListaAdmin(true, true, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [Lokalizowane]
        public string Opis { get; set; }

        [WidoczneListaAdmin(true, true, true,true)]
        [FriendlyName("Kolejność")]
        public int Kolejnosc { get; set; }

        public bool RecznieDodany()
        {
            return true;
        }
        [Ignore]
        public int JezykId { get; set; }
    }
}
