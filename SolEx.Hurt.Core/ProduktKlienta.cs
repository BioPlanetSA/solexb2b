using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ServiceStack.Common;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core
{
    public class ProduktKlienta : ProduktBazowy, IProduktKlienta
    {
        //nadpisuje go wirtualny produkt
        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Id produktu")]
        public virtual long Id => base.Id;

        [Ignore]
        private long IdBazowe { get; set; }

        private readonly ISolexBllCalosc _calosc = SolexBllCalosc.PobierzInstancje;

        /// <summary>
        /// Zwraca Id Cech produktu 
        /// </summary>
      //  public new HashSet<long> IdCechPRoduktu { get; set; }

     //   public new Dictionary<long, CechyBll> Cechy { get; set; }

        //public virtual List<JednostkaProduktu> Jednostki
        //{
        //    get { return base.Jednostki; }
        //}

        private IKlient _klient;

        public ProduktKlienta()
        {
            IdBazowe = (this as ProduktBazowy).Id;
        }

        /// <summary>
        /// Dlatego override bo tutaj IdCechProduktu ma s³owo kluczowe 'new'
        /// </summary>
        /// <param name="cechy"></param>
        /// <param name="maMiecWszystkieCechy"></param>
        /// <returns></returns>
        //public override bool PosiadaCechy(long[] cechy, bool maMiecWszystkieCechy)
        //{
        //    return base.PosiadaCechy(this.IdCechPRoduktu, cechy, maMiecWszystkieCechy);
        //}

        /// <summary>
        /// ten konstruktor jest tylo do testów i w uzasadnionych przypadkach - tworzy fake produkt który NIE jest uzupe³niony prawid³owo o wszystkie propertisy
        /// </summary>
        /// <param name="bazowy"></param>
        /// <param name="klient"></param>
        public ProduktKlienta(ProduktBazowy bazowy, IKlient klient) : base(bazowy)
        {
            _klient = klient;
            IdBazowe = bazowy.Id;
            base.Zamienniki = bazowy.Zamienniki;
            Vat = klient.IndywidualnaStawaVat.GetValueOrDefault(((ProduktBazowy) bazowy).Vat);
        }

        private IFlatCenyBLL _cena;

        public virtual IFlatCenyBLL FlatCeny
        {
            get
            {
                //cena jest cachowana. Jesli jest potrzeba przeliczanie ceny - np. w gradacji to jest metoda WymusPrzeliczanieCeny
                if (_cena == null)
                {
                    _cena = SolexBllCalosc.PobierzInstancje.Rabaty.PobierzCeneProduktuDlaKlienta(this);

                    //przeliczenie ceny za kg i za litr
                    this.CenaZaKG = null;
                    this.CenaZaLitr = null;

                    //cena za kg tylko jesli jest masa uzupelniona
                    if (_cena.CenaNetto.Wartosc != 0)
                    {
                        //masa w bio jest z pola a nie z MASY - bo tak chcieli
                        if (this.PoleLiczba1.HasValue && this.PoleLiczba1 != 0)
                        {
                            this.CenaZaKG = Math.Round( _cena.CenaNetto.Wartosc / this.PoleLiczba1.Value * 1000, 2);
                        }

                        if (this.Objetosc.HasValue && this.Objetosc != 0 )
                        {
                            this.CenaZaLitr = Math.Round( _cena.CenaNetto.Wartosc / this.Objetosc.Value * 1000, 2);
                        }
                    }
                }
                return _cena;
            }
        }

        public decimal? CenaZaKG { get; private set; }

        public decimal? CenaZaLitr { get; private set; }

        /// <summary>
        /// metoda ktora kasuje flat ceny i zmusza do powtornego przeliczanie cena - np. po zmienie gradacji
        /// </summary>
        public void WymusPrzeliczanieCeny()
        {
            _cena = null;
        }

        private KategorieBLL _marka;

        //tylko marki widoczna dla danego klienta        
        public KategorieBLL Marka
        {
            get
            {
                if (_marka != null)
                {
                    return _marka;
                }

                if (base.Marki() == null)
                {
                    return null;
                }
                HashSet<long> dostepne = SolexBllCalosc.PobierzInstancje.ProduktyKlienta.PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(_klient);
                KategorieBLL kategoria = base.Marki().FirstOrDefault(a => SolexBllCalosc.PobierzInstancje.KategorieDostep.JestWidocznaDlaKlienta(a, _klient, dostepne));
                return _marka = kategoria;
            }
        }

        [Ignore]
        public List<Konfekcje> GradacjePosortowane { get; set; }

        [Ignore]
        public HashSet<long> GradacjeProduktyKtorychZakupyLiczycWspolnie { get; set; }

     //   private List<KategorieBLL> _kategorieNadpisaneDlaKlienta = null;
       
        //tylko kategorie widoczna dla danego klienta
        //public List<KategorieBLL> Kategorie
        //{
        //    get
        //    {
        //        if (_kategorieNadpisaneDlaKlienta == null)
        //        {
        //            return base.Kategorie;
        //        }
        //        return _kategorieNadpisaneDlaKlienta;
        //    }
        //    set { _kategorieNadpisaneDlaKlienta = value; }
        //}

        public string OpisZbiorczyZKategorii()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var kat in Kategorie)
            {
                if (!string.IsNullOrEmpty(kat.OpisNaProdukt))
                {
                    sb.Append(kat.OpisNaProdukt);
                    sb.Append("<br/>");
                }
            }
            var grupy = Kategorie.Select(x => x.GrupaId);
            var gru = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<GrupaBLL>(base.JezykId, null, x => grupy.Contains(x.Id)).ToArray();
            foreach (var g in gru)
            {
                if (!string.IsNullOrEmpty(g.OpisZbiorczy))
                {
                    sb.Append(g.OpisZbiorczy);
                    sb.Append("<br/>");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// vat dla klienta - np. zwolnionego
        /// </summary>
        public new decimal Vat { get; set; }

        //public  WartoscLiczbowaZaokraglana StawkaVat()
        //{
        //     return new WartoscLiczbowaZaokraglana(Klient.IndywidualnaStawaVat.GetValueOrDefault(base.Vat)); 
        //}

        [Ignore]
        public virtual decimal? DostepnyLimit
        {
            get { return LimityIloscioweBLL.WyliczLimit(Klient.Id, base.Id); }
        }

        /// <summary>
        /// Wyszukuje w produtkach z rodziny danego prodkuktu, wszystkie wg. podanych ID
        /// </summary>
        /// <param name="idProduktowTylkoZKtorychWybrac">wyciaga tylko produkty ktorych id jest na tej liscie</param>
        /// <returns></returns>
        [FriendlyName("Produkty w rodzinie")]
        public virtual List<IProduktKlienta> ProduktyWRodzinie(HashSet<long> idProduktowTylkoZKtorychWybrac = null)
        {
            if (string.IsNullOrEmpty(this.Rodzina) || this.ProduktyWRodzinieIds == null)
            {
                throw new Exception("Produkt nie jest rodzinowy");
            }
           
             return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktKlienta>(base.JezykId, Klient, x => this.ProduktyWRodzinieIds.Contains(x.Id) && (idProduktowTylkoZKtorychWybrac==null || idProduktowTylkoZKtorychWybrac.Contains(x.Id)))
                    .OrderBy(x=>x.CechaUnikalnaRodzina.Kolejnosc).ThenBy(x=>x.Nazwa).Select(x => x as IProduktKlienta).ToList();
        }

        public bool JestWMoimKatalogu()
        {
            return Klient.MojKatalog != null && Klient.MojKatalog.Contains(Id);
        }

        public bool JestWUlubionych()
        {
            return Klient.IdUlubionych != null && Klient.IdUlubionych.Contains(Id);
        }

        [Ignore]
        public decimal IloscOgraniczonaDoMax
        {
            get
            {
                return IloscLaczna > SolexBllCalosc.PobierzInstancje.Konfiguracja.MaxPokazywanyStan ? SolexBllCalosc.PobierzInstancje.Konfiguracja.MaxPokazywanyStan : IloscLaczna;
            }
        }
        [Ignore]
        public virtual decimal IloscLaczna
        {
            get
            {
                if (Klient.DostepneMagazyny == null || !Klient.DostepneMagazyny.Any())
                {
                    return base.IloscLaczna;
                }
                return PobierzStan(Klient.DostepneMagazyny);
            }
        }

        [Ignore]
        public virtual bool NaStanie
        {
            get
            {
                if (Typ == TypProduktu.Usluga)
                {
                    return true;
                }
                return IloscLaczna > 0;
            }
        }

        public List<CechyBll> PobierzMetkaPozycjaKoszykProdukty(MetkaPozycjaKoszykProdukty pozycja)
        {
            return _calosc.CechyAtrybuty.PobierzMetkaPozycjaKoszykProdukty(pozycja, IdCechPRoduktu, base.JezykId, Klient.Id != 0);
        }

        public List<CechyBll> PobierzMetkiLista(MetkaPozycjaLista pozycja)
        {
            return _calosc.CechyAtrybuty.PobierzMetkiLista(pozycja, IdCechPRoduktu, base.JezykId, Klient.Id!=0);
        }

        public List<CechyBll> PobierzMetkaPozycjaKoszykGratisy(MetkaPozycjaKoszykGratisy pozycja)
        {
            return _calosc.CechyAtrybuty.PobierzMetkaPozycjaKoszykGratisy(pozycja, IdCechPRoduktu, base.JezykId, Klient.Id != 0);
        }

        public List<CechyBll> PobierzMetkaPozycjaKoszykGratisyPopUp(MetkaPozycjaKoszykGratisyPopUp pozycja)
        {
            return _calosc.CechyAtrybuty.PobierzMetkaPozycjaKoszykGratisyPopUp(pozycja, IdCechPRoduktu, base.JezykId, Klient.Id != 0);
        }

        public List<CechyBll> PobierzMetkaPozycjaKoszykAutomatyczne(MetkaPozycjaKoszykAutomatyczne pozycja)
        {
            return _calosc.CechyAtrybuty.PobierzMetkaPozycjaKoszykAutomatyczne(pozycja, IdCechPRoduktu, base.JezykId, Klient.Id != 0);
        }

        public List<CechyBll> PobierzMetkiRodzina(MetkaPozycjaRodziny pozycja)
        {
            return _calosc.CechyAtrybuty.PobierzMetkiRodzina(pozycja, IdCechPRoduktu, base.JezykId, Klient.Id != 0);
        }

        public List<CechyBll> PobierzMetkiSzczegoly(MetkaPozycjaSzczegoly pozycja)
        {
            return _calosc.CechyAtrybuty.PobierzMetkiSzczegoly(pozycja, IdCechPRoduktu, base.JezykId, Klient.Id != 0);
        }

        public List<CechyBll> PobierzMetkiSzczegolyWarianty(MetkaPozycjaSzczegolyWarianty pozycja)
        {
            return _calosc.CechyAtrybuty.PobierzMetkiSzczegolyWarianty(pozycja, IdCechPRoduktu, base.JezykId, Klient.Id != 0);
        }

        public List<CechyBll> PobierzMetkiKafle(MetkaPozycjaKafle pozycja)
        {
            return _calosc.CechyAtrybuty.PobierzMetkiKafleProduktu(pozycja, IdCechPRoduktu, base.JezykId, Klient.Id != 0);
        }

        public bool CzyJestKoncesja()
        {
            if (base.WymaganaKoncesja == null || !base.WymaganaKoncesja.Any()) return true;
            if (_klient.Koncesja == null || !_klient.Koncesja.Any()) return false;
            return base.WymaganaKoncesja.IsSubsetOf(_klient.Koncesja);
        }

        public bool CzyWszystkieDzieciMajaTaSamaCene(ref IFlatCenyBLL cenaMinimalna)
        {
            var prod = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktKlienta>(base.JezykId, Klient, x => this.ProduktyWRodzinieIds.Contains(x.Id));
            IFlatCenyBLL cenaMin = FlatCeny;

            var innaCena = prod.Any(x => x.FlatCeny.CenaNetto.Wartosc != cenaMin.CenaNetto.Wartosc);
            if (innaCena)
            {
                foreach (var fc in prod.Where(x=>x.FlatCeny.CenaNetto>0))
                {
                    if (fc.FlatCeny.CenaNetto < cenaMin.CenaNetto)
                    {
                        cenaMin = fc.FlatCeny;
                    }
                }
                cenaMinimalna = cenaMin;
                return false;
            }
            cenaMinimalna = cenaMin;
            return true;
        }

        [Ignore]
        public IKlient Klient
        {
            get { return _klient; }
            set { _klient = value; }
        }

        private TypStanu? _typstany;

        [Ignore]
        public virtual TypStanu PobierzTypStany
        {
            get
            {
                if (_typstany == null)
                {
                    _typstany = Klient.DostepneMagazyny == null ? base.PobierzTypStany : SolexBllCalosc.PobierzInstancje.ProduktyBazowe.WyliczTypStanu(this);
                }
                return _typstany.Value;

            }
        }
    }

    public class ProduktKlientaWirtualny : ProduktKlienta
    {
        public override string Nazwa { get; set; }

        public override string Rodzina { get; set; }

        public override List<IObrazek> Zdjecia { get; set; }

        public ProduktKlientaWirtualny(ProduktBazowy bazowy, IKlient klient, long idWirtualne) : base(bazowy, klient)
        {
            _idWirtualne = idWirtualne;
            Nazwa = bazowy.Nazwa;           
            ProduktyWRodzinieIds = bazowy.ProduktyWRodzinieIds;
            Zdjecia = bazowy.Zdjecia;
            KategorieId = bazowy.KategorieId;
        }

        public override string FriendlyLinkURL { get; set; }

        public override HashSet<long> KategorieId { get; set; }

        public override long Id { get { return _idWirtualne; } }

        // public override bool Abstrakcyjny { get { return false; } } // produkt wirtualny nie jest nigdy abstrakcyjny bo jest wirtualny ;)

        protected long _idWirtualne;

       // [KolekcjaProduktowDoWybranychProduktow]
        public override HashSet<long> ProduktyWRodzinieIds { get; set; }

    }
}