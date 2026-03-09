using ServiceStack.Common;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using FastMember;
using ServiceStack.OrmLite;
using SolEx.Hurt.Model.Interfaces;
using StringExtensions = ServiceStack.Text.StringExtensions;
using System.Text;

namespace SolEx.Hurt.Core.BLL
{
    public class ProduktyBazowe : BllBazaCalosc, IProduktyBazowe
    {
        public ProduktyBazowe(ISolexBllCalosc calosc)
            : base(calosc)
        {
        }

        public IList<ProduktBazowy> MetodaPrzetwarzajacaPoSelect_UzupelnijProdutkBazowy(int jezykID, IKlient klient,
            IList<ProduktBazowy> listaProduktow, object opcjonalmnyParametr)
        {
            var slownikCechWRabatach = new HashSet<long>( SolexBllCalosc.PobierzInstancje.Rabaty.SlownikRabatow().Values.Where(x => x.CechaId.HasValue).Select(x => x.CechaId.GetValueOrDefault()) );

            var kategorieSlownik = Calosc.ProduktyKategorieDostep.ProduktyKategorieGrupowanePoProdukcie;

            Dictionary<long, KategorieBLL> kategorieDlaKlienta =
                Calosc.DostepDane.Pobierz<KategorieBLL>(jezykID, null).ToDictionary(x => x.Id, x => x);

            //jak sa gradacje, ale graacje nie jest ustawiona to jest problem
            //if (!SolexBllCalosc.PobierzInstancje.Konfiguracja.GradacjeAktywne && konfekcjeWszystkie.Any())
            //{
            //    Calosc.Log.ErrorFormat("Gradacje są NIEaktywne (najcześniej brak ustawienia ZCzegoLiczycGradacje), ale są zdefiniowane. Włącz gradacje, albo przestań ich używać");
            //    konfekcjeWszystkie = new HashSet<Konfekcje>();
            //}

            ProduktyKodyDodatkoweBll kodyDodatkowe = ProduktyKodyDodatkoweBll.PobierzInstancje;
            Dictionary<long, List<ProduktyKodyDodatkowe>> wszystkieKodyDodatkowePerProdukt =
                kodyDodatkowe.KodyKreskoweWgProduktow();

            foreach (var pb in listaProduktow)
            {
                if (!pb.Widoczny)
                {
                    //prodkty niewidoczne nie maja cech itp
                    continue;
                }

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //!!!!!!!!!! wszystkie propertisy tu uzuplenine trzeba dopisać do konstrktora kopiujacego w pb !!!!!!!!!!!!!!
                //!!!!!!!!!!~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                pb.IdCechPRoduktu = Calosc.CechyProduktyDostep.PobierzIdCechProduktu(pb.Id);

                pb.FriendlyLinkURL = Tools.OczyscCiagDoLinkuURL(pb.Nazwa);

                HashSet<long> cechyProduktuRabatowe = new HashSet<long>( slownikCechWRabatach.Intersect(pb.IdCechPRoduktu) );
                pb.CechyProduktuWystepujaceWRabatach = cechyProduktuRabatowe;

                HashSet<long> kategorie = null;
                if (kategorieSlownik.TryGetValue(pb.bazoweID, out kategorie))
                {
                    //musimy uzupelnic o parenty do ktorych nie ma lacznikow
                    pb.KategorieId = kategorie;
                    pb.Kategorie = kategorieDlaKlienta.WhereKeyIsIn(kategorie);
                }

                pb.Zdjecia = Calosc.Pliki.PobierzObrazkiProduktu(pb.Id);

                List<JednostkaProduktu> jednostkiProduktu = null;
                if (Calosc.ProduktyJednostkiDostep.PobierzJednostkiProduktuWgProduktu(pb.JezykId)
                    .TryGetValue(pb.Id, out jednostkiProduktu))
                {
                    pb.Jednostki = jednostkiProduktu;
                }

                //gradacje
                //if (konfekcjeWszystkie.Any())
                //{
                //    //w produktach bazowych NIE mamy klienta wiec pobieramy tylko dla produktu konfekcje pasujace
                //    Func<Konfekcje, bool> warunekDlaSzukaniaGradacji = x => (x.ProduktId.HasValue && x.ProduktId == pb.Id) || (x.CechaId.HasValue && pb.Cechy.ContainsKey(x.CechaId.Value));

                //    pb.GradacjePosortowane =  konfekcjeWszystkie.Where(warunekDlaSzukaniaGradacji).OrderBy(x => x.Ilosc).ToList();

                //    if (pb.GradacjePosortowane.IsEmpty())
                //    {
                //        pb.GradacjePosortowane = null;
                //    }
                //}

                if (wszystkieKodyDodatkowePerProdukt.TryGetValue(pb.Id, out List<ProduktyKodyDodatkowe> kody))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (ProduktyKodyDodatkowe produktyKodyDodatkowe in kody)
                    {
                        sb.Append(produktyKodyDodatkowe.Kod);
                        sb.Append(" ");
                        sb.Append(produktyKodyDodatkowe.Nazwa);
                    }

                    pb.DodatkoweKodyString = sb.ToString();
                }
            }

