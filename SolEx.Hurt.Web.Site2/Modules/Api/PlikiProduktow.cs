using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Dodaje nowe łączniki plików produktów wysłane w obiekcie Data jako Lista<produkty_pliki>
    /// </summary>
    public class PlikiProduktowDodaj : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<ProduktPlik> item = (List<ProduktPlik>)Data;

            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<ProduktPlik>(item);
         return null;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<ProduktPlik>); }
        } 
    }


    /// <summary>
    /// Usuwa łączniki plików produktów wysłane w obiekcie Data jako ListaID<int>
    /// </summary>
    public class PlikiProduktowUsun: ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            HashSet<string> item = (HashSet<string>)Data;
          
            SolexBllCalosc.PobierzInstancje.Pliki.PlikiProduktuUsun(item);
            return null;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof( HashSet<string> ); }
        }
    }

    /// <summary>
    /// Pobiera łączniki plików produktów jako Słownik<klucz string ID, wartość ProduktPlik>
    /// </summary>
    public class PlikiProduktowPobierz : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktPlik>(null).ToDictionary(x => x.Id, x => x);
            //return SolexBllCalosc.PobierzInstancje.Pliki.Laczniki().SelectMany(x=>x.Value).ToDictionary(x=>x.Id,x=>x);
        }
    }

    /// <summary>
    /// Pobiera pliki z platformy dodane przez moduły synchronizatora jako Lista<Plik>
    /// Nie obejmuje plików dodanych z admina (które mają ID < 0 )
    /// Wywołanie handlera usuwa z bazy wpisy z tabeli Pliki, które nie mają rzeczywistych plików na serwerze
    /// </summary>
    public class PlikiPobierz : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            var ids = (HashSet<int>) Data;
            //SolexBllCalosc.PobierzInstancje.Pliki.UsunPlikiZBazyBezPlikowFizycznych();
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Plik>(null,x=>ids.Contains(x.Id)); 
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<int>); }
        }
    }
    /// <summary>
    /// Pobiera id pliki z platformy dodane przez moduły synchronizatora jako Lista<Plik>
    /// Nie obejmuje plików dodanych z admina (które mają ID < 0 )
    /// Wywołanie handlera usuwa z bazy wpisy z tabeli Pliki, które nie mają rzeczywistych plików na serwerze
    /// </summary>
    public class PlikiIdPobierz : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            SolexBllCalosc.PobierzInstancje.Pliki.UsunPlikiZBazyBezPlikowFizycznych();
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Plik>(null,a => a.Id > 0).Select(x=>x.Id).ToList();  //tylko id wieksze niz 0 wysyalmy - autmatycznie importowane
        }
    }
    /// <summary>
    /// Dodaje pliki wysłane w obiekcie Data jako Lista<Plik>
    /// </summary>
    public class PlikiDodaj : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<Plik> item = (List<Plik>)Data;

            for(int i=0;i<item.Count; ++i)
            {
                item[i] = SolexBllCalosc.PobierzInstancje.Pliki.ImportPlikBase64(item[i]);
                item[i].DanePlikBase64 = null; //dzieje sie to wyzej, ale dla pewnosci jeszcze raz
            }
            
            return item;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Plik>); }
        }
    }
    
    /// <summary>
    /// Usuwa pliki wysłane w obiekcie Data jako ListaID<int>
    /// </summary>
    public class PlikiUsun : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<object> item = (List<object>)Data;

            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<Plik, object>(item.ToList());
            return null;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<object>); }
        }
    }

}
