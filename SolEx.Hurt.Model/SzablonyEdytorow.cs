using System;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class SzablonyEdytorow : IHasIntId, IPolaIDentyfikujaceRecznieDodanyObiekt
    {
        [PrimaryKey]
        [WidoczneListaAdmin(true, true, false, false)]
        public int Id { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        public string Nazwa { get; set; }
        
        [WidoczneListaAdmin(true, true, true, true)]
        public string Opis { get; set; }

        [FriendlyName("Treść szablonu")]
        [WidoczneListaAdmin(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string Tresc { get; set; }

        public bool RecznieDodany()
        {
            return Id < 0;
        }
    }
}




 