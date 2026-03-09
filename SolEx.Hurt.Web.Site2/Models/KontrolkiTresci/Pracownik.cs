using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class Pracownik : KontrolkaTresciBaza
    {

        public override string Grupa => "Klienci";

        public override string Nazwa => "Pracownik";

        public override string Kontroler => "Klienci";

        public override string Akcja => "Pracownik";

        public override string Opis => "Kontrolka wyświetlająca pracownika";

        public override string Ikona => "fa fa-user";

        public Pracownik()
        {
            TypPracownika = TypPracownika.Opiekun;
            FormaSkrocona = false;
            OpiekunKlientaTekstNadZdjeciem = "";
            DodatkowyTelefon = "";
        }

        [WidoczneListaAdmin(true, true, true,true)]
        [FriendlyName("Typ pracownika")]
        public TypPracownika TypPracownika { get; set; }



        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Forma skrócona", FriendlyOpis = "Forma skrócona =  bez zdjęcia")]
        public bool FormaSkrocona { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Pokaż pracownika",FriendlyOpis = "Jeżeli nie wybrano żadnego pracownika, zostanie pokazany pracownik wg. wybranego typu powyżej z aktualnego klienta")]
        [PobieranieSlownika(typeof(SlownikPracownikow))]
        [Niewymagane]
        public int? WybranyReczniePracownikID { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Dodatkowy telefon pracownika")]
        [Niewymagane]
        public string DodatkowyTelefon { get; set; }
        
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Tekst nad zdjęciem pracownika")]
        [Niewymagane]
        public string OpiekunKlientaTekstNadZdjeciem { get; set; }


        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Dodatkowe pola do wyświetlenia")]
        [Niewymagane]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Core.ModelBLL.Pracownik))]
        public List<string> PolaDodatkowe { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Rozmiar zdjęcia")]
        [PobieranieSlownika(typeof(SlownikRozmiarZdjec))]
        public string RozmiarZdjecia { get; set; }
    }
}