using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using System;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class ProduktUkryty : IHasLongId
    {

        public ProduktUkryty()
        {
            Synchronizowane = false;
        }
        public ProduktUkryty(ProduktUkryty baza)
        {
          this.KopiujPola(baza);
        }


        [PrimaryKey]     
        public long Id { get; set; }
        [UpdateColumnKey]
        public long? KlientZrodloId { get; set; }

        [UpdateColumnKey]
        public long? ProduktZrodloId { get; set; }
        [UpdateColumnKey]
        public long? KategoriaId { get; set; }
        public KatalogKlientaTypy Tryb { get; set; }
        public int? PrzedstawicielId { get; set; }
        public int? KategoriaKlientowId { get; set; }
        [UpdateColumnKey]
        public long? CechaProduktuId { get; set; }

        public bool Synchronizowane { get; set; }
    }
}

