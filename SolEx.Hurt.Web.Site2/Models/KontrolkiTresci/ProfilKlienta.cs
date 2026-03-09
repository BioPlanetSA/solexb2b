using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class ProfilKlienta : KontrolkaTresciBaza
    {
        public override string Grupa
        {
            get { return "Klienci"; }
        }
        public override string Nazwa
        {
            get { return "Pokaż ustawienia profilu zalogowanego klienta"; }
        }

        public override string Kontroler
        {
            get { return "ProfilKlienta"; }
        }

        public override string Akcja
        {
            get { return "Index"; }
        }

        public ProfilKlienta()
        {
            this.DomyslneWartosciDlaNowejKontrolki = new TrescKolumna();
            DomyslneWartosciDlaNowejKontrolki.Dostep = AccesLevel.Zalogowani;
        }
    }
}