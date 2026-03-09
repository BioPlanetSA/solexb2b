using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Interfaces.Modele
{
    public interface IKlienci:IHasLongId
    {
        bool? WidziPunkty { get; set; }
        string DostepneModulyAdmina { get; set; }
        [PrimaryKey]
        [UpdateColumnKey]
        [FriendlyName("Id")]
        long Id { get; set; }
        [FriendlyName("Nazwa")]
        [ApiPropertyDescriptor("Nazwa klienta")]
        string Nazwa { get; set; }
        [FriendlyName("Symbol klienta")]
        string Symbol { get; set; }
        [FriendlyName("Nip")]
        string Nip { get; set; }
        [FriendlyName("Powód blokady")]
        BlokadaPowod PowodBlokady { get; set; }
        [FriendlyName("Telefon")]

        string Telefon { get; set; }
        [FriendlyName("email")]
        string Email { get; set; }
        [FriendlyName("Id opiekuna")]
        long? OpiekunId { get; set; }
        [FriendlyName("Id przedstawiciela")]
        long? PrzedstawicielId { get; set; }
        [ApiPropertyDescriptor("ID klienta nadrzêdnego")]
        long? KlientNadrzednyId { get; set; }
        bool KlientEu { get; set; }
        bool Eksport { get; set; }
        int JezykId { get; set; }
        string HasloKlienta { get; set; }
        string HasloZrodlowe { get; set; }
        bool Aktywny { get; set; }
        int? PoziomCenowyId { get; set; }
        decimal Rabat { get; set; }
        //bool? faktoring { get; set; }
        decimal LimitKredytu { get; set; }
        decimal IloscWykorzystanegoKredytu { get; set; }
        decimal IloscPozostalegoKredytu { get; set; }
        [FriendlyName("Dodatkowe pole tekstowe 1")]
        string PoleTekst1 { get; set; }
        [FriendlyName("Dodatkowe pole tekstowe 2")]
        string PoleTekst2 { get; set; }
        [FriendlyName("Dodatkowe pole tekstowe 3")]
        string PoleTekst3 { get; set; }
        [FriendlyName("Dodatkowe pole tekstowe 4")]
        string PoleTekst4 { get; set; }
        [FriendlyName("Dodatkowe pole tekstowe 5")]
        string PoleTekst5 { get; set; }
        string MagazynDomyslny { get; set; }
        [FriendlyName("Waluta")]



        long WalutaId { get; set; }
        [FriendlyName("blokada zamówieñ")]
        bool BlokadaZamowien { get; set; }
        [FriendlyName("data ostatniego logowania")]
        DateTime? DataOstatniegoLogowania { get; set; }
      //  int? AdresWysylkiId { get; set; }
        bool AdresWysylkiBlokada { get; set; }
        int FakturyElektroniczne { get; set; }
        [FriendlyName("Id drugiego opiekuna")]
        long? DrugiOpiekunId { get; set; }
        bool PelnaOferta { get; set; }
        int? SubkontoGrupaId { get; set; }
        [FriendlyName("gid który nale¿y dodawaæ do maila o zmianie has³a")]
        string Gid { get; set; }
        [Ignore]
        DateTime DataZmianyHasla { get; set; }
        HashSet<RoleType> Role { get; set; }
        string Skype { get; set; }
        string GaduGadu { get; set; }
        bool WidziWszystkich { get; set; }
        [FriendlyName("Opis klienta")]
        string Opis { get; set; }
        [FriendlyName("Zdjecie klienta")]
        int? ZdjecieId { get; set; }
        SubkontaRodzajAdministratora SubkontaAdministrator { get; set; }
        bool StaleUkrywanieCen { get; set; }
        string KluczSesji { get; set; }
        DateTime? DataZmianyKlucza { get; set; }
        decimal? IndywidualnaStawaVat { get; set; }
        DateTime? DataDodatnia { get; set; }
        [FriendlyName("Login do systemu")]
        string Login { get; set; }
        [FriendlyName("Czy klient widzi stany")]
        bool CzyWidziStany { get; set; }
        [FriendlyName("Osoba polecaj¹ca")]
        string OsobaPolecajaca { get; set; }

        /// <summary>
        /// Klient z id=0 jest NIEzalogowany zawsze, pozostali sa zalogowani
        /// </summary>
        [Ignore]
        AccesLevel Dostep { get; }

        [Ignore]
        bool CzyAdministrator { get; }
        //void przepiszRole();
        string GidIp { get; set; }
        [FriendlyName("Zgoda na newsletter")]
        bool ZgodaNaNewsletter { get; set; }
        int? CenaDetalicznaPoziomID { get; set; }
        bool AdministratorSubkont { get; set; }
        HashSet<string> DostepneMagazyny { get; set; }
        HashSet<long> Koncesja { get; set; }
        long? SzablonAkceptacjiPrzekrocznyLimitId { get; set; }
        string DozwoloneAdresyIp { get; set; }
        long? SzablonLimitowId { get; set; }
        long? SzablonAkceptacjiId { get; set; }
        long? MiejsceKosztowId { get; set; }
        decimal? MinimalnaWartoscZamowienia { get; set; }
        Waluta WalutaKlienta { get; set; }
        [Ignore]
        bool OfertaIndywidualizowana { get; set; }
    }
}