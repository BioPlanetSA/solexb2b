using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Interfaces;
using System;
using SolEx.Hurt.Helpers;
using ServiceStack.OrmLite;
using ServiceStack.Common;

namespace SolEx.Hurt.Core.BLL
{
    public class BlogDostep : LogikaBiznesBaza, IBlogDostep
    {
        public BlogDostep(ISolexBllCalosc calosc) : base(calosc)
        {
        }

        public void AktualizujLacznikiKategorii(IList<BlogWpisBll> obj)
        {
            long idWpisu = 0;
            Dictionary<string, BlogWpisBlogKategoria> doAktualizacji = new Dictionary<string, BlogWpisBlogKategoria>();
            foreach (BlogWpisBll blogWpis in obj)
            {
                foreach (long kat in blogWpis.Kategorie)
                {
                    var newLacznik = new BlogWpisBlogKategoria(blogWpis.Id, (int)kat);
                    if (!doAktualizacji.ContainsKey(newLacznik.Id))
                    {
                        doAktualizacji.Add(newLacznik.Id, newLacznik);
                    }
                    idWpisu = blogWpis.Id;
                }
            }

            var listaLacznikowDoUsuniecia = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<BlogWpisBlogKategoria>(null, x => x.BlogWpisId == idWpisu);
            foreach (var blogWpisBlogKategoria in listaLacznikowDoUsuniecia)
            {
                SolexBllCalosc.PobierzInstancje.DostepDane.UsunPojedynczy<BlogWpisBlogKategoria>(blogWpisBlogKategoria.Id);
            }
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<BlogWpisBlogKategoria>(doAktualizacji.Values.ToList());
        }

        public IList<BlogWpisBll> BindingPoSelect(int jezykId, IKlient zadajacyKlient, IList<BlogWpisBll> listaWpisowZBazy, object parametryDoMetodyPoSelect)
        {
            var ids = listaWpisowZBazy.Select(x => x.Id);
            var wszystkieKategorieBlogow = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<BlogWpisBlogKategoria>(zadajacyKlient, x => Sql.In(x.BlogWpisId, ids)).GroupBy(x => x.BlogWpisId).ToDictionary(x => x.Key, x => new HashSet<long>(x.Select(y => y.BlogKategoriaId)) );

            foreach (var b in listaWpisowZBazy)
            {
                if (b.Aktywny)
                {
                    b.LinkURL = Tools.OczyscCiagDoLinkuURL(b.Tytul);
                }

                if ( wszystkieKategorieBlogow.TryGetValue(b.Id, out HashSet<long> kategorie))
                {
                    b.Kategorie = kategorie;
                }
            }
            return listaWpisowZBazy;
        }
    }
}