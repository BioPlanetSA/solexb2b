using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieKlientow
{
    public class StworzGrupeZKateogrii : SyncModul, IModulKategorieKlientow
    {
        public override string uwagi
        {
            get { return ""; }
        }
        public override string Opis
        {
            get { return "Łączy kategorie klientów w nową grupę"; }
        }
        
        [FriendlyName("Nazwy kategorii do przeniesia do nowej grupy, oddzielone ;")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public virtual string Kategorie { get; set; }
        
        [FriendlyName("Nazwa nowej grupy")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public virtual string Grupa { get; set; }
        public void Przetworz(ref List<KategoriaKlienta> kategorie, ref List<KlientKategoriaKlienta> laczniki)
        {
            if (String.IsNullOrEmpty(Kategorie) || string.IsNullOrEmpty(Grupa)) return;
            string[] kats = Kategorie.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string k in kats)
            {
                KategoriaKlienta kk = kategorie.FirstOrDefault(x => x.Nazwa == k);
                if (kk != null)
                {
                    kk.Grupa = Grupa;
                }
            }
         
        }
    }
}
