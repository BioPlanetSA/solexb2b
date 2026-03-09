using System.Collections.Generic;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class Rejestracja : KontrolkaTresciBaza
    {
        public override string Grupa => "Klienci";

        public override string Nazwa => "Rejestracja";

        public override string Kontroler => "Rejestracja";

        public override string Akcja => "Rejestracja";

        public override string Opis => "Kontrolka rejestracji do systemu";

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Pola do rejestracji")]
        [PobieranieSlownika(typeof(SlownikMapowanePolaRejestracja))]
        public string[] ListaPol { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Pola rejestracji które będą wymagane")]
        [PobieranieSlownika(typeof(SlownikMapowanePolaRejestracja))]
        public string[] ListaPolWymaganych { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Odpowiedź jaką zabaczy klient po zarejestrowaniu")]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string Odpowiedz { get; set; }

        public Rejestracja()
        {
            ListaPol = new[] { "Nazwa", "NIP", "ImieNazwisko", "WiadomoscEmail", "Telefon", "AkceptacjaRegulaminu" };
            ListaPolWymaganych = new[] { "Email" };
        }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać Captche Google")]
        public bool CzyPokazywacCaptche { get; set; }

        
    }
}