using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common.Extensions;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL
{
    [Alias("HistoriaDokumentu")]
    public class DokumentyBll : HistoriaDokumentu, IDokument
    {
        public DokumentyBll() { }

        public DokumentyBll(HistoriaDokumentu baza)
            : base(baza)
            {
            }

        public DokumentyBll(ZamowieniaBLL z) : base(z)
        {
            this.PowiazaneZamowienieB2B = z;
         
            //nazwa ustawiana z nazw erpowych jesli sa podane
            if (z.dokumentyERPStworzoneZZamowienia != null)
            {
                this.NazwaDokumentu = "";   //kasujemy obecny wpis - obecnie pewnie jest wpisany numer tymczasowy
                z.dokumentyERPStworzoneZZamowienia.ForEach(x => this.NazwaDokumentu += x.Value + " ");
               this.NazwaDokumentu = this.NazwaDokumentu.Trim();
            }
        }

        /// <summary>
        /// uzupelnieniane przed pobraniem tylko do widokow
        /// </summary>
        [Ignore]
        public Zamowienie PowiazaneZamowienieB2B { get; private set; }


        public void UstawPowiazaneZamowienieB2B(Zamowienie zam)
        {
            this.PowiazaneZamowienieB2B = zam;
        }



        [Ignore]
        public virtual WartoscLiczbowa DokumentWartoscNalezna
        {
            get
            {
                //BARTEK usuwa warunek faktoringu - czyli jesli jest faktoring to nadal mozemy pokazac  ile ktos wisi kasy. Tak jest lepiej - niech klienci rozliczaja poprawnie faktoring u siebie w systemach || (StatusId!=null && PobierzStatus( SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski).TraktujJakoFaktoring) 
                if (Zaplacono || Rodzaj == RodzajDokumentu.Zamowienie)
                //|| (StatusId != null && PobierzStatus( SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski).TraktujJakoFaktoring)) - pokazujemy wartosc nalezna dla dokumentu faktoringowego. (Powiedziane o tym przy wykonywaniu tasku #8272)
                {
                    return null;
                }
                //if (wartosc_netto < 0)
                //{
                //    return new WartoscLiczbowa(-wartosc_nalezna, waluta);
                //}
                if (StatusId != null && SolexBllCalosc.PobierzInstancje.Konfiguracja.StatusyZamowien[StatusId.Value].TraktujJakoFaktoring)
                {
                    return DokumentWartoscBrutto;
                }
                return new WartoscLiczbowa(WartoscNalezna, walutaB2b);

            }
        }
        
        [Ignore]
        public long DokumentOdbiorcaId
        {
            get
            {
                //jak jest zamowienie i bylo z b2b to pokazujemy pierwtona osobe ktora zlozyla zamowienie - to dla subkont po stronie b2b robionych jest potrzebne
                if (base.Rodzaj == RodzajDokumentu.Zamowienie && this.PowiazaneZamowienieB2B != null)
                {
                    return this.PowiazaneZamowienieB2B.KlientId;
                }
                if (OdbiorcaId.HasValue)
                {
                    return OdbiorcaId.Value;
                }
                return KlientId;
            }
        }

        [Ignore]
        public IKlient DokumentOdbiorca
        {
            get { return SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(DokumentOdbiorcaId); }
        }

      

        [Ignore]
        public IKlient DokumentPlatnik
        {
            get { return this.Klient; }
        }

        [Ignore]
        public DateTime? OfertaDoKiedyWazna
        {
            get { return this.TerminPlatnosci; }
        }

        public virtual int? DniDoTerminuZaplaty()
        {
            return TerminPlatnosci.HasValue ? (TerminPlatnosci.Value - DateTime.Now).Days : 0;
        }

        [Ignore]
        public virtual DateTime? DokumentTerminRealizacji
        {
            get { return base.TerminPlatnosci; }
        }

        [Ignore]
        public virtual int? DokumentDniSpoznienia
        {
            get
            {
                if (!Zaplacono && base.TerminPlatnosci.HasValue && base.TerminPlatnosci.Value < DateTime.Now.Date)
                {
                    return (int) (DateTime.Now.Date - base.TerminPlatnosci.Value).TotalDays;
                }
                return 0;   //musi byc 0 bo to jest  faktura, np. dla powiadomien
            }
        }

        [Ignore]
        public bool DokumentZrealizowany
        {
            get { return base.Zaplacono;  }
        }

        public IEnumerable<DokumentyPozycje> PobierzPozycjeDokumentu()
        {
            if (this.Id > 0)
            {
                return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<DokumentyPozycje>(this.Klient, x => x.DokumentId == this.Id, this).ToList();
            }
            //jesli ID jest mniejsze niz 0 to znaczy ze mamy zamowienie B2B ktore bylo rztowane na dokument historyczny
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZamowieniaProduktyBLL>(this.Klient, x => x.DokumentId == Id, this).Select(x => new DokumentyPozycje(x));

        }

        public virtual bool CzyPrzeterminowany()
        {
            return TerminPlatnosci.HasValue && TerminPlatnosci.Value < DateTime.Now;
        }

        

    }
}
