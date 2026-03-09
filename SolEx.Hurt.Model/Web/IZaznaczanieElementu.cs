using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.Web
{
    public interface IZaznaczanieElementu
    {
        int  IDElementu { get; set; }
        bool Zaznaczony { get; set; }
    }
}
