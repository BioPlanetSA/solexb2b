using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL
{

  

    public class ZamowieniaDostep : BllBazaCalosc, IZamowieniaDostep
    {
        public ZamowieniaDostep(ISolexBllCalosc calosc) : base(calosc)
        {
        }

        public void SprawdzStatusy_UtworzStatusySystemowe()
        {
            int jezyk = Calosc.Konfiguracja.JezykIDDomyslny;
            List<StatusZamowienia> statusy = Calosc.Konfiguracja.StatusyZamowien.Values.ToList();
            List<StatusZamowienia> dozapisu = new List<StatusZamowienia>();

            if (statusy.FirstOrDefault(x =>x.Id == (int) StatusImportuZamowieniaDoErp.Złożone) == null)
            {
                dozapisu.Add(new StatusZamowienia(StatusImportuZamowieniaDoErp.Złożone, true, jezyk));
            }
            if (statusy.FirstOrDefault(x => x.Id == (int)StatusImportuZamowieniaDoErp.Zaimportowane) == null)
            {
                dozapisu.Add(new StatusZamowienia(StatusImportuZamowieniaDoErp.Zaimportowane, false, jezyk));
            }
            if (statusy.FirstOrDefault(x => x.Id == (int)StatusImportuZamowieniaDoErp.Anulowane) == null)
            {
                dozapisu.Add(new StatusZamowienia(StatusImportuZamowieniaDoErp.Anulowane, false, jezyk));
            }
            if (statusy.FirstOrDefault(x => x.Id == (int)StatusImportuZamowieniaDoErp.Błąd) == null)
            {
                dozapisu.Add(new StatusZamowienia(StatusImportuZamowieniaDoErp.Błąd, true, jezyk));
            }
            if (statusy.FirstOrDefault(x => x.Id == (int)StatusImportuZamowieniaDoErp.Usunięte) == null)
            {
                dozapisu.Add(new StatusZamowienia(StatusImportuZamowieniaDoErp.Usunięte, false, jezyk));
            }
            if (statusy.FirstOrDefault(x => x.Id == (int)StatusImportuZamowieniaDoErp.Zrealizowane) == null)
            {
                dozapisu.Add(new StatusZamowienia(StatusImportuZamowieniaDoErp.Zrealizowane, false, jezyk));
            }
            if (dozapisu.Count > 0)
            {
                Calosc.DostepDane.AktualizujListe(dozapisu);
            }

            statusy = Calosc.DostepDane.Pobierz<StatusZamowienia>(null).ToList();
            if (!statusy.Any(p => p.Importowac))
            {
                StatusZamowienia s = statusy.FirstOrDefault();
                if (s != null)
                {
                    s.Importowac = true;
                    Calosc.DostepDane.AktualizujPojedynczy(s);
                }
            }


            //StatusZamowienia sta = statusy.FirstOrDefault(x => x.Id == 1);
            //List<StatusZamowienia> dozapisu = new List<StatusZamowienia>();
            //if (sta == null)
            //{
            //    dozapisu.Add(new StatusZamowienia { Id = 1, Importowac = true, Nazwa = "Złożone", Symbol = "Złożone", Widoczny = false, JezykId = jezyk });
            //}
            //else if (string.IsNullOrEmpty(sta.Symbol))
            //{
            //    sta.Symbol = "Złożone";
            //    dozapisu.Add(sta);
            //}
            //sta = statusy.FirstOrDefault(x => x.Id == 2);
            //if (sta == null)
            //{
            //    dozapisu.Add(new StatusZamowienia { Id = 2, Importowac = false, Nazwa = "Zaimportowane", Symbol = "Zapisane", Widoczny = false, JezykId = jezyk});
            //}
            //else if (string.IsNullOrEmpty(sta.Symbol))
            //{
            //    sta.Symbol = "Zapisane";
            //    dozapisu.Add(sta);
            //}
            //sta = statusy.FirstOrDefault(x => x.Id == 3);
            //if (sta == null)
            //{
            //    dozapisu.Add(new StatusZamowienia { Id = 3, Importowac = false, Nazwa = "Anulowane", Symbol = "Anulowane", Widoczny = false, JezykId = jezyk });
            //}
            //else if (string.IsNullOrEmpty(sta.Symbol))
            //{
            //    sta.Symbol = "Anulowane";
            //    dozapisu.Add(sta);
            //}
            //sta = statusy.FirstOrDefault(x => x.Id == 4);
            //if (sta == null)
            //{
            //    dozapisu.Add(new StatusZamowienia { Id = 4, Importowac = false, Nazwa = "Zrealizowane", Symbol = "Zrealizowane", Widoczny = false, JezykId = jezyk });
            //}
            //else if (string.IsNullOrEmpty(sta.Symbol))
            //{
            //    sta.Symbol = "Zrealizowane";
            //    dozapisu.Add(sta);
            //}
            //sta = statusy.FirstOrDefault(x => x.Id == 5);
            //if (sta == null)
            //{
            //    dozapisu.Add(new StatusZamowienia { Id = 5, Importowac = false, Nazwa = "Błąd", Symbol = "Błąd", Widoczny = false, JezykId = jezyk });
            //}
            //else if (string.IsNullOrEmpty(sta.Symbol))
            //{
            //    sta.Symbol = "Błąd";
            //    dozapisu.Add(sta);
            //}
            //sta = statusy.FirstOrDefault(x => x.Id == 6);
            //if (sta == null)
            //{
            //    dozapisu.Add(new StatusZamowienia { Id = 6, Importowac = false, Nazwa = "Usunięte", Symbol = "Usunięte", Widoczny = false, JezykId = jezyk });
            //}
            //else if (string.IsNullOrEmpty(sta.Symbol))
            //{
            //    sta.Symbol = "Usunięte";
            //    dozapisu.Add(sta);
            //}


        }

        //todo: TEST koniecznie czy dobrze sprawdza. dla przypadku: 1.zamowienie dla konkretnego klienta/ 2. zamowienie dla klienta subkonta 1 i 2 poziomu
        public bool SpradzCzyKlientMaPrawoDoZamowienia(ZamowieniaBLL arg1, IKlient arg2)
        {
            //jesli tu trafil to znaczy ze ktos cos chce podejzec - sprawdzemy tego ktosia
            return arg1.Klient.OddzialDoJakiegoNalezyKlient == arg2.OddzialDoJakiegoNalezyKlient &&
                    (arg1.Klient.KlientPodstawowy().PrzedstawicielId == arg2.Id || arg1.Klient.KlientPodstawowy().OpiekunId == arg2.Id ||
                    arg1.Klient.KlientPodstawowy().DrugiOpiekunId == arg2.Id);
        }


        public bool CzyKlientJestNadrzednymDlaSubkonta(ZamowieniaBLL zam, IKlient klient)
        {
            var k = Calosc.DostepDane.PobierzPojedynczy<Klient>(zam.KlientId);
            if (klient.Id == k.Id || k.KlientNadrzednyId == klient.Id)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Zamowienai sa filtrowane na podstawie zadajacego - np. dla oddzialow
        /// </summary>
        /// <param name="idZadajacego"></param>
        /// <returns></returns>
        public IEnumerable<ZamowienieSynchronizacja> PobierzZamowieniaOczekujaceNaImportDoERP(IKlient pobierajacy)
        {
            IList<int> statusyDoZaimportowania = Calosc.Konfiguracja.StatusyZamowien.Values.Where(p => p.Importowac || p.Symbol== StatusImportuZamowieniaDoErp.Złożone.ToString() 
            || p.Symbol == StatusImportuZamowieniaDoErp.Błąd.ToString()).Select(p => p.Id).ToList();
            
            IList<ZamowieniaBLL> oczekujaceZamowienia = Calosc.DostepDane.Pobierz<ZamowieniaBLL>(pobierajacy, x => Sql.In(x.StatusId, statusyDoZaimportowania)).OrderBy(x => x.DataUtworzenia).ToList();
       
            List<ZamowienieSynchronizacja> dowyslania = new List<ZamowienieSynchronizacja>();
            var poziomy = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<PoziomCenowy>(null);

            foreach (ZamowieniaBLL z in oczekujaceZamowienia)
            {
                ZamowienieSynchronizacja tmp = new ZamowienieSynchronizacja(z);
                tmp.Rozbijaj = true;
                if (z.Klient == null || !z.Klient.Aktywny)
                {                   
                    continue;
                }

                long? klientID = z.Klient.Id;
                while (klientID < 0)
                {
                    IKlient k = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Core.Klient>(klientID.GetValueOrDefault());
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

                if (tmp.WalutaId != null)
                {
                    var pc = poziomy.FirstOrDefault(x => x.WalutaId == tmp.WalutaId);
                    if (pc != null)
                    {
                        tmp.WalutaId = pc.WalutaId;
                    }
                }

                tmp.KlientId = klientID.Value;
                var poz = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZamowieniaProduktyBLL>(pobierajacy, x => x.DokumentId == tmp.Id, z);
                tmp.pozycje = poz.Select(p => new ZamowienieProdukt(p)).ToList();
                dowyslania.Add(tmp);
            }

            return dowyslania;
        }

        public int AktualizujZamowienia(ZamowieniaBLL update, List<ZamowieniaProduktyBLL> pozycje = null)
        {
            if ((pozycje != null && !pozycje.Any()) || (pozycje == null && update.Id == 0))
            {
                throw new InvalidOperationException("Próba zapisu zamówienia bez pozycji");
            }

            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(update);
            if (pozycje != null)
            {
                foreach (var variable in pozycje)
                {
                    variable.DokumentId = update.Id;
                }
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe(pozycje);
            }
            return update.Id;
        }

        public string GenerujNumerZamowieniaDlaOddzialu(IKlient klient, int rok)
        {
            //todo:numeracja PER klient pojedyńcza
            if (klient.OddzialDoJakiegoNalezyKlient != 0)
            {
                int kolejnyNumer = PobierzNumerDokumentuPartneraGlownaMetoda(klient.OddzialDoJakiegoNalezyKlient, rok);
                return $"{kolejnyNumer}/{rok}/{klient.OddzialDoJakiegoNalezyKlientNazwa}";
            }
            long iloscZamowien = 0;
            DateTime startRoku = new DateTime(rok, 1, 1);
            if (Calosc.Konfiguracja.DodajIdKlientaDoTymczasowegoNumeruZamowienia)
            {
                iloscZamowien = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.Count<Zamowienie>(x => x.KlientId == klient.Id && x.DataUtworzenia >= startRoku);
                return $"{iloscZamowien + 1}/{klient.Id}/{startRoku.ToString("yy")[1]}"; //- to nie jest raczej potrzebne skoro w zamowieniabll w polu NazwaDokumentu też coś takiego generuje, 11.07 - jest to potrzebne ze względu na fakt iż zostało stworzone pole tymczosowego numeru zamowienia ktore musi byc uzupelnione niezaleznie czy klient posiada jakis oddzial czy nie
            }
            iloscZamowien = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.Count<Zamowienie>(x => x.DataUtworzenia >= startRoku);
            return $"{iloscZamowien + 1}/{startRoku.ToString("yy")[1]}";
        }

      

        public int PobierzNumerDokumentuPartneraGlownaMetoda(long oddzialDoJakiegoNalezyKlient, int rok)
        {
            DateTime startRoku = new DateTime(rok, 1, 1 );
            DateTime koniecRoku = startRoku.AddYears(1).AddDays(-1);
            
            //bezposrednio uzycie ORM
            long IloscZamowien = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.Count<ZamowieniaBLL>(x => x.DataUtworzenia > startRoku && x.DataUtworzenia < koniecRoku && x.IdOddzialu == oddzialDoJakiegoNalezyKlient);
            return (int)(IloscZamowien + 1);
        }

        public void UsunStatusyCache(IList<object> list)
        {
            Calosc.Konfiguracja.PrzeladujResetujStatusy();
        }


        public List<StatusZamowienia> AktulizujStatusyZamowienien(List<StatusZamowienia> list)
        {
            if (list.Count == 0)
            {
                return new List<StatusZamowienia>();
            }
            foreach (StatusZamowienia s in list)
            {
                if ("#000000".Equals(s.Kolor, StringComparison.InvariantCultureIgnoreCase))
                {
                    s.Kolor = null;
                }
                if ("#000000".Equals(s.KolorCzcionki, StringComparison.InvariantCultureIgnoreCase))
                {
                    s.KolorCzcionki = null;
                }
                if (string.IsNullOrEmpty(s.Symbol))
                {
                    s.Symbol = TextHelper.PobierzInstancje.GetRandomString(8);
                }
            }
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe(list);
            Calosc.Konfiguracja.PrzeladujResetujStatusy();
            return list;
        }

        //public void DodajBrakujaceZamowienia(Dictionary<int, DokumentyBll> result)
        //{
        //    int jezykId = SesjaHelper.PobierzInstancje.JezykID;
        //    IList<ZamowieniaBLL> zamowieniawszystkie =  PobierzZamowienia(jezykId);
        //    foreach (var zamowienia in zamowieniawszystkie)
        //    {
        //        var kolekcja = new List<int>(zamowienia.ListaIdDokumentow());
        //        if (!kolekcja.Any())
        //        {
        //            kolekcja.Add(zamowienia.Id);
        //        }
        //        foreach (var id in kolekcja)
        //        {
        //            if (!result.ContainsKey(id))
        //            {
        //                DokumentyBll z = null;

        //                if (zamowienia.StatusId != 6)
        //                {
        //                    z = zamowienia;
        //                }

        //                if (z != null)
        //                {
        //                    try
        //                    {
        //                        result.Add(id, z);
        //                    }
        //                    catch (Exception)
        //                    {
        //                        SolexBllCalosc.PobierzInstancje.Log.Error("Bład dodawania sztucznego dokumentu " + z.Id);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

    }
}