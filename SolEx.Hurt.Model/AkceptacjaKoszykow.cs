using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class AkceptacjaKoszykow : IHasLongId
    {
        public AkceptacjaKoszykow() { }

        public long Id
        {
            get
            {
                return string.Format("{0}_{1}", KoszykId, KlientId).WygenerujIDObiektuSHAWersjaLong();
            }
        }
        public AkceptacjaKoszykow(long idKoszyka, long idKlienta)
        {
            KoszykId = idKoszyka;
            KlientId = idKlienta;
        }
        public long KoszykId { get; set; }

        public long KlientId { get; set; }
    }
}
