using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model
{
    public class Region : IHasIntId
    {
        public Region() { }
        public Region(int id, string nazwa, int kraj,bool widoczny=true)
        {
            Id = id;
            Nazwa = nazwa;
            KrajId = kraj;
            Widoczny = widoczny;
        }
       
        [PrimaryKey]
        [WidoczneListaAdmin(true, true, false, false)]
        public int Id { get; set; }

        [WidoczneListaAdmin(true, true, true, false)]
        public string Nazwa { get; set; }

        [WidoczneListaAdmin(true, true, true, false)]
        public bool Widoczny { get; set; }

        public int KrajId { get; set; }

        public bool Synchronizowane { get; set; }

        public override string ToString()
        {
            return string.Format("Id {0} Nazwa {1}", Id, Nazwa);
        }
    }
}
