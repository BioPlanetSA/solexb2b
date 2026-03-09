using log4net;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Modele;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;

namespace SolEx.Hurt.Core.BLL
{
    public class Sklepy : LogikaBiznesBaza, ISklepy
    {
        public Sklepy(ISolexBllCalosc calosc)
            : base(calosc)
        {
            
        }

        private string CachePolaczenia()
        {
            _kategorieSklepowIDZSklepamiAktywnymiINaMapie = null;
            return "sklepy_kategorie_polaczenia";
        }

        private ILog _log
        {
            get
            {
                return LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        public string ConstructGeoCodeUrl(string address)
        {
            address = address.Trim();
            address = address.Replace(" ", "+");
            string geoUrl = "https://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false&key=AIzaSyAxaTrE6dR9C4DkP9ycpRZSgYlJQLr-KL4";
            geoUrl = string.Format(geoUrl, address);
            return geoUrl;
        }

        public virtual bool LocationGeoCode(Model.Adres sklep, out decimal lat, out decimal lon)
        {
            string address = string.IsNullOrEmpty(sklep.UlicaNr) ? "" : sklep.UlicaNr.Replace("ul.", "").Replace("ul", "").Trim();
            if (!string.IsNullOrEmpty(sklep.KodPocztowy))
            {
                address += " " + sklep.KodPocztowy;
            }
            string geoUrl = ConstructGeoCodeUrl(sklep.Miasto + " " + address);
            string csvValues = "";
            lat = 0;
            lon = 0;
            bool limitprzekroczony = false;
            try
            {
                System.Threading.Thread.Sleep(500);
                WebRequest objWebRequest = WebRequest.Create(geoUrl);
                WebResponse objWebResponse = objWebRequest.GetResponse();

                Stream objWebStream = objWebResponse.GetResponseStream();
                if (objWebStream != null)
                {
                    using (StreamReader objStreamReader = new StreamReader(objWebStream))
                    {
                        csvValues = objStreamReader.ReadToEnd();
                    }
                }
                if (!(string.IsNullOrEmpty(csvValues)))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(csvValues);
                    const string latpath = "GeocodeResponse/result/geometry/location/lat";
                    const string lonpath = "GeocodeResponse/result/geometry/location/lng";
                    var latnodes = xmlDoc.SelectNodes(latpath);
                    var lonnodes = xmlDoc.SelectNodes(lonpath);
                    if (latnodes != null)
                    {
                        foreach (XmlNode childrenNode in latnodes)
                        {
                            if (!TextHelper.PobierzInstancje.SprobojSparsowac(childrenNode.InnerText, out lon))
                            {
                                throw new Exception("Parsowanie lon");
                            }
                        }
                    }
                    if (lonnodes != null)
                    {
                        foreach (XmlNode childrenNode in lonnodes)
                        {
                            if (!TextHelper.PobierzInstancje.SprobojSparsowac(childrenNode.InnerText, out lat))
                            {
                                throw new Exception("Parsowanie lat");
                            }
                        }
                    }
                }
                if (lat == 0 || lon == 0)
                {
                    throw new Exception(csvValues);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("OVER_QUERY_LIMIT"))
                {
                    limitprzekroczony = true;
                    _log.Info("Przekroczony limit pobierania koordynatów");
                }
                else if (ex.Message.Contains("ZERO_RESULTS"))
                {
                    _log.InfoFormat("Nie udało pobrać się koordynatów dla sklepu o id {0} i nazwie {1}. Google zwróciło 0 wyników dla podanego adresu.", sklep.Id, sklep.Nazwa);
                }
                else
                {
                    _log.Error(string.Format("Google Geocode sklep id {0} nazwa {1}", sklep.Id, sklep.Nazwa));
                    _log.Error(ex);
                }
            }
            return limitprzekroczony;
        }

        public void Zapisz(ISklepyBll item, IKlienci zapisujacy)
        {
            var upd = new Sklep(item);
            Zapisz(new List<Sklep> { upd }, zapisujacy);

            int jezykId = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny;
            if (zapisujacy != null)
            {
                jezykId = zapisujacy.JezykId;
            }

            List<SklepKategoriaSklepu> polaczenia = item.PobierzKategorie(jezykId).Select(k => new SklepKategoriaSklepu { SklepId = upd.Id, KategoriaSklepuId = k.Id }).ToList();
            List<long> ids = Calosc.DostepDane.Pobierz<SklepKategoriaSklepu>(null, x => x.SklepId == item.Id).Select(x => x.Id).ToList();
            Calosc.DostepDane.Usun<SklepKategoriaSklepu,long>(ids);
            Calosc.DostepDane.AktualizujListe<SklepKategoriaSklepu>(polaczenia);
            WyczyscCache();
        }

        public bool UzupelnijWspolrzedne(Model.Adres sklep)
        {
            decimal lat, lon;
            LocationGeoCode(sklep, out lat, out lon);
            if (lat != 0 && lon != 0) //aktualizujemy tylko jeśli udało się pobrać poprawne
            {
                sklep.Lat = lat;
                sklep.Lon = lon;
                return true;
            }
            return false;
        }

        public void Zapisz(IList<Sklep> doZmiany, IKlienci zapisujacy)
        {
            foreach (var sklepy in doZmiany)
            {
                if (zapisujacy != null)
                    sklepy.AutorId = zapisujacy.Id;
                if (sklepy.LinkUrl != null)
                    sklepy.LinkUrl.Url = Tools.PobierzInstancje.PoprawAdresWWW(sklepy.LinkUrl.Url);
            }
            if (doZmiany.Count > 0)
            {

                Calosc.DostepDane.AktualizujListe<Sklep>(doZmiany);
            }
        }

        internal void WyczyscCache()
        {
            Calosc.Cache.UsunObiekt(CachePolaczenia());
        }

        public string PobierzIkoneNaMape(ISklepyBll v, int jezykId)
        {
            string mapa = Calosc.Konfiguracja.IkonaMapy;

            var kat = v.PobierzKategorie(jezykId).FirstOrDefault(x => x.ObrazekPineskaId != null);
            if (kat != null && kat.ObrazekPineskaId.HasValue)
            {
                mapa = PobierzIkoneKategorii(kat);
            }
            return mapa;
        }


        public Dictionary<long, string> PobierzIkonyNaMape(int jezykId)
        {
            Dictionary<long, string> slownikPinesekDlaKategorii = new Dictionary<long, string>();
            IList<KategoriaSklepu> kategorieSklepu = Calosc.DostepDane.Pobierz<KategoriaSklepu>(jezykId, null );
            foreach (var kategoriaSklepu in kategorieSklepu)
            {
                if (kategoriaSklepu.ObrazekPineskaId.HasValue)
                {
                    slownikPinesekDlaKategorii.Add(kategoriaSklepu.Id, PobierzIkoneKategorii(kategoriaSklepu));
                }
            }

            return slownikPinesekDlaKategorii;
        }


        public string PobierzIkoneKategorii(KategoriaSklepu kat)
        {
            string mapa = Calosc.Konfiguracja.IkonaMapy;
            if (kat.ObrazekPineskaId.HasValue)
            {
                mapa = Calosc.Pliki.PobierzObrazek(kat.ObrazekPineskaId.Value).LinkWWersji("pineska-na-mapie-mid");
            }
            return mapa;
        }

        public void AktualizujSklepMapaSklepow()
        {
            KategoriaSklepu kat_Glowna = Calosc.DostepDane.Pobierz<KategoriaSklepu>(null, x => x.Automatyczna).FirstOrDefault();
            if (kat_Glowna == null)
            {
                kat_Glowna = new KategoriaSklepu();
                kat_Glowna.Nazwa = "Siedziba firmy";
                kat_Glowna.Automatyczna = true;
                kat_Glowna.PokazywanaNaMapie = false;
                Calosc.DostepDane.AktualizujPojedynczy(kat_Glowna);
            }

            var item = Calosc.DostepDane.Pobierz<SklepyBll>(null, x => x.Siedziba && x.AdresId != null).FirstOrDefault();
            if (item == null)
            {
                item = new SklepyBll();
                DateTime d = DateTime.Now;
                item.DataUtworzenia = d;
                item.Siedziba = true;
            }

            item.Aktywny = true;
            item.Nazwa = Calosc.Konfiguracja.wlasciciel_nazwa;
            item.Opis = "Głowna siedziba";

            Model.Adres adresSklepu = item.AdresId != null ? Calosc.DostepDane.PobierzPojedynczy<Model.Adres>(item.AdresId) : null;
            if (adresSklepu == null)
            {
                adresSklepu = new Model.Adres();
            }
            adresSklepu.Miasto = Calosc.Konfiguracja.wlasciciel_adres_miasto;
            adresSklepu.KodPocztowy = Calosc.Konfiguracja.wlasciciel_adres_kod;
            adresSklepu.UlicaNr = Calosc.Konfiguracja.wlasciciel_adres_ulica;
            UzupelnijWspolrzedne(adresSklepu);
            Calosc.DostepDane.AktualizujPojedynczy(adresSklepu);
            item.AdresId = adresSklepu.Id;
            item.KategorieId = new[] { kat_Glowna.Id };
            item.AutomatyczneKoordynaty = true;
            Zapisz(item, null);
        }

        /// <summary>
        /// Dodaje Id adresu do sklepu przed jego zapisaniem
        /// </summary>
        /// <param name="obj"></param>
        public void DodajAdresDoSklepu(IList<SklepyBll> obj)
        {
            foreach (var el in obj)
            {
                if (el != null)
                {
                    //czy adres w ogole jest uzupelniony - jak nie to wywalamy go - jak ktos recznie dodaje sklepy to po co mu adres?!
                    if (el.Lat == 0 && el.Lon == 0 && string.IsNullOrEmpty(el.Miasto) && string.IsNullOrEmpty(el.UlicaNr) && string.IsNullOrEmpty(el.KodPocztowy) && string.IsNullOrEmpty(el.Telefon) && string.IsNullOrEmpty(el.Email) &&
                        el.KrajId == null)
                    {
                        el.AdresId = null;
                        continue;
                    }

                    var adres = new Model.Adres
                    {
                        Lat = el.Lat,
                        Lon = el.Lon,
                        Nazwa = el.Nazwa,
                        Miasto = el.Miasto,
                        UlicaNr = el.UlicaNr,
                        KodPocztowy = el.KodPocztowy,
                        KrajId = el.KrajId,
                        RegionId = el.RegionId,
                        Email = el.Email,
                        Telefon = el.Telefon
                    };

                    object o = Calosc.DostepDane.AktualizujPojedynczy(adres);
                    el.AdresId = (long)o;
                }
            }
        }
        
        /// <summary>
        /// Usuwa adresy sklepu przed jego usunięciem 
        /// </summary>
        /// <param name="obj"></param>
        public void UsunAdresSklepu(IList<long> obj)
        {
            foreach (long el in obj)
            {
                long? adresId = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Sklep>(el).AdresId;
                if (adresId != null && adresId<0)
                {
                    Calosc.DostepDane.UsunPojedynczy<Model.Adres>(adresId);
                }
            }
        }
        /// <summary>
        /// Bind po update sklepów która sprawdza czy kategorie dla sklepów uległy zmianie
        /// </summary>
        /// <param name="obj"></param>
        public void ZarzadzajLacznikamiKategoriiSklepu(IList<SklepyBll> obj)
        {
            List<SklepKategoriaSklepu> kategorieDoDodania = new List<SklepKategoriaSklepu>();
            HashSet<long> lacznikiDoUsuniecia = new HashSet<long>();
            //pobieramy id kategorii jakie sklep posiadal
            Dictionary<long, List<SklepKategoriaSklepu>> kategorieSklepow = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<SklepKategoriaSklepu>(null, x => obj.Select(y => y.Id).Contains(x.SklepId)).GroupBy(x => x.SklepId).ToDictionary(x => x.Key, x => x.ToList());
            
            foreach (var el in obj)
            {
                if (el == null) continue;
                List<SklepKategoriaSklepu> kategorieDlaSklepu;
                kategorieSklepow.TryGetValue(el.Id, out kategorieDlaSklepu);
                //slownik gdzie klucz to id kategorii oraz value to id lacznika
                Dictionary<long,long> idKAtegorii = kategorieDlaSklepu?.ToDictionary(x=>x.KategoriaSklepuId, x=>x.Id);
                foreach (var item in el.KategorieId)
                {
                    //sprawdzamy ktore są do dodania
                    if (kategorieDlaSklepu != null && idKAtegorii.ContainsKey(item))
                    {
                        continue;
                    }

                    SklepKategoriaSklepu skl = new SklepKategoriaSklepu
                    {
                        SklepId = el.Id,
                        KategoriaSklepuId = item
                    };
                    kategorieDoDodania.Add(skl);
                }

                //wyciagamy te kategorie ktore sądo wywalenia
                if (kategorieDlaSklepu != null)
                {
                    lacznikiDoUsuniecia.UnionWith(idKAtegorii.Where(x => !el.KategorieId.Contains(x.Key)).Select(x => x.Value));
                }
            }
            //jezeli są jakies laczniki do dodania to je dodajemy
            if (kategorieDoDodania.Any())
            {
                Calosc.DostepDane.AktualizujListe(kategorieDoDodania);
            }
            //jeżeli są jakies laczniki do usuniecia to usuwamy
            if (lacznikiDoUsuniecia.Any())
            {
                Calosc.DostepDane.Usun<SklepKategoriaSklepu, long>(new List<long>(lacznikiDoUsuniecia));
            }
        }

        private HashSet<long> _kategorieSklepowIDZSklepamiAktywnymiINaMapie = null;


        public List<KategoriaSklepu> PobierzKategorieNiepusteIPoprawneKoordynaty(HashSet<long> kategoriaId, int jezyk)
        {
            if (_kategorieSklepowIDZSklepamiAktywnymiINaMapie == null)
            {
                _kategorieSklepowIDZSklepamiAktywnymiINaMapie = new HashSet<long>( this.ListaSklepowDlaWybranychKategorii(null, true).SelectMany(x => x.KategorieId) );
            }

            if (kategoriaId == null)
            {
                return Calosc.DostepDane.Pobierz<KategoriaSklepu>(jezyk, null, x => x.PokazywanaNaMapie && _kategorieSklepowIDZSklepamiAktywnymiINaMapie.Contains(x.Id)).ToList();
            }

            return Calosc.DostepDane.Pobierz<KategoriaSklepu>(jezyk, null, x => x.PokazywanaNaMapie && _kategorieSklepowIDZSklepamiAktywnymiINaMapie.Contains(x.Id) && kategoriaId.Contains(x.Id)).ToList();
        }

        public List<SklepyBll> ListaSklepowDlaWybranychKategorii(HashSet<long> kategorieId, bool tylkoZPoprawnymoKoordynatami = true, string miasto = null, bool zawezajPunktyNaMapie=true)
        {
            SklepyBll[] sklepy = Calosc.DostepDane.Pobierz<SklepyBll>(null, x => x.Aktywny && (!tylkoZPoprawnymoKoordynatami || x.CzyPoprawneKoordynaty) && (kategorieId == null || kategorieId.Overlaps(x.KategorieId)) 
                && (!zawezajPunktyNaMapie || (miasto == null || x.Miasto.Equals(miasto, StringComparison.InvariantCultureIgnoreCase)))).ToArray();

            if (!string.IsNullOrEmpty(miasto) && !zawezajPunktyNaMapie)
            {
                var index = Array.FindIndex(sklepy, row => row.Miasto == miasto);
                if (index != -1)
                {
                    var sklep = sklepy[index];

                    sklepy[index] = sklepy[0];
                    sklepy[0] = sklep;
                }
            }
            return sklepy.ToList();
        }

        /// <summary>
        /// zwraca posortowana miasta dla sklepow bedacych w określonych kategoriach
        /// </summary>
        /// <param name="kategorieId"></param>
        /// <returns></returns>
        public List<string> MiastaDlaWybranychKategorii(HashSet<long> kategorieId)
        {
            return this.ListaSklepowDlaWybranychKategorii(kategorieId).Select(x => x.Miasto).Distinct().OrderBy(x => x).ToList();
        }
    }
}