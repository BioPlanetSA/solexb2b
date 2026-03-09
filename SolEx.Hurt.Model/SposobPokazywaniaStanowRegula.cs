using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    [FriendlyName("Reguła do Sposobu Pokazywania Stanów")]
    public class SposobPokazywaniaStanowRegula : IComparable<SposobPokazywaniaStanowRegula>, IPolaIDentyfikujaceRecznieDodanyObiekt, IHasIntId, IPoleJezyk
    {
        [PrimaryKey]
        [UpdateColumnKey()]
        [AutoIncrement]
        public int Id { get; set; }
        
         [WidoczneListaAdmin(true, true, true, true)]
        public int Kolejnosc { get; set; }


        /*
         *  
         * 
         */
        [FriendlyName("Tekst do pokazania (wynik HTML)", FriendlyOpis = "Tagi jakich można używać (trzeba je wprowadząc w kodzie źródłowym)" +
              "<table>" +
              "<tr><td>  Aby pokazać termin dostawy umieść:</td><td> @Model.Produkt.Dostawa - fraza jest opakowana w tłumaczenie </td></tr>" +
              "<tr><td>Aby pokazać kontrolke informacji o dostepnosci należy użyć:</td><td> @{ Html.InfoODostepnosci(Model.Produkt); } </td></tr>" +
              "<tr><td>Aby pokazać najbliższą datę cyklicznej dostawy wpisz:</td><td> @Model.Produkt.NajblizszaDostawa.Value.ToShortDateString() </td></tr>" +
              "<tr><td>Aby pokazać najbliższy dzień cyklicznej dostawy wpisz:</td><td> @Html.NazwaDnia(Model.Produkt.NajblizszaDostawa.Value.DayOfWeek) - nazwa jest brana zawsze z kultury aktualnie wybranego języka </td></tr>" +
              "<tr><td>Aby pokazać dostępną liczbę poroduktów:</td><td> @Model.Produkt.IloscLaczna  </td></tr>" +
              "<tr><td>Nazwa magazynu dla którego jest stan:</td><td> @Model.StanyNaMagazynie.Magazyn.Symbol </td></tr>" +
              "<tr><td>Ilość dla wybranego magazynu:</td><td>@Model.StanyNaMagazynie.Stan </td></tr>" +
              "<tr><td>Data dostawy:</td><td>@Model.Produkt.Dostawa </td></tr>" +
              "<tr><td>Ilośc dla wybranego magazynu, bez miejsc po przecinku:</td><td>@Model.StanyNaMagazynie.Stan.DoLadnejCyfry(\"####\")</td></tr>" +
              "<tr><td>Ilośc dla wybranego magazynu, do dwóch miejsc po przecinku:</td><td>@Model.StanyNaMagazynie.Stan.DoLadnejCyfry()</td></tr>" +
              "<tr><td colspan='2'>Aby przetłumaczyć frazę należy wybrać język z menu oraz dokonać tłumaczenia</td></tr>" +

              "</table>")]
        [WidoczneListaAdmin(true, true, true, true)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Lokalizowane]
        public string WynikHtml { get; set; }




        public int CompareTo(SposobPokazywaniaStanowRegula obj)
        {
            if (obj.Kolejnosc == this.Kolejnosc)
            {
                return 0;
            }

            if (obj.Kolejnosc > this.Kolejnosc)
            {
                return 1;
            }
                return - 1;     
                   
        }

        public SposobPokazywaniaStanowRegula()
        {
            WarunekIlosci = Warunek.MniejszyRowny;
            WarunekIlosciMinimalnej = Warunek.WiekszaRowna;
            CyklicznaDostawa = CyklkicznaDostawa.NieWplywa;
            WarunekStany = WarunekStanu.NieWplywa;
            TypStanu=TypStanu.na_stanie;
            
        }
        public SposobPokazywaniaStanowRegula(SposobPokazywaniaStanowRegula r):base() {
            Id = r.Id;
            SposobId = r.SposobId;
            Kolejnosc = r.Kolejnosc;
            WynikHtml = r.WynikHtml;
            IloscProduktu = r.IloscProduktu;
            WarunekIlosci = r.WarunekIlosci;
            RazyStanMinimalny = r.RazyStanMinimalny;
            IloscMinimalna = r.IloscMinimalna;
            WarunekIlosciMinimalnej = r.WarunekIlosciMinimalnej;
            CzyTerminDostawy = r.CzyTerminDostawy;
            CyklicznaDostawa = r.CyklicznaDostawa;
            WarunekStany = r.WarunekStany;
            TypStanu = r.TypStanu;
        }

        public bool RecznieDodany()
        {
            return true;
        }


        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [FriendlyName("Jeśli podana ilość jest:")]
        [GrupaAtttribute("Warunek reguły", 1)]
        public Warunek WarunekIlosci { get; set; }

        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [FriendlyName("niż:")]
        [GrupaAtttribute("Warunek reguły", 1)]
        public decimal IloscProduktu { get; set; }

        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [FriendlyName("Mnożymy stan razy stan minimalny")]
        [GrupaAtttribute("Warunek reguły", 1)]
        public bool RazyStanMinimalny { get; set; }

        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [FriendlyName("i stan minimalny")]
        [GrupaAtttribute("Warunek reguły", 1)]
        public Warunek WarunekIlosciMinimalnej { get; set; }


        [WidoczneListaAdminAttribute(false, false, true, false)]
        [FriendlyName("Ilość minimalna")]
        [GrupaAtttribute("Warunek reguły", 1)]
        public decimal IloscMinimalna { get; set; }

        
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [FriendlyName("Czy produkt musi posiadać termin dostawy")]
        [GrupaAtttribute("Warunek reguły", 1)]
        public bool CzyTerminDostawy { get; set; }

        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [FriendlyName("Cykliczna dostawa")]
        [GrupaAtttribute("Warunek reguły", 1)]
        public CyklkicznaDostawa CyklicznaDostawa { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin]
        [FriendlyName("Produkty ze stanem")]
        [GrupaAtttribute("Warunek reguły", 1)]
        public WarunekStanu WarunekStany { get; set; }

        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [FriendlyName("Typ stanu")]
        [GrupaAtttribute("Warunek reguły", 1)]
        public TypStanu TypStanu { get; set; }



        public string TypKontrolki()
        {
            var e = GetType();
            return string.Format("{0},{1}", e.FullName, e.Assembly.GetName().Name);
        }

        [IdentyfikatorObiektuNadrzednego("SolEx.Hurt.Core.ModelBLL.SposobPokazywaniaStanow,SolEx.Hurt.Core")]
        [WidoczneListaAdmin(false, false, true, false)]
        public int SposobId { get; set; }
        [Ignore]
        public int JezykId { get; set; }
    }


    public enum Warunek
    {
        [FriendlyName("Mniejsza bądź równa")]
        //[Display(Name = "Mniejsza bądź równa")]
        MniejszyRowny=0,
        [FriendlyName( "Większa")]
        Wiesze = 1,
        [FriendlyName("Większa bądź równa")]
        WiekszaRowna = 2,
        [FriendlyName("Mniejsza")]
        Mniejszy = 3,
        [FriendlyName("Równa")]
        Rowna = 4,
    }
    public enum CyklkicznaDostawa
    {
        [FriendlyName("Nie wpływa")]
        NieWplywa = 0,
        [FriendlyName("Nie posiada cyklicznej dostawy")]
        NiePosiada = 1,
        [FriendlyName("Posiada cykliczną dostawę")]
        Posiada = 2,
    }
    public enum WarunekStanu
    {
        [FriendlyName("Nie wpływa")]
        NieWplywa = 0,
        [FriendlyName("Równy")]
        Rowny = 1,
        [FriendlyName("Różny")]
        Rozny = 2,
    }

    public class StanNaMagazynie
    {
        public StanNaMagazynie(Magazyn m, decimal d, int idReguly)
        {
            Magazyn = m;
            Stan = d;
            IdReguly = idReguly;
        }

        public Magazyn Magazyn { get; set; }
        public decimal Stan { get; set; }
        public int IdReguly { get; set; }
    }

}
