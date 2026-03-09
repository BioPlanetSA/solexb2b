using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using System.Web.Http.ModelBinding;
using ServiceStack.Common;

namespace SolEx.Hurt.Web.Site2.Bindowania
{
    public class ArrayNumberBinder : HttpParameterBinding
    {
        HttpParameterBinding _defaultUriBinding;
        HttpParameterBinding _defaultFormatterBinding;
        public string _nazwaPropertisa;
        public Type _typListy;
        public ArrayNumberBinder(HttpParameterDescriptor desc) : base(desc)
        {
            _nazwaPropertisa = desc.ParameterName;
            _typListy = desc.ParameterType.GetElementType();
            _defaultUriBinding = new FromUriAttribute().GetBinding(desc);
            _defaultFormatterBinding = new FromBodyAttribute().GetBinding(desc);
        }
        /// <summary>
        /// Bind parametrów api, metoda do obslugi parametrów z querystringa.
        /// </summary>
        /// <param name="metadataProvider"></param>
        /// <param name="actionContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider, HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var wartosc = QueryStringValues(actionContext.Request);
            if (string.IsNullOrEmpty(wartosc))
            {
                //Jezeli nie ma odpalamy domyslny binder
                return _defaultUriBinding.ExecuteBindingAsync(metadataProvider, actionContext, cancellationToken);
            }
            if (_typListy == typeof(long))
            {
                long[] result = wartosc.Split(',').Select(long.Parse).ToArray();
                SetValue(actionContext, result);
            }
            else if(_typListy == typeof(int))
            {
                int[] result = wartosc.Split(',').Select(int.Parse).ToArray();
                SetValue(actionContext, result);
            }
            

            TaskCompletionSource<AsyncVoid> tcs = new TaskCompletionSource<AsyncVoid>();
            tcs.SetResult(default(AsyncVoid));
            return tcs.Task;
        }

        private string QueryStringValues(HttpRequestMessage request)
        {
            var queryString = string.Join(string.Empty, request.RequestUri.ToString().Split('?').Skip(1));
            NameValueCollection queryStringValues = HttpUtility.ParseQueryString(queryString);
            return queryStringValues[_nazwaPropertisa];
        }
        private struct AsyncVoid
        {
        }
    }

}