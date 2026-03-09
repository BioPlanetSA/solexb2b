using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryDoListyDokumentow
    {
        public ParametryDoListyDokumentow() { }
        public ParametryDoListyDokumentow(RodzajDokumentu rodzaj, string przelew, bool platnosc, DateTime dataTrybuPlatnosci, List<DaneDoTabow> taby = null, bool status = false, bool zrealizowane = false, bool niezrealizowane=false)
        {
            Rodzaj = rodzaj;
            DaneDoPrzelewu = przelew;
            PlatnoscOpline = platnosc;
            Taby = taby;
            PokazujStatus = status;
            PokazujKolumneZrealizowane = zrealizowane;
            PokazTylkoNiezrealizowane = niezrealizowane;
            OdKiedyTrybuPlatnosci = dataTrybuPlatnosci;
        }

        public RodzajDokumentu Rodzaj { get; set; }
        public string DaneDoPrzelewu { get; set; }
        public bool PlatnoscOpline { get; set; }
        public List<DaneDoTabow> Taby { get; set; }
        public bool PokazujStatus { get; set; }
        public bool PokazujKolumneZrealizowane { get; set; }
        public bool PokazTylkoNiezrealizowane { get; set; }
        public DateTime? OdKiedyTrybuPlatnosci { get; set; }
        public DateTime? OdKiedyTrybuPrzegladania { get; set; }
        public DateTime OdKiedyPobieracDokumenty { get; set; }
        public bool WybraneTylkoNiezrealizowane { get; set; }
        public bool WybraneTylkoNiezaplacone { get; set; }
        public bool WybraneTylkoPrzeterminowane { get; set; }

    }
}