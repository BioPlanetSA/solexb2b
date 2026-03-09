using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using log4net;
using Newtonsoft.Json;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Web;
using SolEx.Hurt.Model.Extensions;

namespace SolEx.Hurt.Web.Site2.Bindowania
{
    public class UstawieniaKoszykaBinder : DefaultModelBinder
    {
        protected ILog Log
        {
            get
            {
                return LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            object o = Activator.CreateInstance(bindingContext.ModelType);
            foreach (PropertyInfo p in bindingContext.ModelType.GetProperties())
            {
                ValueProviderResult w = bindingContext.ValueProvider.GetValue(p.Name);
                if (w == null || w.AttemptedValue == null)
                {
                    continue;
                }
                string st = w.AttemptedValue.Trim('"');
                var war = st.Parsuj(p.PropertyType);
                p.SetValue(o, war);

            }
            return o;
        }
    }
}