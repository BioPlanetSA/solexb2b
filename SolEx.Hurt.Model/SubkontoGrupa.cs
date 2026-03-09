using System.Net.Sockets;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using System;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class SubkontoGrupa : IBindowalny,IHasIntId
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string Nazwa { get; set; }
        public long? SzablonLimitowId { get; set; }
        public long? SzablonAkceptacjiId { get; set; }
        public long KlientId { get; set; }
        public int? MiejsceKosztowId { get; set; }
        public long? SzablonAkceptacjiPrzekrocznyLimitId { get; set; }
        public SubkontoGrupa()
        {

        }

        public SubkontoGrupa(SubkontoGrupa baza) : this()
        {
            this.KopiujPola(baza);
        }
    }
}
