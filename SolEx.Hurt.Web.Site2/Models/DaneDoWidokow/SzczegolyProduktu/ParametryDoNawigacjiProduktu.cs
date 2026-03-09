using System.Collections.Generic;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models
{
  public class ParametryDoNawigacjiProduktu
    {
      public ParametryDoNawigacjiProduktu(IProduktKlienta n, IProduktKlienta p, int iloscWszystkichPozycji, int aktualnaPozycja)
        {       
            Prev = p;
            Next = n;
          IloscWszystkichProduktów = iloscWszystkichPozycji;
          PozycjaAktualnegoProduktu = aktualnaPozycja;
        }
    
        public IProduktKlienta Next { get; set; }
        public IProduktKlienta Prev { get; set; }

        public int IloscWszystkichProduktów { get; set; }
        public int PozycjaAktualnegoProduktu { get; set; }

    }
}