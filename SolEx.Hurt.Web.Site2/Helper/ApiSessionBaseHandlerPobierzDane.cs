using System;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Web.Site2.Helper
{
    public abstract class ApiSessionBaseHandlerPobierzDane : ApiSessionBaseHandler
    {
        private object _criteria = null;
        public new virtual object SearchCriteriaObject
        {
            get
            {
                string data = base.SearchCriteriaObject;
                if (!string.IsNullOrEmpty(data) && _criteria == null)
                {
                    _criteria = JSonHelper.Deserialize(data, SearchCriteriaType);
                }
                if (_criteria == null)
                {
                    _criteria = Activator.CreateInstance(SearchCriteriaType);
                }
                return _criteria;
            }
            set { _criteria = value; }
        }
        protected abstract Type SearchCriteriaType { get; }
    }
}
