using System;

namespace SolEx.Hurt.Model.Interfaces.SyncModuly
{
    public class SyncJakaOperacjaAttribute : Attribute
    {
        public Enums.ElementySynchronizacji operacja
        {
            get;
            set;
        }
        
        public SyncJakaOperacjaAttribute(Enums.ElementySynchronizacji e){
            operacja = e;
        }
    }
}
