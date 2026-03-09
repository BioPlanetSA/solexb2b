using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.ModelBLL.Interfejsy
{
    public interface IProduktKlienta:IProduktBazowy, IPoleKlient
    {
        
        [Ignore]    
        IFlatCenyBLL FlatCeny { get; }

        decimal? DostepnyLimit { get; }

        List<IProduktKlienta> ProduktyWRodzinie(HashSet<long> idProduktow = null);
        
        bool JestWMoimKatalogu();
       
        new decimal Vat { get; set; }

        List<CechyBll> PobierzMetkiLista(MetkaPozycjaLista pozycja);
        List<CechyBll> PobierzMetkaPozycjaKoszykGratisy(MetkaPozycjaKoszykGratisy pozycja);
        List<CechyBll> PobierzMetkaPozycjaKoszykGratisyPopUp(MetkaPozycjaKoszykGratisyPopUp pozycja);
        List<CechyBll> PobierzMetkaPozycjaKoszykAutomatyczne(MetkaPozycjaKoszykAutomatyczne pozycja);
        List<CechyBll> PobierzMetkiRodzina(MetkaPozycjaRodziny pozycja);
        List<CechyBll> PobierzMetkiSzczegoly(MetkaPozycjaSzczegoly pozycja);
        List<CechyBll> PobierzMetkiSzczegolyWarianty(MetkaPozycjaSzczegolyWarianty pozycja);
        List<CechyBll> PobierzMetkiKafle(MetkaPozycjaKafle pozycja);
        //new HashSet<long> IdCechPRoduktu { get; set; }

        List<CechyBll> PobierzMetkaPozycjaKoszykProdukty(MetkaPozycjaKoszykProdukty pozycja);
        bool CzyWszystkieDzieciMajaTaSamaCene(ref IFlatCenyBLL cenaMinimalna);
        
        decimal IloscOgraniczonaDoMax { get; }
        decimal IloscLaczna { get; }
        TypStanu PobierzTypStany { get; }
        bool NaStanie { get; }
        HashSet<long> WymaganaKoncesja { get; }
        new Dictionary<long, bool> Zamienniki { get; }
        //HashSet<long> ZamiennikiJednostrone { get; }
        //HashSet<long> ZamiennikiDwustronne { get; }
        HashSet<long> GradacjeProduktyKtorychZakupyLiczycWspolnie { get; set; }
        new decimal IloscMinimalna { get; set; }
        decimal? CenaZaKG { get; }
        decimal? CenaZaLitr { get; }
        bool CzyJestKoncesja();

        /// <summary>
        /// metoda ktora kasuje flat ceny i zmusza do powtornego przeliczanie cena - np. po zmienie gradacji
        /// </summary>
        void WymusPrzeliczanieCeny();
    }
}