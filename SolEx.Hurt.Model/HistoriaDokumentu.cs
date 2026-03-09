using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model
{
    using System;
    using Core;
    using ServiceStack.DataAnnotations;

    /// <summary>
    /// Model dokumentu (zamowienia, faktury, itd.)
    /// </summary>

    [ApiTypeDescriptor(ApiGrupy.Dokumenty, "Model dokumentu")]
    public class HistoriaDokumentu : DokumentBazowy, IDocumentApiTypeVisible
    {

        /// <summary>
        /// wartoœæ VAT - liczone ksiêgowo, nie brutto - netto. Czyli robimy slownik stawek Vat i dla azdej stawki przeliczanie z zaokragleniem do 2 miejsc +  na koniec suma. Zobacz w tescie dokumentu jak liczone
        /// </summary>
        public decimal WartoscVat { get; set; }

        /// <summary>
        /// wartoœæ dokumentu VAT z walut¹ - tylko geter
        /// </summary>
        [Ignore]
        public virtual WartoscLiczbowa DokumentWartoscVat
        {
            get { return new WartoscLiczbowa(WartoscVat, walutaB2b); }
        }

        public HistoriaDokumentu()
        {
            
        }

        [FriendlyName("P³atnik ID")]
        [Ignore]
        public long DokumentPlatnikId
        {
            get
            {
                return KlientId;
            }
        }
        

        public RodzajDokumentu Rodzaj { get; set; }
        
        [FriendlyName("Nazwa dokumentu")]
        [WidoczneListaAdmin(true, true, false, false)]
        public virtual string NazwaDokumentu { get; set; }

               
        [FriendlyName("Zap³acony")]
        [WidoczneListaAdmin(true, true, false, false)]
        public bool Zaplacono { get; set; }
        
        //[FriendlyName("Id powi¹zanego zamówienia B2B")]
        //public int? ZamowienieB2BPowiazane { get; set; }
        
      
        [FriendlyName("Wartoœæ nale¿na brutto dokumentu")]
        [WidoczneListaAdmin(true, true, false, false)]
        public decimal WartoscNalezna { get; set; }
        
        [FriendlyName("Termin p³atnoœci")]
        [WymuszonyTypEdytora(TypEdytora.PoleDatowe)]
        [WidoczneListaAdmin(true, true, false, false)]
        public virtual DateTime? TerminPlatnosci { get; set; }
        

        [FriendlyName("Data dodania")]
        [WidoczneListaAdmin(true, true, false, false)]
        public DateTime DataDodania { get; set; }

        [FriendlyName("Odbiorca")]
        [WidoczneListaAdmin(true, true, false, false)]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKlientow,SolEx.Hurt.Core")]
        [WymuszonyTypEdytora(TypEdytora.PoleZeSlownikiem)]
        public long? OdbiorcaId { get; set; }

        public bool Rezerwacja { get; set; }

        [FriendlyName("Numer obcy")]
        [WidoczneListaAdmin(true, false, false, false)]
        public string NumerObcy { get; set; }

        

        [FriendlyName("Data wys³ania")]
        [WidoczneListaAdmin(true, false, false, false)]
        public DateTime? DataWyslaniaDokumentu { get; set; }


        [Ignore]
        public bool CenyLiczoneOdBrutto { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="customerId"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="createDate">DataUtworzenia</param>
        /// <param name="isPaid">Zaplacono</param>
        /// <param name="relatedDocId">DokumentPowiazanyId</param>
        /// <param name="relatedDoc"></param>
        /// <param name="opis"></param>
        /// <param name="wartoscNalezna">WartoscNalezna</param>
        /// <param name="wartoscNetto">WartoscNetto</param>
        /// <param name="wartoscBrutto">WartoscBrutto</param>
        /// <param name="walutaId">WalutaId</param>
        /// <param name="nazwaPlatnosci">NazwaPlatnosci</param>
        /// <param name="dataPlatnosci">TerminPlatnosci</param>
        /// <param name="booked">Rezerwacja</param>
        public HistoriaDokumentu(int id, long customerId, RodzajDokumentu type, string name, DateTime createDate
        , bool isPaid, int? relatedDocId, string relatedDoc, string opis, decimal wartoscNalezna, decimal wartoscNetto
        , decimal wartoscBrutto, long? walutaId, string nazwaPlatnosci, DateTime? dataPlatnosci, bool booked):this()
        {
            this.Id = id;
            KlientId = customerId;
            Rodzaj = type;
            NazwaDokumentu = name;
            DataUtworzenia = createDate;
            Zaplacono = isPaid;
            Uwagi = opis;
            WartoscNalezna = wartoscNalezna;
            WartoscNetto = wartoscNetto;
            WartoscBrutto = wartoscBrutto;
            WalutaId = walutaId;
            NazwaPlatnosci = nazwaPlatnosci;
            TerminPlatnosci = dataPlatnosci;
            Rezerwacja = booked;
            DokumentPowiazanyId = relatedDocId;
        }
        
        public HistoriaDokumentu(Zamowienie z):base(z)
        {
            //tylko pola z obecngo obiekty - reszta idzie z bazowego
            this.NazwaDokumentu = z.NumerTymczasowyZamowienia;
            this.OdbiorcaId = z.KlientId;
            this.Rodzaj = RodzajDokumentu.Zamowienie;
        }
        public int? DokumentPowiazanyId { get; set; }
        public HistoriaDokumentu(HistoriaDokumentu baza):base(baza)
        {
            if (baza == null)
            {
                return;
                
            }
            //tylko pola z obecngo obiekty - reszta idzie z bazowego
            Rodzaj = baza.Rodzaj;
            NazwaDokumentu = baza.NazwaDokumentu;
            Zaplacono = baza.Zaplacono;
            WartoscNalezna = baza.WartoscNalezna;
            TerminPlatnosci = baza.TerminPlatnosci;
            DataDodania = baza.DataDodania;
            OdbiorcaId = baza.OdbiorcaId;
            Rezerwacja = baza.Rezerwacja;
            NumerObcy = baza.NumerObcy;
            IdOddzialu = baza.IdOddzialu;
            DataWyslaniaDokumentu = baza.DataWyslaniaDokumentu;
            DokumentPowiazanyId = baza.DokumentPowiazanyId;
        }
    }
}

