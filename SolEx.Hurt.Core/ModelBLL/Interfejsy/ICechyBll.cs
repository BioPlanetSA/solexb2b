using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.Interfejsy
{
    public interface ICechyBll
    {
        [Ignore]
        IObrazek Ikona { get; }

        [Ignore]
        string NazwaAtrybutu { get; }

        /// <summary>
        /// ID cechy
        /// </summary>
        [UpdateColumnKey]
        [PrimaryKey]

        long Id { get; set; }

        /// <summary>
        /// Nazwa cechy
        /// </summary>
       
        string Nazwa { get; set; }

        /// <summary>
        /// Czy cecha jest wyœwietlana
        /// </summary>
       
        bool Widoczna { get; set; }

        /// <summary>
        /// Symbol cechy
        /// </summary>
       
        string Symbol { get; set; }

        /// <summary>
        /// ID obrazka przypisanego cesze
        /// </summary>
        int? ObrazekId { get; set; }

        /// <summary>
        /// Kolejnoœæ pokazywania cechy
        /// </summary>

        long? Kolejnosc { get; set; }

        /// <summary>
        /// Id atrybutu do którego nale¿y cecha
        /// </summary>
        int? AtrybutId { get; set; }

        /// <summary>
        /// Opis cechy
        /// </summary>
        string Opis { get; set; }


        /// <summary>
        /// MEtka na hurcie
        /// </summary>
        
        string MetkaOpis { get; set; }

        
        MetkaPozycjaSzczegoly MetkaPozycjaSzczegoly { get; set; }

        
        MetkaPozycjaLista MetkaPozycjaLista { get; set; }

        
        MetkaPozycjaKoszykGratisy MetkaPozycjaKoszykGratisy { get; set; }

        
        MetkaPozycjaKoszykGratisyPopUp MetkaPozycjaKoszykGratisyPopUp { get; set; }

        
        MetkaPozycjaKoszykAutomatyczne MetkaPozycjaKoszykAutomatyczne { get; set; }
        
        
        MetkaPozycjaRodziny MetkaPozycjaRodziny { get; set; }

        
        MetkaPozycjaSzczegolyWarianty MetkaPozycjaSzczegolyWarianty { get; set; }

        MetkaPozycjaKafle MetkaPozycjaKafle { get; set; }
       MetkaPozycjaKoszykProdukty MetkaPozycjaKoszykProdukty { get; set; }

        /// <summary>
        /// MEtka na katalogu
        /// </summary>
        
        string MetkaKatalog { get; set; }

        /// <summary>
        /// Nazwa dla u¿ytkownika koñcowego, pokazuje zawartoœc pola nazwa, jeœli jest puste to symbol
        /// </summary>
        [Ignore]
        string PobierzWyswietlanaNazwe { get; }

        /// <summary>
        /// Pobiera atrybut do którego przypisana jest cecha
        /// </summary>
        /// <returns></returns>
        AtrybutBll PobierzAtrybut();

        string ToString();
        string FormatujWyswietlanaNazwe(string format);
    }
}