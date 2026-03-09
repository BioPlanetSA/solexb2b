using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class UstawieniaZamiennik
    {
        public UstawieniaZamiennik(ListaDane param)
        {
            CenaPoRabacieWariantyPokazuj = param.AktualneParametry.KontrolkaProduktow.CenaPoRabacie;
            PokazywacBelkeDostepnosci = param.AktualneParametry.KontrolkaProduktow.BelkaDostepnosci;
            GradacjeZamiennikiSposobPokazywania = param.AktualneParametry.KontrolkaProduktow.LGradacja;
            KodKreskowyZamiennikiPokazuj = param.AktualneParametry.KontrolkaProduktow.LKodKreskowy;
            SymbolZamiennikiPokazuj = param.AktualneParametry.KontrolkaProduktow.LSymbol;
        }
        
        public JakieCenyPokazywac CenaPoRabacieWariantyPokazuj { get; set; }
        public bool PokazywacBelkeDostepnosci { get; set; }
        public GradacjaSposobPokazywania GradacjeZamiennikiSposobPokazywania { get; set; }
        public bool KodKreskowyZamiennikiPokazuj { get; set; }
        public bool SymbolZamiennikiPokazuj { get; set; }

        public IProduktKlienta Produkt { get; set; }
    }
}