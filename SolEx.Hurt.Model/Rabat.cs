using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Model
{
    public class Rabat : IHasLongId
    {
     
        public Rabat()
        {
            Aktywny = true;
            TypWartosci = RabatSposob.Procentowy;
            TypRabatu = RabatTyp.Prosty;
        }

        public Rabat(Rabat bazowy)
        {
            if (bazowy == null) return;
            Wartosc1 = bazowy.Wartosc1;
            Wartosc2 = bazowy.Wartosc2;
            Wartosc3 = bazowy.Wartosc3;
            TypWartosci = bazowy.TypWartosci;
            OdKiedy = bazowy.OdKiedy;
            DoKiedy = bazowy.DoKiedy;
            KlientId = bazowy.KlientId;
            KategoriaProduktowId = bazowy.KategoriaProduktowId;
            ProduktId = bazowy.ProduktId;
            DodanyPrzez = bazowy.DodanyPrzez;
            KategoriaKlientowId = bazowy.KategoriaKlientowId;
            WalutaId = bazowy.WalutaId;
            Aktywny = bazowy.Aktywny;
            TypRabatu = bazowy.TypRabatu;
            PoziomCenyId = bazowy.PoziomCenyId;
            CechaId = bazowy.CechaId;
            Wartosc4 = bazowy.Wartosc4;
            Wartosc5 = bazowy.Wartosc5;
        }

        public Rabat(long klientId, long produktId, decimal produktCenaStala) : this()
        {
            KlientId = klientId;
            ProduktId = produktId;
            Wartosc5 = Wartosc4 = Wartosc1 = Wartosc2 = Wartosc3 = produktCenaStala;
            TypRabatu = RabatTyp.Zaawansowany;
            TypWartosci = RabatSposob.StalaCena;
        }
        
        public Rabat(RabatTyp typ, long? klientId, int? kategoriaKlientow, long? produktId, long? kategoriaPRoduktow,int? cecha)
            : this()
        {
            TypRabatu = typ;
            KlientId = klientId;
            KategoriaKlientowId = kategoriaKlientow;
            ProduktId=produktId;
            KategoriaProduktowId = kategoriaPRoduktow;
            CechaId = cecha;
        }
     

        [FriendlyName("Wartość 1")]
        [WidoczneListaAdmin(true, true, false, false)]
        public decimal? Wartosc1 { get; set; }

        [FriendlyName("Wartość 2")]
        [WidoczneListaAdmin(true, false, false, false)]
        public decimal? Wartosc2 { get; set; }
        
        [FriendlyName("Wartość 3")]
        [WidoczneListaAdmin(true, false, false, false)]
        public decimal? Wartosc3 { get; set; }

        [FriendlyName("Wartość 4")]
        [WidoczneListaAdmin(true, false, false, false)]
        public decimal? Wartosc4 { get; set; }

        [FriendlyName("Wartość 5")]
        [WidoczneListaAdmin(true, false, false, false)]
        public decimal? Wartosc5 { get; set; }

        [FriendlyName("Sposób liczenia ")]
        [WidoczneListaAdmin(true, true, true, false)]
        public RabatSposob TypWartosci { get; set; }

        [FriendlyName("Od kiedy")]
        [WidoczneListaAdmin(true,true, false, false)]
        public DateTime? OdKiedy { get; set; }

        [FriendlyName("Do kiedy")]
        [WidoczneListaAdmin(true, true, false, false)]
        public DateTime? DoKiedy { get; set; }

        [FriendlyName("Klient id")]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKlientow,SolEx.Hurt.Core")]
        [WidoczneListaAdmin(true, false, true, false)]
        public long? KlientId { get; set; }

        [FriendlyName("Kategoria klientów id")]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKategoriiKlienta,SolEx.Hurt.Core")]
        [WidoczneListaAdmin(true, false, true, false)]
        public int? KategoriaKlientowId { get; set; }
        
        [FriendlyName("Kategoria produktów id")]
        [WidoczneListaAdmin(true, false, false, false)]
        public long? KategoriaProduktowId { get; set; }

        [FriendlyName("Produkt id")]
        [WidoczneListaAdmin(true, false, false, false)]
        public long? ProduktId { get; set; }

        [FriendlyName("Dodany przez kogo")]
        [WidoczneListaAdmin(true, false, false, false)]
        public string DodanyPrzez { get; set; }

        [FriendlyName("Waluta")]
        [WidoczneListaAdmin(true, false, false, false)]
        public long? WalutaId { get; set; }

        [FriendlyName("Aktywny")]
        [WidoczneListaAdmin(true, true, true, false)]
        public bool Aktywny { get; set; }

        [FriendlyName("Typ rabatu")]
        [WidoczneListaAdmin(true, true, true, false)]
        public RabatTyp TypRabatu { get; set; }

        [FriendlyName("Poziom cen")]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikPoziomuCen,SolEx.Hurt.Core")]
        [WidoczneListaAdmin(true, false, true, false)]
        public int? PoziomCenyId { get; set; }

        [FriendlyName("Cecha id")]
        [WidoczneListaAdmin(true, false, false, false)]
        public long? CechaId { get; set; }

        public long Id
        {
            get
            {
                //mnozna by tu dodać prywatne id cachowane, ale trzeba by gwarantowac ze nie zmienia sie aprametry - tylko getery
                return WyliczID(this.TypRabatu, this.KlientId, this.KategoriaKlientowId, this.ProduktId, this.KategoriaProduktowId, this.CechaId, this.WalutaId);
            }
        }

        public static long WyliczID(RabatTyp typ, long? klient, int? kategoriaKlienta, long? produktId, long? kategorieProduktow, long? cecha, long? waluta)
        {
            return (string.Concat(typ, "_", klient, "_", kategoriaKlienta, "_", produktId, "_", kategorieProduktow, "_", cecha, "_", waluta)).WygenerujIDObiektuSHAWersjaLong();
        }
    }
}