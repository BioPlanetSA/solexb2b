using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu
{
    public class KoszykProduktu : SzczegolyProduktuBaza
    {
        public KoszykProduktu()
        {
            DodawanieTekstowe = true;
        }

        public override string Nazwa
        {
            get { return "Koszyk"; }
        }

        public override string Opis
        {
            get { return "Wyswietla opcje dodaj do koszyka."; }
        }
        
        public override string Akcja
        {
            get { return "DodawanieProduktu"; }
        }

        public override string Kontroler
        {
            get { return "Koszyk"; }
        }

        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Tekstowy tryb pokazywania koszyka")]
        public bool DodawanieTekstowe { get; set; }
    }
}