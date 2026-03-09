using System.Collections.Generic;
using System.Web;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Bazowe
{
    public abstract class LosowaListaProduktowWybraneIdProduktow : ListaProduktowBaza
    {
        public LosowaListaProduktowWybraneIdProduktow()
        {
            Przesuwanie = true;
            Szablon = "Kafle";
            IleProduktowWWierszu = 4;
            PrzesuwanieIleWierszy = 1;
            PokazujKoszyk = true;
            ZdjecieZamiennikiPokazuj = true;
            PokazywacZdjecieProduktyRodzinowego = true;
        }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Produkty do wyświetlenia")]
        [PobieranieSlownika(typeof(SlownikProduktow))]
        [Niewymagane]
        public HashSet<long> ListaProduktow { get; set; }

        public override string Kontroler
        {
            get { return "Produkty"; }
        }

        public override string Akcja
        {
            get { return "WybraneProdukty"; }
        }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy ma być przesuwanie")]
        public bool Przesuwanie { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Widok generowany dla listy produktów")]
        [PobieranieSlownika(typeof(SlownikWidokowDlaWybranychProduktow))]
        public string Szablon { get; set; }




        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Ilość produktów w wierszu. Wpisanie 0 powoduje automatyczne wyliczenie")]
        public int IleProduktowWWierszu { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Ilość przesuwanych wierszy ")]
        public int PrzesuwanieIleWierszy { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Cena po rabacie produktu lista")]
        [GrupaAtttribute("Lista", 2)]
        public JakieCenyPokazywac CenaPoRabacieLista { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Pokazywać belkę dostępności na liście produktów")]
        [GrupaAtttribute("Lista", 2)]
        public bool PokazywacBelkeDostepnosci { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać zdjęcie produktu rodzinowego")]
        [GrupaAtttribute("Produkty rodzinowe", 5)]
        public bool PokazywacZdjecieProduktyRodzinowego { get; set; }

        public abstract HashSet<long> ListaProduktowId { get; set; }


        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać zdjęcie produktu na dla zamienników")]
        [GrupaAtttribute("Zamienniki", 3)]
        public bool ZdjecieZamiennikiPokazuj { get; set; }


    }
}