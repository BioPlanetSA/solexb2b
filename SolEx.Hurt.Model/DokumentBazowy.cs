using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class DokumentBazowy: IHasIntId
    {

        public DokumentBazowy() { }

        public DokumentBazowy(DokumentBazowy baza)
        {
            this.Id = baza.Id;
            this.DataUtworzenia = baza.DataUtworzenia;
            this.Uwagi = baza.Uwagi;
            this.WartoscNetto = baza.WartoscNetto;
            this.WartoscBrutto = baza.WartoscBrutto;
            this.walutaB2b = baza.walutaB2b;
            this.WalutaId = baza.WalutaId;
            this.NazwaPlatnosci = baza.NazwaPlatnosci;
            this.StatusId = baza.StatusId;
            this.KlientId = baza.KlientId;
            this.Klient = baza.Klient;
            this.ListyPrzewozowe = baza.ListyPrzewozowe;
        }

        [UpdateColumnKey]
        [ApiPropertyDescriptor("Id dokumentu - klucz")]
        [PrimaryKey]
        [FriendlyName("Id dokumentu - klucz")]
        [WidoczneListaAdmin(true, false, false, false)]
        public int Id { get; set; }


        [FriendlyName("Data utworzenia")]
        [WidoczneListaAdmin(true, true, false, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleDatoweZCzasem)]
        public virtual DateTime DataUtworzenia { get; set; }


        [FriendlyName("Uwagi")]
        [WidoczneListaAdmin(true, true, false, false)]
        public string Uwagi { get; set; }


        [FriendlyName("Wartość netto dokumentu")]
        [WidoczneListaAdmin(true, true, false, false)]
        public decimal WartoscNetto { get; set; }

        [FriendlyName("Wartość brutto dokumentu")]
        [WidoczneListaAdmin(true, true, false, false)]
        public decimal WartoscBrutto { get; set; }


        [FriendlyName("Waluta dokumentu")]
        [WidoczneListaAdmin(true, true, false, false)]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikWalut,SolEx.Hurt.Core")]
        [WymuszonyTypEdytora(TypEdytora.PoleZeSlownikiem)]
        public long? WalutaId { get; set; }

        [FriendlyName("Nazwa płatności")]
        public virtual string NazwaPlatnosci { get; set; }

        [FriendlyName("Status")]
        [WidoczneListaAdmin(true, true, false, false)]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikStatusowDokumentow,SolEx.Hurt.Core")]
        [WymuszonyTypEdytora(TypEdytora.PoleZeSlownikiem)]
        public virtual int? StatusId { get; set; }

        /// <summary>
        /// wartość dokumenty netto z walutą - tylko geter
        /// </summary>
        [Ignore]
        public virtual WartoscLiczbowa DokumentWartoscNetto
        {
            get { return new WartoscLiczbowa(WartoscNetto, walutaB2b); }
        }

        /// <summary>
        /// wartość dokumenty brutto z walutą - tylko geter
        /// </summary>
        [Ignore]
        public virtual WartoscLiczbowa DokumentWartoscBrutto
        {
            get { return new WartoscLiczbowa(WartoscBrutto, walutaB2b); }
        }

        /// <summary>
        /// Zwraca opis w formie HTMLowej - z dodanymi BR
        /// </summary>
        /// <returns></returns>
        [FriendlyName("Uwagi HTML")]
        public virtual string GetUwagiHTML()
        {
            if (!string.IsNullOrEmpty(Uwagi))
            {
                return Uwagi.ZamienZnakKoncaLiniNaWebowy();
            }
            return null;
        }

        /// <summary>
        /// Zwraca opis w formie winodowsowej. Bez tagów html, nowe linie windowsowe
        /// </summary>
        /// <returns></returns>
        public string GetUwagiText()
        {
            if (!string.IsNullOrEmpty(Uwagi))
            {
                return Uwagi.UsunFormatowanieHTML();
            }
            return null;
        }

        /// <summary>
        /// AUTOuzupelnienie przy pobieraniu DAL
        /// </summary>
        [Ignore]
        public virtual IKlient Klient { get; set; }

        /// <summary>
        /// AUTOuzupelnienie przy pobieraniu DAL
        /// </summary>
        [Ignore]
        public virtual string walutaB2b
        {
            get;
            set;
        }


        [Ignore]
        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Nazwa klienta")]
        public string NazwaKlienta
        {
            get
            {
                if (this.Klient != null)
                {
                    return Klient.Nazwa;
                }
                return null;
            }
        }

        [FriendlyName("Klient ID")]
        [WidoczneListaAdmin(true, true, false, false)]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKlientow,SolEx.Hurt.Core")]
        [WymuszonyTypEdytora(TypEdytora.PoleZeSlownikiem)]
        public long KlientId { get; set; }

        [FriendlyName("Oddział")]
        [WidoczneListaAdmin(true, false, false, false)]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKlientow,SolEx.Hurt.Core")]
        [WymuszonyTypEdytora(TypEdytora.PoleZeSlownikiem)]
        public long IdOddzialu { get; set; }

        [Ignore]
        [FriendlyName("Listy przewozowe")]
        [WidoczneListaAdmin(true, false, false, false,false)]
        public HashSet<HistoriaDokumentuListPrzewozowy> ListyPrzewozowe { get; set; }

    }
}
