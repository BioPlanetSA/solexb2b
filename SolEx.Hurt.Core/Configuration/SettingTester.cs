using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Core.Configuration
{
    public static class SettingTester
    {
        public static List<SettingTestResult> Test()
        {
            List<SettingTestResult> result = new List<SettingTestResult>
            {
                TestEmailSend(TypyUstawieniaSkrzynek.Ogolne),
                TestEmailSend(TypyUstawieniaSkrzynek.Newsletter)
            };
            return result;
        }

        private static SettingTestResult TestEmailSend(TypyUstawieniaSkrzynek typ)
        {
            string DoKogo = "solex.test.maila@solex.net.pl";

            SettingTestResult result = new SettingTestResult
            {
                Module = "Wysyłanie wiadomości testowej ze skrzynki: " + typ + $" na adres email: {DoKogo}",
                Result = true
            };
            try
            {
                Exception ex;
                var wiadomoscTestowa = new Model.WiadomoscEmail
                {
                    Tytul = "test maila B2B",
                    TrescWiadomosci = "test maila B2B",
                    DoKogo = "solex.test.maila@solex.net.pl"
                };

                if (!SolexBllCalosc.PobierzInstancje.MaileBLL.WyslijEmaila(wiadomoscTestowa, null, typ, out ex))
                {
                    result.Result = false;
                    result.Error = ex != null ? ex.Message : "Bład sprawdź dziennik zdarzeń";
                    result.StackTrace = ex != null ? ex.StackTrace : "";
                }
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
                result.StackTrace = ex.StackTrace;
                result.Result = false;
            }

            return result;
        }
    }
    public class SettingTestResult
    {
        public string Module{get;set;}
        public string Error{get;set;}
        public string StackTrace { get; set; }
        public bool Result{get;set;}
    }
}
