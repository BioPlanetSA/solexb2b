using System;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model grupy produktów
    /// </summary>
    [FriendlyName("Grupa produktu")]
    public class Grupa : IPolaIDentyfikujaceRecznieDodanyObiekt, IObiektWidocznyDlaOkreslonychGrupKlientow, IPoleJezyk
    {
        [PrimaryKey]
        [UpdateColumnKey]
        [AutoIncrement]
        [WidoczneListaAdmin(true, true, false, false)]
        public long  Id {get;set;}

        [WidoczneListaAdmin(true, true, true, false)]
        public string Nazwa {get;set;}

        [WidoczneListaAdmin(true, true, true, false)]
        public bool Widoczna {get;set;}

        [WidoczneListaAdmin(true, true, true, false)]
        [FriendlyName("Grupa producencka:")]
        public bool Producencka { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Początek cechy z której ma być budowane drzewo kategorii (np. cecha1: ). Może być użyte średnik ; jeśli kilka parametrów.")]
        public string Parametry { get; set; }


        public string[] ParametryTablica()
        {
           return this.Parametry.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
        }

        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Kolejność na stronie:")]
        public int Kolejnosc { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikGrupy,SolEx.Hurt.Core")]
        [FriendlyName("Grupuj po:")]
        public int? GrupaKomplementarnaId {get;set;}

        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Lista z obrazkami:")]
        public bool ListaZObrazkami {get;set;}

        
        public int? ObrazekId { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Dostęp")]
        public AccesLevel Dostep {get;set;}

        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Opis zbiorczy na produkty w tej grupie:")]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [Lokalizowane]
        public string OpisZbiorczy { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Grupuj wyniki szukania wg tej grupy:")]
        public bool GrupujWyszukiwanie { get; set; }

        [FriendlyName("Widoczność")]
        [WymuszonyTypEdytora(TypEdytora.WidocznoscDlaKlientow)]
        [Ignore]
        [WidoczneListaAdmin(true, false, true, false)]
        public WidocznosciTypow Widocznosc { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Symbol treści z szablonem opisu grupy - dedykowana strona opisująca całą grupę kategorii - np. kuchnie, wg. przeznaczenia itp.")]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikTresciSystemowych,SolEx.Hurt.Core")]
        public string SymbolTresciOpisuGrupy { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("SzablonListyProduktow opisu podkategorii nad produktami - szablon opisu dla kategorii w danej grupie, pokazywany nad produktami wybranej kategorii")]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikTresciSystemowych,SolEx.Hurt.Core")]
        public string SymbolTresciOpisuKategorii { get; set; }

        public Grupa() { Dostep = AccesLevel.Wszyscy; }
        
        public Grupa(Grupa g):this()
        {

            Id = g.Id;
            Nazwa = g.Nazwa;
            GrupujWyszukiwanie=g.GrupujWyszukiwanie;
            Dostep=g.Dostep;
            GrupaKomplementarnaId=g.GrupaKomplementarnaId;
            Kolejnosc = g.Kolejnosc;
            ListaZObrazkami = g.ListaZObrazkami;
            ObrazekId = g.ObrazekId;
            OpisZbiorczy= g.OpisZbiorczy;
            Parametry =g.Parametry;
            Producencka =g.Producencka;
            Widoczna = g.Widoczna;
        }

        public bool RecznieDodany()
        {
            return true;
        }
        [Ignore]
        public int JezykId { get; set; }
    }
}
