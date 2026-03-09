using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Server;
using ServiceStack.Common;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Helper;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje zamówienia wysłane w obiekcie Data jako Lista<ZamowieniaSynchronizacja>
    /// Jeśli status zamówienia został zmieniony będzie wysłane powiadomienie dla danego statusu
    /// </summary>
    public class AktualizujZamowienia : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<ZamowienieSynchronizacja> listaZamowienSynchronizacji = (List<ZamowienieSynchronizacja>)Data;

            foreach (var x in listaZamowienSynchronizacji)
            {
                IKlient klient = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(Customer.Id);
                ZamowieniaBLL nowe = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ZamowieniaBLL>(x.Id,klient);
                if (nowe.StatusId != x.StatusId)
                {
                    nowe.StatusId = x.StatusId;
                }

                if (nowe.BladKomunikat != x.BladKomunikat)
                {
                    nowe.BladKomunikat = x.BladKomunikat;
                }
               
                //nowe.DokumentNazwaSynchronizacja = x.DokumentNazwaSynchronizacja;
                SolexBllCalosc.PobierzInstancje.ZamowieniaDostep.AktualizujZamowienia(nowe);

                //pomijanie gdy IdDokumentu ==0 i nazwa z erap jest pusta
                List<ZamowienieDokumenty> listaDokumentowZamowienia = x.ListaDokumentowZamowienia;
                if (listaDokumentowZamowienia != null)
                {
                    listaDokumentowZamowienia = x.ListaDokumentowZamowienia.Where(y => y.IdDokumentu != 0 && !string.IsNullOrEmpty(y.NazwaERP)).ToList();
                }
                if (listaDokumentowZamowienia != null && listaDokumentowZamowienia.Any())
                {
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<ZamowienieDokumenty>(listaDokumentowZamowienia);
                }
                //blad importu byl
                if (nowe.StatusId == StatusImportuZamowieniaDoErp.Błąd)
                {
                    SolexBllCalosc.PobierzInstancje.Statystyki.ZdarzenieBladImportu(nowe, nowe.Klient);
                }
                else
                {
                    //zaimportowane poprawnie
                    SolexBllCalosc.PobierzInstancje.Statystyki.ZdarzenieNoweZamowienie_PoImporcieERP(nowe, nowe.Klient);
                }
            }

           return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<ZamowienieSynchronizacja>); }
        }
    }

    /// <summary>
    /// Pobiera listę zamówień do importu jako Lista<ZamowienieSynchronizacja>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    public class PobierzZamowienia : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            return SolexBllCalosc.PobierzInstancje.ZamowieniaDostep.PobierzZamowieniaOczekujaceNaImportDoERP(Customer);
            
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<ZamowienieSynchronizacja>); }
        }

    }

    /// <summary>
    /// Pobiera listę wszystkich zamówień jako Lista<ZamowienieSynchronizacja>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    public class PobierzWszystkieZamowienia : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {

        protected override object Handle()
        {
            Dictionary<int, ZamowieniaBLL> oczekujaceZamowienia = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZamowieniaBLL>(null).ToDictionary(x=>x.Id,x=>x);
            List<ZamowienieSynchronizacja> dowyslania = new List<ZamowienieSynchronizacja>();
            var poziomy = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<PoziomCenowy>(null);
            foreach (ZamowieniaBLL z in oczekujaceZamowienia.Values)
            {
                ZamowienieSynchronizacja tmp = new ZamowienieSynchronizacja(z);
                tmp.Rozbijaj = true;
                long? klientID = z.Klient.Id;
                while (klientID < 0)
                {
                    IKlient k = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(klientID.GetValueOrDefault());
                    klientID = k.KlientNadrzednyId;
                    if (klientID == null)
                    {
                        break;
                    }
                }
                if (klientID == null || klientID < 0)
                {
                    continue;
                }
                if (tmp.WalutaId!=null)
                {
                    var pc = poziomy.FirstOrDefault(x => x.WalutaId == tmp.WalutaId);
                    if (pc != null)
                    {
                        tmp.WalutaId = pc.WalutaId;
                    }
                }
                tmp.KlientId = klientID.Value;
                tmp.pozycje = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZamowieniaProduktyBLL>(null, x => x.DokumentId == tmp.Id).Select(p => new ZamowienieProdukt(p)).ToList();
                dowyslania.Add(tmp);
            }

            return dowyslania;
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<ZamowienieSynchronizacja>); }
        }

    }



    /// <summary>
    /// Pobiera wszystkie dostępne statusy zamówień jako Lista<zamowienia_statusy>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Pracownik)]
    public class PobierzZamowieniaStatusy : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {

        protected override object Handle()
        {
            return SolexBllCalosc.PobierzInstancje.Konfiguracja.StatusyZamowien.Values.ToList();
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(Model.StatusZamowienia); }
        }
    }
    /// <summary>
    /// Aktualizuje  statusy zamówień 
    /// </summary>
    [ApiUprawnioneRole(RoleType.Pracownik)]
    public class AktualizujZamowieniaStatusy : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {

        protected override object Handle()
        {
            SolexBllCalosc.PobierzInstancje.ZamowieniaDostep.AktulizujStatusyZamowienien((List<StatusZamowienia>) Data);
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<StatusZamowienia>); }
        }
    }
    /// <summary>
    /// Usuwa  statusy zamówień 
    /// </summary>
    [ApiUprawnioneRole(RoleType.Pracownik)]
    public class UsunZamowieniaStatusy : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {

        protected override object Handle()
        {
            var dane = (HashSet<int>) Data;
            if (dane != null && dane.Any())
            {
                SolexBllCalosc.PobierzInstancje.DostepDane.Usun<StatusZamowienia, int>(dane.ToList());
            }
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<int>); }
        }
    }
}
