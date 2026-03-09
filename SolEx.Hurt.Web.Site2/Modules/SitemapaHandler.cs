using System;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules
{
    /// <summary>
    /// Handeler generujący sitemapę
    /// </summary>
    public class SitemapaHandler : WebSiteBaseHandler
    {
        const string SITEMAP_ARTICLE_CATEGORY = @" <url>
        <loc>{0}</loc>
        <lastmod>{1}</lastmod>
        <changefreq>daily</changefreq>
        <priority>0.8</priority>
         </url>";
        const string SITEMAP_ARTICLE = @" <url>
        <loc>{0}</loc>
        <lastmod>{1}</lastmod>
        <changefreq>daily</changefreq>
        <priority>0.7</priority>
         </url>";
        const string SITEMAP_PRODUCTS = @" <url>
        <loc>{0}</loc>
        <lastmod>{1}</lastmod>
        <changefreq>daily</changefreq>
        <priority>0.5</priority>
         </url>";
        public override void HandleRequest(System.Web.HttpContext context)
        {
            throw new NotImplementedException();
         
          //  List<jezyki> langs =   JezykiDAL.Pobierz(new Model.CustomSearchCriteria.JezykiSearchCriteria());
          //string host = "http://" + context.Request.Url.Host   + (context.Request.Url.Port!=80?(":" + context.Request.Url.Port.ToString()):"");
          //StringBuilder sb = new StringBuilder(20000);

          //sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?><urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
          //sb.AppendFormat("<url><loc>{0}</loc><lastmod>{1}</lastmod><changefreq>daily</changefreq><priority>1.0</priority></url>", host, DateTime.Now.ToString("yyyy-MM-dd"));
          //foreach (jezyki l in langs)
          //{
          //    klienci c = CustomerDAO.GetCustomerByLogin(l.id);//klient niezalogowany dla wybranego języka
          //  List<Category> menuItems = ArticleDAO.GetArticleCategoriesTree2(null, c, true);
          //  foreach (Category child in menuItems)
          //  {
          //      sb.Append(BuildCategory(child,Config.GetLanguageSymbol(c.jezyk_id), host));
          //  }
          //  List<GrupyBLL> group =  CoreManager.GetCategoriesTree("produkt", 0 /*SelectedTabId*/, Customer.Klient.jezyk_id, Customer.Klient.pole_tekst1, "", Customer.Klient, 0,"");
          //  foreach (GrupyBLL g in group)
          //  {
          //    foreach (KategorieBLL cat in g.Kategorie.Where(p=>p.Widoczna))
          //    {
          //        sb.Append(BuildGroup(cat, Config.GetLanguageSymbol(c.jezyk_id), host));
          //    }
          //}
          //List<ProductBLL> items = ProductBLL.Pobierz(new Model.CustomSearchCriteria.ProduktySearchCriteria(), l.id);
          //foreach (Model.produkty child in items.Select(p=>p.produkty))
          //  {
          //      sb.AppendFormat(SITEMAP_PRODUCTS, host + "/" + LinkBuilder.GenerateLink(child, Config.GetLanguageSymbol(c.jezyk_id), AccesLevel.UnLoged), DateTime.Now.ToString("yyyy-MM-dd"));
          //  }
          //}
          //sb.Append("</urlset>");
          //string text = sb.ToString();
          //File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory+"\\sitemap.xml", text);
          //SendText(context, text, Tools.PobierzInstancje.GetMimeType(".xml"));
        }

        //private string BuildGroup(KategorieBLL c, string lang, string host)
        //{
        //    StringBuilder sb = new StringBuilder(20000);
        //    sb.AppendFormat(SITEMAP_ARTICLE_CATEGORY, host + "/" + LinkBuilder.StworzLinkDoObiektu(c, lang, AccesLevel.Niezalogowani), DateTime.Now.ToString("yyyy-MM-dd"));
        //    foreach (KategorieBLL child in c.KategoriePodrzedne.Where(p => p.Widoczna))
        //    {
        //    //    sb.Append(BuildCategory(child, lang, host));
        //    }
        //    return sb.ToString();
        //}
        //private string BuildCategory(Category c,string lang,string host)
        //{
        //    StringBuilder sb = new StringBuilder(20000);
        //    foreach (artykuly a in c.Articles)
        //    {
        //        sb.AppendFormat(SITEMAP_ARTICLE, host + "/" + LinkBuilder.StworzLinkDoObiektu(a, lang, AccesLevel.Niezalogowani), a.data_utworzenia.ToShortDateString());
        //    }
        //    sb.AppendFormat(SITEMAP_ARTICLE_CATEGORY, host + "/" + LinkBuilder.StworzLinkDoObiektu(c, lang, AccesLevel.Niezalogowani), DateTime.Now.ToString("yyyy-MM-dd"));
        //    foreach (Category child in c.SubItems)
        //    {
        //        sb.Append(BuildCategory(child, lang, host));
        //    }
        //    return sb.ToString();
        //}

    }
}
