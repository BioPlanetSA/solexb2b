using System;
using System.Reflection;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ServiceStack.DataAnnotations;

namespace SolEx.Hurt.Model.Helpers
{
    public class BezIngorwanychResolver: DefaultContractResolver
    {

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {

            JsonProperty property = base.CreateProperty(member, memberSerialization);

            property.ShouldSerialize = x=>member.GetCustomAttribute<IgnoreAttribute>()==null;
            return property;
        }
    }
    public class JSonHelper
    {
        private static  ILog log
        {
            get
            {
                return log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        /// <summary>
        /// Konwertuje dane do Jsona
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Serialize(object data)
        {

            var wynik = JsonConvert.SerializeObject(data, Settings());
            return wynik;
        }

        /// <summary>
        /// Konwertuje dane do Jsona
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SerializeBezIgnorowanych(object data)
        {
            var setting = Settings();
            setting.ContractResolver = new BezIngorwanychResolver();
            var wynik = JsonConvert.SerializeObject(data, setting);
            return wynik;
        }

        private static JsonSerializerSettings Settings()
        {
               JsonSerializerSettings sett=new JsonSerializerSettings();
                sett.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
                sett.NullValueHandling = NullValueHandling.Ignore;
            sett.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
         
            return sett;
        }
        public static object Deserialize(string data, Type type)
        {
            try
            {
                    return JsonConvert.DeserializeObject(data, type, Settings());
            }
            catch (Exception)
            {
                log.Error(data);
                throw;
            }
        }
        public static T Deserialize<T>(string data)
        {
            //    data=FixDate(data);
            try
            {
                    Type type = typeof (T);
                    return (T)JsonConvert.DeserializeObject(data, type, Settings());
            }
            catch (Exception e)
            {
                log.Error(data);
                throw e;
            }
        }
        public static object Deserialize(string data)
        {
            //    data=FixDate(data);
            try
            {
                return JsonConvert.DeserializeObject(data, Settings());
            }
            catch (Exception e)
            {
                log.Error(data);
                throw e;
            }
        }

        private static string FixDate(string data)
        {
            string start="\\/Date(";
            string end=")\\/";
            int idxs = -1;
            int idxe = 0;
            idxs = data.IndexOf(start);
            if (idxs > -1)
            {
                idxe = data.IndexOf(end,idxs+1);
            }
            while (idxs > -1)
            {
                string sub = data.Substring(idxs, idxe - idxs);
                data= data.Replace(sub,sub.Replace(" ","+"));

                idxs = data.IndexOf(start,idxe);
                if (idxs > -1)
                {
                    idxe = data.IndexOf(end, idxs + 1);
                }
            }
            return data;
        }



        public static System.Xml.XmlDocument DeserializeXML(string respJson, string wezel)
        {
            return JsonConvert.DeserializeXmlNode(respJson, wezel);
        }
    }
}
