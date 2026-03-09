using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ExcelDataReader;
using SolEx.Hurt.Core.Importy.Model;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.Importy.Koszyk
{
    public class FriscoXls : ImportBaza
    {
        public override string LadnaNazwa
        {
            get { return "Import zamówień FRISCO"; }
        }

        public override List<string> Rozszerzenia
        {
            get { return new List<string>{"xls"};}
        }

        public override List<PozycjaKoszykaImportowana> Przetworz(string dane, out List<Komunikat> bledy, Stream stumien)
        {
            List<PozycjaKoszykaImportowana> wynik=new List<PozycjaKoszykaImportowana>();
            bledy=new List<Komunikat>();

            IExcelDataReader excelReader = null;

            try
            {
                excelReader = ExcelReaderFactory.CreateBinaryReader(stumien);
            }
            catch
            {
                excelReader =  ExcelReaderFactory.CreateOpenXmlReader(stumien);
            }

            //excelReader. IsFirstRowAsColumnNames = false;
            int nrKolumnyKodKreskowy = -1;
            int nrKolumnyilosc = -1;
            while (excelReader.Read())//petla wyciagajaca naglowki kolumna
            {
                for (int i = 0; i < excelReader.FieldCount; i++)
                {
                    string pole = (excelReader.GetValue(i)??"").ToString().Trim();
                    if (pole == NazwaKolumnyKodKreskowy)
                    {
                        nrKolumnyKodKreskowy = i;
                    }
                    if (pole == NazwaKolumnyIlosc)
                    {
                        nrKolumnyilosc = i;
                    }
                }
                if (nrKolumnyKodKreskowy > -1 && nrKolumnyilosc > -1)
                {
                    break; 
                }
            }
            if (nrKolumnyKodKreskowy > -1 && nrKolumnyilosc > -1)
            {
                while (excelReader.Read()) //petla wyciagajaca produkty
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < excelReader.FieldCount; i++)
                    {
                        sb.AppendFormat("{0};", excelReader.GetValue(i));
                    }
                   
                    string kod = (excelReader.GetValue(nrKolumnyKodKreskowy) ?? "").ToString();
                    string ilosc = (excelReader.GetValue(nrKolumnyilosc) ?? "").ToString();
                    if (string.IsNullOrEmpty(ilosc))
                    {
                        continue; 
                    }
                    if (string.IsNullOrEmpty(kod) && sb.ToString().Contains("RAZEM"))
                    {
                        continue;
                    }
                    ZnajdzProdukt(kod, ilosc, sb.ToString(), wynik, bledy);
                    if (ZaDuzoElementow)
                    {
                        break;
                    }
                }
            }
            else
            {
                bledy.Add(new Komunikat(string.Format("W pliku brakuje kolumny {0} lub {1}", NazwaKolumnyIlosc, NazwaKolumnyKodKreskowy),KomunikatRodzaj.danger,GetType().Name));
            }
            return wynik;
        }

        protected virtual string NazwaKolumnyIlosc
        {
            get { return "qty_p"; }
        }

        protected virtual string NazwaKolumnyKodKreskowy
        {
            get { return "code_ean"; }
        }
    }
}
