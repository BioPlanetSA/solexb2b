using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class TranslatorGoogle : KontrolkaTresciBaza
    {
        public override string Nazwa
        {
            get { return "Tłumacz Google"; }
        }

        public override string Kontroler
        {
            get { return "Wyglad"; }
        }

        public override string Akcja
        {
            get { return "Translator"; }
        }

        [WidoczneListaAdmin(true,true,true,true)]
        [Niewymagane]
        [FriendlyName("Język wyświetlania",FriendlyOpis = "Jeżeli puste pokazane zostaną wszystkie dostępne języki. Jeżeli chcemy pokazać wybrane języki wpisujemy symbol języka np.: en,us,fr,de")]
        public string JezykWyswietlania { get;set;}

        [PobieranieSlownika(typeof(SlownikGoogleTlumaczTrybyWyswietlania))]
        [WidoczneListaAdmin(true, true, true, true)]
        [Wymagane]
        [FriendlyName("Tryb wyświetlania")]
        [WymuszonyTypEdytora(TypEdytora.PoleDropDown)]
        public string TrybWyswietlania { get; set; }
   
    } 
}