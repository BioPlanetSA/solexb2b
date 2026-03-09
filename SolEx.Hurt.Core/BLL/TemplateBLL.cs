using log4net;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using System;
using System.IO;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Core.BLL
{
    public class TemplateBLL
    {
        private static ILog Log
        {
            get
            {
                return LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        public static void WczytajDomyslneSzablony()
        {
            string sciezkaDomyslnegoKatalogu = PobierzKatalogSzablonow();
            if (!Directory.Exists(sciezkaDomyslnegoKatalogu))
            {
                Directory.CreateDirectory(sciezkaDomyslnegoKatalogu);
                return;
            }

            string[] szablony = Directory.GetFiles(sciezkaDomyslnegoKatalogu, "*.json", SearchOption.AllDirectories);
            foreach (string s in szablony)
            {
                try
                {
                    var plik = File.ReadAllText(s);
                    SzablonyEdytorow nowe = JSonHelper.Deserialize<SzablonyEdytorow>(plik);
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(nowe);
                }
                catch (Exception ex)
                {
                    Log.Error("Nie udało się wczytać szablonu newslettera: " + s);
                    Log.Error(ex);
                }
            }
        }

        private static string PobierzKatalogSzablonow()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DomyslnaKonfiguracja", "SzablonyNewsletterow");
        }
    }
}