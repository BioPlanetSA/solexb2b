using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    [FriendlyClassName("Model kategorii klientów")]
    [TworzDynamicznieTabele]
    public class KategoriaKlienta : IHasIntId,IPoleJezyk
    {
        [WidoczneListaAdmin(true, true, false, false)]
        public int Id {get;set;}
        
        [FriendlyName("Nazwa")]
        [WidoczneListaAdmin(true, true, true, false)]
        [Lokalizowane]
        public string Nazwa { get; set; }
        
        [FriendlyName("Opis")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Lokalizowane]
        public string Opis { get; set; }
       
        [FriendlyName("Nazwa grupy")]
        [WidoczneListaAdmin(true, true, true, false)]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKategoriiKlienta,SolEx.Hurt.Core")]
        [Lokalizowane]
        public string Grupa { get; set; }
        
        [FriendlyName("Widoczne dla klienta")]
        [WidoczneListaAdmin(true, false, true, false)]
        public bool PokazujKlientowi { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Grupa))
            {
                return Grupa + ":" + Nazwa;
            }
            return Nazwa;
        }
        [Ignore]
        public int JezykId { get; set; }
    }
}
