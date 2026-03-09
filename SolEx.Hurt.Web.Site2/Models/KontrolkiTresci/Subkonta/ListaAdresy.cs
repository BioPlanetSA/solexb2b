using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Subkonta
{
    public class ListaAdresy : KontrolkaTresciBaza
    {
        public override string Nazwa
        {
            get { return "Lista adresów"; }
        }

        public override string Kontroler
        {
            get { return "Adresy"; }
        }

        public override string Akcja
        {
            get { return "Lista"; }
        }
        public override string Grupa
        {
            get { return "Subkonta"; }
        }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy na liście adresów ma być dostępna opcja dodawania nowego adresu")]
        public bool DodawanieNowegoAdresu { get; set; }
    }
}