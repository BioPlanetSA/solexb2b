using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class GrupySubkont : KontrolkaTresciBaza
    {
        public override string Nazwa
        {
            get { return "Grupy subkont"; }
        }

        public override string Kontroler
        {
            get { return "Klienci"; }
        }

        public override string Akcja
        {
            get { return "GrupySubkont"; }
        }
    }
}