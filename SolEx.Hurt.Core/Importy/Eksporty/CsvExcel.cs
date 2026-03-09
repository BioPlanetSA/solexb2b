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
    [FriendlyName("Format CSV jest dedykowany do budowy własnych rozwiązań informatycznych. Plik można otworzyć np. w Excelu, OpenOffice lub notatniku. Zawartość pliku to po prostu pozycje dokumentu gdzie identyfikatorem produktu będzie kod kreskowy")]
    public class CsvExcel : Csv
    {
        public override string Nazwa => "Csv - windows1250";
        public override Encoding Kodowanie => Encoding.GetEncoding("Windows-1250");
    }
}
