using System;

namespace SolEx.Hurt.Model.Enums
{
    public class ZdarzenieGrupaAttribute : Attribute
    {
        public ZdarzenieGrupa grupa { get; set; }

        public ZdarzenieGrupaAttribute(ZdarzenieGrupa dokumenty)
        {
            this.grupa = dokumenty;
        }
    }
}