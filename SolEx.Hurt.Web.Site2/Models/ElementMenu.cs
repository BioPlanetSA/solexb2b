using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ElementMenu
    {
        private readonly TrescBll _tresc,_reklama;

        /// <summary>
        /// Tworzy nowy element menu na podstawie tresci
        /// </summary>
        /// <param name="tresc"></param>
        /// <param name="wybrany"></param>
        /// <param name="reklama"></param>
        /// <param name="klient"></param>
        public ElementMenu(TrescBll tresc, TrescBll reklama)
        {
            _tresc = tresc;
            _reklama = reklama;
        }

        public long Id
        {
            get { return _tresc.Id; }
        }

        public long? Nadrzedna
        {
            get { return _tresc.NadrzednaId; }
        }

        public string Nazwa
        {
            get { return _tresc.Nazwa; }
        }
        public TrescBll TrescBll
        {
            get { return _tresc; }
        }
        public TrescBll Reklama
        {
            get { return _reklama; }
        }
     
        public decimal LiczbaProduktowJesliKategoriaProduktowa { get; set; }

        public bool Wybrany { get; set; }

        public override string ToString()
        {
            return _tresc.Nazwa;
        }
    }
}