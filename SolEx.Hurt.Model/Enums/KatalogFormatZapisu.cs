using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    public enum KatalogFormatZapisu
    {
        [FriendlyName("Excel")]
        Xlsx = 1,
        [FriendlyName("Word")]
        Docx = 2,
        [FriendlyName("Pdf")]
        Pdf = 3
    }
}
