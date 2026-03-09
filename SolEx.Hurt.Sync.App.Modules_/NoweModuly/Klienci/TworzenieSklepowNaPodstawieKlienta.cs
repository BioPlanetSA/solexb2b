using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    public class TworzenieSklepowNaPodstawieKlienta : SyncModul, IModulKlienci
    {
        public ILogiFormatki LogiFormatki = Hurt.Core.Sync.LogiFormatki.PobierzInstancje;
        public IConfigSynchro Config = SyncManager.PobierzInstancje.Konfiguracja;
        public TworzenieSklepowNaPodstawieKlienta()
        {
            NazwaPole  = "nazwa";
        }
        public override string uwagi
        {
            get { return "Tworzy sklepy na podstawie klienta, główny adres klienta jest adresem sklepu"; }
        }
        
        [FriendlyName("Z którego pola brać nazwę sklepu, jesli jest puste to nazwą będzie nazwa klienta")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaPole { get; set; }
        
        [FriendlyName("Z którego pola brać adres www sklepu")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string AdresWWWPole { get; set; }

        [Niewymagane]
        [FriendlyName("Z którego pola brać lokalizację sklepu (opcjonalne)- (współrzedne zapisane w formaie - lat(liczba) lon(liczba))")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Lokalizacja { get; set; }

        [Niewymagane]
        [PobieranieSlownika(typeof(SlownikKategoriiKlienta))]
        [FriendlyName("Kategoria klientów do której musi należeć klient")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public HashSet<int> KategoriaKlienta { get; set; }
        
        [FriendlyName("Kategoria sklepów")]
        [PobieranieSlownika(typeof(SlownikKategoriiSklepow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string KategoriaSklepow { get; set; }
        
        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {           
            KategoriaSklepu sk = KategorieB2B.FirstOrDefault(x => x.Id.ToString()==KategoriaSklepow);
            if (sk == null)
            {
                LogiFormatki.LogujInfo("Brak kategorii sklepów o id {0}, moduł kończy działanie",KategoriaSklepow);
                return;                
            }

            List<Sklep> nowe=new List<Sklep>();
            HashSet<long> klienciZWlasciwymiLacznikami = new HashSet<long>( laczniki.Where(x => this.KategoriaKlienta.Contains(x.KategoriaKlientaId)).Select(x => x.KlientId) );
            List<Klient> listaKlientow = listaWejsciowa.Where(x => x.Value.Aktywny && klienciZWlasciwymiLacznikami.Contains(x.Key)).Select(x => x.Value).ToList();

            if (!klienciZWlasciwymiLacznikami.Any())
            {
                LogiFormatki.LogujInfo("Brak aktywnych klientów w kategorii o id {0}, moduł kończy działanie", KategoriaKlienta.Select(x=>x.ToString()).ToList().Join(",") );
                return;
            }

            LogiFormatki.LogujInfo($"Pasujcych klientów: {klienciZWlasciwymiLacznikami.Count}");
            var propertisy = typeof(Klient).Properties();
            //PropertyInfo propnazwa = propertisy.First(x => x.Name == NazwaPole);
            //PropertyInfo propwww = null;
            //if (!string.IsNullOrEmpty(AdresWWWPole))
            //{
            //   propwww= propertisy.First(x => x.Name == AdresWWWPole);
            //}

            //PropertyInfo proplokalizacja = null;
            //if (!string.IsNullOrEmpty(Lokalizacja))
            //{
            //    proplokalizacja = propertisy.First(x => x.Name == Lokalizacja);
            //}

            List<Sklep> sklepyb2b = ApiWywolanie.PobierzSklepy().Values.Where(x => x.Id > 0).ToList();
            Dictionary<long, Adres> adresyb2b = ApiWywolanie.PobierzAdresy();
            LogiFormatki.LogujInfo($"Ilość sklepów na platformie: {sklepyb2b.Count}, ilość adresów na B2B: {adresyb2b.Count}.");

            var akcesor = typeof(Klient).PobierzRefleksja();

               foreach (Klient kl in listaKlientow)
            {
                Sklep istniejacySklepNaB2B = sklepyb2b.FirstOrDefault(x => x.Id == kl.Id);

                if (istniejacySklepNaB2B == null)
                {
                    LogiFormatki.LogujDebug($"Brak sklepu dla klienta: {kl.Id}");
                }
                
                Sklep tmp = new Sklep();
                nowe.Add(tmp);
               
                tmp.Id = kl.Id;
                object name = akcesor[kl, NazwaPole];
                tmp.Nazwa = (name ?? kl.Nazwa).ToString();
                tmp.Aktywny = true;
                tmp.DataUtworzenia = istniejacySklepNaB2B?.DataUtworzenia ?? DateTime.Now;
                tmp.AutorId = istniejacySklepNaB2B?.AutorId;
                tmp.AutomatyczneKoordynaty = istniejacySklepNaB2B?.AutomatyczneKoordynaty ?? true;
                tmp.KoordynatyZERP = istniejacySklepNaB2B?.KoordynatyZERP ?? true;

                Adres adres = adresyWErp.Where(x => x.Value.KlientId == kl.Id).Select(x=>x.Key).FirstOrDefault();
                if (adres == null)
                {
                    LogiFormatki.LogujDebug("Brak adresu");
                    adres = new Adres();
                }

                adres.Email = kl.Email;
                Adres adresTmp = new Adres();
                adresTmp.Id = adres.Id;
                adresTmp.KodPocztowy = adres.KodPocztowy;
                adresTmp.Miasto = adres.Miasto;
                adresTmp.KrajId = adres.KrajId == 0 ? null : adres.KrajId;
                adresTmp.Telefon = kl.Telefon;
                adresTmp.UlicaNr = adres.UlicaNr;
                adresTmp.Email = adres.Email;
                adresTmp.DataDodania =DateTime.Now; 

                tmp.Opis = kl.Opis;
                if (!string.IsNullOrEmpty(AdresWWWPole))
                {
                    object www = akcesor[kl, AdresWWWPole];
                    tmp.LinkUrl = new AdresUrl(Tools.PobierzInstancje.PoprawAdresWWW((www ?? "").ToString()));
                    tmp.LinkUrl.Tryb = TrybOtwierania.NoweOkno;
                }

                string lokalizacja = null;
                if (!string.IsNullOrEmpty(Lokalizacja))
                {
                    var wartosc = akcesor[kl, Lokalizacja];
                    if (wartosc != null)
                    {
                        lokalizacja = wartosc.ToString();
                    }
                }

                //czy adres już istnieje
                Adres istniejacyAdres = null;
                if (istniejacySklepNaB2B != null && istniejacySklepNaB2B.AdresId.HasValue)
                {
                    if (!adresyb2b.TryGetValue(istniejacySklepNaB2B.AdresId.Value, out istniejacyAdres))
                    {
                        throw  new Exception($"Nie można pobrać adresu id: {istniejacySklepNaB2B.AdresId.Value} - dla sklepu id: {istniejacySklepNaB2B.Id}. Sklep ma ten adres na B2B ustawiony, ale widocznie brakuje adresu na B2B - błąd bazy danych B2B. Koniec pracy.");
                    }

                    if (!istniejacySklepNaB2B.AutomatyczneKoordynaty || istniejacyAdres.TakiSamAdres(adresTmp))
                    {
                        tmp.AdresId = istniejacySklepNaB2B.AdresId;
                        if (!CzyPoprawneKoordynaty(istniejacyAdres))
                        {
                            if (istniejacySklepNaB2B.AutomatyczneKoordynaty)
                            {
                                LogiFormatki.LogujInfo($"Sklep {tmp.Nazwa} (adres id: {istniejacyAdres.Id}) nie posiada koordynatów, będą one pobrane. Obecne koordynaty: lat{istniejacyAdres.Lat} lon{istniejacyAdres.Lon}. Koordynaty z ERP: '{lokalizacja}'.");
                                AktualizujLokalizacje(lokalizacja, ref istniejacyAdres, tmp);
                            }
                            else
                            {
                                LogiFormatki.LogujInfo($"Sklep { tmp.Nazwa} posiada koordynaty wpisane w adminie ale nie są poprawne.");
                            }
                        }
                        AktualizujAdres(ref adresyWErp, istniejacyAdres);
                        continue;
                    }
                }
                
                AktualizujLokalizacje(lokalizacja, ref adresTmp, tmp);
                AktualizujAdres(ref adresyWErp, adresTmp);

                if (adresTmp.Id > 0)
                {
                    tmp.AdresId = adresTmp.Id;
                }
            }

            LogiFormatki.LogujInfo($"Liczba sklepów po przejściu modułu: {nowe.Count}");
            foreach (var n in nowe)
            {
                DodajLacznik(sklpeylaczniki, n, sk);
                if (sklepy.All(x => x.Id != n.Id))
                {
                    sklepy.Add(new Sklep(n));
                }
            }
        }

        private void AktualizujLokalizacje(string lokalizacja, ref Adres adresTmp, Sklep tmp)
        {
            if (!string.IsNullOrEmpty(lokalizacja))
            {
                string[] koordynaty = lokalizacja.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                decimal lat;
                decimal lon;
                if (koordynaty.Length == 2 && TextHelper.PobierzInstancje.SprobojSparsowac(koordynaty[0], out lon) && TextHelper.PobierzInstancje.SprobojSparsowac(koordynaty[1], out lat))
                {
                    adresTmp.Lat = lat;
                    adresTmp.Lon = lon;
                }
                Log.Debug($"Dla sklepu {tmp.Nazwa} pobrano koordynaty z pola ERP: {lokalizacja}: lat: { adresTmp.Lat}, lon: {adresTmp.Lon}.");
            }

            if (!CzyPoprawneKoordynaty(adresTmp))
            {
                SolexBllCalosc.PobierzInstancje.Sklepy.UzupelnijWspolrzedne(adresTmp);
                if (CzyPoprawneKoordynaty(adresTmp))
                {
                    Log.Debug($"Dla sklepu {tmp.Nazwa} pobrano koordynaty automatyczne lat: {adresTmp.Lat}, lon: {adresTmp.Lon}.");
                }
                else
                {
                    LogiFormatki.LogujInfo($"Błąd - dla sklepu {tmp.Nazwa} nie można pobrać koordynatów! Wyłącz sklep lub popraw dane. Pomijam sklep.");
                }
            }
        }

        private static void AktualizujAdres(ref Dictionary<Adres, KlientAdres> adresyWErp, Adres adresTmp)
        {
            Dictionary<Adres, KlientAdres> tmpAdresyWErp = new Dictionary<Adres, KlientAdres>();
            foreach (var adr in adresyWErp)
            {
                Adres key = adr.Key;
                if (adr.Key.Id == adresTmp.Id)
                {
                    key = adresTmp;
                }
                tmpAdresyWErp.Add(key, adr.Value);
            }
            adresyWErp = tmpAdresyWErp;
        }

        private bool CzyPoprawneKoordynaty(Adres adr) { return adr.Lat != 0 && adr.Lon != 0 && adr.Lat != -1 && adr.Lon != -1; }

        public void DodajLacznik(List<SklepKategoriaSklepu> laczniki, Sklep sklep, KategoriaSklepu kategoria)
        {
            if (!laczniki.Any(x => x.SklepId == sklep.Id && x.KategoriaSklepuId == kategoria.Id))
            {
                laczniki.Add(new SklepKategoriaSklepu { SklepId = sklep.Id, KategoriaSklepuId = kategoria.Id });
            }
        }
        private List<KategoriaSklepu> _kategorieB2B;

       // private List<Sklep> _sklepyB2B;
        public virtual List<KategoriaSklepu> KategorieB2B
        {
            get
            {
                if (_kategorieB2B == null)
                {
                    _kategorieB2B = ApiWywolanie.PobierzSklepyKategorie().Values.ToList();
                }
                return _kategorieB2B;
            }
        }
    }
}
