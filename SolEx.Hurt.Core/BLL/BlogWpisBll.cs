using System.Collections.Generic;
using ServiceStack.Common.Extensions;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System.Linq;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.DostepDane.DaneObiektu;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL
{
    [Alias("BlogWpis")]
     [FriendlyName("Wpis bloga", FriendlyOpis = "")]
    [EdytowalnyAdmin]
    public class BlogWpisBll : BlogWpis, IPoleJezyk
    {
        private HashSet<long> _kategorie;

        [Ignore]
        [Niewymagane]
        [FriendlyName("Kategorie Bloga")]
        [GrupaAtttribute("Ogólne", 1)]
        [WidoczneListaAdmin(true, true, true, false)]
        [PobieranieSlownika(typeof(SlownikKategoriiBloga))]
        public virtual HashSet<long> Kategorie { get; set; }

        [Ignore]
        [FriendlyName("Skrócona Data dodania ")]
        public string SkroconaDataDodania
        {
            get
            {
                return DataDodania.ToShortDateString();
            }
        }

        [Ignore]
        public IObrazek Zdjecie
        {
            get
            {
                if (ZdjecieId == null) return null;
                return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(ZdjecieId.Value);
            }
        }

        [FriendlyName("Blog Zdjęcie 1", true)]
        [Ignore]
        public IObrazek Zdjecie1
        {
            get
            {
                if (ZdjecieId1 == null) return null;
                return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(ZdjecieId1.Value);
            }
        }

        [FriendlyName("Blog Zdjęcie 2", true)]
        [Ignore]
        public IObrazek Zdjecie2
        {
            get
            {
                if (ZdjecieId2 == null) return null;
                return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(ZdjecieId2.Value);
            }
        }

        [FriendlyName("Blog Zdjęcie 3", true)]
        [Ignore]
        public IObrazek Zdjecie3
        {
            get
            {
                if (ZdjecieId3 == null) return null;
                return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(ZdjecieId3.Value);
            }
        }

        [FriendlyName("Blog Zdjęcie 4", true)]
        [Ignore]
        public IObrazek Zdjecie4
        {
            get
            {
                if (ZdjecieId4 == null) return null;
                return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(ZdjecieId4.Value);
            }
        }

        [FriendlyName("Blog Zdjęcie 5", true)]
        [Ignore]
        public IObrazek Zdjecie5
        {
            get
            {
                if (ZdjecieId5 == null) return null;
                return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(ZdjecieId5.Value);
            }
        }

        [Ignore]
        public IKlient Autor
        {
            get
            {
                if (AutorId == null) return null;
                return SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(AutorId);
            }
        }
        [Ignore]
        public int JezykId { get; set; }

        //autouzupelnienie
        [Ignore]
        public string LinkURL { get; set; }
    }
}