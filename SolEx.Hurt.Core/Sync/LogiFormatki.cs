using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Sync
{
    public interface ILogiFormatki
    {
        void LogujInfo(string komunikat, params object[] format);
        void LogujDebug(string komunikat);

        /// <summary>
        /// Loguje w debug komunikaty ale tylko jeśli projekt jest skompilowany ze zmienną TESTLOG (jest to zrobione w konfiguracji TEST czyli dodając jakieś śmieciowe logi zmieniamy konfigurację
        /// z debug na test
        /// </summary>
        /// <param name="komunikat"></param>
        void LogujDebugTestowy(string komunikat);

        void LogujError(Exception e);
        void WyslijMailaZBledami(string email, string tekstBledow = null);
        void InicjalizujLogi(ref RichTextBox r);
    }

    public class LogiFormatki : ILogiFormatki
    {
        private StringBuilder bledyDoWyslania=new StringBuilder();
        private static LogiFormatki _logiformatki = new LogiFormatki();

        public static LogiFormatki PobierzInstancje
        {
            get { return _logiformatki; }
        }

        private  RichTextBox _logText;
        private  log4net.ILog _log
        {
            get
            {
                return log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        public object lok = new object();

        private void WyswietlKomunikat(string komunikat)
        {           
            try
            {
                lock (lok)
                {
                    _logText.Text += komunikat + Environment.NewLine;

                    _logText.SelectionStart = _logText.Text.Length;
                    _logText.ScrollToCaret();
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                _log.Error("Błąd wyświetlania logu",ex);
                if (ex.InnerException != null)
                {
                    _log.Error(ex.InnerException);
                }
          
            }
        }
      
        public virtual void LogujInfo(string komunikat, params object[] format)
        {
            if (format != null && format.Any())
            {
                komunikat = string.Format(komunikat, format);
            }
            
            WyswietlKomunikat(komunikat);
            _log.Info(komunikat);
        }

        public  void LogujDebug(string komunikat)
        {
            _log.Debug(komunikat);
        }

        public virtual void LogujDebug(string komunikat, params object[] format)
        {
            if (format != null && format.Any())
            {
                komunikat = string.Format(komunikat, format);
            }

            komunikat = string.Format(komunikat, format);
            _log.Debug(komunikat);
        }


        /// <summary>
        /// Loguje w debug komunikaty ale tylko jeśli projekt jest skompilowany ze zmienną TESTLOG (jest to zrobione w konfiguracji TEST czyli dodając jakieś śmieciowe logi zmieniamy konfigurację
        /// z debug na test
        /// </summary>
        /// <param name="komunikat"></param>
        public void LogujDebugTestowy(string komunikat)
        {
#if(TESTLOG)
            _log.Debug(komunikat);
#endif
        }

        public void LogujError(Exception e)
        {
            string msg = string.Format("Błąd: {0}{1}", e.Message, Environment.NewLine);

            _log.Error(e);
            if (e.InnerException != null)
            {
                _log.Error(e.InnerException);
                msg += string.Format("Błąd: {0}{1}", e.InnerException.Message, Environment.NewLine);
            }
            bledyDoWyslania.AppendLine(msg);
            WyswietlKomunikat(msg);
         
        }

        public void WyslijMailaZBledami(string email, string wiadomosc = null)
        {
            if (bledyDoWyslania.Length > 0 ||!string.IsNullOrEmpty(wiadomosc))
            {
                WiadomoscEmail mail=new WiadomoscEmail();
                mail.Tytul = "Błędy synchronizacji B2B - " + APIWywolania.PobierzInstancje.URL;
                string szablonMaila = "Nastąpiły błędy podczas synchronizacji Platformy B2B pod adresem: {0}. <br/>" +
                                      "Poniżej znajdzisz listę najważniejszych problemów - być może są inne.  <br/>" +
                                      "Cała lista problemów jest dostępna w pliku log.txt w katalogu z synchronizatorem.  <br/> <br/>" +
                                      "{1}";
                mail.TrescWiadomosci = string.Format(szablonMaila, APIWywolania.PobierzInstancje.URL, bledyDoWyslania + " <br/>" + wiadomosc);
                mail.WyslijJakoHTML = true;
                mail.DodajDoKogo(email);
                APIWywolania.PobierzInstancje.WyslijMaileBladSynchronizatora(mail);
            }

            bledyDoWyslania.Clear();
        }
        public  void InicjalizujLogi(ref RichTextBox r)
        {
            _logText = r;
        }
    }
}
