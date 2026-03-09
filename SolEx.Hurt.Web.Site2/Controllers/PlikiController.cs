
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("Pliki")]
    public class PlikiController : SolexControler
    {
        [Route("ZdjeciaLista/{id}")]
        public PartialViewResult ZdjeciaLista(string id,  string wymuszonyTypPlikow = null)
        {
            string linkDoWyboruPlikow = string.Format(@"/filemanager/dialog.php?field_id={0}&fldr=", id);
            if (!string.IsNullOrEmpty(wymuszonyTypPlikow))
            {
                linkDoWyboruPlikow += string.Format("&extensions=[{0}]", wymuszonyTypPlikow);
            }
            else
            {
                //tylko zdjecia typ == 1
                linkDoWyboruPlikow += "&type=1";
            }

            return PartialView("ZdjeciaLista", new ParametryDoFileManagera(linkDoWyboruPlikow, "Wybierz plik"));
        }

        [Route("MenadzerPlikow/m")]
        public PartialViewResult MenadzerPlikow()
        {
            string link = @"/filemanager/dialog.php";
            string nazwaNaglowka = "Pliki";
                
            return PartialView("ZdjeciaLista", new ParametryDoFileManagera(link,nazwaNaglowka));
        }

        /// <summary>
        /// Metoda odpowiedzialna za wrzucanie plików na serwer wybranych w file manager
        /// </summary>
        /// <param name="plik">link do pliku</param>
        /// <returns></returns>
        [Route("PobierzId")]
        public int PobierzId(string plik)
        {
            //Dekodujemy znaki specjalne z otrzymanego linku
            string oczyszczony = HttpUtility.UrlDecode(plik);
            if (string.IsNullOrEmpty(oczyszczony))
            {
                throw new Exception("Nie udało się zdekodować otrzymanej ścieżki");
            }

            //Tworzymy sobie Uri z otrzymanej scieżki żeby w łatwy sposób usunąć domene
            Uri adres = new Uri(oczyszczony);
            //Tworzymy sciezke bez domeny

            //TrimStart usuwa wszystkie znakiznajdujące się w tablicy znaków ale nie bierze pod uwagę kolejności wiec chyba lepiej dać zamiast trimstart replace
            //string sciezkaBezDomeny = oczyszczony.TrimStart(adres.GetLeftPart(UriPartial.Authority).ToCharArray());
            string sciezkaBezDomeny = oczyszczony.Replace(adres.GetLeftPart(UriPartial.Authority),"").TrimStart('/');

            string sciezkaPliku = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sciezkaBezDomeny);
            //sprawdzamy czy szukany plik w ogóle istnieje
            if (!System.IO.File.Exists(sciezkaPliku))
            {
                throw new FileNotFoundException($"Nie znaleziono pliku: {sciezkaPliku}");
            }

            //Pobieramy info o pliku żeby przesłać później rozmiar pliku, date pliku i nazwę pliku
            FileInfo info = new FileInfo(sciezkaPliku);
            DateTime data = info.LastWriteTime.ToUniversalTime().AddMilliseconds(-info.LastWriteTime.ToUniversalTime().Millisecond);

            //Tworzymy plik, potrzebny jest / na początku ze względu na fakt ze jest to scieżka względna
            Plik p = new Plik(data, info.Name, (int)info.Length, "/"+sciezkaBezDomeny.TrimEnd(info.Name));
            
            //Zapisujemy plik do bazy z plikami
            SolexBllCalosc.PobierzInstancje.Pliki.DodajPlikUzytkownika(p);
            return p.Id;
        }

        [Route("UploadAjax")]
        public int UploadAjax(HttpPostedFileBase file,string katalog)
        {
            throw new Exception("co to jest?");
            //string sciezka = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Zasoby", katalog);
            //if (!Directory.Exists(sciezka))
            //{
            //    Directory.CreateDirectory(sciezka);
            //}
            //sciezka = Path.Combine(sciezka, DateTime.Now.Ticks.ToString());
            //if (!Directory.Exists(sciezka))
            //{
            //    Directory.CreateDirectory(sciezka);
            //}
            //var plik = Path.Combine(sciezka, file.FileName);
            //file.SaveAs(plik);

            //plik = plik.Replace(AppDomain.CurrentDomain.BaseDirectory, "http://localhost/");
            //return PobierzId(plik);
        }
    }
}