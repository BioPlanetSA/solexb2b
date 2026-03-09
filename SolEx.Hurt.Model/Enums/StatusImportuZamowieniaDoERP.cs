using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    public enum StatusImportuZamowieniaDoErp
    {
        [FriendlyName("Złożone")]
        Złożone = 1,
        [FriendlyName("Zaimportowane")]
        Zaimportowane = 2,
        [FriendlyName("Anulowane")]
        Anulowane = 3,
        [FriendlyName("Zrealizowane")]
        Zrealizowane = 4,
        [FriendlyName("Błąd importu")]
        Błąd = 5,
        [FriendlyName("Usunięte")]
        Usunięte = 6

    }
}
