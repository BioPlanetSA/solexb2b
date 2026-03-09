using System;
using System.Runtime.CompilerServices;
using System.Web;

namespace SolEx.Hurt.Helpers
{
    public interface ICookieHelper
    {
        void DeleteCookie(string symbol);

        /// <summary>
        /// Pobiera zawartość ciastecza
        /// </summary>
        /// <param name="name">Nazwa ciasteczka</param>
        /// <returns></returns>
        string GetCookieValue(string name);

        /// <summary>
        /// Pobiera zawartość ciastecza
        /// </summary>
        /// <param name="name">Nazwa ciasteczka</param>
        /// <returns></returns>
        bool GetCookieValueBool(string name);

        /// <summary>
        /// Ustawia ciasteczko
        /// </summary>
        /// <param name="name">Nazwa ciasteczka</param>
        /// <param name="value">Wartość ciasteczka</param>
        void SetCookie(string name, string value);

        void SetCookie(string name, int value);
    }

    /// <summary>
    /// Klasa pomocnicza do zarządzania ciasteczkami
    /// </summary>
    public class CookieHelper : ICookieHelper
    {
        private CookieHelper()
        {
        }

        private static readonly Lazy<CookieHelper> Lazy = new Lazy<CookieHelper>(() => new CookieHelper());

        public static CookieHelper PobierzInstancje
        {
            get { return Lazy.Value; }
        }
        
        public void DeleteCookie(string symbol)
        {
            var v = HttpContext.Current.Request.Cookies[symbol];
            if (v != null)
            {
                v.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Set(v);
            }
        }

        /// <summary>
        /// Pobiera zawartość ciastecza
        /// </summary>
        /// <param name="name">Nazwa ciasteczka</param>
        /// <returns></returns>
        public string GetCookieValue(string name)
        {
            try
            {
                HttpCookie v2 = HttpContext.Current.Request.Cookies[name];
                if (v2 != null && !string.IsNullOrEmpty(v2.Value)) // && v2.Expires > DateTime.Now
                {
                    try
                    {
                        return v2.Value;
                    }
                    catch
                    {
                        DeleteCookie(name);
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// Pobiera zawartość ciastecza
        /// </summary>
        /// <param name="name">Nazwa ciasteczka</param>
        /// <returns></returns>
        public bool GetCookieValueBool(string name)
        {
            HttpCookie v2 = HttpContext.Current.Request.Cookies[name];
            if (v2 != null)
            {
                return bool.Parse(v2.Value);
            }
            return false;
        }

        /// <summary>
        /// Ustawia ciasteczko
        /// </summary>
        /// <param name="name">Nazwa ciasteczka</param>
        /// <param name="value">Wartość ciasteczka</param>
        public void SetCookie(string name, string value)
        {
            try
            {
                var v = HttpContext.Current.Request.Cookies[name];

                if (v == null)
                {
                    v = new HttpCookie(name, value);
                    v.Expires = DateTime.Now.AddDays(90);
                    HttpContext.Current.Response.Cookies.Set(v);
                }
                else
                {
                    //   HttpContext.Current.Response.Cookies.Remove(name);
                    v.Value = value;
                    v.Expires = DateTime.Now.AddDays(90);
                    HttpContext.Current.Response.Cookies.Set(v);
                }
            }
            catch (Exception)
            {
            }
        }

        public void SetCookie(string name, int value)
        {
            SetCookie(name, value.ToString());
        }
    }

}
