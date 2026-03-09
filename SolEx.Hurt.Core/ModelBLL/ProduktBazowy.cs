using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Common;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.DostepDane.DaneObiektu;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL
{
    [Alias("Produkt")]
    [EdytowalnyAdmin]
    [FriendlyName("Produkt", FriendlyOpis = "")]
    public class ProduktBazowy : Produkt, IProduktBazowy, IStringIntern
    {
        public ISolexBllCalosc Calosc = SolexBllCalosc.PobierzInstancje;
        //autouzupelnienie po selecie
        [Ignore]
        public List<Konfekcje> GradacjePosortowane { get; set; }

        //pole uzupelniane magicznie w bindingu selecta
        [Ignore]
        public HashSet<Indywidualizacja> IndiwidualizacjeProduktu { get; set; }

        ///// <summary>
        ///// s³ownik zamienników. Jako klucz id roduktu, jako wartoœæ informacja czy zamiennik jest dwustronny. Jeœli NIE to znaczy ¿e jest jednostronny
        ///// </summary>
        [WidoczneListaAdmin(true, false, false, false)]
        [Ignore]
        public Dictionary<long,bool> Zamienniki { get; set; }
        
        [Ignore]
        public long bazoweID => base.Id;

        [Ignore]
        [FriendlyName("Menad¿er produktu")]
        public IKlient MenagerProduktu
        {
            get
            {
                if (MenagerId != null)
                {
                    return Calosc.DostepDane.PobierzPojedynczy<Klient>(MenagerId.Value);
                }
                return null;
            }
        }

        /// <summary>
        /// termin najblizszej dostawy CYKLICZNEJ. nie mylic z terminem dostawy erp
        /// </summary>
        [Ignore]
        [FriendlyName("Termin najbli¿szej dostawy cyklicznej")]
        [WidoczneListaAdmin(true, false, false, false)]
        public virtual DateTime? NajblizszaDostawa => Calosc.ProduktyBazowe.NajblizszaDostawa(this);

        private List<JednostkaProduktu> _jednoski;

        [Ignore]
        [FriendlyName("Jednostki dla produktu - lista obiektów")]
        public virtual List<JednostkaProduktu> Jednostki
        {
            get { return _jednoski; }
            set { _jednoski = value; }
        }

        [Ignore]
        [FriendlyName("Cecha rodzinna")]
        [WidoczneListaAdmin(true, false, false, false)]
        public CechyBll CechaUnikalnaRodzina { get; set; }

        private HashSet<long> _produktyWRodzinieIds = null;

        [Ignore]
        [FriendlyName("Produkty w rodzinie")]
        [WidoczneListaAdmin(true, false, false, false)]
        public virtual HashSet<long> ProduktyWRodzinieIds
        {
            get { return _produktyWRodzinieIds; }
            set { _produktyWRodzinieIds = value; }
        }

        [Ignore]
        [FriendlyName("Jednostka podstawowa")]
        [WidoczneListaAdmin(true, false, false, false)]
        public virtual JednostkaProduktu JednostkaPodstawowa
        {
            get
            {
                if (Jednostki == null)
                {
                    return null;
                }
                return Jednostki.FirstOrDefault(p => p.Podstawowa);
            }
        }


        [Ignore]
        [FriendlyName("Nazwa producenta")]
        [WidoczneListaAdmin(true, false, false, false)]
        public string MarkaNazwa
        {
            get
            {
                string wynik= null;
                KategorieBLL kat = Marki().FirstOrDefault();
                if (kat != null)
                {
                    if (!string.IsNullOrEmpty(kat.Nazwa))
                    {
                        wynik = kat.Nazwa;
                    }
                }
                return wynik;
            }
        }

        [Ignore]
        [FriendlyName("Dodatkowe kody produktów jako ci¹g znaków")]
        [WidoczneListaAdmin(true, false, false, false)]
        public string DodatkoweKodyString { get; set; }

        public List<KategorieBLL> Marki()
        {
            List<KategorieBLL> wynik = new List<KategorieBLL>();
            var grupy = Calosc.DostepDane.Pobierz<GrupaBLL>(null);
            if (Kategorie == null)
            {
                return wynik;
            }
            List<KategorieBLL> bazowe = Kategorie.Where(a => grupy.Where(p => p.Producencka).Select(p => p.Id).Contains(a.GrupaId)).ToList();
            foreach (KategorieBLL kategoria in bazowe)
            {
                if (kategoria != null && kategoria.ParentId != null)
                {
                    KategorieBLL kategoriaParent = Calosc.DostepDane.Pobierz<KategorieBLL>(JezykId, null,
                        x => x.ParentId == kategoria.ParentId.Value).First();
                    wynik.Add(kategoriaParent);
                }
                wynik.Add(kategoria);
            }
            return wynik;
        }

        [FriendlyName("Minimum logistyczne")]
        [WidoczneListaAdmin(true, false, false, false)]
        public new decimal IloscMinimalna
        {
            get
            {
                if (WymaganeOz && base.IloscMinimalna < IloscWOpakowaniu)
                {
                    return IloscWOpakowaniu;
                }
                return base.IloscMinimalna;
            }
            set { base.IloscMinimalna = value; }
        }

        [Ignore]
        public HashSet<long> CechyProduktuWystepujaceWRabatach { get; set; }

        public ProduktBazowy() {}

        public ProduktBazowy(int jezykId): this()
        {
            JezykId = jezykId;
        }

        internal ProduktBazowy(ProduktBazowy bazowy): base(bazowy)
        {
            if (bazowy != null)
            {
                //properotsy magienicze uzupelnie trzeba tu przepisac - wyliczane zostawiamy bo sie wylicza jeszcze raz ;)
                JezykId = bazowy.JezykId;
                this.CechyProduktuWystepujaceWRabatach = bazowy.CechyProduktuWystepujaceWRabatach;
                this.Zamienniki = bazowy.Zamienniki;
                this.CechaUnikalnaRodzina = bazowy.CechaUnikalnaRodzina;
                this._produktyWRodzinieIds = bazowy._produktyWRodzinieIds;
                this._kategoriaID = bazowy._kategoriaID;
                this.Kategorie = bazowy.Kategorie;
                this.IndiwidualizacjeProduktu = bazowy.IndiwidualizacjeProduktu;
              
                //bch: zmieniam bo juz nie mam pomyslu co sie pieprzy tak cache klientow
                this.IdCechPRoduktu = new HashSet<long>( bazowy.IdCechPRoduktu);


                this.GradacjePosortowane = bazowy.GradacjePosortowane;
                this._friendlyLinkURL = bazowy._friendlyLinkURL;
                this._zdjecia = bazowy._zdjecia;
                this.Calosc = bazowy.Calosc;
                this.DostawaData = bazowy.DostawaData;
                this.CenyPoziomy = bazowy.CenyPoziomy;
                this._jednoski = bazowy._jednoski;
                this.PobierzTypStany = bazowy.PobierzTypStany;
                this.DodatkoweKodyString = bazowy.DodatkoweKodyString;
            }
        }

        private HashSet<long> _kategoriaID = null;

        [Ignore]
        public virtual HashSet<long> KategorieId {
            get { return this._kategoriaID; }
            set { _kategoriaID = value; } }

        [Ignore]
        [FriendlyName("Kategorie produktu")]
        [GrupaAtttribute("Kategorie produktu", 3)]
        [WidoczneListaAdmin(true, true, true, false, true, new[] { typeof(ProduktBazowy) })]
        public List<KategorieBLL> Kategorie { get; set; }

        private List<IObrazek> _zdjecia;

        [Ignore]
        [FriendlyName("Zdjêcia produktu")]
        public virtual List<IObrazek> Zdjecia
        {
            get {   return _zdjecia; }
            set { _zdjecia = value; }
        }

        [Ignore]
        public IObrazek ZdjecieGlowne
        {
            get
            {
                return Zdjecia != null?Zdjecia.FirstOrDefault():null;
            }
            set
            {
                if (Zdjecia.Count == 0)
                {
                    Zdjecia.Add(value);
                    return;
                }
                Zdjecia[0] = value;
            }
        }

        private long? _obrazek;

        private DateTime? _dostawaData = DateTime.MinValue;

        [Ignore]
        [FriendlyName("Data dostawy")]
        [WidoczneListaAdmin(true, false, false, false)]
        public virtual DateTime? DostawaData
        {
            get
            {
                if (_dostawaData == DateTime.MinValue)
                {
                    DateTime d;
                    if (TextHelper.PobierzInstancje.SprobojSparsowac(Dostawa, out d))
                    {
                        _dostawaData = d;
                    }
                    else
                    {
                        _dostawaData = null;
                    }
                }
                return _dostawaData;
            }
            private set { _dostawaData = value; }
        }
      
        public List<CechyBll> CechyDlaAtrybutu(int atrybutId, bool tylkoWidoczne = true)
        {
            if (tylkoWidoczne)
            {
                return Cechy.Where(p => p.Value.AtrybutId == atrybutId && p.Value.Widoczna).Select(x => x.Value).ToList();
            }
            else
            {
                return Cechy.Where(p => p.Value.AtrybutId == atrybutId).Select(x => x.Value).ToList();
            }
        }

        public CechyBll PobierzCeche(int cechaid)
        {
            if (Cechy.ContainsKey(cechaid) && Cechy[cechaid].Widoczna)
            {
                return Cechy[cechaid];
            }
            return null;
        }

        private Dictionary<int, List<CechyBll>> _listaCechWgAtrybutow = null;

        public Dictionary<int, List<CechyBll>> ListaCechWgAtrybutow()
        {
            if (_listaCechWgAtrybutow == null)
            {
                _listaCechWgAtrybutow = this.Cechy.Values.Where(x=> x.AtrybutId.HasValue).GroupBy(x=>x.AtrybutId.Value).ToDictionary(x => x.Key, x=> x.ToList());
            }
            return _listaCechWgAtrybutow;
        }

        [Ignore]
        [FriendlyName("Cechy produktu")]
        [WidoczneListaAdmin(true, true, true, false, true, new[] {typeof(ProduktBazowy)})]
        [GrupaAtttribute("Lista cech", 2)]
        public List<CechyBll> ListaCech => Cechy.Values.ToList();

        [Ignore]
        public virtual Dictionary<long, CechyBll> Cechy
        {
            get
            {
                if (IdCechPRoduktu != null && IdCechPRoduktu.Any())
                {
                    return Calosc.CechyAtrybuty.PobierzCechyOId(IdCechPRoduktu, JezykId).ToDictionary(x => x.Id, x => x);
                }
                return new Dictionary<long, CechyBll>();
            }
        }

        public virtual bool PosiadaCechy(long[] cechy, bool maMiecWszystkieCechy)
        {
            return PosiadaCechy(this.IdCechPRoduktu, cechy, maMiecWszystkieCechy);
        }

        /// <summary>
        /// Funkcja sprawdzaj¹ca, czy cechyProduktu znajduj¹ siê w kolekcji oczekiwanch cech.
        /// Dlatego w osobnej funkcji bo klasa dziedzicz¹ca 'ProduktKlienta' ma s³owo kluczowe 'new' na propercie IdCechProduktu
        /// </summary>
        /// <param name="cechyProduktu"></param>
        /// <param name="oczekiwaneCechy"></param>
        /// <param name="maMiecWszystkieCechy"></param>
        /// <returns></returns>
        protected bool PosiadaCechy(HashSet<long> cechyProduktu, long[] oczekiwaneCechy, bool maMiecWszystkieCechy)
        {
            if (cechyProduktu == null || !cechyProduktu.Any() || oczekiwaneCechy == null || !oczekiwaneCechy.Any())
            {
                return false;
            }

            if (maMiecWszystkieCechy)
            {
                return oczekiwaneCechy.All(x => cechyProduktu.Contains(x));
            }

            return cechyProduktu.Overlaps(oczekiwaneCechy);
        }

        private Dictionary<int, CenaPoziomu> _ceny;

        [Ignore]
        [FriendlyName("Poziomy cen")]
        public Dictionary<int, CenaPoziomu> CenyPoziomy
        {
            get
            {
                if (_ceny == null || !_ceny.Any())
                {
                    _ceny= Calosc.CenyPoziomy.PobierzCenyProduktu(Id);
                }
                return _ceny;
            }
            private set { _ceny = value; }
        }


        [Ignore]
        [FriendlyName("Id cech produktu")]
        public  HashSet<long> IdCechPRoduktu { get; set; }

        [Ignore]
        public int JezykId { get; set; }


        private TypStanu? _typstany = null;

        [Ignore]
        public virtual TypStanu PobierzTypStany
        {
            get
            {
                if (_typstany == null)
                {
                    _typstany = Calosc.ProduktyBazowe.WyliczTypStanu(this);
                }
                return _typstany.Value;
            }
            private set { _typstany = value; }
        }


        [Ignore]
        [FriendlyName("Stan")]
        [WidoczneListaAdmin(true, false, false, false)]
        public virtual decimal IloscLaczna
        {
            get
            {
                return PobierzStan((HashSet<int>)null);
            }
        }

        [Ignore]
        [FriendlyName("Czy produkt jest aktualnie na stanie")]
        [WidoczneListaAdmin(true, false, false, false)]
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

        /// <summary>
        /// Pobieramy stan produktu dla wybranego mazazynu
        /// </summary>
        /// <param name="idMagazynow">HashSet z id magazynów</param>
        /// <returns></returns>
        public decimal PobierzStan(HashSet<int> idMagazynow)
        {
            Dictionary<int, Dictionary<long, decimal>> stany = Calosc.ProduktyStanBll.WszystkieStanyDlaMagazynow();

            if (stany.IsEmpty())
            {
                return 0;
            }

            decimal s = 0;
            if (idMagazynow == null || !idMagazynow.Any())
            {
                idMagazynow = new HashSet<int>( stany.Keys );
            }

            foreach (var i in idMagazynow)
            {
                Dictionary<long, decimal> stanyDlaMagazynu;
                if (!stany.TryGetValue(i, out stanyDlaMagazynu))
                {
                    throw new Exception($"Brak magazynu o id: {i}. Usuñ magazyn lub zrezygnuj z pokazywania stanów tego magazynu");
                }

                decimal stanTemp;
                if (stanyDlaMagazynu != null && stanyDlaMagazynu.Any() && stanyDlaMagazynu.TryGetValue(this.bazoweID, out stanTemp))
                {
                    s += stanTemp;
                }
            }
            return s;
        }

        /// <summary>
        /// Pobiera stan produktu dla wybranych magazynów
        /// </summary>
        /// <param name="nazwyMagazynow">HashSet z nazwami magazynów</param>
        /// <returns></returns>
        public decimal PobierzStan(HashSet<string> nazwyMagazynow)
        {
            HashSet<int> idMagazynow;
            if (nazwyMagazynow == null || !nazwyMagazynow.Any())
            {
                idMagazynow = null;
            }
            else
            {
                idMagazynow = new HashSet<int>( Calosc.Konfiguracja.SlownikMagazynowPoSymbolu.WhereKeyIsIn(nazwyMagazynow).Select(x => x.Id) );
            }

            return PobierzStan(idMagazynow);
        }

        private string _friendlyLinkURL = null;

        //autowyliczane w pobieraniu z bazy
        [Ignore]
        public virtual string FriendlyLinkURL { get { return _friendlyLinkURL; } set { _friendlyLinkURL = value; } }
    }
}