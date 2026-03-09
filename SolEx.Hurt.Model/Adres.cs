using System;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    using ServiceStack.DataAnnotations;

    /// <summary>
    /// Model adesu zamieszkania
    /// </summary>
    [FriendlyClassName("Adres")]
    public class Adres : IAdres, IModelSearcharable, IBindowalny, IHasLongId, IPolaIDentyfikujaceRecznieDodanyObiekt
    {
        public Adres(IAdres adres)
        {
            if (adres == null)
            {
                return;
            }
            Id = adres.Id;
            UlicaNr = adres.UlicaNr;
            KodPocztowy = adres.KodPocztowy;
            Miasto = adres.Miasto;
            Telefon = adres.Telefon;
            Email = adres.Email;
            KrajId = adres.KrajId;
            RegionId = adres.RegionId;
            Nazwa = adres.Nazwa;
            Lat = adres.Lat;
            Lon = adres.Lon;
            AutorId = adres.AutorId;
            DataDodania = adres.DataDodania;
            TypAdresu = adres.TypAdresu;
        }

        public Adres()
        {
            KodPocztowy = Miasto = Nazwa = UlicaNr = "";
            TypAdresu = TypAdresu.Brak;
        }

        public Adres(long id, string ulica, string kod, string miasto, int? kraj = null, int? region = null, string telefon = "", string nazwa = "", string email = "",bool jednorazowy=false,DateTime? dataDodania=null,int? autorId=null, TypAdresu typ = TypAdresu.Brak)
        {
            Id = id;
            UlicaNr = ulica;
            KodPocztowy = kod;
            Miasto = miasto;
            Telefon = telefon;
            Email = email;
            KrajId = kraj;
            RegionId = region;
            Nazwa = nazwa;
            DataDodania = dataDodania;
            AutorId = autorId;
            TypAdresu = typ;
        }


        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Miasto")]
        public string Miasto { get; set; }

        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKrajow,SolEx.Hurt.Core")]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Kraj")]
        public int? KrajId { get; set; }

       

        [PrimaryKey]

        public long Id { get; set; }

        //public bool Glowny { get; set; }
        //public bool Domyslny { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Nazwa adresu")]
        public string Nazwa { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Kod pocztowy")]
        public string KodPocztowy { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Ulica")]
        public string UlicaNr { get; set; }

        //[WidoczneListaAdmin(true, true, true, true)]
        //[FriendlyName("Jednorazowy")]
        //public bool Jednorazowy { get; set; }

        public string Telefon { get; set; }

        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikRegionow,SolEx.Hurt.Core")]
        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Region")]
        public int? RegionId { get; set; }


        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Typ adresu")]
        public TypAdresu TypAdresu { get; set; }

        public string Email { get; set; }

        public decimal Lat { get; set; }

        public decimal Lon { get; set; }
        
        public DateTime? DataDodania { get; set; }

        public long? AutorId { get; set; }

        [Ignore]
        [FriendlyName("Klient")]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKlientow,SolEx.Hurt.Core")]
        [WidoczneListaAdmin(false, false, true, false)]
        public long? KlientId { get; set; }

        public string Nip { get; set; }

        public bool RecznieDodany()
        {
            return Id <= 0;
        }

        [Ignore]
        public bool CzyPoprawneKoordynaty
        {
            get { return Lat != 0 && Lon != 0 && Lat != -1 && Lon != -1; }
        }

        [Ignore]
        public bool MoznaEdytowac { get; set; }

        [Ignore]
        public bool CzyUzyty { get; set; }

        [Ignore]
        [FriendlyName("Region")]
        public string Region { get; set; }

        [Ignore]
        [FriendlyName("Symbol Pañstwa")]
        public string KrajSymbol { get; set; }

        [Ignore]
        [FriendlyName("Pañstwo")]
        public string Kraj { get; set; }

        public bool TakiSamAdres(Adres s)
        {
            if (s.UlicaNr == UlicaNr && s.Miasto == Miasto && s.KodPocztowy == KodPocztowy && s.KrajId == KrajId)
            {
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            return ((string.IsNullOrEmpty(Nazwa) ? "" : Nazwa + ", ")
                + (string.IsNullOrEmpty(UlicaNr) ? "" : UlicaNr + ", ")
                + (string.IsNullOrEmpty(Miasto) ? "" : (KodPocztowy + " " + Miasto + ", "))
               + (string.IsNullOrEmpty(Kraj) ? "" : Kraj + ", ")
                + (string.IsNullOrEmpty(Telefon) ? "" : Telefon + ", ")
                 + (string.IsNullOrEmpty(Email) ? "" : Email + ", ")).Trim(new[] { ' ', ',' });
        }
    }
}

