using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.OrmLite;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL
{
    public class AdresyBLL : LogikaBiznesBaza, IAdresyDostep
    {
        public AdresyBLL(ISolexBllCalosc calosc) : base(calosc)
        {
        }

        /// <summary>
        /// Metoda pobierająca adresy klientow
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public IList<IAdres> PobierzAdresyKlienta(IKlient k)
        {
            List<long> laczniki = Calosc.DostepDane.Pobierz<KlientAdres>(k, x => x.KlientId == k.Id).Select(x => x.AdresId).ToList();
            List<IAdres> wynik = Calosc.DostepDane.Pobierz<Adres>(null).Where(x => Sql.In(x.Id,laczniki) || x.AutorId == k.Id).Select(x => x as IAdres).ToList();

            if (k.KlientNadrzedny != null)
            {
                wynik.AddRange(PobierzAdresyKlienta(k.KlientNadrzedny));
            }
            return wynik.Distinct().ToList();
        }

        public void AktualizujLacznikiKlientow(IList<Adres> obj)
        {
            Dictionary<long, KlientAdres> doAktualizacji = new Dictionary<long, KlientAdres>();
            foreach (Adres adres in obj)
            {
                if (adres.KlientId == null && adres.AutorId == null) continue;
                var newLacznik = new KlientAdres();
                newLacznik.KlientId = adres.KlientId??(long)adres.AutorId;
                newLacznik.AdresId = adres.Id;
                newLacznik.TypAdresu = adres.TypAdresu;
                if (!doAktualizacji.ContainsKey(newLacznik.Id))
                    doAktualizacji.Add(newLacznik.Id, newLacznik);
            }
            Calosc.DostepDane.AktualizujListe<KlientAdres>(doAktualizacji.Values.ToList());

            Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd(Calosc.DostepDane.KluczCacheTypDanych<Adres>());
            Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd(Calosc.DostepDane.KluczCacheTypDanych<KlientAdres>());
        }

        public IList<Adres> UzupelnijPozycjePoSelect(int jezykId, IKlient zadajacy, IList<Adres> objekty, object parametrDoMetodyPoSelect)
        {
            Dictionary<int, Kraje> slownikKrajow = Calosc.DostepDane.Pobierz<Kraje>(jezykId,null).ToDictionary(x=>x.Id,x=>x);
            Dictionary<int, Region> slownikRegionow = Calosc.DostepDane.Pobierz<Region>(jezykId, null).ToDictionary(x => x.Id, x => x);

            string sqlZamowienia = "SELECT distinct AdresId FROM Zamowienie";
            List<long> adresyIdUzyte = Calosc.DostepDane.DbORM.Select<long>(sqlZamowienia);

            foreach (Adres adres in objekty)
            {
                if (adres.KrajId.HasValue)
                {
                    Kraje kraj;
                    if(slownikKrajow.TryGetValue(adres.KrajId.Value, out kraj))
                    {
                        adres.Kraj = kraj.Nazwa;
                        adres.KrajSymbol = kraj.Symbol;
                    }
                }
                if (adres.RegionId.HasValue)
                {
                    Region region;
                    if (slownikRegionow.TryGetValue(adres.RegionId.Value, out region))
                    {
                        adres.Region = region.Nazwa;
                    }
                }


                adres.MoznaEdytowac = !adresyIdUzyte.Contains(adres.Id);
                adres.CzyUzyty = adres.MoznaEdytowac && Calosc.DostepDane.Pobierz<KlientAdres>(null,x => x.AdresId == adres.Id).Any();
            }
            return objekty;
        }
    }
}