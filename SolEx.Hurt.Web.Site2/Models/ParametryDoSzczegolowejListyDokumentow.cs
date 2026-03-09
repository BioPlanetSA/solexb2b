using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryDoSzczegolowejListyDokumentow
    {
        public ParametryDoSzczegolowejListyDokumentow(List<DokumentyBll> dok,  DocumentSummary wykres, bool platnosc,bool dane, bool status, bool zrealizowane=false)
        {
            Dokumenty = dok;
            DaneDoWykresu = wykres;
            PlatnosciOnline = platnosc;
            DaneDoPrzelewu = dane;
            PokazujStatus = status;
            PokazujKolumneZrealizowane = zrealizowane;
        }

        public List<DokumentyBll> Dokumenty { get; set; }
        public DocumentSummary DaneDoWykresu { get; set; }
        public bool PlatnosciOnline { get; set; }
        public bool DaneDoPrzelewu { get; set; }
        public bool PokazujStatus { get; set; }
        public bool PokazujKolumneZrealizowane { get; set; }
    }
}