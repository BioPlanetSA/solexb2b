using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.Importy.Model
{
    [FriendlyClassNameAttribute("Model pozycji w koszyku")]
  
    public class PozycjaKoszykaImportowana :ProduktBazowy
    {
        public PozycjaKoszykaImportowana(ProduktBazowy pb) : base(pb)
        {
        }
        public PozycjaKoszykaImportowana() 
        {
        }
        public decimal Ilosc { get; set; }
          public string Jednostka { get; set; }
          public long Produkt { get; set; }
    }
}
