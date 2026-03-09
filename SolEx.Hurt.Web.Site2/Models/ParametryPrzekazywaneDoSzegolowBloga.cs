using System.Collections.Generic;
using SolEx.Hurt.Core;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryPrzekazywaneDoSzegolow : PageMetaDataBase
    {
        private readonly string _metaTytul;
        private readonly string _metaOpis;
        private readonly string _metaTagi;

        public override string MetaDescription
        {
            get { return _metaOpis; }
        }

        public override string MetaKeywords
        {
            get { return _metaTagi; }
        }
        public override string PageTitle
        {
            get { return _metaTytul; }
        }
        public ParametryPrzekazywaneDoSzegolow(string naglowek, object wartosc, bool pokazywacGrupe,List<ProduktKlienta> produkty,string metaTytul,string metaOpis,string metaTagi, string stopka, string format="{0}")
        {
            _metaTytul = metaTytul;
            _metaOpis = metaOpis;
            _metaTagi = metaTagi;
            Naglowek = naglowek;
            Wartosc = wartosc;
            Produkty = produkty;
            PokazywacNazweGrupy = pokazywacGrupe;
            Stopka = stopka;
            Format = format;
        }
        public string Naglowek { get; set; }
        public string Stopka { get; set; }
        public object Wartosc { get; set; }
        public List<ProduktKlienta> Produkty { get; set; }

        public string Format { get; set; }
    
        public bool PokazywacNazweGrupy
        {
            get;
            set;
        }

    }
}