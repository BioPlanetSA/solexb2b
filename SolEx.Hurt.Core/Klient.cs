using ServiceStack.Common;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.DostepDane.DaneObiektu;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core
{
    [EdytowalnyAdmin]
    [FriendlyName("Klient", FriendlyOpis = "")]
    public class Klient : Model.Klient, IKlient
    {
        private ISolexBllCalosc Calosc = SolexBllCalosc.PobierzInstancje;
        private IObrazek _obrazek;
        [Ignore]
        public IObrazek Obrazek
        {
            get
            {
                if (_obrazek == null && ZdjecieId.HasValue)
                {
                    _obrazek = Calosc.Pliki.PobierzObrazek(ZdjecieId.Value);
                }
       
                return _obrazek;
            }
        }


        public override string MagazynDomyslny
        {
            get
            {
                if (string.IsNullOrEmpty(base.MagazynDomyslny) &&KlientNadrzedny!=null)
                {
                    return KlientNadrzedny.MagazynDomyslny;
                }
                return base.MagazynDomyslny;
            }
            set { base.MagazynDomyslny = value; }
        }

        /// <summary>
        /// jeśli to oddział, to ma swoj katalog  z zasobami. jesli nie bedzie to oddzial to wywali blad
        /// </summary>
        public string PobierzKatalogZasobowOddzialu()
        {
            if (SolexBllCalosc.PobierzInstancje.Konfiguracja.GetLicense(Licencje.Partnerzy))
                {
                    if (OddzialDoJakiegoNalezyKlient != 0)
                    {
                        return $"\\zasoby\\partnerzy\\{OddzialDoJakiegoNalezyKlient}\\";
                    }
                    throw new Exception("Klient nie należy do żadnego oddziału!");
                }
                return "";
        }

        [Ignore]
        public HashSet<Komunikaty> Komunikaty { get; set; }


        private int[] _kategorie;

        [Niewymagane]
        [Ignore]
        [FriendlyName("Kategorie klienta")]
        [WidoczneListaAdmin(true, false, true, false)]
        [PobieranieSlownika(typeof(SlownikKategoriiKlienta))]
        public virtual int[] Kategorie
        {
            get
            {
                if (KlientNadrzednyId != null) return KlientNadrzedny.Kategorie;
                if (_kategorie == null)
                {
                    _kategorie = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KlientKategoriaKlienta>(null,x => x.KlientId == Id).Select(p => p.KategoriaKlientaId).ToArray();
                }
                return _kategorie;
            }
            set { _kategorie = value; }
        }

        private IKlient _klientNadrzedny = null;

        [Ignore]
        public IKlient KlientNadrzedny
        {
            get
            {
                if (_klientNadrzedny != null)
                {
                    return _klientNadrzedny;
                }
                if (KlientNadrzednyId == null) return _klientNadrzedny;
                if (KlientNadrzednyId == this.Id)
                {
                    _klientNadrzedny = this;
                }
                else
                {
                    _klientNadrzedny = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(KlientNadrzednyId.Value);
                }
                return _klientNadrzedny;
            }
        }

        private IKlient _klientPodstawowy = null;

        /// <summary>
        /// IKlient pierowtny- IKlient nadrzędny który nie ma już ojca, lub jeśli IKlient nie ma ojca to ten sam klient
        /// </summary>
        public virtual IKlient KlientPodstawowy()
        {
            if (_klientPodstawowy != null)
            {
                return _klientPodstawowy;
            }

            IKlient wynik = this;
            while (wynik.KlientNadrzednyId != null)
            {
                wynik = wynik.KlientNadrzedny;
            }

            if (wynik.Id == this.Id)
            {
                _klientPodstawowy = this;
            }
            else
            {
                _klientPodstawowy = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(wynik.Id);
            }
            return _klientPodstawowy;
        }
        
        public new bool PelnaOferta
        {
            get
            {
                if (KlientNadrzedny != null)
                {
                    //jesli ojciec ma NIE pelna oferte, to dziecko tez 
                    if (KlientNadrzedny.PelnaOferta == false)
                    {
                        return false;
                    }
                }
                return base.PelnaOferta;
            }
            set { base.PelnaOferta = value; }
        }

        private HashSet<long> _mojKatalog = null;

        [Ignore]
        public HashSet<long> MojKatalog
        {
            get
            {
                if (!SolexBllCalosc.PobierzInstancje.Konfiguracja.GetLicense(Licencje.moj_katalog))
                {
                    return null;
                }
                //z tym cachowaniem moze byc klopot bo sie nie odswierzy szybko - ale uznamy ze obiekt klienta i tak sie cachuje co jakis czas
                if (_mojKatalog != null) return _mojKatalog;

                _mojKatalog = SolexBllCalosc.PobierzInstancje.ProduktyUkryteBll.Get(KlientPodstawowy(), KatalogKlientaTypy.MojKatalog) ?? new HashSet<long>();

                return _mojKatalog;
            }
        }

        [Ignore]
        [FriendlyName("Adresy klienta")]
        [WidoczneListaAdmin(false,false,true,false,true,new[]{typeof(Klient)})]
        [GrupaAtttribute("Adresy",1)]
        [WymuszonyTypEdytora(TypEdytora.ListaPodrzedna,typeof(Adres))]
        public IList<IAdres> Adresy => Calosc.AdresyDostep.PobierzAdresyKlienta(this);

        [Ignore]
        public IAdres DomyslnyAdres
        {
            get
            {
                var adr = Adresy.ToList();
                if (adr.Any())
                {
                    IAdres a = adr.FirstOrDefault(x => x.TypAdresu == TypAdresu.Domyslny);
                    if (a == null)
                        a = adr.FirstOrDefault(x => x.TypAdresu == TypAdresu.Glowny);
                    if (a == null)
                    {
                        a = adr.First();
                    }
                    return a;
                }
                return new Adres(null) as IAdres;
            }
        }
        public Klient()
        {         
                //nie chcemy odwolan do statycznych klas tu   
                //JezykId = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny;
        }

        public Klient(Model.Klient baza): base(baza)
        {
        }
   
        [Ignore]
        public virtual IKlient Opiekun
        {
            get
            {
                //niezalogowani nie mają mieć przedstawicieli ani opiekunów
                if (Id == 0)
                {
                    return null;
                }
                if (CzyAdministrator)
                {
                    return null;
                }
                long? opiekun = null;

                //bierzemy z klienta nadrzednego zawsze najpier
                if (KlientNadrzedny != null)
                {
                    if (KlientNadrzedny.OpiekunId != null)
                    {
                        opiekun = KlientNadrzedny.OpiekunId;
                    }
                    else
                    {
                        //ale jak nie ma nadrzednego opiekuna to bierzemy z obecnego
                        if (OpiekunId != null)
                        {
                            opiekun = OpiekunId;
                        }
                    }
                }
                else
                {
                    opiekun = OpiekunId;
                }

                if (opiekun.HasValue && opiekun!=Id)
                {
                    return SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(opiekun.Value);
                }
                try
                {
                    long id =  SolexBllCalosc.PobierzInstancje.Konfiguracja.IdDomyslnyOpiekun;
                    if (id != 0 && id!=Id)
                    {
                        return SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(id);
                    }
                    return null;
                }
                catch
                {
                    SolexBllCalosc.PobierzInstancje.Konfiguracja.IdDomyslnyOpiekun = 0;
                    // SolexBllCalosc.PobierzInstancje.Konfiguracja.Settings.SetSetting("domyslny_opiekun_id", "0", "int");
                    return null;
                }
            }
        }

        [Ignore]
        public virtual IKlient Przedstawiciel
        {
            get
            {
                //niezalogowani nie mają mieć przedstawicieli ani opiekunów
                if (Id == 0)
                {
                    return null;
                }
                //admini NIGDY nie moge miec przedstawicieli ani opiekunow - bo sie namiszaja oddzialy
                if (CzyAdministrator)
                {
                    return null;
                }
                long? opiekun = null;

                //bierzemy z klienta nadrzednego zawsze najpier
                if (KlientNadrzedny != null)
                {
                    if (KlientNadrzedny.PrzedstawicielId != null)
                    {
                        opiekun = KlientNadrzedny.PrzedstawicielId;
                    }
                    else
                    {
                        //ale jak nie ma nadrzednego opiekuna to bierzemy z obecnego
                        if (PrzedstawicielId != null)
                        {
                            opiekun = PrzedstawicielId;
                        }
                    }
                }
                else
                {
                    opiekun = PrzedstawicielId;
                }

                if (opiekun.HasValue && opiekun != Id)
                {
                    return SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(opiekun.Value);
                }
                try
                {
                    long id = SolexBllCalosc.PobierzInstancje.Konfiguracja.IdDomyslnyPrzedstawiciel;
                        
                    if (id != 0 && id != Id)
                    {
                        return SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(id);
                    }
                    return null;
                }
                catch
                {
                    SolexBllCalosc.PobierzInstancje.Konfiguracja.IdDomyslnyPrzedstawiciel = 0;
                    // SolexBllCalosc.PobierzInstancje.Konfiguracja.Settings.SetSetting("domyslny_przedstawiciel_id", "0", "int");
                    return null;
                }
            }
        }

        [Ignore]
        public virtual IKlient DrugiOpiekun
        {
            get
            {
                //niezalogowani nie mają mieć przedstawicieli ani opiekunów
                if (Id == 0)
                {
                    return null;
                }
                if (CzyAdministrator)
                {
                    return null;
                }
                long? opiekun = (KlientNadrzedny != null ? KlientNadrzedny.DrugiOpiekunId : DrugiOpiekunId);
                if (opiekun.HasValue && opiekun != Id)
                {
                    return SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(opiekun.Value);
                }
                try
                {

                    long id = SolexBllCalosc.PobierzInstancje.Konfiguracja.IdDrugiDomyslnyOpiekun;
                    if (id != 0 && id != Id)
                    {
                        return SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(id);
                    }
                    return null;
                }
                catch
                {
                    SolexBllCalosc.PobierzInstancje.Konfiguracja.IdDrugiDomyslnyOpiekun = 0;
                    // SolexBllCalosc.PobierzInstancje.Konfiguracja.Settings.SetSetting("domyslny_drugi_opiekun_id", "0", "int");
                    return null;
                }
            }
        }

        private long? _oddzialKlienta;

        [Ignore]
        public long OddzialDoJakiegoNalezyKlient
        {
            get
            {
                if (!Calosc.Konfiguracja.GetLicense(Licencje.Partnerzy))
                {
                    return 0;
                }

                if (!_oddzialKlienta.HasValue)
                {
                    //dla klientów nie zalogowaych ZAWSZE po domenie patrzymy i nie moze byc zadnego cache
                    //todo: porpawić
                    //if (Id == 0)
                    //{
                    //    return SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzOddzialIDWgDomeny();
                    //}

                    //czy IKlient jest sam sobie oddzialem
                    if (Role.Contains(RoleType.Oddzial))
                    {
                        _oddzialKlienta = Id;
                    }
                    else if (KlientNadrzednyId.HasValue && 
                        Calosc.DostepDane.PobierzPojedynczy<Klient>(KlientNadrzednyId.Value).Role.Contains(RoleType.Oddzial))
                    {
                        _oddzialKlienta = KlientNadrzednyId;
                    }
                    else if (Przedstawiciel != null && Przedstawiciel.Id != 0)
                    {
                        _oddzialKlienta = Przedstawiciel.OddzialDoJakiegoNalezyKlient;
                     
                    }
                    else if (Opiekun != null && Opiekun.Id != 0)
                    {
                        _oddzialKlienta = Opiekun.OddzialDoJakiegoNalezyKlient;

                    }
                    else if (DrugiOpiekun != null && DrugiOpiekun.Id != 0)
                    {
                        _oddzialKlienta = DrugiOpiekun.OddzialDoJakiegoNalezyKlient;

                    }
                    else
                    {
                        _oddzialKlienta = 0;
                    }
                }
                return _oddzialKlienta.Value;
            }
        }
        //automatyczne uzupelnienie w bindingu
        [Ignore]
        public List<Magazyn> DostepneMagazynyDlaKlienta { get; set; }
        
        [Ignore]
       
        [FriendlyName("Oddział")]
        public string OddzialDoJakiegoNalezyKlientNazwa
        {
            get
            {
                if (OddzialDoJakiegoNalezyKlient != 0)
                {
                    return SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(OddzialDoJakiegoNalezyKlient).Symbol;
                }
                return "";
            }
        }
        [FriendlyName("Dostęp do punktów")]
        [Ignore]
        [WidoczneListaAdmin(false, false, true, false, true, new[] { "SolEx.Hurt.Core.Klient,SolEx.Hurt.Core" })]
        public bool KlientWidziPunkty
        {
            get
            {
                if (WidziPunkty.HasValue)
                {
                    return WidziPunkty.Value;
                }
                return SolexBllCalosc.PobierzInstancje.Konfiguracja.DomyslnaWidocznoscPunktow;
            }
            set {   WidziPunkty = value; }
        }

        public List<IKlient> Subkonta()
        {
            var k = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz(null, x => x.KlientNadrzednyId == Id, new [] {new SortowanieKryteria<Klient>(x => x.Nazwa, KolejnoscSortowania.asc, "Nazwa") });
                return k.Select(x=>(IKlient)x).ToList();
        }

        public virtual List<IKlient> WszystkieKontaPodrzedne()
        {
            List<IKlient> wynik = new List<IKlient>();
            foreach (IKlient klient in Subkonta())
                {
                    wynik.Add(klient);
                    wynik.AddRange(klient.WszystkieKontaPodrzedne());
                }
                return wynik;
        }
        public HashSet<IKlient> WszystkieKontaPowiazane()
        {
            return new HashSet<IKlient>( KlientPodstawowy().WszystkieKontaPodrzedne() );
        }


        public bool CzyKlientPosiadaLimity()
        {
            return false;
        }
    }
    
}