            return listaProduktow;
        }

        public static HashSet<long> GetIds()
        {
            IEnumerable<long> temp = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Produkt>(null).Select(x => x.Id);
            return new HashSet<long>(temp);
        }

        private int? _atrybutDostawa;
        private int? _dostawaIleGodzin;

        /// <summary>
        /// termin najblizszej dostawy CYKLICZNEJ. nie mylic z terminem dostawy erp
        /// </summary>
        public DateTime? NajblizszaDostawa(ProduktBazowy produkt)
        {
            if (_atrybutDostawa == null)
            {
                _atrybutDostawa = Calosc.Konfiguracja.IdAtrybutuDostawy;
            }
            if (_atrybutDostawa == null || _atrybutDostawa.Value == 0)
            {
                return null;
            }
            if (_dostawaIleGodzin == null)
            {
                _dostawaIleGodzin = Calosc.Konfiguracja.IleWczesniejZmianaDostawa;
            }
            if (_dostawaIleGodzin == null || _dostawaIleGodzin.Value == 0)
            {
                return null;
            }

            CechyBll c = produkt.ListaCech.FirstOrDefault(x => x.AtrybutId.HasValue && x.AtrybutId == _atrybutDostawa.Value); //PobierzCecheNajblizszejDostawy(produkt.Id, _atrybutDostawa.Value);//produkt.Cechy.Values.FirstOrDefault(x => x.AtrybutId == _atrybutDostawa.Value);
            if (c != null)
            {
                int tmp;
                List<int> dni = c.Nazwa.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Where(a => int.TryParse(a, out tmp)).Select(int.Parse).OrderBy(x => x).ToList();

                if (dni.Count == 0)
                    return null;

                int kiedyNajwczescniej = (int)DateTime.Now.AddHours(_dostawaIleGodzin.Value).DayOfWeek;
                int termin = dni.Any(x => x >= kiedyNajwczescniej) ? dni.First(x => x >= kiedyNajwczescniej) : dni[0];
                return DateTimeHelper.PobierzInstancje.Nastepny((DayOfWeek)termin);
            }
            return null;
        }

