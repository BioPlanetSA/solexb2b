using System;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model
{
    public class PunktyWpisy
    {
        public PunktyWpisy()
        {
            
        }

        public override string ToString()
        {
            return string.Format("klient {0} opis {1} data {2} autor {3}", KlientId, Opis, Data, Autor);}

        public PunktyWpisy(long klient,decimal ilosc,DateTime data,string autor,string opis)
        {
            KlientId = klient;
            IloscPunktow = ilosc;
            Data = data;
            Autor = autor;
            Opis = opis;
        }
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        
        public string Opis { get; set; }
        public long KlientId { get; set; }
        public int? ZamowienieId { get; set; }

        
        public decimal IloscPunktow { get; set; }

       
        public DateTime Data { get; set; }

       
        public string Autor { get; set; }
    }
}
