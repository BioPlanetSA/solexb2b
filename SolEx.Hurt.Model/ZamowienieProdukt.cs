using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model
{
    public class ZamowienieProdukt: DokumentuPozycjaBazowa
    {  
        public virtual decimal JednostkaPrzelicznik { get; set; }

        public long? PrzedstawicielId { get; set; }

        public DateTime? DataZmiany { get; set; }
        
        public TypPozycjiZamowienia TypPozycji { get; set; }

        public ZamowienieProdukt(){}

        public ZamowienieProdukt(ZamowienieProdukt baza)
        {
            if (baza == null)
            {
                return;
            }
            Id = baza.Id;
            DokumentId = baza.DokumentId;
            ProduktId = baza.ProduktId;
            ProduktIdBazowy = baza.ProduktIdBazowy;
            Ilosc = baza.Ilosc;
            CenaNetto = baza.CenaNetto;
            CenaBrutto = baza.CenaBrutto;
            this.UstawJednostke(baza.Jednostka);
            JednostkaMiary = baza.JednostkaMiary;
            JednostkaPrzelicznik = baza.JednostkaPrzelicznik;
            DataZmiany = baza.DataZmiany;
            Opis = baza.Opis;
            Opis2 = baza.Opis2;
            TypPozycji = baza.TypPozycji;
            this.walutaB2b = baza.walutaB2b;
        }
    }
}
