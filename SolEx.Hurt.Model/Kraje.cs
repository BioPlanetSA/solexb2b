using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class Kraje : IHasIntId, IPoleJezyk
    {
        public Kraje() { }
        public Kraje(int id, string nazwa,string symbol,bool widoczny=true)
        {
            Id = id;
            Nazwa = nazwa;
            Widoczny = widoczny;
            Symbol = symbol;
        }
        [PrimaryKey]
        [WidoczneListaAdmin(true, true, false, false)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        public int Id { get; set; }
       
        [FriendlyName("Nazwa kraju")]
        [WidoczneListaAdmin(true, true, true, false)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [Lokalizowane]
        [Wymagane]
        public string Nazwa { get; set; }

        [FriendlyName("Symbol kraju")]
        [WidoczneListaAdmin(true, true, true, false)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [Wymagane]
        public string Symbol { get; set; }
        
        public bool Synchronizowane { get; set; }
        
        [FriendlyName("Widoczność kraju")]
        [WidoczneListaAdmin(true, true, true, false)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [Wymagane]
        public bool Widoczny { get; set; }
        [Ignore]
        public int JezykId { get; set; }
    }
}
