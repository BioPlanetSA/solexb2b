using System;
using System.Collections;
using System.Collections.Generic;
using log4net;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Helper
{
    public abstract class SlownikBazowy : ISlownik, IEnumerable
    {
        private  ILog _log
        {
            get
            {
                return LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
        public Dictionary<string, object> PobierzWartosci()
        {
            Dictionary<string, object> slownik; //SolexBllCalosc.PobierzInstancje.Cache.PobierzObiekt<Dictionary<string, object>>(klucz);
                try
                {
                    slownik = WygenerujSlownik;
                }
                catch (Exception ex)
                {
                    _log.Debug("pobiernie słownika " + GetType().FullName, ex);
                    throw;
                    //return new Dictionary<string, object>();
                }
            return slownik;
        }

        protected abstract Dictionary<string, object> WygenerujSlownik { get; }

        public IEnumerator GetEnumerator()
        {
            return PobierzWartosci().GetEnumerator();
        }
    }
}
