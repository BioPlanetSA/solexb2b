using System.Text;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class TrescWiersz : IHasIntId, IPolaIDentyfikujaceRecznieDodanyObiekt, IStringIntern
    {
        public TrescWiersz()
        {
            Dostep = AccesLevel.Wszyscy;
            DodatkoweKlasyCssReczne = "";
            Szerokosc = 12;
        }

        [PrimaryKeyAttribute]
        [AutoIncrement]
        public int Id { get; set; }

        [IdentyfikatorObiektuNadrzednego(typeof (TrescKolumna))]
        public int TrescId { get; set; }

        public int Kolejnosc { get; set; }

        [StringInternuj]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Kolor tła pojemnika")]
        [WymuszonyTypEdytora(TypEdytora.PoleKolor)]
        public string KolorTla { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Zdjęcia tła pojemnika")]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ObrazekTla { get; set; }

        [StringInternuj]
        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Margines - góra prawa dół lewo - wszystko w px - np. 5px 0px 5px 0px")]
        public string Marginesy { get; set; }

        [StringInternuj]
        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Odstęp (padding) - góra prawa dół lewo - wszystko w px - np. 5px 0px 5px 0px")]
        public string Paddingi { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Rozciągnij zawawartość",FriendlyOpis = "Rozciągnij zawartość na całą szerokość")]
        public bool RozciagnijCalaSzerokosc { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Dodatkowe klasy css")]
        [Niewymagane]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKlassCss,SolEx.Hurt.Core")]
        public string[] DodatkoweKlasyCss { get; set; }

        [StringInternuj]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Dodatkowe klasy css ręcznie wpisane")]
        [Niewymagane]
        public string DodatkoweKlasyCssReczne { get; set; }

        //[WidoczneListaAdmin(true, true, true, true)]
        //[FriendlyName("Wyrównanie treści")]
        //[Niewymagane]
        //public Wyrownanie? Wyrownanie { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        public AccesLevel Dostep { get; set; }


        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Szerokość pojemnika")]
        [Niewymagane]
        public int Szerokosc { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Opis kontenera")]
        [GrupaAtttribute("Ogólne", 1)]
        public string OpisKontenera { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin]
        [FriendlyName("Nazwa pojemnika")]
        [GrupaAtttribute("Zawijanie", 3)]
        public string AcordionNazwa { get; set; }

        [WidoczneListaAdmin]
        [FriendlyName("Domyślne zwinięty")]
        [GrupaAtttribute("Zawijanie", 3)]
        public bool AcordionZwiniety { get; set; }


        public bool RecznieDodany()
        {
            return true;
        }

        public string PobierzCss()
        {
            StringBuilder sb = new StringBuilder();
            if (DodatkoweKlasyCss != null)
            {
                foreach (var d in DodatkoweKlasyCss)
                {
                    sb.AppendFormat("{0} ", d);
                }
            }
            sb.Append(DodatkoweKlasyCssReczne);
            return sb.ToString();
        }

    }
}
