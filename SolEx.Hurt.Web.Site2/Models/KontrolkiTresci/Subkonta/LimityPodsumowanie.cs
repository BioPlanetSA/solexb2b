using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Subkonta
{
    public class LimityPodsumowanie : KontrolkaTresciBaza
    {
        public override string Nazwa
        {
            get { return "Podsumowanie limitów dla subkonta"; }
        }

        public override string Kontroler
        {
            get { return "Subkonta"; }
        }

        public override string Akcja
        {
            get { return "PodsumowanieLimity"; }
        }
        public override string Grupa
        {
            get { return "Subkonta"; }
        }
        [FriendlyName("Dla jakiego rodzaju limitu wyświetlić podsumowanie")]
        [WidoczneListaAdmin(true, true, true, true)]
        [Wymagane]
        public RodzajLimitu Rodzaj { get; set; }
    }
}
