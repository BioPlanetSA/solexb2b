using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Factories
{
    public class CustomControllerFactory : IControllerFactory
    {
        private readonly string _controllerNamespace;
        public CustomControllerFactory(string controllerNamespace)
        {
            _controllerNamespace = controllerNamespace;
        }
        public IController CreateController(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            Type controllerType = null;
            try
            {
                controllerType = Type.GetType(string.Concat(_controllerNamespace, ".", controllerName, "Controller"), true, true);
            }catch(Exception ex)
            {
                throw new HttpException(404, "Not found");
            }
            
            SolexControler controller = Activator.CreateInstance(controllerType) as SolexControler;

            var solexHelper = SolexHelper.PobierzInstancjeZCache();

            controller.SolexHelper = solexHelper;

            return controller;
        }

        public void ReleaseController(IController controller)
        {
            IDisposable disposable = controller as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }

        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return SessionStateBehavior.Disabled;
        }
    }
}