using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class ZmianaHasla : KontrolkaTresciBaza
    {
        public override string Grupa
        {
            get { return "Klienci"; }
        }

        public override string Nazwa
        {
            get { return "Zmiana hasła"; }
        }

        public override string Kontroler
        {
            get { return "MojeDane"; }
        }

        public override string Akcja
        {
            get { return "ZmianaHasla"; }
        }

        public override string Opis
        {
            get { return "Kontrolka zmiany hasła"; }
        }
    }
}