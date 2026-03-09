using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [FriendlyName("Ustaw wybraną definicję dla dokumentu w ERP", FriendlyOpis = "Dla systemów ERP które umożliwiają ustawienie definicji dokumentów (np. CDN Optima) można tym modułem zmieniać definicje dokumentów w zależności od zamówienia.")]
    public class UstawDefinicjeDokumentuERP : ZadanieCalegoKoszyka, IZadaniePoFinalizacji
    {
        [FriendlyName("Nazwa definicji dokumentu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaDefinicji { get; set; }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            koszyk.DefinicjaDokumentuERP = NazwaDefinicji;

            return true;
        }
    }
}