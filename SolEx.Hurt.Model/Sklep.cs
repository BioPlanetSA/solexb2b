using System;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model sklepu
    /// </summary>
    public class Sklep : ISklep, IPolaIDentyfikujaceRecznieDodanyObiekt
    {
        [PrimaryKey]
        [UpdateColumnKey]
        
        [FriendlyName("Id sklepu")]
        [WidoczneListaAdmin(true, true, false, false)]
        public long Id { get; set; }
       
        [FriendlyName("Nazwa")]
        [WidoczneListaAdmin(true, true, true, false)]
        [GrupaAtttribute("Szczegóły", 0)]
        [Wymagane]
        public string Nazwa { get; set; }

        //adres na bazie ktorego powstał sklep
        [FriendlyName("Adres na bazie którego powstał sklep")]
        [WidoczneListaAdmin(true, false, false, false)]
        [GrupaAtttribute("Szczegóły", 0)]
        [Niewymagane]
        public long? AdresId { get; set; }

        [GrupaAtttribute("Szczegóły", 0)]
        [WidoczneListaAdmin(true, true, true, false)]
        public bool Aktywny { get; set; }
       
        [FriendlyName("Data dodania")]
        [GrupaAtttribute("Szczegóły", 0)]
        [WidoczneListaAdmin(true, false, false, false)]
        public DateTime DataUtworzenia { get; set; }
       
        [FriendlyName("Link URL", FriendlyOpis = "Link do strony www sklepu.")]
        [GrupaAtttribute("Szczegóły", 0)]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        public AdresUrl LinkUrl { get; set; }
       
       
        public long? AutorId { get; set; }

        [GrupaAtttribute("Szczegóły", 0)]
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Obrazek")]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ObrazekId { get; set; }

     
        [FriendlyName("Opis")]
        [GrupaAtttribute("Szczegóły", 0)]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        public string Opis { get; set; }
        
        [FriendlyName("Koordynaty generowane automatycznie")]
        [WidoczneListaAdmin(true, false, true, false)]
        public bool AutomatyczneKoordynaty { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        public bool Siedziba { get; set; }

        //[Ignore]
        //[FriendlyName("Poprawne koordynaty")]
        //[WidoczneListaAdmin(true, true, false, false)]
        //public bool CzyPoprawneKoordynaty { get { return lat != 0 && lon != 0 && lat != -1 && lon != -1; } }

        [WidoczneListaAdmin(true, false, false, false)]
        public bool KoordynatyZERP { get; set; }
        
        public Sklep()
        {
            AutomatyczneKoordynaty = true;
        }
        public Sklep(ISklep bazowy)
        {
           //this.KopiujPola(bazowy);
            Id = bazowy.Id;
            Nazwa=bazowy.Nazwa;
            Aktywny=bazowy.Aktywny;
            DataUtworzenia=bazowy.DataUtworzenia;
            LinkUrl=bazowy.LinkUrl;
            AutorId=bazowy.AutorId;
            AdresId=bazowy.AdresId;
            ObrazekId=bazowy.ObrazekId;
            Opis=bazowy.Opis;
            AutomatyczneKoordynaty=bazowy.AutomatyczneKoordynaty;
            KoordynatyZERP=bazowy.KoordynatyZERP;
            Siedziba = bazowy.Siedziba;
        }

        public bool RecznieDodany()
        {
            return Id <= 0;
        }
    }
}
