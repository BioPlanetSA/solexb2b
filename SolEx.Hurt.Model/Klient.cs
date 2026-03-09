using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Modele;

namespace SolEx.Hurt.Model
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Model kontrahenta
    /// </summary>

    [FriendlyClassName("Klient")]
    [ApiTypeDescriptor(ApiGrupy.Klienci, "Model klienta")]
    public class Klient : Core.IDocumentApiTypeVisible, IKlienci, IBindowalny, IPolaIDentyfikujaceRecznieDodanyObiekt
    {
        public Klient()
        {
            Aktywny = true;
            //domyslnie maja pelna oferte
            PelnaOferta = true;
            SubkontaAdministrator = SubkontaRodzajAdministratora.Brak;
            CzyWidziStany = true;
            PowodBlokady = BlokadaPowod.Brak;
            PrzedstawicieleOpiekunowie = new Dictionary<OpiekunowiePrzedstawiciele, string>(3);
            Role = new HashSet<RoleType>(){RoleType.Klient};
            this.DataZmianyHasla = SqlDateTime.MinValue.Value;   //musi byc min time SQLowy bo C3 jest zbyt dlugi zeby zapisac do bazy
        }
        public Klient(long id)
           : this()
        {
            Id = id;
        }

        public Klient(Klient c): this()
        {
            if (c != null)
            {
                Id = c.Id;
                Nazwa = c.Nazwa;
                Symbol = c.Symbol;
                Nip = c.Nip;
                Telefon = c.Telefon;
                Email = c.Email;
                OpiekunId = c.OpiekunId;
                PrzedstawicielId = c.PrzedstawicielId;
                KlientNadrzednyId = c.KlientNadrzednyId;
                KlientEu = c.KlientEu;
                Eksport = c.Eksport;
                JezykId = c.JezykId;
                HasloKlienta = c.HasloKlienta;
                HasloZrodlowe = c.HasloZrodlowe;
                Aktywny = c.Aktywny;
                PoziomCenowyId = c.PoziomCenowyId;
                Rabat = c.Rabat;
                LimitKredytu = c.LimitKredytu;
                IloscWykorzystanegoKredytu = c.IloscWykorzystanegoKredytu;
                PoleTekst1 = c.PoleTekst1;
                PoleTekst2 = c.PoleTekst2;
                PoleTekst3 = c.PoleTekst3;
                MagazynDomyslny = c.MagazynDomyslny;
                WalutaId = c.WalutaId;
                BlokadaZamowien = c.BlokadaZamowien;
                DataOstatniegoLogowania = c.DataOstatniegoLogowania;
                AdresWysylkiBlokada = c.AdresWysylkiBlokada;
                FakturyElektroniczne = c.FakturyElektroniczne;
                PoleTekst4 = c.PoleTekst4;
                PoleTekst5 = c.PoleTekst5;
                DrugiOpiekunId = c.DrugiOpiekunId;
                PelnaOferta = c.PelnaOferta;
                SubkontoGrupaId = c.SubkontoGrupaId;
                Gid = c.Gid;
                MinimalnaWartoscZamowienia = c.MinimalnaWartoscZamowienia;
                DataZmianyHasla = c.DataZmianyHasla;
                Role = c.Role;
                Skype = c.Skype;
                GaduGadu = c.GaduGadu;
                WidziWszystkich = c.WidziWszystkich;
                Opis = c.Opis;
                ZdjecieId = c.ZdjecieId;
                SubkontaAdministrator = c.SubkontaAdministrator;
                StaleUkrywanieCen = c.StaleUkrywanieCen;
                KluczSesji = c.KluczSesji;
                DataZmianyKlucza = c.DataZmianyKlucza;
                WidziPunkty = c.WidziPunkty;
                IndywidualnaStawaVat = c.IndywidualnaStawaVat;
                DataDodatnia = c.DataDodatnia;
                Login = c.Login;
                CzyWidziStany = c.CzyWidziStany;
                OsobaPolecajaca = c.OsobaPolecajaca;
                PowodBlokady = c.PowodBlokady;
                GidIp = c.GidIp;
                ZgodaNaNewsletter = c.ZgodaNaNewsletter;
                DostepneModulyAdmina = c.DostepneModulyAdmina;
                CenaDetalicznaPoziomID = c.CenaDetalicznaPoziomID;
                SzablonLimitowId = c.SzablonLimitowId;
                SzablonAkceptacjiId = c.SzablonAkceptacjiId;
                MiejsceKosztowId = c.MiejsceKosztowId;
                SzablonAkceptacjiPrzekrocznyLimitId = c.SzablonAkceptacjiPrzekrocznyLimitId;
                AdministratorSubkont = c.AdministratorSubkont;
                HasloOdkryte = c.HasloOdkryte;
                IloscPozostalegoKredytu = c.IloscPozostalegoKredytu;
                DostepneMagazyny = c.DostepneMagazyny;
                Koncesja = c.Koncesja;
                IdUlubionych = c.IdUlubionych;
                IdInfoODostepnosci = c.IdInfoODostepnosci;
                this.WalutaKlienta = c.WalutaKlienta;
            }
        }

        [PrimaryKey]
        [UpdateColumnKey]
        [FriendlyName("Id")]
        [WidoczneListaAdmin(true, true, false, false)]
        public virtual long Id { get; set; }

        [WidoczneListaAdmin(false, false, true, false)]
        [FriendlyName("Hasło klient")]
        [Niewymagane]
        [WymuszonyTypEdytora(TypEdytora.PoleHaslo)]
        [Ignore]
        public string HasloOdkryte { get; set; }

        [FriendlyName("Nazwa")]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [WidoczneListaAdmin(true, true, true, true)]
        [ApiPropertyDescriptor("Nazwa klienta")]

        public virtual string Nazwa { get; set; }

        [GrupaAtttribute("Dane podstawowe", 0)]
        [FriendlyName("Symbol")]
        [WidoczneListaAdmin(true, true, true, true)]

        public virtual string Symbol { get; set; }

        [Niewymagane]
        [FriendlyName("Nip")]
        [WidoczneListaAdmin(true, false, true, true)]
        public string Nip { get; set; }

        [FriendlyName("Telefon")]
        [GrupaAtttribute("Dane kontaktowe", 1)]
        [WidoczneListaAdmin(true, false, true, true)]
        [Niewymagane]
        public string Telefon { get; set; }
        public long? SzablonLimitowId { get; set; }
        public long? SzablonAkceptacjiId { get; set; }
        public long? MiejsceKosztowId { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [GrupaAtttribute("Dane kontaktowe", 1)]
        [FriendlyName("Email")]
        [WymuszonyTypEdytora(TypEdytora.PoleAdresEmail)]
        [Niewymagane]
        [WalidatorDanych("SolEx.Hurt.Core.BLL.WalidatoryDanych.Klienci.WalidatorMaila,SolEx.Hurt.Core")]
        public string Email { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(false, false, true, false, true, new[] { "SolEx.Hurt.Core.Klient,SolEx.Hurt.Core" })]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikPracownikow,SolEx.Hurt.Core")]
        [FriendlyName("Opiekun")]
        public long? OpiekunId { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(false, false, true, false, true, new[] { "SolEx.Hurt.Core.Klient,SolEx.Hurt.Core" })]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikPracownikow,SolEx.Hurt.Core")]
        [FriendlyName("Przedstawiciel")]
        public long? PrzedstawicielId { get; set; }
        
        [Niewymagane]
        [WidoczneListaAdmin(true, false, true, false, true, new[] { "SolEx.Hurt.Core.Klient,SolEx.Hurt.Core" })]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKlientow,SolEx.Hurt.Core")]
        [FriendlyName("Klient nadrzędny")]
        public long? KlientNadrzednyId { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [GrupaAtttribute("Warunki sprzedaży", 2)]
        public bool KlientEu { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [GrupaAtttribute("Warunki sprzedaży", 2)]
        public bool Eksport { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [GrupaAtttribute("Dane podstawowe", 1)]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikJezykow,SolEx.Hurt.Core")]
        [FriendlyName("Język w jakim klient widzi platformę")]
        public int JezykId { get; set; }

        public string HasloKlienta { get; set; }
        public string HasloZrodlowe { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [GrupaAtttribute("Dane podstawowe", 1)]
        [FriendlyName("Aktywny")]
        public bool Aktywny { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [GrupaAtttribute("Warunki sprzedaży", 2)]
        [Niewymagane]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikPoziomuCen,SolEx.Hurt.Core")]
        [FriendlyName("Poziom ceny przy sprzedaży")]

        public int? PoziomCenowyId { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [GrupaAtttribute("Warunki sprzedaży", 2)]
        [FriendlyName("Podstawowy rabat klienta")]
        public decimal Rabat { get; set; }


        [WidoczneListaAdmin(true, false, false, false)]
        public decimal LimitKredytu { get; set; }

        [WidoczneListaAdmin(true, false, false, false)]
        public decimal IloscWykorzystanegoKredytu { get; set; }

        [WidoczneListaAdmin(true, false, false, false)]
        public decimal IloscPozostalegoKredytu { get; set; }

        [Niewymagane]
        [FriendlyName("Dodatkowe pole tekstowe 1")]
        [GrupaAtttribute("Dodatkowe pola", 3)]
        [MaksymalnaLiczbaZnakow(500)]
        [WidoczneListaAdmin(true, false, true, true)]
        public string PoleTekst1 { get; set; }

        [Niewymagane]
        [FriendlyName("Dodatkowe pole tekstowe 2")]
        [GrupaAtttribute("Dodatkowe pola", 3)]
        [MaksymalnaLiczbaZnakow(100)]
        [WidoczneListaAdmin(true, false, true, true)]
        public string PoleTekst2 { get; set; }

        [Niewymagane]
        [FriendlyName("Dodatkowe pole tekstowe 3")]
        [GrupaAtttribute("Dodatkowe pola", 3)]
        [MaksymalnaLiczbaZnakow(100)]
        [WidoczneListaAdmin(true, false, true, true)]
        public string PoleTekst3 { get; set; }

        [Niewymagane]
        [FriendlyName("Dodatkowe pole tekstowe 4")]
        [GrupaAtttribute("Dodatkowe pola", 3)]
        [MaksymalnaLiczbaZnakow(100)]
        [WidoczneListaAdmin(true, false, true, true)]
        public string PoleTekst4 { get; set; }

        [Niewymagane]
        [FriendlyName("Dodatkowe pole tekstowe 5")]
        [GrupaAtttribute("Dodatkowe pola", 3)]
        [WidoczneListaAdmin(true, false, true, true)]
        public string PoleTekst5 { get; set; }

     
        public virtual string MagazynDomyslny { get; set; }


        [WidoczneListaAdmin(true, true, true, true, true)]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikWalut,SolEx.Hurt.Core")]
        [GrupaAtttribute("Warunki sprzedaży", 2)]
        [FriendlyName("Waluta")]
        public long WalutaId { get; set; }

        //pole automatycznie uzupelniane w bindingu
        [Ignore]
        public Waluta WalutaKlienta { get; set; }


        [FriendlyName("blokada logowania")]
        public bool BlokadaZamowien { get; set; }

        [WidoczneListaAdmin(true, true, false, false, true, new[] { "SolEx.Hurt.Core.Klient,SolEx.Hurt.Core" })]
        [FriendlyName("data ostatniego logowania")]
        public DateTime? DataOstatniegoLogowania { get; set; }
        public bool AdresWysylkiBlokada { get; set; }
       // public string AlternatywnyEmail { get; set; }
        public int FakturyElektroniczne { get; set; }


        [Niewymagane]
        [WidoczneListaAdmin(false, false, true, false, true, new[] { "SolEx.Hurt.Core.Klient,SolEx.Hurt.Core" })]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikPracownikow,SolEx.Hurt.Core")]
        [FriendlyName("Drugi opiekun")]
        public long? DrugiOpiekunId { get; set; }

        public bool PelnaOferta { get; set; }
        public int? SubkontoGrupaId { get; set; }

        [FriendlyName("gid który należy dodawać do maila o zmianie hasła")]
        public string Gid { get; set; }

        public decimal? MinimalnaWartoscZamowienia { get; set; }

        public DateTime DataZmianyHasla { get; set; }

        [WidoczneListaAdmin(true, false, true, false, true, new[] { "SolEx.Hurt.Core.ModelBLL.Pracownik,SolEx.Hurt.Core" })]
        [FriendlyName("Role które posiada pracownik")]
        public virtual HashSet<RoleType> Role { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, false, true, true)]
        [GrupaAtttribute("Dane kontaktowe", 1)]
        public string Skype { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, false, true, true)]
        [GrupaAtttribute("Dane kontaktowe", 1)]
        public string GaduGadu { get; set; }

        [WidoczneListaAdmin(true, true, true, false, true, new[] { "SolEx.Hurt.Core.ModelBLL.Pracownik,SolEx.Hurt.Core" })]
        [FriendlyName("Czy pracownik ma widzieć wszystkich klientów")]
        public bool WidziWszystkich { get; set; }


        [FriendlyName("Opis klienta")]
        public string Opis { get; set; }

        [FriendlyName("Zdjecie klienta")]
        [WidoczneListaAdmin(false, false, true, false, true, new[] { "SolEx.Hurt.Core.ModelBLL.Pracownik,SolEx.Hurt.Core" })]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ZdjecieId { get; set; }

        public SubkontaRodzajAdministratora SubkontaAdministrator { get; set; }

        public bool StaleUkrywanieCen { get; set; }

        [FriendlyName("Klucz sesji")]
        [WidoczneListaAdmin(false, false, true,false)]
        [Niewymagane]
        public string KluczSesji { get; set; }

        public DateTime? DataZmianyKlucza { get; set; }

        public bool? WidziPunkty { get; set; }

        public decimal? IndywidualnaStawaVat { get; set; }


        [WidoczneListaAdmin(true, false, false, false)]
        [FriendlyName("Data dodania")]
        [WymuszonyTypEdytora(TypEdytora.PoleDatowe)]
        [DisplayFormat(DataFormatString = "{dd.mm.yyyy}")]
        public DateTime? DataDodatnia { get; set; }

        [FriendlyName("Login do systemu")]
        public string Login { get; set; }

        [FriendlyName("Czy klient widzi stany")]
        [WidoczneListaAdmin(false, false, true, false, true, new[] { "SolEx.Hurt.Core.Klient,SolEx.Hurt.Core" })]
        public bool CzyWidziStany { get; set; }

        [FriendlyName("Osoba polecająca")]
        public string OsobaPolecajaca { get; set; }

        [FriendlyName("Powód blokady")]
        [WidoczneListaAdmin(false, false, true, false, true, new[] { "SolEx.Hurt.Core.Klient,SolEx.Hurt.Core" })]
        public BlokadaPowod PowodBlokady { get; set; }

        public string GidIp { get; set; }

        public int? CenaDetalicznaPoziomID { get; set; }
        
        [FriendlyName("Zgoda na newsletter")]
        [WidoczneListaAdmin(false, false, true, false, true, new[] { "SolEx.Hurt.Core.Klient,SolEx.Hurt.Core" })]
        public bool ZgodaNaNewsletter { get; set; }

        [FriendlyName("Moduly dostepne w adminie")]
        public string DostepneModulyAdmina { get; set; }

        public string DozwoloneAdresyIp { get; set; }
        public long? SzablonAkceptacjiPrzekrocznyLimitId { get; set; }
        public bool AdministratorSubkont { get; set; }
        /// <summary>
        /// Klient z id=0 jest NIEzalogowany zawsze, pozostali sa zalogowani
        /// </summary>
        [Ignore]
        public AccesLevel Dostep
        {
            get { return Id == 0 ? AccesLevel.Niezalogowani : AccesLevel.Zalogowani; }
        }

        [Ignore]
        public bool CzyAdministrator
        {
            get { return Role.Contains(RoleType.Administrator); }
        }

        private List<string> _elementyhMenu = null;
        public object Value;

        [WidoczneListaAdmin(false, false, true, false, true, new[] { "SolEx.Hurt.Core.ModelBLL.Pracownik,SolEx.Hurt.Core" })]
        [Ignore]
        [FriendlyName("Elementy menu widoczne dla pracownika")]
        [PobieranieSlownika("SolEx.Hurt.Web.Site2.Helper.SlownikElementowMenu,SolEx.Hurt.Web.Site2")]
        [Niewymagane]
        public List<string> JakieElementyMenu
        {
            get
            {
                if (!string.IsNullOrEmpty(DostepneModulyAdmina) && _elementyhMenu == null)
                {
                    _elementyhMenu = new List<string>();
                    foreach (var el in DostepneModulyAdmina.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        _elementyhMenu.Add(el);
                    }
                }
                return _elementyhMenu;
            }
            set { _elementyhMenu = value; }
        }

        

        [WidoczneListaAdmin(false, false, true, false)]
        [FriendlyName("Dostępne magazyny dla klienta")]
        [Niewymagane]
        public HashSet<string> DostepneMagazyny { get; set; }

        [WidoczneListaAdmin(true, false, false, false)]
        [FriendlyName("Przyznane koncesje")]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikCech,SolEx.Hurt.Core")]
        [Niewymagane]
        public HashSet<long> Koncesja { get; set; }

        [Ignore]
        public Dictionary<OpiekunowiePrzedstawiciele, string> PrzedstawicieleOpiekunowie { get; set; }

        public bool RecznieDodany()
        {
            return Id <= 0;
        }

        public HashSet<long> IdUlubionych { get; set; }

        public HashSet<long> IdInfoODostepnosci { get; set; }

        [Ignore]
        public bool OfertaIndywidualizowana { get; set; }
    }
}

