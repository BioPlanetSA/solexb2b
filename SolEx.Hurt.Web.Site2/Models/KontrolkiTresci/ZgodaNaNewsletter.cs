using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class ZgodaNaNewsletter : KontrolkaTresciBaza
    {
        public override string Grupa
        {
            get { return "Klienci"; }
        }

        public override string Nazwa
        {
            get { return "Zgoda na newsletter"; }
        }

        public override string Kontroler
        {
            get { return "Newsletter"; }
        }

        public override string Akcja
        {
            get { return "ZgodaNaNewsletter"; }
        }
    }
}