        public TypStanu WyliczTypStanu(IProduktBazowy produkt)
        {
            bool jestnastanie = produkt.IloscLaczna > 0 || produkt.Typ == TypProduktu.Usluga;
            var wynik = TypStanu.brak;
            HashSet<long> cechyproduktu = null;

            //pobieramy cechy tylko jak jest taka potrzeba - dlateg pusta lite robiemy - znak ze bylo pobrane juz
            if (Calosc.Konfiguracja.ProduktyNieDostepnePrzezDluzszyCzasCecha.HasValue || Calosc.Konfiguracja.ProduktyDropshipingCechaID.HasValue
                || Calosc.Konfiguracja.ProduktyNaZamowienieCechaID.HasValue)
            {
                cechyproduktu = Calosc.CechyProduktyDostep.PobierzIdCechProduktu(produkt.Id);
            }

            if (Calosc.Konfiguracja.ProduktyNieDostepnePrzezDluzszyCzasCecha.HasValue)
            {
                if (cechyproduktu != null && cechyproduktu.Contains(Calosc.Konfiguracja.ProduktyNieDostepnePrzezDluzszyCzasCecha.Value))
                {
                    wynik = TypStanu.niedostepny_dluzszy_czas;
                }
            }
            else if (Calosc.Konfiguracja.ProduktyDropshipingCechaID.HasValue && cechyproduktu != null && cechyproduktu.Contains(Calosc.Konfiguracja.ProduktyDropshipingCechaID.Value))
            {
                if (Calosc.Konfiguracja.ProduktyDropshipingPokazujNaStanieJesliJest && jestnastanie)
                {
                    wynik = TypStanu.na_stanie;
                }
                else
                {
                    wynik = TypStanu.dropshiping;
                }
            }
            else if (produkt.NajblizszaDostawa != null)
            {
                //BIOPLANTE nie chce tego warunku - jesli jest najblizsza dsotawa to ma byc CYKLICZNA dostawa
                //przewchwy - jesli jest cykliczna dostawa to nadal moze byc niedostepny dluzszy czas
                //if (!string.IsNullOrEmpty(produkt.Dostawa))
                //{
                //    //nie bedzie dostepny przez dluzszy czas
                //    if (Calosc.Konfiguracja.ProduktyNieDostepnePrzezDluzszyCzas_iloscDni > 0 && produkt.IloscLaczna == 0 && produkt.DostawaData.HasValue)
                //    {
                //        if ((produkt.DostawaData.Value - DateTime.Now).Days > Calosc.Konfiguracja.ProduktyNieDostepnePrzezDluzszyCzas_iloscDni)
                //        {
                //            wynik = TypStanu.niedostepny_dluzszy_czas;
                //        }
                //    }
                //}
                //else
                //{
                    wynik = TypStanu.cykliczna_dostawa;
               // }
            }
            //na wyczerpaniu
            else if (jestnastanie)
            {
                if ((Calosc.Konfiguracja.ProduktyNaWyczerpaniu_procentStanuMinimalnego > 0 && produkt.StanMin > 0) && (produkt.IloscLaczna < Calosc.Konfiguracja.ProduktyNaWyczerpaniu_procentStanuMinimalnego * produkt.StanMin))
                {
                    wynik = TypStanu.na_wyczerpaniu;
                }
                else
                {
                    wynik = TypStanu.na_stanie;
                }
            }
            else if (!string.IsNullOrEmpty(produkt.Dostawa))
            {
                //nie bedzie dostepny przez dluzszy czas
                if (Calosc.Konfiguracja.ProduktyNieDostepnePrzezDluzszyCzas_iloscDni > 0 && produkt.IloscLaczna == 0 && produkt.DostawaData.HasValue)
                {
                    if ((produkt.DostawaData.Value - DateTime.Now).Days > Calosc.Konfiguracja.ProduktyNieDostepnePrzezDluzszyCzas_iloscDni)
                    {
                        wynik = TypStanu.niedostepny_dluzszy_czas;
                    }
                }
                else
                {
                    wynik = TypStanu.w_dostawie;
                }
            }
            else if (Calosc.Konfiguracja.ProduktyNaZamowienieCechaID.HasValue && cechyproduktu != null && cechyproduktu.Contains(Calosc.Konfiguracja.ProduktyNaZamowienieCechaID.Value))
            {
                wynik = TypStanu.na_zamowienie;
            }
            return wynik;
        }

        private readonly List<TypStanu> _nieDozwoloneStanyDododawania = new List<TypStanu> { TypStanu.brak, TypStanu.w_dostawie };

        //public bool MoznaDodacDoKoszyka(IProduktKlienta p)
        //{
        //    string temp = "";
        //    return this.MoznaDodacDoKoszyka(p, out temp);
        //}

        public bool MoznaDodacDoKoszyka(IProduktKlienta p, out string info, out bool pokazujCene)
        {
            pokazujCene = false;
            info = null;
            if (p == null) return false;
            if (p.PobierzTypStany == TypStanu.niedostepny_dluzszy_czas)
            {
                info = "Produkt niedostępny - nie można dodać do koszyka";
                return false;
            }

            if (Calosc.Konfiguracja.BlokujDodawanieDoKoszykaDlaBrakujacychProduktow && _nieDozwoloneStanyDododawania.Contains(p.PobierzTypStany))
            {
                info = "Produkt niedostępny - nie można dodać do koszyka";
                return false;
            }
            if (Calosc.Konfiguracja.BlokujDodawanieDoKoszykaDlaProduktowZCenaZerowa && p.FlatCeny.CenaNetto==0)
            {
                info = "Produkt z ceną zerową - nie można dodać do koszyka";
                if (!SolexBllCalosc.PobierzInstancje.Konfiguracja.ZastepujCeneZerowaIkonkaTelefonu)
                {
                    return false;
                }
            }
            if (!p.CzyJestKoncesja())
            {
                info = "Produkt wymaga koncesji - aby kupować prosimy o kontakt z opiekunem";
                pokazujCene = true;
                return false;
            }
            pokazujCene = true;
            return true;
        }

