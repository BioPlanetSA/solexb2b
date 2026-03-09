using System.Collections.Generic;
using System.Text;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Importy.Eksporty
{
    [FriendlyName("Format CSV jest dedykowany do budowy własnych rozwiązań informatycznych. Plik można otworzyć np. w Excelu, OpenOffice lub notatniku. Zawartość pliku to po prostu pozycje dokumentu gdzie identyfikatorem produktu będzie symbol")]
    public class CsvSymbol : Csv
    {
        public override Licencje? WymaganaLicencja
        {
            get { return Licencje.DokumentyCSVSymbol; }
        }
        public override string Nazwa
        {
            get { return "Csv - Symbol"; }
        }

        public override string IdentyfikatorProduktu
        {
            get { return "Kod"; }
        }
    }
}
