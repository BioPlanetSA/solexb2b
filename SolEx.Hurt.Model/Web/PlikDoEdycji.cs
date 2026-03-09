using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model.Web
{
    public class PlikDoEdycji : IBindowalny
    {

        public PlikDoEdycji()
        {
        }

        public PlikDoEdycji(string nazwa, string sciezka, string hash,string rozszerzenie,string opis,string zawartosc)
        {
            Nazwa = nazwa;
            Sciezka = sciezka;
            Hash = hash;
            Typ = rozszerzenie;
            Opis = opis;
            Zawartosc = zawartosc;
        }
        public string Opis { get; set; }
        public string Nazwa { get; set; }

        public string Sciezka { get; set; }

        public string Hash { get; set; }
        public string Typ { get; set; }

        public string Zawartosc { get; set; }
    }
}