        public bool MoznaDodacDoPoinformujODostepnosci(IProduktKlienta produkt)
        {
            if (_nieDozwoloneStanyDododawania.Contains(produkt.PobierzTypStany))
            {
                return true;
            }
            return false;
        }

        public ProduktBazowy Pobierz(string symbol, int jezykId, IKlient kontekst = null)
        {
            var o = Calosc.DostepDane.Pobierz<ProduktBazowy>(jezykId, null, p => p.Kod.Trim().ToLower() == symbol.Trim().ToLower()).FirstOrDefault();

            if (o != null && kontekst != null && kontekst.OddzialDoJakiegoNalezyKlient != 0)
            {
                if (o.PrzedstawicielId == null || o.PrzedstawicielId == kontekst.OddzialDoJakiegoNalezyKlient)
                {
                    return o;
                }
                return null;
            }
            return o;
        }

        public void ZapiszJednostkiProduktow(List<ProduktBazowy> listaProduktow)
        {
            List<ProduktJednostka> pj = new List<ProduktJednostka>();
            HashSet<long> laczniki = new HashSet<long>();
            foreach (ProduktBazowy produkt in listaProduktow)
            {
                foreach (var j in produkt.Jednostki)
                {
                    pj.Add(new ProduktJednostka { Podstawowa = j.Podstawowa, JednostkaId = j.Id, ProduktId = produkt.Id, PrzelicznikIlosc = j.Przelicznik });
                    laczniki.Add(j.Lacznik.Id);
                }
            }

            Calosc.DostepDane.Usun<ProduktJednostka, long>(laczniki.ToList());
            Calosc.DostepDane.AktualizujListe(pj);
        }

        public void ZapiszZdjecieProduktu(IList<ProduktBazowy> obj)
        {
            var pz = (from produkt in obj where produkt.ZdjecieGlowne != null select new ProduktPlik { ProduktId = produkt.Id, PlikId = produkt.ZdjecieGlowne.Id, Glowny = true }).ToList();
            foreach (ProduktBazowy produkt in obj)
            {
                long id = produkt.Id;
                var listaLacznikowDoUsuniecia = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktPlik>(null, x => x.ProduktId == id && x.Glowny);
                SolexBllCalosc.PobierzInstancje.DostepDane.Usun<ProduktPlik, long>(listaLacznikowDoUsuniecia.Select(x=>x.Id).ToList());
            }
            Calosc.DostepDane.AktualizujListe<ProduktPlik>(pz);
        }

        //public List<Podpowiedz> Znajdz(string searchString, int count, int lang, string provider)
        //{
        //    var lacznie = ZnajdzProdukty(searchString, lang);
        //    List<Podpowiedz> wynik =
        //        lacznie.Take(count).Select(x => new Podpowiedz(x.Id.ToString(CultureInfo.InvariantCulture), x.Nazwa, PodpowiedziModul.Produkty)).ToList();
        //    return wynik;
        //}

        public List<ProduktBazowy> ZnajdzProdukty(string searchString, int? lang=null)
        {
            // List<Regex> regexy = Szukanie.PobierzInstancje.PobierzWyszukiwanieRegex(searchString);
            List<string> pola = new List<string> { "KodKreskowy", "DodatkoweKodyString", "Kod" };//kod kreskowy, dodatkowe kody kreskowe, symbol
            TypeAccessor akcesor = typeof(ProduktBazowy).PobierzRefleksja();
            if (lang != null)
            {
                return Calosc.DostepDane.Pobierz<ProduktBazowy>((int) lang, null, x => x.Widoczny && PasujeDoSzukania(x, searchString, pola, akcesor)).ToList();
            }
            return Calosc.DostepDane.Pobierz<ProduktBazowy>(null, x => x.Widoczny && PasujeDoSzukania(x, searchString, pola, akcesor) ).ToList();
        }

        public bool PasujeDoSzukania(ProduktBazowy pk, string fraza, List<string> pola, TypeAccessor akcesorRefleksji)
        {
            //Pola do importu są na sztywno dodane 
            if (pola == null)
            {
                pola = new List<string> {"KodKreskowy", "DodatkoweKodyString", "Kod"}; //kod kreskowy, dodatkowe kody kreskowe, symbol
            }

            foreach (string pole in pola)
            {
                string wartosc = (akcesorRefleksji[pk, pole]?? "").ToString();
                //w przypadku dodatkowych kodów kody sa połączone ze sobą spacją więc splituje i przeszukuje 
                string[] tabWartosci = wartosc.Split(' ');
                if (wartosc.Equals(fraza, StringComparison.InvariantCultureIgnoreCase) ||(tabWartosci.Any(x=>x.Equals(fraza,StringComparison.InvariantCultureIgnoreCase))))
                {
                    return true;
                }
            }
            return false;
        }

