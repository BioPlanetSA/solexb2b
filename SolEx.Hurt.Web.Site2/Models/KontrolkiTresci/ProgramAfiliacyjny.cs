using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class ProgramAfiliacyjny : KontrolkaTresciBaza
    {
        public override string Nazwa
        {
            get { return "Program afiliacyjny"; }
        }

        public override string Kontroler
        {
            get { return "Afiliacyjny"; }
        }

        public override string Akcja
        {
            get { return "Lista"; }
        }
    }
}