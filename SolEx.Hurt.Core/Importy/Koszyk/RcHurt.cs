using System.Collections.Generic;
using SocialExplorer.IO.FastDBF;
using SolEx.Hurt.Core.Importy.Model;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.Importy.Koszyk
{
   public class RcHurt : ImportBaza
    {
        public override string LadnaNazwa
        {
            get { return "Import w formacie rc-hurt (RC Sklep)"; }
        }

        public override List<string> Rozszerzenia
        {
            get { return new List<string>{"dbf"};}
        }
        
        protected virtual string KolumnaKodKReskowy 
       {
           get { return "KOD_KRESKO"; }
       }
        public override List<PozycjaKoszykaImportowana> Przetworz(string dane, out List<Komunikat> bledy, System.IO.Stream stumien)
        { 
            List<PozycjaKoszykaImportowana> wynik=new List<PozycjaKoszykaImportowana>();
            bledy=new List<Komunikat>();
            DbfFile odbf = new DbfFile();
            odbf.Open(stumien);


            int? kolumnaKod=null;
            int? kolumnaIlosc=null;
           for (int i = 0; i < odbf.Header.ColumnCount; i++)
           {
               if (odbf.Header[i].Name == KolumnaKodKReskowy)
               {
                   kolumnaKod = i;
               }
               if (odbf.Header[i].Name == "ILOSC")
               {
                   kolumnaIlosc = i;
               }
           }
            if (kolumnaKod.HasValue && kolumnaIlosc.HasValue)
            {
                //read and print records to screen...
                DbfRecord orec = new DbfRecord(odbf.Header);
                for (int i = 0; i < odbf.Header.RecordCount; i++)
                {
                    if (!odbf.Read(i, orec))
                        break;
                    string kod = orec[kolumnaKod.Value];
                    string ilosc = orec[kolumnaIlosc.Value];

                    if (!string.IsNullOrEmpty(kod) && !string.IsNullOrEmpty(ilosc))
                    {
                        ZnajdzProdukt(kod, ilosc, orec.ToString(), wynik, bledy);
                        if (ZaDuzoElementow)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                bledy.Add(new Komunikat("W pliku nie znalezono kolumny: " + KolumnaKodKReskowy,KomunikatRodzaj.danger, GetType().Name + "NieZnaleziono"));
            }
            return wynik;
        }
    }
}
