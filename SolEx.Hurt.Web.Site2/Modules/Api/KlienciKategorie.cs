using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Helper;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje łączniki kategorii klientów wysłanych w obiekcie Data jako Lista<klienci_kategorie>
    /// </summary>
    public class AktualizujKlienciKategorie : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<KlientKategoriaKlienta> doZmiany = (List<KlientKategoriaKlienta>)Data;
            if (doZmiany.Count > 0)
            {
                HashSet<long> klienci = new HashSet<long>( SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(null).Select(x=>x.Id) );
                HashSet<int> kategorie_Klienciw = new HashSet<int>( SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategoriaKlienta>(null).Select(x => x.Id) );
                doZmiany.RemoveAll(p => !klienci.Contains(p.KlientId));
                doZmiany.RemoveAll(p => !kategorie_Klienciw.Contains(p.KategoriaKlientaId));
                int packageNr = 0;
                int packegeSize = 1000;
                List<KlientKategoriaKlienta> paczka;
                do
                {
                    paczka = doZmiany.Skip(packageNr * packegeSize).Take(packegeSize).ToList();
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe < KlientKategoriaKlienta>(paczka);
                    packageNr++;
                }
                while (paczka.Count == packegeSize);
            }
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<KlientKategoriaKlienta>); }
        }
    }

    /// <summary>
    /// Pobiera łączniki kategorii klientów jako Słownik<klucz int ID, wartość klienci_kategorie> 
    /// </summary>
    public class PobierzKlienciKategorie : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            Dictionary<string, object> parametry = (Dictionary<string, object>)Data;
            bool wszystkie = false;

            if (parametry != null)
            {
                if (parametry.ContainsKey("wszystkie"))
                    wszystkie = (bool)parametry["wszystkie"];
            }
            Dictionary<long, KlientKategoriaKlienta> levels = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KlientKategoriaKlienta>(null,x => wszystkie || (x.KlientId > 0 && x.KategoriaKlientaId > 0)).ToDictionary(x => x.Id, x => x);
            return levels;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(Dictionary<string, object>); }
        }
    }

    /// <summary>
    /// Usuwa łączniki kategorii klientów wysłanych jako ListaID<int> w obiekcie Data
    /// </summary>
    public class UsunKlienciKategorie : ApiSessionBaseHandler
    {
        protected override object Handle()
        {

            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<KlientKategoriaKlienta, object>((List<object>)Data);
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<object>); }
        }
    }
}
