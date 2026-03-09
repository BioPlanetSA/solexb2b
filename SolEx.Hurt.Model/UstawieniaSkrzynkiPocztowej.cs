using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model
{
    public class UstawieniaSkrzynkiPocztowej
    {
        public string serwer { get; set; }
        public string uzytkownik { get; set; }
        public string haslo { get; set; }
        public int port { get; set; }
        public bool uzywajSSL { get; set; }
        public int timeout { get; set; }
        public string NadawcaPrzyjaznaNazwa { get; set; }

        public string NadawcaEmail { get; set; }

        public string replayTo { get; set; }
    }
}
