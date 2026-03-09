using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL
{
    public class PunktNaMape
    {
        public PunktNaMape()
        {}

        public PunktNaMape(SklepyBll sklep, string obrazekPineskaId)
        {
            this.Title = sklep.Nazwa;
            this.Lat = sklep.Lat;
            this.Lon = sklep.Lon;
            this.Id = sklep.Id;
            this.ObrazekPineskaId = obrazekPineskaId;
        }

        public string Title { get; set; }
        public decimal Lat { get; set; }
        public decimal Lon { get; set; }
        public string Desc { get; set; }
        public long Id { get; set; }
        public string ObrazekPineskaId { get; set; }
    }


    [Alias("Sklep")]
    [FriendlyName("Sklep", FriendlyOpis = "")]
    public class SklepyBll : Sklep, ISklepyBll
    {
        public ISolexBllCalosc SolexBll = SolexBllCalosc.PobierzInstancje;
        private SesjaHelper _sesjaHelper = SesjaHelper.PobierzInstancje;
        private IAdres _adres;

        public SklepyBll()
        {
            DataUtworzenia = DateTime.Now;
            AdresId = null;
        }

        [Ignore]
        public IAdres Adres
        {
            get
            {
                _adres = new Adres() { KodPocztowy = KodPocztowy, KrajId = KrajId, UlicaNr = UlicaNr, Miasto = Miasto, Lat = Lat, Lon = Lon };
                return new Adres(_adres);
            }
        }

        private string _miasto = "";

        [Ignore]
        [FriendlyName("Miasto")]
        [GrupaAtttribute("Szczegóły", 0)]
        [WidoczneListaAdmin(true, true, true, false)]
        [Niewymagane]
        public string Miasto
        {
            get
            {
                if (AdresId == null || AdresId == 0)
                {
                    return _miasto;
                }
                return SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId).Miasto;
            }
            set
            {
                if (AdresId == null || AdresId == 0)
                {
                    _miasto = value;
                }
                else
                {
                    Model.Adres adres = SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId);
                    adres.Miasto = value;
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(adres);
                }
            }
        }

        private string _ulicaNr = "";

        [Ignore]
        [FriendlyName("Ulica")]
        [GrupaAtttribute("Szczegóły", 0)]
        [WidoczneListaAdmin(true, true, true, false)]
        [Niewymagane]
        public string UlicaNr
        {
            get
            {
                if (AdresId == null || AdresId == 0)
                {
                    return _ulicaNr;
                }
                return SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId).UlicaNr;
            }
            set
            {
                if (AdresId == null || AdresId == 0)
                {
                    _ulicaNr = value;
                }
                else
                {
                    Model.Adres adres = SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId);
                    adres.UlicaNr = value;
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(adres);
                }
            }
        }

        private string _kodPocztowy;
        [Ignore]
        [FriendlyName("Kod pocztowy")]
        [GrupaAtttribute("Szczegóły", 0)]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        public string KodPocztowy
        {
            get
            {
                if (AdresId == null || AdresId == 0)
                {
                    return _kodPocztowy;
                }
                return SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId).KodPocztowy;
            }
            set
            {
                if (AdresId == null || AdresId == 0)
                {
                    _kodPocztowy = value;
                }
                else
                {
                    Model.Adres adres = SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId);
                    adres.KodPocztowy = value;
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(adres);
                } 
            }
        }
        private int? _kraj;

        [Ignore]
        [FriendlyName("Kraj")]
        [PobieranieSlownika(typeof(SlownikKrajow))]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        public int? KrajId
        {
            get
            {
                if (AdresId == null || AdresId == null && AdresId == 0)
                {
                    return _kraj;
                }
                return SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId).KrajId;
            }
            set
            {
                if (AdresId == null || AdresId == 0)
                {
                    _kraj = value;
                }
                else
                {
                    Model.Adres adres = SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId);
                    adres.KrajId = value;
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(adres);
                }
            }
        }
        private int? _region;
        [PobieranieSlownika(typeof(SlownikRegionow))]
        [Niewymagane]
        [Ignore]
        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Szczegóły", 0)]
        [FriendlyName("Region")]
        public int? RegionId
        {
            get
            {
                if (AdresId == null || AdresId == null && AdresId == 0)
                {
                    return _region;
                }
                return SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId).RegionId;
            }
            set
            {
                if (AdresId == null || AdresId == 0)
                {
                    _region = value;
                }
                else
                {
                    Model.Adres adres = SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId);
                    adres.RegionId = value;
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(adres);
                }
            }
        }

        private decimal _lat = 0;
        [Ignore]
        [GrupaAtttribute("Współrzędne", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        public decimal Lat
        {
            get
            {
                if (AdresId == null || AdresId == 0)
                {
                    return _lat;
                }
                return SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId).Lat;
            }
            set
            {
                if (AdresId == null || AdresId == 0)
                {
                    _lat = value;
                }
                else
                {
                    Model.Adres adres = SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId);
                    adres.Lat = value;
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(adres);
                } 
            }
        }

        private decimal _lon = 0;
        [Ignore]
        [GrupaAtttribute("Współrzędne", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        public decimal Lon
        {
            get
            {
               if (AdresId == null || AdresId == 0)
                {
                    return _lon;
                }
                return SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId).Lon;
            }
            set
            {
                if (AdresId == null || AdresId == 0)
                {
                    _lon = value;
                }
                else
                {
                    Model.Adres adres = SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId);
                    adres.Lon = value;
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(adres);
                }
            }
        }

        private string _telefon = "";
        [Ignore]
        [FriendlyName("Telefon")]
        [GrupaAtttribute("Szczegóły", 0)]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        public string Telefon
        {
            get
            {
                if (AdresId == null || AdresId == 0)
                {
                    return _telefon;
                }
                return SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId).Telefon;
            }
            set
            {
                if (AdresId == null || AdresId == 0)
                {
                    _telefon = value;
                }
                else
                {
                    Model.Adres adres = SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId);
                    adres.Telefon = value;
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(adres);
                }
            }
        }

        private string _email = "";
        [Ignore]
        [FriendlyName("WiadomoscEmail")]
        [GrupaAtttribute("Szczegóły", 0)]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        public string Email
        {
            get
            {
                if (AdresId == null || AdresId == 0)
                {
                    return _email;
                }
                return SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId).Email;
            }
            set
            {
                if (AdresId == null || AdresId == 0)
                {
                    _email = value;
                }
                else
                {
                    Model.Adres adres = SolexBll.DostepDane.PobierzPojedynczy<Model.Adres>(AdresId);
                    adres.Email = value;
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(adres);
                }
            }
        }

        [Ignore]
        [FriendlyName("Osoba która dodała sklep")]
        public string Autor
        {
            get
            {
                if (AutorId.HasValue)
                {
                    return SolexBll.DostepDane.PobierzPojedynczy<Klient>(AutorId.Value).Nazwa;
                }
                return "";
            }
        }

        public SklepyBll(Sklep bazowy) : base(bazowy)
        {
        }

        private long[] kategorieId = null;
      
        [Ignore]
        [FriendlyName("Kategorie do których przypisany jest sklep")]
        [PobieranieSlownika(typeof(SlownikKategoriiSklepow))]
        [WidoczneListaAdmin(true, false, true, false)]
        [GrupaAtttribute("Szczegóły", 0)]
        public long[] KategorieId
        {
            get
            {
                if (kategorieId == null)
                {
                    List<SklepKategoriaSklepu> obj = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<SklepKategoriaSklepu>(null).Where(x => x.SklepId == this.Id).ToList();
                    kategorieId = obj.Select(x => x.KategoriaSklepuId).ToArray();
                }
                return kategorieId;
            }
            set { kategorieId = value; }
           
        }


        public IList<KategoriaSklepu> PobierzKategorie(int jezykId)
        {
            List<KategoriaSklepu> lista = null;
            if (KategorieId.Any())
            {

                lista =
                    SolexBll.DostepDane.Pobierz<KategoriaSklepu>(jezykId, null,
                        x => Sql.In(x.Id, KategorieId)).ToList();
            }
            else
            {
                lista =
                   SolexBll.DostepDane.Pobierz<KategoriaSklepu>(jezykId, null, null).ToList();
            }
            return lista;
        }

        private IObrazek _obrazek;

        [Ignore]
        public IObrazek Obrazek
        {
            get
            {
                if (_obrazek == null && ObrazekId.HasValue)
                {
                    _obrazek = SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(ObrazekId.Value);
                }
                if (_obrazek == null && Id > 0)
                {
                    IKlient k = SolexBll.DostepDane.PobierzPojedynczy<Klient>(Id);
                    if (k != null)
                    {
                        return k.Obrazek;
                    }
                }
                return _obrazek;
            }
        }

        [Ignore]
        [FriendlyName("Poprawne koordynaty")]
        [WidoczneListaAdmin(true, true, false, false)]
        public bool CzyPoprawneKoordynaty { get { return Lat != 0 && Lon != 0 && Lat != -1 && Lon != -1; } }

    }
}
