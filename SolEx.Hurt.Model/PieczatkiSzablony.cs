using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;

namespace SolEx.Hurt.Model
{
    public class PieczatkiSzablony : IHasIntId
    {

        public PieczatkiSzablony()
        {
        }

        public PieczatkiSzablony(PieczatkiSzablony baza)
        {
            this.KopiujPola(baza);
        }

        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public int TypId { get; set; }
        public string Nazwa { get; set; }
        public string Opis { get; set; }
        public string SciezkaDoPlikuSzablonuJSON { get; set; }
        public string SciezkaDoPlikuSzablonuSVG { get; set; }
        public bool Zablokowany { get; set; }
    }
}
