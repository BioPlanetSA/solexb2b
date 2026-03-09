using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Validation;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Helpers
{
    public class ListaPlikowModel
    {
        /// <summary>
        /// sama nazwa pliku bez rozszerzenia
        /// </summary>
        public string NazwaBezRoszerzenia
        {
            get { return _nazwaBezRoszerzenia; }
            set
            {
                NazwaOryginalna = value;
                this._nazwaBezRoszerzenia = value.Replace("_", " ").Replace("-", " ").Trim().OczyscCiagZZbednychSpacji();
            }
        }

        private string _nazwaBezRoszerzenia = null;


        /// <summary>
        /// oryginalan nazwa -przydatna do testow glownie
        /// </summary>
        public string NazwaOryginalna { get; set; }

        /// <summary>
        /// samo rozszerzenie
        /// </summary>
        public string Roszerzenie { get; set; }

        public string LinkDoIkony { get; set; }

        public decimal RozmiarMB { get; set; }

        public DateTime Data { get; set; }

        /// <summary>
        /// link do pliku
        /// </summary>
        public string Link { get; set; }

        public TypZasobu Typ { get; set; }
    }
}
