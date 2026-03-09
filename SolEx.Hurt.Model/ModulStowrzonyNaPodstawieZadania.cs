using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using System;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Model
{
     public abstract class ModulStowrzonyNaPodstawieZadania : IPolaIDentyfikujaceRecznieDodanyObiekt, IHasIntId
    {
        public int Id { get; set; }
        public ModulStowrzonyNaPodstawieZadania()
        {
            MozeDzialacDoGodziny = 0;
            MozeDzialacDoGodziny = 24;
            IleMinutCzekacDoKolejnegoUruchomienia = 0;
        }
        public int MozeDzialacOdGodziny { get; set; }
        public int MozeDzialacDoGodziny { get; set; }
        public int IleMinutCzekacDoKolejnegoUruchomienia { get; set; }
        protected ILog Log
        {
            get
            {
                return LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
        [Ignore]
        [ObsoleteAttribute("do dodawania nazwy i opisów proszę uzywać atrybut FriendlyName")]
        [WidoczneListaAdmin(true, false, false, false,false)]
        public virtual string Opis
        {
            get { return ""; }
        }
        [Ignore]
        [WidoczneListaAdmin(true, false, false, false, false)]
        public string Nazwa
        {
            get
            {
                string wynik = string.Empty;
                var oba = GetType().GetCustomAttribute<ObsoleteAttribute>();
                if (oba != null)
                {
                    wynik += "Przestarzałe ";
                    if (!string.IsNullOrEmpty(oba.Message))
                    {
                        wynik += "Info: " + oba.Message;
                    }
                }
                else
                {
                    wynik = PokazywanaNazwa;
                }

                return wynik;
            }
        }
        [Ignore]
        public virtual string PokazywanaNazwa
        {
            get
            {
                return GetType().Name;
            }
        }
       
        [Ignore]
        [FriendlyName("Komentarz w jakim celu jest używany moduł (widoczny tylko w adminie)")]
        [WidoczneListaAdminAttribute(true, true, true, false)]
        public string Komentarz { get; set; }


        public Zadanie ZadanieBazowe { get; set; }
        public bool RecznieDodany()
        {
            return true;
        }
        [Ignore]
        public List<ElementySynchronizacji> JakiejOperacjiSynchronizacjiDotyczy
        {
            get
            {
                List<ElementySynchronizacji> lista = new List<ElementySynchronizacji>();
                var interfejsy = GetType().GetInterfaces();
                foreach (var inter in interfejsy)
                {
                    List<SyncJakaOperacjaAttribute> syncAtr = inter.GetCustomAttributes(typeof(SyncJakaOperacjaAttribute), true).Select(a => a as SyncJakaOperacjaAttribute).ToList();
                    foreach (SyncJakaOperacjaAttribute syncJakaOperacjaAttribute in syncAtr)
                    {
                        lista.Add(syncJakaOperacjaAttribute.operacja);
                    }
                }
                if (lista.Count == 0)
                {
                    return null;
                }
                //moze byc już null bo moduly koszykowe beda miec zawsze null tutaj
                return lista;
            }
        }
    }




}
