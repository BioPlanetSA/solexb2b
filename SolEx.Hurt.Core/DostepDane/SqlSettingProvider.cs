using System;
using System.Collections.Generic;
using System.Linq;
using FastMember;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using System.Reflection;

namespace SolEx.Hurt.Core.DostepDane
{
   public class SqlSettingProvider :BazaDostepDane, IConfigDataProvider
    {
  
        public List<Ustawienie> PobierzWszystkieUstawienia()
        {
            var ustawienia = Db.Select<Ustawienie>();
            foreach (var u in ustawienia)
            {
                SolexBllCalosc.PobierzInstancje.Cache.InternujStringiWObiekcie(u, akcesorUstawienia, polaDoInternacji);
            }
            return ustawienia;
        }

        public Dictionary<int, Jezyk> PobierzJezyki(){
            return Db.Select<Jezyk>().ToDictionary(x => x.Id, x => x);
        }

        public List<Tlumaczenie> PobierzSystemPolaLokalizowane(string typ, int jezyk)
        {
           return Db.Where<Tlumaczenie>(x => x.Typ == typ && x.JezykId == jezyk);
        }

        public void DodajSystemPole(TlumaczeniePole sp)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(sp);
        }

        public void DodajSlownik(Tlumaczenie s)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(s);
        }

       public List<TlumaczeniePole> GetSystemNames()
       {
           return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<TlumaczeniePole>(null).ToList();
        }

        public List<StatusZamowienia> PobierzStatusyZamowien()
        {
            return Db.Select<StatusZamowienia>();
        }

        public void AktualizujUstawienie(Ustawienie u)
        {
            Db.Save(u);
        }

        public void AktualizujUstawienie(List<Ustawienie> u)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.SaveAll_UzupelnijKlucze(u);
        }

       public void DodajJezyk(Jezyk tmp)
       {
           Db.Save(tmp);
       }

       public void UsunUstawienie(Ustawienie u)
       {
           Db.DeleteById<Ustawienie>(u.Id); 
       }

        private TypeAccessor akcesorUstawienia = typeof(Ustawienie).PobierzRefleksja();
        Dictionary<string, PropertyInfo> polaDoInternacji = typeof(Ustawienie).Properties(typeof(StringInternuj));

        public Ustawienie PobierzUstawienie(string symbol)
        {
            //na wszelki wypadek żeby wielkość znaków symbolu ustawienia nie miała większego znaczenia bo ktoś wszystkie symbole przekszcałca na małe litery w SettingCollection linia 99 (stan na dzien 24-04-2017 8:45)
            Ustawienie ustawienie =  Db.FirstOrDefault<Ustawienie>(x => x.OddzialId == null && x.Symbol == symbol);

            if (ustawienie != null)
            {
                try
                {
                    SolexBllCalosc.PobierzInstancje.Cache.InternujStringiWObiekcie(ustawienie, akcesorUstawienia, polaDoInternacji);
                } catch (Exception e)
                {
                    throw new Exception($"Bład pobierania ustawienia: {symbol}\r\n{e}");
                }
            }

            return ustawienie;
        }
    }
}
