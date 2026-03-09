using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
namespace SolEx.Hurt.Core.ModelBLL.Interfejsy
{
    public interface IProduktBazowy : IProdukty, IPoleJezyk
    {
        [Ignore]
        long bazoweID { get; }

        [Ignore]
        [FriendlyName("Zamienniki")]
        [WidoczneListaAdmin(true, false, false, false)]
        Dictionary<long, bool> Zamienniki { get; }

        [FriendlyName("Menad¿er produktu")]
        IKlient MenagerProduktu { get; }
        
        /// <summary>
        /// termin najblizszej dostawy CYKLICZNEJ. nie mylic z terminem dostawy erp
        /// </summary>
        [Ignore]
        [FriendlyName("Termin najbli¿szej dostawy")]
        DateTime? NajblizszaDostawa { get; }

        [FriendlyName("Jednostki dla produktu - lista obiektów")]
        List<JednostkaProduktu> Jednostki { get; }

        [Ignore]
        [FriendlyName("Cecha rodzinna")]
        CechyBll CechaUnikalnaRodzina { get; set; }

        [Ignore]
        [FriendlyName("Produkty w rodzinie")]
        HashSet<long> ProduktyWRodzinieIds { get; set; }
        
        JednostkaProduktu JednostkaPodstawowa { get; }
        
        string MarkaNazwa { get; }

        [FriendlyName("Dodatkowe kody produktów jako ci¹g znaków")]
        string DodatkoweKodyString { get; }

        [Ignore]
        [FriendlyName("Zdjêcia produktu")]
        List<IObrazek> Zdjecia { get; }

        [FriendlyName("Data dostawy")]
        DateTime? DostawaData { get; }

       
        [Ignore]
        [FriendlyName("G³ówne zdjêcie produktu")]
        IObrazek ZdjecieGlowne { get; set; }

        [Ignore]
        [FriendlyName("Cechy produktu")]
        Dictionary<long, CechyBll> Cechy { get; }
        
        [Ignore]
        [FriendlyName("Poziomy cen")]
        Dictionary<int, CenaPoziomu> CenyPoziomy { get; }


        [FriendlyName("Id cech produktu")]
        HashSet<long>IdCechPRoduktu { get; }
        
        List<KategorieBLL> Marki();

        [Ignore]
        List<KategorieBLL> Kategorie { get; set; }

        List<CechyBll> CechyDlaAtrybutu(int atrybutId, bool tylkoWidoczne = true);

        decimal IloscLaczna { get; }
        TypStanu PobierzTypStany { get; }
        bool NaStanie { get; }

        [Ignore]
        HashSet<long> KategorieId { get; }

        HashSet<long> CechyProduktuWystepujaceWRabatach { get; set; }

        [Ignore]
        HashSet<Indywidualizacja> IndiwidualizacjeProduktu { get; set; }

        [Ignore]
        List<Konfekcje> GradacjePosortowane { get; set; }
        decimal PobierzStan(HashSet<int> nazwyMagazynow);
        string FriendlyLinkURL { get; set; }
        bool PosiadaCechy(long[] cechy, bool maMiecWszystkieCechy);
    }
}