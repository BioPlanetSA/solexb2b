using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Helper
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString MenuAdmin(this HtmlHelper helper, IKlient klient)
        {
            IList<DaneListaAdmin.ElementMenu> menu = AdminHelper.PobierzInstancje.PobierzMenu(klient).ToArray();
            return helper.Partial("Menu", menu);
        }

        public static SolexHelper SolexHelper(this HtmlHelper helper)
        {
            var SolexControler = (helper.ViewContext.Controller as SolexControler);
            if (SolexControler != null)
            {
                return SolexControler.SolexHelper;
            }

            return Helper.SolexHelper.PobierzInstancjeZCache();
        }

        public static MvcHtmlString ZmianaJezyka(this HtmlHelper helper)
        {
            //czy sa jezyki
            if (SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykiWSystemie.Count <= 1)
            {
                return null;
            }

            //czy klient moze zmienic jezyk
            if (helper.SolexHelper().AktualnyKlient.CzyAdministrator || (SolexBllCalosc.PobierzInstancje.Konfiguracja.KlientMozeZmianiacJezyk))
            {
                if (SolexBllCalosc.PobierzInstancje.Konfiguracja.KlientMozeZmianiacJezyk)
                    return helper.Partial("ListaJezykow");
            }

            return null;
        }

        public static MvcHtmlString Image(this HtmlHelper helper, string url, string preset, string altText, object htmlAttributes, string id = null)
        {
            TagBuilder builder = new TagBuilder("img");
            builder.Attributes.Add("src", url + "?preset=" + preset);
            builder.Attributes.Add("alt", altText);
            if (id != null)
            {
                builder.GenerateId(id);
            }
            if (htmlAttributes != null)
            {
                if (htmlAttributes.GetType() == typeof(Dictionary<string, string>))
                {
                    builder.MergeAttributes((IDictionary<string, string>)(htmlAttributes));
                }
                else
                {
                    builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
                }
            }
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString Image(this HtmlHelper helper, IObrazek obrazek, string preset, string altText, object htmlAttributes = null)
        {
            return Image(helper, obrazek.LinkOryginal, preset, altText, htmlAttributes, "zdj" + obrazek.Id);
        }

        /// <summary>
        /// Metoda zwracająca Html.Raw-a przetłumaczonej frazy
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="jezyk">jezyk tłumaczenia</param>
        /// <param name="fraza">Fraza do tłumaczenia</param>
        /// <param name="miejsce">Lokalizacja frazy</param>
        /// <param name="parametry">dodatkowe parametry</param>
        /// <returns></returns>
        public static IHtmlString PobierzTlumaczenie(this HtmlHelper helper, int jezyk, string fraza, MiejsceFrazy miejsce = MiejsceFrazy.Brak, params object[] parametry)
        {
            long hashMd5;
            string tlumaczenie = SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(jezyk, fraza, fraza, out hashMd5, miejsce, parametry);

            if (helper.SolexHelper().TlumaczenieWLocie)
            {
                return
                    helper.Raw(
                        string.Format(
                            "<span class='tlumaczenie-w-locie' data-hash='{1}'>{0}<i class='hover-box fa  fa-asterisk  fa-2x'>   </i></span>",
                            tlumaczenie, hashMd5));
            }
            return helper.Raw(tlumaczenie);
        }

        public static IHtmlString PobierzTlumaczenie(this HtmlHelper helper, string fraza, params object[] parametry)
        {
            return helper.Raw(PobierzTlumaczenie(helper, (helper.ViewContext.Controller as SolexControler).SolexHelper.AktualnyJezyk.Id, fraza,MiejsceFrazy.Brak, parametry));
        }


        public static void InfoODostepnosci(this HtmlHelper helper, IProduktKlienta produkt)
        {
            helper.RenderPartial("~/Views/Shared/_InfoDostepnosc.cshtml", produkt.Id);
        }

        public static MvcHtmlString RenderujTuProduktyNewslettera(this HtmlHelper helper)
        {
            try
            {
                SzablonMailaNewslettera model = helper.ViewData.Model as SzablonMailaNewslettera;
                return helper.Partial(model.SzablonListyProduktow, model);
            }
            catch (Exception )
            {
                throw;
            }
        }

        public static MvcHtmlString NazwaDnia(this HtmlHelper helper, DayOfWeek dzien)
        {
            return new MvcHtmlString(DateTimeFormatInfo.CurrentInfo.GetDayName(dzien));
        }

        /// <summary>
        /// Renderowanie stanów
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="produkt"></param>
        /// <param name="pozycjaLista"></param>
        /// <param name="wszystkieSposobyKlienta"></param>
        public static void StanyProduktowWszystkie(this HtmlHelper helper, IProduktKlienta produkt, Dictionary<long, SposobPokazywaniaStanow> wszystkieSposobyKlienta)
        {
            var stanyNaMagazynie = StanyProduktu(helper, produkt, wszystkieSposobyKlienta);
            if (stanyNaMagazynie != null && stanyNaMagazynie.Any())
            {
                helper.StanyDlaMagaznow(stanyNaMagazynie,produkt);
            }
        }

        public static void StanyDlaMagaznow(this HtmlHelper helper, Dictionary<int, List<StanNaMagazynie>> slownikStanow, IProduktKlienta produkt)
        {
            foreach (var s in slownikStanow)
            {
                string widok = $"Stany/{s.Key}_{helper.SolexHelper().AktualnyJezyk.Symbol}";
                foreach (var stan in s.Value)
                {
                    helper.RenderPartial(widok, new ParametryStany(produkt, stan));
                }
            }
        }

        /// <summary>
        /// Extension odpowiedzialny za zwrócenie spełnionych reguł dla konkretnej pozycji
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="produkt"></param>
        /// <param name="wszystkieSposobyKlienta"></param>
        /// <param name="pozycjaLista"></param>
        /// <returns></returns>
        public static Dictionary<int, List<StanNaMagazynie>> StanyProduktu(this HtmlHelper helper, IProduktKlienta produkt,Dictionary<long, SposobPokazywaniaStanow> wszystkieSposobyKlienta)
        {
            Dictionary<int, List<StanNaMagazynie>> reguly = new Dictionary<int, List<StanNaMagazynie>>();
            if (wszystkieSposobyKlienta != null && wszystkieSposobyKlienta.Any())
            {
                foreach (var sposobPokazywaniaStanow in wszystkieSposobyKlienta)
                {
                    var stany = SolexBllCalosc.PobierzInstancje.SposobyPokazywaniaStanowBll.PobierzStanProduktuWgSposobu(sposobPokazywaniaStanow.Value, produkt, produkt.JezykId, produkt.Klient);
                    foreach (var s in stany)
                    {
                        if (reguly.ContainsKey(s.Key))
                        {
                            reguly[s.Key].AddRange(s.Value);
                        }
                        else
                        {
                            reguly.Add(s.Key, s.Value);
                        }
                    }
                }

                //if (pozycjaLista != null && !wszystkieSposobyKlienta.ContainsKey(pozycjaLista.Value))
                //{
                //    return null;
                //}
                //foreach (var w in wszystkieSposobyKlienta)
                //{
                //    //if (pozycjaLista != null && !w.Key.Equals(pozycjaLista.Value))
                //    //{
                //    //    continue;
                //    //}

                //}
            }

            return reguly;
        }

        public static MvcHtmlString ActionLinkWithList(this HtmlHelper helper, string text, string action, string controller, object routeData, object htmlAttributes)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            string href = urlHelper.ActionLinkWithList(action, controller, routeData);

            TagBuilder builder = new TagBuilder("a");
            builder.Attributes.Add("href", href);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            builder.SetInnerText(text);
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }

        private static object GetModelStateValue(this HtmlHelper htmlHelper, string key, Type destinationType)
        {
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
            {
                if (modelState.Value != null)
                {
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
                }
            }
            return null;
        }

        public static MvcHtmlString SimpleLink(this HtmlHelper htmlHelper, string url, string text, object htmlAttributes)
        {
            return SimpleLink(htmlHelper, url, text, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString SimpleLink(this HtmlHelper htmlHelper, string url, string text, IDictionary<string, object> htmlAttributes)
        {
            TagBuilder tagBuilder = new TagBuilder("a")
            {
                InnerHtml = text
            };
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("href", url, true);

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString ExtendedDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<ExtendedSelectListItem> selectList, string optionLabel, object htmlAttributes, bool multiple = false)
        {
            return SelectInternal(htmlHelper, optionLabel, ExpressionHelper.GetExpressionText(expression), selectList, multiple /* allowMultiple */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ExtendedDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<ExtendedSelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes, bool multiple = false)
        {
            return SelectInternal(htmlHelper, optionLabel, ExpressionHelper.GetExpressionText(expression), selectList, multiple /* allowMultiple */, htmlAttributes);
        }

        private static MvcHtmlString SelectInternal(this HtmlHelper htmlHelper, string optionLabel, string name, IEnumerable<ExtendedSelectListItem> selectList, bool allowMultiple, IDictionary<string, object> htmlAttributes)
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName))
                throw new ArgumentException("No name");

            if (selectList == null)
                throw new ArgumentException("No selectlist");

            object defaultValue = (allowMultiple) ? htmlHelper.GetModelStateValue(fullName, typeof(string[])) : htmlHelper.GetModelStateValue(fullName, typeof(string));

            // If we haven't already used ViewData to get the entire list of items then we need to
            // use the ViewData-supplied value before using the parameter-supplied value.
            if (defaultValue == null)
                defaultValue = htmlHelper.ViewData.Eval(fullName);

            if (defaultValue != null)
            {
                IEnumerable defaultValues = (allowMultiple) ? defaultValue as IEnumerable : new[] { defaultValue };
                IEnumerable<string> values = from object value in defaultValues select Convert.ToString(value, CultureInfo.CurrentCulture);
                HashSet<string> selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
                List<ExtendedSelectListItem> newSelectList = new List<ExtendedSelectListItem>();

                foreach (ExtendedSelectListItem item in selectList)
                {
                    item.Selected = (item.Value != null) ? selectedValues.Contains(item.Value) : selectedValues.Contains(item.Text);
                    newSelectList.Add(item);
                }
                selectList = newSelectList;
            }

            // Convert each ListItem to an <option> tag
            StringBuilder listItemBuilder = new StringBuilder();

            // Make optionLabel the first item that gets rendered.
            if (optionLabel != null)
                listItemBuilder.Append(ListItemToOption(new ExtendedSelectListItem() { Text = optionLabel, Value = String.Empty, Selected = false }));

            foreach (ExtendedSelectListItem item in selectList)
            {
                listItemBuilder.Append(ListItemToOption(item));
            }

            TagBuilder tagBuilder = new TagBuilder("select")
            {
                InnerHtml = listItemBuilder.ToString()
            };
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("name", fullName, true /* replaceExisting */);
            tagBuilder.GenerateId(fullName);
            if (allowMultiple)
                tagBuilder.MergeAttribute("multiple", "multiple");

            // If there are any errors for a named field, we add the css attribute.
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name));

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }

        internal static string ListItemToOption(ExtendedSelectListItem item)
        {
            TagBuilder builder = new TagBuilder("option")
            {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };
            if (item.Value != null)
            {
                builder.Attributes["value"] = item.Value;
            }
            if (item.Selected)
            {
                builder.Attributes["selected"] = "selected";
            }
            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(item.htmlAttributes));
            return builder.ToString(TagRenderMode.Normal);
        }
        


    }
}