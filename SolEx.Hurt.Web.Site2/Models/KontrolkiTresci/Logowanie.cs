using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class Logowanie : KontrolkaTresciBaza
    {
        public Logowanie()
        {
            LogowaniePokazujCaptcha = false;
        }

        public override string Grupa
        {
            get { return "Klienci"; }
        }

        public override string Nazwa
        {
            get { return "Logowanie"; }
        }

        public override string Kontroler
        {
            get { return "Logowanie"; }
        }

        public override string Opis
        {
            get { return "Kontrolka logowania do systemu"; }
        }

        public override string Akcja
        {
            get { return "Logowanie"; }
        }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Pokazuj kodu Captcha przy logowaniu")]
        [Niewymagane]
        public bool LogowaniePokazujCaptcha { get; set;  }
    }
}