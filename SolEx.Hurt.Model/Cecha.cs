using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model cechy
    /// </summary>
    [TworzDynamicznieTabele]
    public class Cecha : IHasLongId, IPolaIDentyfikujaceRecznieDodanyObiekt, IPoleJezyk
    {
        public override string ToString() => $"Nazwa: {Nazwa}, Id: {Id}, Symbol: {Symbol}";

        public bool RecznieDodany()
        {
            return Id < 0;
        }

        public Cecha(Cecha c)
        {
            if (c == null)
            {
                return;
            }
            Id = c.Id;
            Nazwa = c.Nazwa;
            Widoczna = c.Widoczna;
            Symbol = c.Symbol;
            ObrazekId = c.ObrazekId;
            Kolejnosc = c.Kolejnosc;
            AtrybutId = c.AtrybutId;
            Opis = c.Opis;
    
            MetkaOpis = c.MetkaOpis;
            MetkaPozycjaSzczegoly = c.MetkaPozycjaSzczegoly;
            MetkaKatalog = c.MetkaKatalog;
            MetkaPozycjaLista = c.MetkaPozycjaLista;
            MetkaPozycjaRodziny = c.MetkaPozycjaRodziny;
            MetkaPozycjaSzczegolyWarianty = c.MetkaPozycjaSzczegolyWarianty;
            OpisNaProdukcie = c.OpisNaProdukcie;
            MetkaPozycjaKoszykProdukty = c.MetkaPozycjaKoszykProdukty;
            MetkaPozycjaKoszykAutomatyczne = c.MetkaPozycjaKoszykAutomatyczne;
            MetkaPozycjaKoszykGratisy = c.MetkaPozycjaKoszykGratisy;
            MetkaPozycjaKoszykGratisyPopUp = c.MetkaPozycjaKoszykGratisyPopUp;
            CechyNadrzedne = c.CechyNadrzedne;
            MetkaPozycjaKafle = c.MetkaPozycjaKafle;
            CssKlasy = c.CssKlasy;
            JezykId = c.JezykId;
        }

        public Cecha(string nazwa, string symbol):this()
        {
            Nazwa = nazwa.Trim();
            Symbol = symbol.Trim().ToLower();
        }

        public Cecha()
        {
            CechyNadrzedne=new HashSet<long>();
            MetkaPozycjaSzczegoly = MetkaPozycjaSzczegoly.Brak;
            MetkaPozycjaLista = MetkaPozycjaLista.Brak;
            MetkaPozycjaRodziny = MetkaPozycjaRodziny.Brak;
            MetkaPozycjaSzczegolyWarianty = MetkaPozycjaSzczegolyWarianty.Brak;
            MetkaPozycjaKoszykProdukty = MetkaPozycjaKoszykProdukty.Brak;
            MetkaPozycjaKoszykAutomatyczne = MetkaPozycjaKoszykAutomatyczne.Brak;
            MetkaPozycjaKoszykGratisy = MetkaPozycjaKoszykGratisy.Brak;
            MetkaPozycjaKoszykGratisyPopUp = MetkaPozycjaKoszykGratisyPopUp.Brak;
            MetkaPozycjaKafle = MetkaPozycjaKafle.Brak;
           Widoczna = true;
        }
        
        /// <summary>
        /// ID cechy
        /// </summary>
        [UpdateColumnKey]
        [PrimaryKey]
        [WidoczneListaAdmin(true, true, false, false)]
        public long Id { get; set; }
        
        /// <summary>
        /// Nazwa cechy
        /// </summary>
        [WidoczneListaAdmin(true, true, true, false)]
        [GrupaAtttribute("Dane podstawowe", 1)]
        [Lokalizowane]
        //[MaksymalnaLiczbaZnakow(2000)]    - bartek zmieniam zeby nie bylo limitu dla nazwy cechy
        public string Nazwa {get;set;}
        
        /// <summary>
        /// Czy cecha jest wyœwietlana
        /// </summary>
        [WidoczneListaAdmin(true, false, true, true)]
        [GrupaAtttribute("Dane podstawowe", 1)]
        public bool Widoczna { get; set; }
        
        /// <summary>
        /// Symbol cechy
        /// </summary>
        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Dane podstawowe", 1)]
        [MaksymalnaLiczbaZnakow(2000)]
        public string Symbol { get; set; }
        
        /// <summary>
        /// ID obrazka przypisanego cesze
        /// </summary>
        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Dane podstawowe", 1)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ObrazekId { get; set; }
        
        /// <summary>
        /// Kolejnoœæ pokazywania cechy
        /// </summary> 
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Kolejnoœæ")]
        [GrupaAtttribute("Dane podstawowe", 1)]
        public long? Kolejnosc { get; set; }
        
        //[ForeignKey(typeof(atrybuty), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        /// <summary>
        /// Id atrybutu do którego nale¿y cecha
        /// </summary>
        public int? AtrybutId { get; set; }
        
        /// <summary>
        /// Opis cechy
        /// </summary>
        [WidoczneListaAdmin(true, false, true, true)]
        [Niewymagane]
        [GrupaAtttribute("Dane podstawowe", 1)]
        public string Opis { get; set; }


        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Opis na produkcie")]
        [Niewymagane]
        [GrupaAtttribute("Dane podstawowe", 1)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string OpisNaProdukcie { get; set; }
        
        /// <summary>
        /// Metka na hurcie
        /// </summary> 
        [WidoczneListaAdmin(true, true, true, false)]
        [FriendlyName("Metka")]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [GrupaAtttribute("Metki", 2)]
        [Lokalizowane]
        public string MetkaOpis { get; set; }

        /// <summary>
        /// Metka na katalogu
        /// </summary>
        [WidoczneListaAdmin(true, true,true, false)]
        [FriendlyName("Metka katalog")]
        [GrupaAtttribute("Metki", 2)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [Lokalizowane]
        public string MetkaKatalog { get; set; }

        [FriendlyName("Metka pozycja koszyk lista automatycznych produktów")]
        [WidoczneListaAdmin(true, false, true, true)]
        [GrupaAtttribute("Metki", 2)]
        
        public MetkaPozycjaKoszykAutomatyczne MetkaPozycjaKoszykAutomatyczne { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Metka pozycja koszyk lista produktów dodanych przez klienta")]
        [GrupaAtttribute("Metki", 2)]
        public MetkaPozycjaKoszykProdukty MetkaPozycjaKoszykProdukty { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Metka pozycja na karcie produktu")]
        [GrupaAtttribute("Metki", 2)]
        public MetkaPozycjaSzczegoly MetkaPozycjaSzczegoly { get; set; }

        [FriendlyName("Metka pozycja lista produktów")]
        [WidoczneListaAdmin(true, false, true, true)]
        [GrupaAtttribute("Metki", 2)]
        public MetkaPozycjaLista MetkaPozycjaLista { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Metka pozycja na widoku rodzinowym")]
        [GrupaAtttribute("Metki", 2)]
        public MetkaPozycjaRodziny MetkaPozycjaRodziny { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Metka pozycja koszyk dla produktów gratisowych")]
        [GrupaAtttribute("Metki", 2)]
        public MetkaPozycjaKoszykGratisy MetkaPozycjaKoszykGratisy { get; set; }

        [FriendlyName("Metka pozycja koszyk lista gratisowych produktów - popup")]
        [WidoczneListaAdmin(true, false, true, true)]
        [GrupaAtttribute("Metki", 2)]
        public MetkaPozycjaKoszykGratisyPopUp MetkaPozycjaKoszykGratisyPopUp { get; set; }
         

        [WidoczneListaAdmin(false, false, true, true)]
        [GrupaAtttribute("Metki", 2)]
        [FriendlyName("Metka pozycja dla wariantów na karcie produktu")]
        public MetkaPozycjaSzczegolyWarianty MetkaPozycjaSzczegolyWarianty { get; set; }


        [WidoczneListaAdmin(false, false, true, true)]
        [GrupaAtttribute("Metki", 2)]
        [FriendlyName("Metka pozycja na kaflu produktu")]
        public MetkaPozycjaKafle MetkaPozycjaKafle { get; set; }
       
        /// <summary>
        /// Nazwa dla u¿ytkownika koñcowego, pokazuje zawartoœc pola nazwa, jeœli jest puste to symbol
        /// </summary>
        [Ignore]
        public string PobierzWyswietlanaNazwe => string.IsNullOrEmpty(Nazwa) ? Symbol : Nazwa;

        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikCech,SolEx.Hurt.Core")]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Cechy nadrzêdne")]
        [Niewymagane]
        public HashSet<long> CechyNadrzedne { get; set; }

        public string FormatujWyswietlanaNazwe(string format=null)
        {
            return string.IsNullOrEmpty(format) ? PobierzWyswietlanaNazwe : string.Format(format, PobierzWyswietlanaNazwe);
        }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Opcjonalna klasa css")]
        [Niewymagane]
        public string CssKlasy { get; set; }

        [Ignore]
        public int JezykId { get; set; }
    }
}
