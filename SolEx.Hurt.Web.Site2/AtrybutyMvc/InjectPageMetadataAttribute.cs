using System.Collections.Generic;
using System.Web.Mvc;

namespace SolEx.Hurt.Web.Site2.AtrybutyMvc
{
    public interface IPageMetadata
    {
   
        string PageTitle { get; }
        string MetaDescription { get; }
        string MetaKeywords { get; }
        string IdentyfikatorObiektu { get; }
    }

  
    public class InjectPageMetadataAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewResultBase viewResult = filterContext.Result as PartialViewResult;

            if (viewResult == null)
            {
                viewResult = filterContext.Result as ViewResult;
            }
            if (viewResult == null)
            {
                return;
            }
            ControllerBase controller = filterContext.Controller;

            while (controller.ControllerContext.IsChildAction)  //traverse hierachy to get root controller
            {
                controller = controller.ControllerContext.ParentActionViewContext.Controller;
            }
            var metadata = viewResult.Model as IPageMetadata;
           
            if (metadata != null)
            {
                HashSet<string> przerobione = controller.ViewBag.Przetworzone ?? new HashSet<string>();
                if (przerobione.Contains(metadata.IdentyfikatorObiektu))
                {
                    return;
                }
                przerobione.Add(metadata.IdentyfikatorObiektu);
                string title = controller.ViewBag.Title;
                string description = controller.ViewBag.MetaDescription;
                string keywords = controller.ViewBag.MetaKeywords;

                title = metadata.PageTitle + " " + title;
                description += " " + metadata.MetaDescription;
                keywords += " " + metadata.MetaKeywords;

                controller.ViewBag.Przetworzone = przerobione;
                controller.ViewBag.Title = title.Trim();
                controller.ViewBag.MetaDescription = description.Trim();
                controller.ViewBag.MetaKeywords = keywords.Trim();
            }

            base.OnActionExecuted(filterContext);
        }
    }
}