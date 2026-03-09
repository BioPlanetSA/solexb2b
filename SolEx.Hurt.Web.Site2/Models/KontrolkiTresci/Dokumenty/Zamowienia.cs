using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Dokumenty
{
    public class Zamowienia : DokumentyBaza
    {
       public override RodzajDokumentu Typ
        {
            get { return RodzajDokumentu.Zamowienie; }
        }

        public override string Nazwa
        {
            get { return "Zamowienia"; }
        }

        public override string Opis
        {
            get { return "Lista zamówień"; }
        }

        [FriendlyName("Czy pokazywać kolumnę zrealizowane")]
        [Niewymagane]
        [WidoczneListaAdmin]
        public bool PokazKolumneZrealizowane { get; set; }

        [FriendlyName("Czy pokazywać tylko zamówienia niezrealizowane")]
        [Niewymagane]
        [WidoczneListaAdmin]
        public bool PokazTylkoNiezrealizowane { get; set; }
    }
}