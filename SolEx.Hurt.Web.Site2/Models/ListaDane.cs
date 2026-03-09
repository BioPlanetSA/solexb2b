using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ListaDane
    {
        public ListaProduktowZKategoriam UstawieniaListyProduktow { get; set; }

        public ListaDane(int lacznie, ListaProduktowZKategoriam ust, ParametryPrzekazywaneDoListyProduktow aktaulneParametry,
            IKlient klient, TrescBll tresc, TrescBll trescKAtegorie, string opisProduktowZamiastOpisuKategorii,string uklad)
        {
            LiczbaProduktow = lacznie;
            UstawieniaListyProduktow = ust;
            AktualneParametry = aktaulneParametry;
            Klient = klient;
            TrescOpisowa = tresc;
            KategoriaOpis = trescKAtegorie;
            OpisProduktowZamiastOpisuKategorii = opisProduktowZamiastOpisuKategorii;
            SzablonListy = uklad;
        }
        public ListaDane() { }
        public int LiczbaProduktow { get; set; }
        public IKlient Klient { get; set; }

        public ParametryPrzekazywaneDoListyProduktow AktualneParametry { get; set; }

        public TrescBll TrescOpisowa { get; set; }

        public TrescBll KategoriaOpis { get; set; }

        public string OpisProduktowZamiastOpisuKategorii { get; set; }
        public string SzablonListy { get; set; }

        /// <summary>
        /// ustawienia slajdera jesli ma byc wlaczony - moze byc NULL!!
        /// </summary>
        public Slajder slajder { get; set; }

        public bool KlientWidziCeneHurtowa { get; set; }
        public bool SortowanieJednostek { get; set; }

        public bool PokazujJednostki { get; set; }

        public Dictionary<PozycjaLista, Dictionary<long, SposobPokazywaniaStanow>> WszystkieSposobyPokazywaniaStanowKlienta { get; set; }
    }
}