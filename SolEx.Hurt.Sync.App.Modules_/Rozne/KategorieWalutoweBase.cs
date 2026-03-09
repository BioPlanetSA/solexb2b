using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Sync.App.Modules_.Rozne
{
    public abstract class KategorieWalutoweBase : KategorieKlientow
    {

        public override string uwagi
        {
            get { return ""; }
        }

        [FriendlyName("Domyślna waluta, jeśli klient nie ma wpisanej np. PLN")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public virtual string DomyslnaWaluta { get; set; }

        internal string grupa = "Waluta";

    }
}
