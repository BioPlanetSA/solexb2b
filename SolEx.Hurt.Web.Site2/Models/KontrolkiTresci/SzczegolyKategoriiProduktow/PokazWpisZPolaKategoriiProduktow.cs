using System.Web;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Bazowe;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyKategoriiProduktow
{
    public class PokazWpisZPolaKategoriiProduktow : PokazWpisBaza, IPoleJezyk
    {
        public override string Kontroler
        {
            get { return "OpisKategoriiProduktow"; }
        }

        public override string Akcja
        {
            get { return "OpisKategorii"; }
        }

        public override string Grupa
        {
            get { return "Kategorie produktów"; }
        }

        public override string Nazwa
        {
            get { return "Opis kategorii produktów"; }
        }

        [PobieranieSlownika(typeof (SlownikPolKategoriaProduktu))]
        [FriendlyName("Pole")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleDropDown)]
        public string Pole { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [Niewymagane]
        [FriendlyName("Tekst gdy pole nie ma wartości",FriendlyOpis = "Tekst który ma się pokazać jeśli kategoria nie ma wprowadzonej wartości do wybranego pola")]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Lokalizowane]
        public string TextDomyslny { get; set; }

        public override string SymbolIdentyfikatora
        {
            get { return "kategoria"; }
        }
        public override string ModelObiektu
        {
            get { return "Kategoria"; }
        }

        public object IdentyfikatorObiektu
        {
            get
            {
                string val = PobierzIdentyfikator(SymbolIdentyfikatora, true, ModelObiektu).ToString();
                long wynik = JSonHelper.Deserialize<long>(val);
                if (wynik == 0)
                {
                    throw new HttpException(500, "Nie znaleziono ID kategorii");
                }
                return wynik;
            }
        }
    }
}