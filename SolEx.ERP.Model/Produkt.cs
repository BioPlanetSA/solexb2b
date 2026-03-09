using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.ERP.Model
{
    public class Produkt
    {
        public string Nazwa { get; set; }
        public string KodKreskowy { get; set; }
        public string KodKreskowy2 { get; set; }
        public string KodKreskowy3 { get; set; }
        public string Symbol { get; set; }
        public decimal Ilosc { get; set; }
        public int Id { get; set; }
        public string NumerSeryjny { get; set; }
        public string StawkaVat { get; set; }
        public decimal CenaDetalicznaNetto { get; set; }
        public decimal CenaDetalicznaBrutto { get; set; }
        public string Dostawca { get; set; }
        public string SymbolUDostawcy { get; set; }
        public int SredniCzasDostepnosci { get; set; }
        public string GrupaTowarow { get; set; }
        public string Www { get; set; }
        public string Uwagi { get; set; }
        public string Opis { get; set; }
        public string PodstJmiary { get; set; }
        public string Charakterystyka { get; set; }
        public string Producent { get; set; }
        public string DodatkowaJM { get; set; }
        public decimal PrzelicznikDodatkowejJM { get; set; }

        public Produkt()
        {

        }

        public Produkt(string nazwa, string kodEan, string symbol, decimal ilosc, int id, decimal cenaNetto, decimal cenaBrutto, string uwagi)
        {
            Nazwa = nazwa;
            KodKreskowy = kodEan;
            Symbol = symbol;
            Ilosc = ilosc;
            Id = id;
            CenaDetalicznaNetto = cenaNetto;
            CenaDetalicznaBrutto = cenaBrutto;
            Uwagi = string.IsNullOrEmpty(uwagi) ? "" : uwagi;
        }

        public Produkt(string nazwa, string kodEan, string symbol, decimal ilosc)
        {
            Nazwa = nazwa;
            KodKreskowy = kodEan;
            Symbol = symbol;
            Ilosc = ilosc;
        }

        public Produkt(string nazwa, string kodEan, string symbol, decimal ilosc, string jednostkamiary, int id, string uwagi)
        {
            Nazwa = nazwa;
            KodKreskowy = kodEan;
            Symbol = symbol;
            Ilosc = ilosc;
            Id = id;
            PodstJmiary = jednostkamiary;
            Uwagi = uwagi;
        }

        public Produkt(string nazwa, string kodEan, string symbol, decimal ilosc, string jednostkamiary, int id, string dodatkowajm, decimal przelicznikDjm, string uwagi)
        {
            Nazwa = nazwa;
            KodKreskowy = kodEan;
            Symbol = symbol;
            Ilosc = ilosc;
            Id = id;
            PodstJmiary = jednostkamiary;
            DodatkowaJM = dodatkowajm;
            PrzelicznikDodatkowejJM = przelicznikDjm;
            Uwagi = uwagi;
        }

        public Produkt(string nazwa, string kodEan, string symbol, decimal ilosc, int id, string opis)
        {
            Nazwa = nazwa;
            KodKreskowy = kodEan;
            Symbol = symbol;
            Ilosc = ilosc;
            Id = id;
            Opis = opis;
        }


    }
}
