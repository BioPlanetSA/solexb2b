using System;

namespace SolEx.Hurt.Model.Enums
{
    public class StatusKoszykaHistoria
    {
        public long KlientId { get; set; }
        public Enums.StatusKoszyka Staus { get; set; }
        public DateTime Data { get; set; }
    }
}
