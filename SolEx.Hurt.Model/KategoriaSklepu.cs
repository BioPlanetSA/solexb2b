using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using System;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    [FriendlyName("Kategorie sklepów")]
    public class KategoriaSklepu : IHasLongId , IPolaIDentyfikujaceRecznieDodanyObiekt
    {
        [PrimaryKey]
        [WidoczneListaAdmin(true, false, false, false)]
        public long Id { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        public string Nazwa { get; set; }

        [FriendlyName("Widoczna na mapie")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool  PokazywanaNaMapie{ get; set; }

        [FriendlyName("Ikona na mapie")]
        [WidoczneListaAdmin(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ObrazekPineskaId{ get; set; }
        
        [WidoczneListaAdmin(true, false, false, false)]
        public bool Automatyczna { get; set; }

        public KategoriaSklepu()
        {
            PokazywanaNaMapie = true;
        }

        public KategoriaSklepu(KategoriaSklepu bazowy)
        {
            if (bazowy == null) return;
            Type type = typeof (KategoriaSklepu);
            var akcesor = type.PobierzRefleksja();

            foreach (var field in type.Properties().Values)
            {
                if (field.GetSetMethod() != null)
                {
                    field.SetValue(this, akcesor[bazowy, field.Name], null);
                }
            }
        }

        public bool RecznieDodany()
        {
            return Id <= 0;
        }
    }
}
