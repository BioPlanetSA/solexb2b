using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Core.ModelBLL
{
    public abstract  class SyncModul:ModulStowrzonyNaPodstawieZadania, IZadanieSynchronizacji
    {
        public IAPIWywolania ApiWywolanie { get; set; }

        public KategorieKlientowWyszukiwanie KategorieKlientowWyszukiwanie { get; set; }

        public virtual bool UruchamiacModul => true;

        private string koncowka = ", ";

        public virtual  string uwagi => "";

        [Ignore]
        [WidoczneListaAdmin(true, true, false, false,true, new[] { "SolEx.Hurt.Core.ModelBLL.ModulSynchronizacji,SolEx.Hurt.Core", "SolEx.Hurt.Core.ModelBLL.ModulKoszyka,SolEx.Hurt.Core" })]
        public string JakiejOperacjiSynchronizacjiDotyczyAdmin
        {
            get
            {
                string operacje = string.Empty;
                var interfejsy = this.GetType().GetInterfaces();
                foreach (var inter in interfejsy)
                {
                    List<SyncJakaOperacjaAttribute> syncAtr = inter.GetCustomAttributes(typeof(SyncJakaOperacjaAttribute), true).Select(a => a as SyncJakaOperacjaAttribute).ToList();
                    foreach (SyncJakaOperacjaAttribute syncJakaOperacjaAttribute in syncAtr)
                    {
                        operacje += syncJakaOperacjaAttribute.operacja + koncowka;
                    }
                }

                if (string.IsNullOrEmpty(operacje))
                {
                    throw new Exception("Klasa modulu nie implementuje żadnego interfejsu ze zdefiniowanym atrybutem SyncJakaOperacjaAttribute");
                }
                return operacje.TrimEnd(koncowka);
            }
        }

       [Ignore]
       [FriendlyName("Grupa zadań")]
        [WidoczneListaAdminAttribute(true, true, false, false)]
        public string GrupaZadan
        {
            get
            {
                string grupa = string.Empty;
                var interfejsy = this.GetType().GetInterfaces();
                foreach (var inter in interfejsy)
                {
                    List<SyncJakaOperacjaAttribute> syncAtr =
                        inter.GetCustomAttributes(typeof(SyncJakaOperacjaAttribute), true).Select(a => a as SyncJakaOperacjaAttribute).ToList();
                    foreach (SyncJakaOperacjaAttribute syncJakaOperacjaAttribute in syncAtr)
                    {
                        //TODO: PobierzAtrybutDlaEnuma z refleksji
                        Type typEnuma = typeof(ElementySynchronizacji);
                        var memInfo = typEnuma.GetMember(syncJakaOperacjaAttribute.operacja.ToString());
                        var attributes = memInfo[0].GetCustomAttributes(typeof(GrupaZadanSynchronizacjiAttribute),false);
                        string nazwa = ((GrupaZadanSynchronizacjiAttribute)attributes[0]).NazwaGrupy;

                        if (!grupa.Contains(nazwa + koncowka))
                            grupa += nazwa + koncowka;
                    }
                }

                if (string.IsNullOrEmpty(grupa))
                {
                    throw new Exception($"Klasa: {this.GetType().Name} nie implementuje żadnego interfejsu ze zdefiniowanym atrybutem SyncJakaOperacjaAttribute, lub określony enum nie implementuje atrybutu GrupaZadanSynchronizacjiAttribute");
                }

                return grupa.TrimEnd(koncowka);
            }
        }
     

       
    }
}
