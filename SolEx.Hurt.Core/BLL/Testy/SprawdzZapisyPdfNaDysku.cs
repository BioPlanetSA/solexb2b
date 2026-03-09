using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.BLL.Testy
{
    internal class SprawdzZapisyPdfNaDysku : TestKonfiguracjiBaza
    {
        public override string Opis
        {
            get { return "Test czy był jakis dzien kiedy ilość zapisanych PDF na dysku > ilość faktur z tego dnia + 10%"; }
        }

        public override List<string> Test()
        {
            string sciezka = "sfera/dokumenty/";
            return Sprawdz(sciezka);
        }

        public List<string> Sprawdz(string sciezka)
        {
            List<string> listaBledow = new List<string>();

            string docelowa = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sciezka);
            if (Directory.Exists(docelowa))
            {
                var dir = new DirectoryInfo(docelowa);

                for (int i = -7; i <= 0; i++)
                {
                    DateTime wybranyDzien = DateTime.Now.Date.AddDays(i);
                    List<FileInfo> pdfNaDysku = dir.GetFiles().Where(x => x.LastWriteTime.Date == wybranyDzien).ToList();
                    List<HistoriaDokumentu> dokumenty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<HistoriaDokumentu>(null).Where(x => x.DataUtworzenia.Date == wybranyDzien).ToList();

                    double iloscZapisanychWBazie = dokumenty.Count;
                    iloscZapisanychWBazie = (iloscZapisanychWBazie * 0.10) + iloscZapisanychWBazie;

                    if (pdfNaDysku.Count > iloscZapisanychWBazie)
                    {
                        listaBledow.Add(string.Format("Podejrzanie dużo zapisów PDF w dniu: " + wybranyDzien.ToString("dd-MM-yyyy")));
                    }
                }
            }
            else
            {
                listaBledow.Add("Nie można wykonać. Sprawdź poprawność wykonania testu 'Test dostępu do katalogu pdf'");
            }

            return listaBledow;
        }
    }
}