        //public bool PasujeDoSzukania(ProduktBazowy pk, List<Regex> regexy)
        //{
        //    Dictionary<Regex, bool> wystepowanieFraz = new Dictionary<Regex, bool>();
        //    foreach (Regex s in regexy)
        //    {
        //        wystepowanieFraz.Add(s, false);
        //    }
        //    List<string> pola = Calosc.Konfiguracja.ProduktyWyszukiwanie;
        //    foreach (string pole in pola)
        //    {
        //        string wartosc = (Refleksja.PobierzWartosc(pk, pole) ?? "").ToString();
        //        foreach (Regex wf in regexy)
        //        {
        //            if (wf.IsMatch(wartosc))
        //            {
        //                wystepowanieFraz[wf] = true;
        //            }
        //        }
        //    }
        //    return wystepowanieFraz.All(p => p.Value);
        //}


        public IEnumerable<long> PobierzProduktyPasujaceDoszukania(int jezyk, HashSet<long> produktyIds, string szukane)
        {
            if (string.IsNullOrEmpty(szukane))
            {
                return Calosc.DostepDane.Pobierz<ProduktBazowy>(null).Select(x => x.Id);
            }
            var produkty = Calosc.DostepDane.Pobierz<ProduktBazowy>(null, x => produktyIds.Contains(x.Id));
            var znalezione = SolexBllCalosc.PobierzInstancje.Szukanie.WyszukajObiekty(produkty, szukane, Calosc.Konfiguracja.ProduktyWyszukiwanie).Select(x => x.Id);
            return znalezione;
        }

        public IEnumerable<long> PobierzProduktyPasujaceDoszukania(int jezyk, string szukane)
        {
            return PobierzProduktyPasujaceDoszukania(jezyk, new HashSet<long>(Calosc.DostepDane.Pobierz<ProduktBazowy>(null).Select(x => x.Id)), szukane);
        }

        public IList<Indywidualizacja> BindPoSelectIndywidualizacji(int jezykId, IKlient zadajacy,IList<Indywidualizacja> obj, object parametrDoMetodyPoSelect = null)
        {
            if (obj.Any())
            {
                Dictionary<long, Waluta> waluty = SolexBllCalosc.PobierzInstancje.Konfiguracja.SlownikWalut;
                HashSet<long> idIndywidualizacja = new HashSet<long>( obj.Select(x => x.Id) );
                Dictionary<long, List<IndywidualizacjaCena>> ceny = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<IndywidualizacjaCena>(zadajacy, x => Sql.In(x.IdIndywidualizacji, idIndywidualizacja) && Sql.In(x.WalutaId, waluty.Keys)).GroupBy(x => x.IdIndywidualizacji).ToDictionary(x => x.Key, x => x.ToList());
                foreach (var indywidualizacja in obj)
                {
                    var cenyIndy = ceny[indywidualizacja.Id];
                    
                    HashSet<long> brakujaceWaluty = new HashSet<long>( waluty.Keys.Except(cenyIndy.Select(x => x.WalutaId)) );
                    if (brakujaceWaluty.Any())
                    {
                        foreach (long waluta in waluty.Keys.Where(x => brakujaceWaluty.Contains(x)))
                        {
                            cenyIndy.Add(new IndywidualizacjaCena(waluta));
                        }
                    }

                    indywidualizacja.CenyIndywidualizacja = cenyIndy.ToArray();
                }
            }
            return obj;
        }

        public void BindPoAktualizacjiIndywidualizacji(IList<Indywidualizacja> obj)
        {
            List<IndywidualizacjaCena> wynik = new List<IndywidualizacjaCena>();
            foreach (var indywidualizacja in obj)
            {
                foreach (var cena in indywidualizacja.CenyIndywidualizacja)
                {
                    if (cena.Cena == 0 && cena.NarzutTyp != NarzutTyp.Brak)
                    {
                        cena.NarzutTyp = NarzutTyp.Brak;
                    }
                    if (cena.NarzutTyp == NarzutTyp.Brak)
                    {
                        cena.Cena = null;
                    }
                    cena.IdIndywidualizacji = indywidualizacja.Id;
                    wynik.Add(cena);
                }
            }
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<IndywidualizacjaCena>(wynik);
        }


    }
}