using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class RabatyKlienta : KontrolkaTresciBaza
    {
        public RabatyKlienta()
        {
            PokazujRabatNaProdukty = true;
            PokazujRabatNaKategorie = true;
            PokazujRabatOgolnyKlienta = true;

        }

        public override string Grupa
        {
            get { return "Klienci"; }
        }

        public override string Nazwa
        {
            get { return "Rabaty klienta"; }
        }

        public override string Kontroler
        {
            get { return "Klienci"; }
        }

        public override string Akcja
        {
            get { return "Rabaty"; }
        }

        [FriendlyName("Pokazuj rabat na produkty")]
        [WidoczneListaAdmin(true,true,true,true)]
        public bool PokazujRabatNaProdukty { get; set; }

        [FriendlyName("Pokazuj rabat na kategorie")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool PokazujRabatNaKategorie { get; set; }

        [FriendlyName("Pokazuj rabat ogólny klienta")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool PokazujRabatOgolnyKlienta { get; set; }
    }
}