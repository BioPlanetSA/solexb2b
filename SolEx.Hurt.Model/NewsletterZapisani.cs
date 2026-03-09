using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model
{
    public class NewsletterZapisani : IHasIntId
    {
        [AutoIncrement]
        [PrimaryKey]
        [WidoczneListaAdmin(true, false, false, false)]
        public int Id { get; set; }

        [FriendlyName("Data zapisania")]
        [WidoczneListaAdmin(true, true, false, false)]
        public virtual DateTime DataZapisania { get; set; }

        [FriendlyName("Data wypisania")]
        [WidoczneListaAdmin(true, true, false, false)]
        public DateTime? DataWypisania { get; set; }

        [FriendlyName("Adres IP")]
        [WidoczneListaAdmin(true, true, false, false)]
        public virtual string AdersIp { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        public string Email { get; set; }

    }
}
