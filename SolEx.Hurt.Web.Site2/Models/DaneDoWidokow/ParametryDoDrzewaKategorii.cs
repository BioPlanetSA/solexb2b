using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci;

namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow
{
    public class ParametryDoDrzewaKategorii
    {
        public ParametryDoDrzewaKategorii() { }
        public ParametryDoDrzewaKategorii(Dictionary<long, List<KategorieBLL>> wszystkieWidoczneKategorie, IKlient klient, Dictionary<int, HashSet<long>> staleWybraneFiltry,
                                          Dictionary<long, int> slownikIlosciProduktowWKategoriach,IDrzewoKategorii kontrolka, IList<GrupaBLL> posortowaneGrupyKategorii)
        {
            WszystkieWidoczneKategorie = wszystkieWidoczneKategorie;
            Klient = klient;
            WybraneStaleFiltry = staleWybraneFiltry;
            PosortowaneGrupyKategorii = posortowaneGrupyKategorii;
            SlownikIlosciProduktowWKategoriach = slownikIlosciProduktowWKategoriach;
            Kontrolka = kontrolka;
            IdKontrolki = kontrolka.Id;
        }

        public IDrzewoKategorii Kontrolka { get; set; }

        public string szukanieGlobalne { get; set; }

        public int IdKontrolki { get; set; }
        public bool PrzepelnienieNaLiscie { get; private set; }

        public long? WybranaKategoriaID { get; set; }

        public IList<GrupaBLL> PosortowaneGrupyKategorii { get; private set; }

        public Dictionary<long, int> SlownikIlosciProduktowWKategoriach { get; private set; }

        public Dictionary<long, List<KategorieBLL>> WszystkieWidoczneKategorie { get; private set; }
        public IKlient Klient { get; set; }
        public long? WybranaGrupa { get; set; }
        public Dictionary<int, HashSet<long>> WybraneStaleFiltry { get; set; }
    }

    public class ParametryDoElementuDrzewaKategorii
    {
        public ParametryDoElementuDrzewaKategorii(KategorieBLL kategoriaAktualnieRenderowana, int poziom, ParametryDoDrzewaKategorii parametryDrzewaKategorii)
        {
            Kategoria = kategoriaAktualnieRenderowana;
            Poziom = poziom;
            ParametryDrzewaKategorii = parametryDrzewaKategorii;
            widoczneDzieciKategorii = ParametryDrzewaKategorii.WszystkieWidoczneKategorie[Kategoria.GrupaId].Where(x => x.ParentId == Kategoria.Id).ToList();
            //widoczneDzieciKategorii = parametryDrzewaKategorii.WszystkieWidoczneKategorie[this.Kategoria.Grupa].Where(x => x.ParentId == this.Kategoria.Id).ToList();
        }
        public ParametryDoDrzewaKategorii ParametryDrzewaKategorii { get; private set; }
        public KategorieBLL Kategoria { get; set; }
        public int Poziom { get; set; }
        public IList<KategorieBLL> widoczneDzieciKategorii { get; private set; }

    }
}
