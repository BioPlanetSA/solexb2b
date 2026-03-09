using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Koszyk
{
    public class KoszykPodglad : KontrolkaTresciBaza
    {
        public KoszykPodglad()
        {
            IloscPozycji = 5;
            CenaPoRabacie = JakieCenyPokazywac.NettoBrutto;
        }

        public override string Grupa
        {
            get { return "Koszyk"; }
        }

        public override string Nazwa
        {
            get { return "Podgląd koszyka"; }
        }

        public override string Kontroler
        {
            get { return "Koszyk"; }
        }

        public override string Akcja
        {
            get { return "Koszyk"; }
        }

        public override string Ikona
        {
            get { return "fa fa-shopping-cart"; }
        }

        [FriendlyName("Maksymalna ilość pokazywanych pozycji")]
        [WidoczneListaAdmin(true,true,true,true)]
        public int IloscPozycji { get; set; }


        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Cena po rabacie lista produktów", FriendlyOpis = "Jakie ceny pokazywać na liście produktów dla ceny po rabacie")]
        [GrupaAtttribute("Lista", 2)]
        [Niewymagane]
        public JakieCenyPokazywac CenaPoRabacie { get; set; }


    }
}