using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.Importy.Model;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.DAL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using System.Text;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Importy
{
    [Serializable]
    public class ProduktyImport : ProduktyImport<ProduktyImportModel>
    {
    }

    [Serializable]
    public class ProduktyImport<T> : ObslugaEpp<EppHelper.TowarDoGeneracjiEpp,T >
    {
        public override ISlownik SlownikPrzetwarzajacy
        {
            get { return new SlownikCech(); }
        }
        private const string Reg = @"[^0-9a-zA-Z-,ążśźćńłęóŻŹŃĄŚŁĘÓ\-&$\+.]+";
        private static readonly Regex RegexOczyszczanie = new Regex(Reg);

        public override List<object> EksportDanych(IKlient zadajacy, List<object> parametry, out string nazwa)
        {
            nazwa = "produkty";
            bool jakoKlienta = false;
            var grupy = SolexBllCalosc.PobierzInstancje.Grupy.Wszystykie(ConfigBLL.PobierzInstancje.JezykIDPolski);
            ProduktySearchCriteria criteria=new ProduktySearchCriteria();
            if (zadajacy.CzyAdministrator)
            {
                criteria.przedstawiciel_id.Add(null);
            }
            else   if (zadajacy.Roles.Contains(RoleType.Oddzial))
            {
            criteria.przedstawiciel_id.Add(zadajacy.klient_id);
            }
            else
            {
                jakoKlienta = true;
            }
           var tmp = SolexBllCalosc.PobierzInstancje.ProduktyBazowe.Pobierz(criteria, ConfigBLL.PobierzInstancje.JezykIDPolski, zadajacy);
            if (jakoKlienta)
            {
                HashSet<int> ids = BLL.SolexBllCalosc.PobierzInstancje.ProduktyKlienta.PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(zadajacy);
                tmp = tmp.Where(x =>ids.Contains(x.produkt_id)).ToList();
            }
            if (ObiektImportu.FiltryKolekcja.Any())
            {
                HashSet<int> idscecha = ObiektImportu.FiltryKolekcja.Select(int.Parse).ToHashSet();
                tmp = tmp.Where(x => x.Cechy.Any(y=>idscecha.Contains(y.Key))).ToList();
            }
           List<ProduktyImportModel> wynik = new List<ProduktyImportModel>();
            int detalpoziom = ConfigBLL.PobierzInstancje.GetPriceLevelDetal;
            int hurtpoziom = ConfigBLL.PobierzInstancje.GetPriceLevelHurt;
            foreach (var produktyBazowy in tmp)
            {

                var x = new ProduktyImportModel(ConfigBLL.PobierzInstancje.JezykIDPolski, produktyBazowy) {marka_nazwa = produktyBazowy.marka_nazwa};
                x.LinkDoProduktu = LinkBuilder.PobierzInstancje.PobierzLink(x, true,null);
                StringBuilder sb = new StringBuilder();
                StringBuilder sbl = new StringBuilder();
                foreach (var k in produktyBazowy.KategorieWidoczne.Where(a => grupy.Where(p => p.producencka == false).Select(p => p.id).Contains(a.grupa_id)))
                {
                    sb.AppendFormat("{0};", k.Sciezka);
                    sbl.AppendFormat("{0};", k.nazwa);  //.Replace("/", ">") - wywalam. bartek, jesli juz standaryzujemy to wywalamy smieci stare
                }
                x.Kategoria = sb.ToString().Trim(';').Trim();
                x.KategoriaLiscie = sbl.ToString().Trim(';').Trim();
                if (produktyBazowy.CenyPoziomy.ContainsKey(hurtpoziom))
                {
                    x.CenaPrzedRabatemNetto = produktyBazowy.CenyPoziomy[hurtpoziom].netto;
                    x.CenaPrzedRabatemBrutto = Kwoty.WyliczBrutto(x.CenaPrzedRabatemNetto, x.vat, zadajacy);
                }
                if (produktyBazowy.CenyPoziomy.ContainsKey(detalpoziom))
                {
                    x.CenaDetalicznaNetto = produktyBazowy.CenyPoziomy[detalpoziom].netto;
                    x.CenaDetalicznaBrutto = Math.Round(Kwoty.WyliczBrutto(x.CenaDetalicznaNetto, x.vat),2);
                }
          
                var cenaklienta = SolexBllCalosc.PobierzInstancje.Rabaty.PobierzCeneProduktuDlaKlienta(zadajacy.klient_id, x);
                x.CenaPoRabacieNetto = cenaklienta.cena_netto;
                x.CenaPoRabacieBrutto = Math.Round(cenaklienta.cena_brutto, 2);
                wynik.Add(x);
            }
            return wynik.OrderBy(x=>x.KodUrl()).Select(p => (object) p).ToList();
        }
        private bool BylImportowany(ProduktyImportModel data,  IEnumerable<object> zmienione)
        {
            List<ProduktBazowy>    zmienionebazowe = zmienione.OfType<ProduktBazowy>().ToList();
                    ProduktBazowy znaleziony = null;
            if (data.produkt_id != 0)
            {
                znaleziony = zmienionebazowe.FirstOrDefault(x => x.produkt_id == data.produkt_id);
            }
            if (znaleziony == null && !string.IsNullOrEmpty(data.kod))
            {
                znaleziony = zmienionebazowe.FirstOrDefault(x => x.kod == data.kod);
            }
            if (znaleziony == null && !string.IsNullOrEmpty(data.kod_kreskowy))
            {
                znaleziony = zmienionebazowe.FirstOrDefault(x => x.kod_kreskowy == data.kod_kreskowy);
            }
            return znaleziony != null;
        }
        protected ProduktBazowy ZnajdzProdukt(ProduktyImportModel data, IKlient zadajacy)
        {

            if (data.kod_kreskowy!=null && data.kod_kreskowy.Contains("E+"))
            {
                throw new Exception("Kod kreskowy w złym formacie - sprawdz dane importu. Kod kreskowy: {0}" + data.kod_kreskowy);
            }

            if (string.IsNullOrEmpty(data.kod) && data.produkt_id == 0 && string.IsNullOrEmpty(data.kod_kreskowy))
            {
                throw new Exception("Nie podano symbolu lub produkt_id lub kod_kreskowy");
            }
            ProduktBazowy znaleziony = null;
            if (data.produkt_id != 0)
            {
                znaleziony = SolexBllCalosc.PobierzInstancje.ProduktyBazowe.Pobierz(data.produkt_id);
            }
            if (znaleziony == null && !string.IsNullOrEmpty(data.kod))
            {

                //pierw wyszukuje wszystkie widoczne produkty i zwraca pierwszy z listy 
                //a jak nie znalazł żadnych aktywnych produktów to wyszukuje po wszytkich(wydocznych i niewidocznych) i zwraca pierwszy znaleziony 
                var tmp = SolexBllCalosc.PobierzInstancje.ProduktyBazowe.Wszystykie().Where(x => !string.IsNullOrEmpty(x.kod) && x.kod.Equals(data.kod, StringComparison.InvariantCultureIgnoreCase) && x.widoczny).ToList();
                znaleziony = tmp.Any() ? tmp.FirstOrDefault() : SolexBllCalosc.PobierzInstancje.ProduktyBazowe.Wszystykie().FirstOrDefault(x =>!string.IsNullOrEmpty(x.kod) && x.kod.Equals(data.kod, StringComparison.InvariantCultureIgnoreCase));
            }
            if (znaleziony == null && !string.IsNullOrEmpty(data.kod_kreskowy))
            {
                
                //znaleziony = SolexBllCalosc.PobierzInstancje.ProduktyBazowe.Wszystykie().FirstOrDefault(x => !string.IsNullOrEmpty(x.kod_kreskowy) && x.kod_kreskowy.Equals(data.kod_kreskowy, StringComparison.InvariantCultureIgnoreCase));
                
                //pierw wyszukuje wszystkie widoczne produkty i zwraca pierwszy z listy 
                //a jak nie znalazł żadnych aktywnych produktów to wyszukuje po wszytkich(wydocznych i niewidocznych) i zwraca pierwszy znaleziony 
                var tmp = 
                    SolexBllCalosc.PobierzInstancje.ProduktyBazowe.Wszystykie().Where(x => !string.IsNullOrEmpty(x.kod_kreskowy) && x.kod_kreskowy.Equals(data.kod_kreskowy, StringComparison.InvariantCultureIgnoreCase) && x.widoczny).ToList();
                znaleziony = tmp.Any() ? tmp.FirstOrDefault() : SolexBllCalosc.PobierzInstancje.ProduktyBazowe.Wszystykie().FirstOrDefault(x => !string.IsNullOrEmpty(x.kod_kreskowy) && x.kod.Equals(data.kod_kreskowy, StringComparison.InvariantCultureIgnoreCase));
                if (znaleziony == null)
                {
                    var kodd= ProduktyKodyDodatkoweBll.PobierzInstancje.WszystkieKody().FirstOrDefault(x => x.Kod.Equals(data.kod_kreskowy, StringComparison.InvariantCultureIgnoreCase));

                    if (kodd != null)
                    {
                        znaleziony = SolexBllCalosc.PobierzInstancje.ProduktyBazowe.Pobierz(kodd.ProduktId);
                    }
                }
            }
            if (znaleziony != null && (znaleziony.przedstawiciel_id == null || znaleziony.przedstawiciel_id == zadajacy.OddzialDoJakiegoNalezyKlient))
            {
                return znaleziony;
            }
            return null;
        }

        private int _minid;
        public override bool ImportDanych(IKlient zadajacy, object dane, List<object> parametry, List<WierszMapowania> mapowania, ref List<object> zmienioneDane)
        {
            ProduktyImportModel data = (ProduktyImportModel)dane;
         if (_minid == 0)
         {
             _minid =(int) MainDAO.GetAutoID(typeof(produkty));
         }
            bool nowyProdukt = false;
           
            ProduktBazowy znaleziony = ZnajdzProdukt(data, zadajacy);
            if (znaleziony == null)
            {
                //zakladanie noweg produktu TYLKO jesli sa pelne dane
                if (string.IsNullOrEmpty(data.nazwa) || string.IsNullOrEmpty(data.kod) )
                {
                     throw new Exception("Nie podano symbolu lub nazwy - nie można założyć nowego produktu");
                }

                znaleziony=new ProduktBazowy(ConfigBLL.PobierzInstancje.JezykIDPolski);
                nowyProdukt = true;
            }

            var jednostkaPodstawowa = mapowania.FirstOrDefault(p => p.Pole == "JednostkaPodstawowa");//czy jest jednostka
            if (jednostkaPodstawowa != null)
            {
                string nazwaJEdnostki = data.JednostkaPodstawowa;
                znaleziony.UstawJednostki(new[] { new JednostkaProduktu { Nazwa = nazwaJEdnostki, Podstawowa = true, Przelicznik = 1 } });
               
            }

            znaleziony = (ProduktBazowy)KopiujPolaWgMapowan(znaleziony, data, mapowania);
            //oczyszczanie nazwy /symbolu
            if (nowyProdukt)
            {
                znaleziony.nazwa = RegexOczyszczanie.Replace(znaleziony.nazwa, " ");
                znaleziony.kod = RegexOczyszczanie.Replace(znaleziony.kod, " ");
            }
            if (!zadajacy.CzyAdministrator)
            {
                znaleziony.przedstawiciel_id = zadajacy.klient_id;
            }
            if (znaleziony.produkt_id == 0)
            {
                znaleziony.produkt_id = _minid;
                _minid--;

            }
            if (BylImportowany(data, zmienioneDane))
            {
                throw new Exception("Produkt już był wcześniej wpisany w pliku, pomijamy");
            }
        // ProduktyBazowe.Zapisz(znaleziony).produkt_id;
            var grupy = SolexBllCalosc.PobierzInstancje.Grupy.Wszystykie(ConfigBLL.PobierzInstancje.JezykIDPolski);  
            var laczniku = new List<produkty_kategorie>();
            if (!string.IsNullOrEmpty(data.KategorieId))
            {
                foreach (string pk in data.KategorieId.Split(new []{";"},StringSplitOptions.RemoveEmptyEntries))
                {
                    int id;
                    if (int.TryParse(pk, out id))
                    {
                        laczniku.Add(new produkty_kategorie{produkt_id = znaleziony.produkt_id,kategoria_id =id});
                    }
                }
            }

            var mapowienieKategorii = mapowania.FirstOrDefault(p => p.Pole == "Kategoria");//czy w tym imporcie były kategorie
            if (mapowienieKategorii != null)
            {
               laczniku.AddRange(MapujKAtegorie(grupy,znaleziony,data.Kategoria,false));
            }
            var mapowanieMarek = mapowania.FirstOrDefault(p => p.Pole == "marka_nazwa");//czy w tym imporcie były kategorie
            if (mapowanieMarek != null)
            {
                laczniku.AddRange(MapujKAtegorie(grupy, znaleziony, data.marka_nazwa, true));
            }
            zmienioneDane.Add(znaleziony);
            zmienioneDane.Add(laczniku);

            return true;
        }

        private IEnumerable<produkty_kategorie> MapujKAtegorie(IList<GrupaBLL> grupy, ProduktBazowy znaleziony, string dane, bool producencka)
        {
            List<produkty_kategorie> lacznik=new List<produkty_kategorie>();
            List<int> idkategori=new List<int>();
            List<GrupaBLL> pasujace = grupy.Where(p => p.producencka == producencka).ToList();
            foreach (var grupaBLL in pasujace)
            {
                GrupaBLL bll = grupaBLL;
                idkategori.AddRange(  SolexBllCalosc.PobierzInstancje.KategorieDostep.Wszystykie().Where(x=>x.grupa_id==bll.id).Select(x=>x.kategoria_id));
            }

            if (SolexBllCalosc.PobierzInstancje.KategorieDostep.ProduktyKategorieGrupowanePoProdukcie.ContainsKey(znaleziony.produkt_id))
            {
                ProduktyKategorieSearchCriteria pksc=new ProduktyKategorieSearchCriteria();
                pksc.produkt_id.Add(znaleziony.produkt_id);
                pksc.kategoria_id.AddRange(idkategori);
               HashSet<int> ids=SolexBllCalosc.PobierzInstancje.KategorieDostep.ZnajdLaczniki(pksc).Select(x => x.id).ToHashSet();
               SolexBllCalosc.PobierzInstancje.KategorieDostep.UsunLacznikiZKategoriami(ids);
            }

              
                if (!string.IsNullOrWhiteSpace(dane))
                {
                    foreach (var kategoria in dane.Split(',', ';'))
                    {

                        KategorieBLL k = null;
                        foreach (var g in pasujace)
                        {
                            k = g.PobierzKategorie().FirstOrDefault(p => p.Sciezka == kategoria.Trim());
                            if (k != null)
                            {

                                break;
                            }
                        }
                        if (k == null)
                        {
                            try
                            {
                                string[] sciezka = kategoria.Trim()
                                                            .Split(new[] { "\\", "/",">" },
                                                                   StringSplitOptions.RemoveEmptyEntries);
                                int? parent_ID = null;
                                GrupaBLL grupa = grupy.First(x => x.producencka==producencka);

                                foreach (string nazwa in sciezka)
                                {
                                    kategorie tmp = SolexBllCalosc.PobierzInstancje.KategorieDostep.Wszystykie().FirstOrDefault(x => x.nazwa == nazwa && x.grupa_id == grupa.id);
                                    if (tmp == null)
                                    {
                                        tmp = new kategorie
                                        {
                                            nazwa = nazwa,
                                            widoczna = true,
                                            grupa_id = grupa.id,
                                            parent_id = parent_ID
                                        };
                                        SolexBllCalosc.PobierzInstancje.KategorieDostep.Aktualizuj(new List<kategorie> {tmp});
                                    }
                                    parent_ID = tmp.kategoria_id;
                                }

                                if (parent_ID != null)
                                {
                                    k = SolexBllCalosc.PobierzInstancje.KategorieDostep.Pobierz(parent_ID.GetValueOrDefault(), ConfigBLL.PobierzInstancje.JezykIDPolski);
                                }
                            }
                            catch
                            {
                                throw new Exception("Nie można wczytać pierwszej kategorii, nie producenckiej. Upewnij się że jest jakaś dodana w systemie.");
                            }
                        }
                        if (k != null)
                        {
                            lacznik.Add(new produkty_kategorie { kategoria_id = k.kategoria_id, produkt_id = znaleziony.produkt_id });
                        }
                    }
            }
            return lacznik;
        }

        public  override void PoImporcie(IKlient zadajacy, List<object> parametry, List<WierszMapowania> mapowania, ref List<object> zmienioneDane)
        {
            List<produkty_kategorie > pk=new List<produkty_kategorie>();
            List<ProduktBazowy> produkty=new List<ProduktBazowy>();
        
            foreach (object o in zmienioneDane)
            {
                ProduktBazowy item = o as ProduktBazowy;
                if (item != null)
               {
                    if (produkty.All(x => x.produkt_id != item.produkt_id))
                    {
                        produkty.Add(item);
                    }
               }

                List<produkty_kategorie> item2 = o as List<produkty_kategorie>;
                if (item2 != null)
                {
                    pk.AddRange(item2);
                }
            }

            //zapis produktow
            SolexBllCalosc.PobierzInstancje.ProduktyBazowe.Aktualizuj(produkty.Select(x => new produkty(x)).ToList() );
            
            //zapis jednostek
            SolexBllCalosc.PobierzInstancje.ProduktyBazowe.ZapiszJednostkiProduktow(produkty);

            HashSet<int> pkids = produkty.Select(x => x.produkt_id).ToHashSet();
            HashSet<int> ids=new HashSet<int>();
            foreach (var pkid in  SolexBllCalosc.PobierzInstancje.KategorieDostep.ProduktyKategorieGrupowanePoProdukcie.WhereKeyIsIn(pkids))
            {
                foreach (int i in pkid)
                {
                    ids.Add(i);
                }
            }
            SolexBllCalosc.PobierzInstancje.KategorieDostep.UsunLacznikiZKategoriami(ids);
            SolexBllCalosc.PobierzInstancje.KategorieDostep.AktualizujProduktyKategorieLaczniki(pk);
        }

 

        public override string WygenerujEpp(List<object> dane, IKlient zadajacy, List<object> parametry, ref string nazwa)
        {
            List<EppHelper.TowarDoGeneracjiEpp> produkty = new List<EppHelper.TowarDoGeneracjiEpp>();
            var mpowiania = ObiektImportu.Mapowania.Where(x => x.Widoczne).ToDictionary(x => x.Pole, x => x.Nazwa);
            foreach (var data in dane)
            {
                EppHelper.TowarDoGeneracjiEpp produkt = new EppHelper.TowarDoGeneracjiEpp();
                produkt.KopiujPola(data, mpowiania);
                if (string.IsNullOrEmpty(produkt.Kod))
                {
                    produkt.Kod = produkt.KodKreskowy;
                }
                produkty.Add(produkt);
            }
           
            StringBuilder sb = new StringBuilder();
            sb.Append(EppHelper.PobierzInstancje.WygenerujNaglowek(ConfigBLL.PobierzInstancje.GetOwner(), DateTime.Now));
            sb.Append(EppHelper.PobierzInstancje.WygenrujSekcjeTowary(produkty));
            sb.Append(EppHelper.PobierzInstancje.WygenerujSekcjeCechy(produkty));
            sb.Append(EppHelper.PobierzInstancje.WygerujSekcjeCeny(produkty));
            return sb.ToString();
        }
    }
   
}
