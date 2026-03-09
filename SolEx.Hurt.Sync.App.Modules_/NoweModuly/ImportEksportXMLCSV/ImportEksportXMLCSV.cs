using System.Reflection;
using System.Text;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Web;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.ImportEksportXMLCSV
{
    public class ImportEksportXMLCSV : SyncModul, Model.Interfaces.SyncModuly.IModulEksportImportXMLCSV
    {
        public override string Opis
        {
            get { return "Wysyła podany plik przez API i zapisuje do podanego pliku to co API zwróci."; }
        }

        [FriendlyName("ID szablonu do importu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public virtual string IDszablonu { get; set; }

        [FriendlyName("Ścieżka lokalna do importu")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public virtual string SciezkaLokalnaImport { get; set; }

        [FriendlyName("Ścieżka lokalna do eksportu")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string SciezkaLokalnaEksport { get; set; }

        [FriendlyName("Wiele plików wg maski - cała ścieżka np. d:\\twojekatalogi\\*.csv . Szuka również w podkatalogach")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string WielePlikowWgMaski { get; set; }

        public ImportEksportXMLCSV()
        {
            IDszablonu = "";
            SciezkaLokalnaEksport = "";
            SciezkaLokalnaImport = "";
            WielePlikowWgMaski = "";
        }

        public override string uwagi
        {
            get { return ""; }
        }

        public void Przetworz()
        {
            if (string.IsNullOrEmpty(IDszablonu))
            {
                Log.Error("Nie podano ID szablonu!");
                return;
            }

            if (!string.IsNullOrEmpty(WielePlikowWgMaski))
            {
                int indexSlash = WielePlikowWgMaski.LastIndexOf('\\');
                string maskaPliku = WielePlikowWgMaski.Substring(indexSlash + 1);
                string katalog = WielePlikowWgMaski.Substring(0, indexSlash);
                string[] listaPlikow = Directory.GetFiles(katalog, maskaPliku, SearchOption.AllDirectories);

                foreach (string plik in listaPlikow)
                {
                    EksportujPlik(plik);
                }
            }
            else if (!string.IsNullOrEmpty(SciezkaLokalnaImport))
            {
                if (File.Exists(SciezkaLokalnaImport))
                {
                    EksportujPlik(SciezkaLokalnaImport);
                }
                else
                {
                    Log.Error(string.Format("Podana ścieżka lokalna importu nie istnieje. Ścieżka- {0}",SciezkaLokalnaImport));
                }
            }
            else
            {
                Log.Error("Nie podano ścieżki lokalnej importu");
            }
        }

        public void EksportujPlik(string sciezka)
        {
            string dane = File.ReadAllText(sciezka, Encoding.Default);
           LogiFormatki.PobierzInstancje.LogujInfo("Wczytano plik: " + sciezka + " uruchamianie profilu id: " + IDszablonu);
            List<Komunikat> zwrocone = ApiWywolanie.WywolajImportEksport(Convert.ToInt32(IDszablonu), dane);
            string zwrocone_dane = JSonHelper.Serialize(zwrocone);
            if (Log.IsDebugEnabled)
            {
                Log.Debug("Zwrócone dane: " + zwrocone_dane);
            }
            if (!string.IsNullOrEmpty(zwrocone_dane) && !string.IsNullOrEmpty(SciezkaLokalnaEksport))
            {
                File.WriteAllText(SciezkaLokalnaEksport, zwrocone_dane);
            }
        }
    }
}
