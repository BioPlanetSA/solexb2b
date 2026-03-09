using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Dokumenty
{
    public abstract class DokumentyBaza : KontrolkaTresciBaza
    {
        public override string Grupa
        {
            get { return "Dokumenty"; }
        }


        public override string Kontroler
        {
            get { return "Dokumenty"; }
        }

        public override string Akcja
        {
            get { return "Lista"; }
        }

      
        public abstract RodzajDokumentu Typ { get; }
        [WidoczneListaAdmin]
        [FriendlyName("Aktywne platnosci online - wymagane uzupełnienie klucza API ustawienie 'Klucz do płatności API'")]
        public bool PlatnosciOnline { get; set; }
        [WidoczneListaAdmin]
        [FriendlyName("Opis płatności - symbol strony z opisem. Pole obowiązkowe, jeśli mają się pokazać dane do zapłaty")]
        [PobieranieSlownika(typeof(SlownikTresciSystemowych))]
        [Niewymagane]
        public string PlatnoscDane { get; set; }

        [FriendlyName("Czy pokazać kolumnę status na dokumentach")]
        [Niewymagane]
        [WidoczneListaAdmin]
        public bool PokazStatus { get; set; }
        
    }
}