using System;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class HistoriaDokumentuListPrzewozowy :  IHasIntId
    {
     
       public HistoriaDokumentuListPrzewozowy()
        {
        }
        public HistoriaDokumentuListPrzewozowy(int dokumentid,string numerlistu,string link)
        {
            DokumentId = dokumentid;
            NumerListu = numerlistu;
            this.Link = link;
        }

        public HistoriaDokumentuListPrzewozowy(HistoriaDokumentuListPrzewozowy c)
        {
            DokumentId = c.DokumentId;
            NumerListu = c.NumerListu;
            Link = c.Link;
        }

        //todo: id longowe zmienic
        [PrimaryKey]
        public int Id { get { return (DokumentId + "|" + NumerListu).WygenerujIDObiektuSHA(1); } } 

        public int DokumentId { get; set; }

        [FriendlyName("Numer listu")]
        public virtual string NumerListu { get; set; }
        [FriendlyName("Link do trackingu")]
        public virtual string Link { get; set; }

        public object PolaDoKlucza()
        {
            return new { DokumentId, NumerListu };
        }
    }
}
