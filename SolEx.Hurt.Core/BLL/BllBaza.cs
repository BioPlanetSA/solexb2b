using log4net;
using System;

namespace SolEx.Hurt.Core.BLL
{
    public class BllBaza<T>
    where T : new()
    {
        public BllBaza()
        {
        }

        public ILog Log
        {
            get
            {
                return LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        private static readonly Lazy<T> Lazy = new Lazy<T>(() => new T());

        public static T PobierzInstancje
        {
            get { return Lazy.Value; }
        }
    }

    public class LogikaBiznesBaza
    {
        private ISolexBllCalosc calosc;

        public LogikaBiznesBaza(ISolexBllCalosc calosc)
        {
            this.calosc = calosc;
        }

        /// <summary>
        /// Udostepnia dostep do calosci logiki
        /// </summary>
        public ISolexBllCalosc Calosc
        {
            get { return calosc; }
        }
    }
}