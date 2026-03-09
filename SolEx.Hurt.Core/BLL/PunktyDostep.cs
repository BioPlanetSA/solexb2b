using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.RegulyPunktowe;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL
{
    public class PunktyDostep : LogikaBiznesBaza, IPunktyDostep
    {
        public PunktyDostep(ISolexBllCalosc calosc) : base(calosc)
        {
        }

        public List<PunktyWpisy> PobierzWpisyZBazy(long klient)
        {
            return Calosc.DostepDane.Pobierz<PunktyWpisy>(null, x => x.KlientId == klient).ToList();
        }

        public void DodajPunkty(PunktyWpisy wpis)
        {
            List<PunktyWpisy> tab = new List<PunktyWpisy> { wpis };
            Calosc.DostepDane.AktualizujListe(tab);
        }

        public void ZdejmijPunkty(PunktyWpisy wpis)
        {
            wpis.IloscPunktow = (-1) * wpis.IloscPunktow;
            List<PunktyWpisy> tab = new List<PunktyWpisy> { wpis };
            Calosc.DostepDane.AktualizujListe<PunktyWpisy>(tab);
        }

        public void UsunPunkty(int id)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.UsunPojedynczy<PunktyWpisy>(id);
        }

        public List<PunktyWpisy> PobierzPunktyKlienta(IKlient klient)
        {
            List<PunktyWpisy> wynik = new List<PunktyWpisy>();

            var tmp = Calosc.ZadaniaBLL.PobierzZadania<RegulaPunktowa, RegulaPunktowa>(Calosc.Konfiguracja.JezykIDPolski, klient).ToArray();
            var regdok = tmp.Where(x => x as RegulaPunktowaCalegoDokumentu != null).Select(x => (RegulaPunktowaCalegoDokumentu)x).ToList();
            var regpoz = tmp.Where(x => x as RegulaPunktowaPozycjiDokumentu != null).Select(x => (RegulaPunktowaPozycjiDokumentu)x).ToList();
            if (regdok.Any() || regpoz.Any())
            {
                IList<DokumentyBll> fakturyklienta = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<DokumentyBll>(klient, x => ((HistoriaDokumentu) x).Rodzaj == RodzajDokumentu.Faktura && x.KlientId==klient.Id);
                foreach (DokumentyBll dokument in fakturyklienta)
                {
                    decimal punktyZmodulowPozycjiDokumentow = 0;
                    int punktyZmodulowCalego = 0;
                    foreach (var rpcd in regdok)
                    {
                        bool przetwarzacNastepny = true;
                        if (rpcd.CzySpelniaKryteria(dokument))
                        {
                            punktyZmodulowCalego += rpcd.WyliczPunkty(dokument, punktyZmodulowCalego);
                            przetwarzacNastepny = rpcd.PrzetwarzacNastepneRegulyDlaDokumentu;
                        }

                        if (!przetwarzacNastepny)
                        {
                            break;
                        }
                    }

                    decimal punktyzatendok = 0;
                    if (regpoz.Any())
                    {
                        foreach (var pozycja in dokument.PobierzPozycjeDokumentu())
                        {
                            foreach (var rppd in regpoz)
                            {
                                bool przetwarzacNastepny = true;
                                if (rppd.CzySpelniaKryteria(pozycja, dokument))
                                {
                                    punktyzatendok += rppd.WyliczPunkty(pozycja, dokument, punktyzatendok);
                                    przetwarzacNastepny = rppd.PrzetwarzacNastepneRegulyDlaDokumentu;
                                }

                                if (!przetwarzacNastepny)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    punktyZmodulowPozycjiDokumentow += (int)Math.Round(punktyzatendok);
                    if (punktyZmodulowCalego + punktyZmodulowPozycjiDokumentow != 0)
                    {
                        wynik.Add(new PunktyWpisy(klient.Id, punktyZmodulowCalego + punktyZmodulowPozycjiDokumentow, dokument.DataUtworzenia.Date, "System", dokument.NazwaDokumentu));
                    }
                }
            }
            wynik.AddRange(PobierzWpisyZBazy(klient.Id));
            return wynik.OrderBy(x => x.Data).ToList();
        }

        public decimal PobierzPunktyKlientaLacznie(IKlient klient)
        {
            return
                PobierzPunktyKlienta(klient).Sum(x => x.IloscPunktow);
        }

        public List<PunktyWpisy> PobierzPunktyKlienta(IKlient klient, DateTime odKiedy, DateTime doKiedy, string sortowanie, KolejnoscSortowania kierunek, string szukanieFraza)
        {
            List<PunktyWpisy> docs = PobierzPunktyKlienta(klient).Where(x => x.Data.Date.JestWPrzedziale(odKiedy, doKiedy)).ToList();
            docs = SolexBllCalosc.PobierzInstancje.Szukanie.WyszukajObiekty(docs, szukanieFraza, new[] { "Opis" }).ToList();
            docs = SolexBllCalosc.PobierzInstancje.Szukanie.SortujObiekty(docs, sortowanie, kierunek).ToList();
            return docs;
        }

        public bool KlientMaDostepDoModulu(IKlient klient)
        {
            if (!SolexBllCalosc.PobierzInstancje.Konfiguracja.GetLicense(Licencje.Punkty))
            {
                return false;
            }
            return klient.WidziPunkty.GetValueOrDefault(SolexBllCalosc.PobierzInstancje.Konfiguracja.DomyslnaWidocznoscPunktow);
        }
    }
}