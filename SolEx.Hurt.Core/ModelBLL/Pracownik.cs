using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.ModelBLL
{
    [Alias("Klient")]
    [FriendlyName("Pracownik")]
    public class Pracownik : Klient
    {

        public Pracownik() { }
        public Pracownik(Klient x):base(x)
        {
           DomyslnyOpiekun= Id == SolexBllCalosc.PobierzInstancje.Konfiguracja.IdDomyslnyOpiekun;
           DomyslnyDrugiOpiekun = Id == SolexBllCalosc.PobierzInstancje.Konfiguracja.IdDrugiDomyslnyOpiekun;
           DomyslnyPrzedstawiciel = Id == SolexBllCalosc.PobierzInstancje.Konfiguracja.IdDomyslnyPrzedstawiciel;
            WidziWszystkich = false;

        }
        [WidoczneListaAdmin(true, true, true, false, true, new[] { typeof(Pracownik) })]
        [Ignore]
        public bool DomyslnyOpiekun{get; set;}

        [WidoczneListaAdmin(true, true, true, false, true, new[] { typeof(Pracownik) })]
        [Ignore]
        public bool DomyslnyPrzedstawiciel{get;set;}

        [WidoczneListaAdmin(true, true, true, false, true, new[] { typeof(Pracownik) })]
        [Ignore]
        public bool DomyslnyDrugiOpiekun{get;set;}


    }

    
}
