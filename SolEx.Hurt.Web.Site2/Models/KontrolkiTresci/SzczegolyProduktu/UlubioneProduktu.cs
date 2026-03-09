using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu
{
    public class UlubioneProduktu : SzczegolyProduktuBaza
    {
        public override string Nazwa
        {
            get { return "Dodaj do ulubionych"; }
        }

        public override string Opis
        {
            get { return "Wyswietla opcje dodaj do ulubionych."; }
        }
        
        public override string Akcja
        {
            get { return "Ulubione"; }
        }

        public override string Kontroler
        {
            get { return "Koszyk"; }
        }

        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Tryb tekstowy pokazuje dodatkowy opis obok ikony - przydatne na karcie produktu")]
        public bool TrybTekstowy { get; set; }
    }
}