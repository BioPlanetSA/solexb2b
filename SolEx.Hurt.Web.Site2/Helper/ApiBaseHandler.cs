using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Web.Site2.Helper
{
      
    public abstract class ApiSessionBaseHandler : WebSiteBaseHandler
    {
        protected   ILog log => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected abstract object Handle();

        public virtual bool WymagajAktywnejSesji => true;

        private Klient _klient;
        public virtual Klient Customer
        {
            get
            {
                if (_klient == null)
                {
                    if (!string.IsNullOrEmpty(KluczSesji))
                    {
                        _klient = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(x => x.KluczSesji == KluczSesji, null);
                    }

                    if (_klient == null && SesjaHelper.PobierzInstancje.KlientID != 0)
                    {
                        _klient = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(x => x.Id == SesjaHelper.PobierzInstancje.KlientID, null);
                        if (_klient == null && !string.IsNullOrEmpty(KluczSesji))
                        {
                            throw new Exception("Błąd logowania - błędny klucz");
                        }
                    }

                    else if (_klient != null && SesjaHelper.PobierzInstancje.KlientID != 0 && _klient.Id != SesjaHelper.PobierzInstancje.KlientID)
                    {
                        throw new Exception("Błąd logowania - klucz niezgodny z zalogowanym klientem");
                    }
                    else if (_klient != null && !_klient.Aktywny)
                    {
                        throw new Exception("Błąd logowania - konto jest nieaktywne");
                    }
                }
                return _klient;
            }
        }

        public void Wykonaj(System.Web.HttpContext context)
        {
            Status s = null;
            if (Customer == null || Customer.Dostep == AccesLevel.Niezalogowani)
            {
                s = Status.Utworz(StatusApi.BladLogowania);
            }

            if (s == null && WymagajAktywnejSesji && Customer != null && !CheckPermissions())
            {
                s = Status.Utworz(StatusApi.BrakUprawnien);
            }

            if (s == null)
            {
                try
                {
                    //zanim wejdziemy w watek tzeba mu dane sciagnc z requestu bo watek nie widzi tego
                    s = new Status();
                    s.Data = Data;
                    s.Data = SearchCriteriaObject;
                    s = null;
                    bool keepAlive = context.Request.QueryString["keepalive"] != null &&
                                     context.Request.QueryString["keepalive"] == "1";

                    Thread watekObliczenia = new Thread(delegate()
                    {
                        object zwrocil = Handle();
                        if (zwrocil is Status)
                        {
                            s = zwrocil as Status;
                        }
                        else
                        {
                            s = Status.Utworz(StatusApi.Ok);
                            s.Data = zwrocil;
                        }
                    });

                    if (keepAlive)
                    {
                        watekObliczenia.Start();
                    }
                    else
                    {
                        object zwrocil = Handle();
                        if (zwrocil is Status)
                        {
                            s = zwrocil as Status;
                        }
                        else
                        {
                            s = Status.Utworz(StatusApi.Ok);
                            s.Data = zwrocil;
                        }
                    }

                    int licznikKomunikatuWait = 0;
                    while (watekObliczenia.IsAlive)
                    {
                        Thread.Sleep(1000);
                        if (keepAlive)
                        {
                            if (++licznikKomunikatuWait > 10)
                            {
                                context.Response.Write("wait" + Environment.NewLine);
                                context.Response.Flush();
                                licznikKomunikatuWait = 0;
                            }
                        }
                    }
                }
                catch (APIException e)
                {
                    log.Error(e);
                    s = Status.Utworz(StatusApi.BladOgolny, e.PrzyjemnaTrescBledu);
                }
                catch (OutOfMemoryException )
                {
                    throw;
                }
                catch (Exception e)
                {
                    log.Error(e);
                    s = Status.Utworz(StatusApi.BladOgolny, "Błąd API - zobacz logi");
                }
            }
            if (ZwracaWynik)
            {
                if (context.Request.QueryString["xml"] == null || context.Request.QueryString["xml"] != "1")
                {
                    SendJson(context, s);
                }
                else
                {
                    bool minimalizowac = context.Request.QueryString["optymalizacja"] != null && context.Request.QueryString["optymalizacja"] == "1";
                    SendXML(context, s, minimalizowac);
                }
            }
        }
        public override void HandleRequest(System.Web.HttpContext context)
        {
           Wykonaj(context);
        }

        protected virtual bool ZwracaWynik
        {
            get { return true; }
        }

        private bool CheckPermissions()
        {
            foreach (RoleType rt in DopuszczalneRole)
            {
                if (Customer.Role.Any(p => p == rt))
                {
                    return true;
                }
            }
            return false;
        }

        private string KluczSesji
        {
            get
            {
                if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Headers["X-Solex-API"]))
                {
                    return System.Web.HttpContext.Current.Request.Headers["X-Solex-API"];
                }
                return System.Web.HttpContext.Current.Request["key"];
            }
        }
        public RoleType[] DopuszczalneRole
        {
            get
            {
                ApiUprawnioneRoleAttribute atrybutUprawnien = GetType().GetCustomAttribute<ApiUprawnioneRoleAttribute>( true);

                if (atrybutUprawnien == null)
                {
                    return new [] { RoleType.Administrator };
                }

                return atrybutUprawnien.role;
            }
        }

        private object _data = null;
        protected override bool Resusable
        {
            get
            {
                return false;
            }
        }

        private string _criteria = null;
        protected string SearchCriteriaObject
        {
            get
            {
                if (_criteria == null)
                {
                    try
                    {
                        _criteria = System.Web.HttpContext.Current.Request["criteria"];
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        return "";
                    }
                }
                return _criteria;
            }
        }


        public object Data
        {
            get
            {
                if (PrzyjmowanyTyp == null)
                {
                    return null;
                }

                if (_data == null)
                {
                    string data = System.Web.HttpContext.Current.Request["data"];
                    if (!string.IsNullOrEmpty(data))
                    {
                        try
                        {
                            _data = JSonHelper.Deserialize(data, PrzyjmowanyTyp);
                        }
                        catch { }
                    }
                }
                return _data;
            }
        }

        public virtual Type PrzyjmowanyTyp { get { return null; } }
    }
}
