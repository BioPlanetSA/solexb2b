using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu
{
    public class Zamienniki : SzczegolyProduktuBaza
    {
        public Zamienniki()
        {
            PokazywacBelkeDostepnosci = true;
            CenaPoRabacieWariantyPokazuj = JakieCenyPokazywac.NettoBrutto;
            SymbolZamiennikiPokazuj = true;
            KodKreskowyZamiennikiPokazuj = true;
        }
        public override string Nazwa
        {
            get { return "Zamienniki - prosta lista"; }
        }

        public override string Opis
        {
            get { return "Wyswietla zamienniki na karcie produktu."; }
        }

        public override string Akcja
        {
            get { return "Zamienniki"; }
        }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy grupować akcesoria na karcie produktu w formę tabów")]
        public bool AkcesoriaJakoTaby { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Jakie ceny po rabacie pokazywać na liście wariantów")]
        public JakieCenyPokazywac CenaPoRabacieWariantyPokazuj { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Pokazywać belkę dostępności na liście produktów")]
        public bool PokazywacBelkeDostepnosci { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Sposób pokazywania gradacji")]
        public GradacjaSposobPokazywania GradacjeZamiennikiSposobPokazywania { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać symbol produktów w zamiennikach")]
        public bool SymbolZamiennikiPokazuj { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać kod kreskowy produktów w zamiennikach")]
        public bool KodKreskowyZamiennikiPokazuj { get; set; }
    }
}