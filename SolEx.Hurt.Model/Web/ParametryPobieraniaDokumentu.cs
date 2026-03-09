using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.Web
{
    public class ParametryPobieraniaDokumentu
    {
        public  ParametryPobieraniaDokumentu(string nazwa,string  nazwaPliku , string modul,int idDokumentu)
        {
            Nazwa = nazwa;
            NazwaPliku = nazwaPliku;
            Modul = modul;
            IdDoumentu = idDokumentu;
        }
        public string Nazwa { get; set; }
        public string NazwaPliku { get; set; }
        public string Modul { get; set; }
        public int IdDoumentu { get; set; }

        public string WygenerujLink()
        {
            return string.Format("/dokumenty/Pobierz/{0}/{1}/", IdDoumentu, Modul);
        }
    }
}
