using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Koszyk
{
    [Licencja(Licencje.Wielokoszykowosc)]
    public class KoszykWybor:KontrolkaTresciBaza
    {
        public override string Nazwa
        {
            get { return "Wybór aktualnego koszyka"; }
        }

        [WidoczneListaAdmin]
        [FriendlyName("Pokazuj przycisk usuwania")]
        public bool PokazujUsuwanie { get; set; }
        [WidoczneListaAdmin]
        [FriendlyName("Pokazuj datę utworzenia")]
        public bool PokazujDaty { get; set; }
        [WidoczneListaAdmin]
        [FriendlyName("Pokazuj historie zmian")]
        public bool PokazujHistorieZmian { get; set; }
        public override string Kontroler
        {
            get { return "Koszyk"; }
        }

        public override string Akcja
        {
            get { return "KoszykWybor"; }
        }
        public override string Grupa
        {
            get { return "Koszyk"; }
        }
    }
}