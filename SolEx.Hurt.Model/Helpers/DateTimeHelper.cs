using System;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Helpers
{
    public class DateTimeHelper
    {
         public DateTimeHelper(){}
         static DateTimeHelper() { }

         private static readonly DateTimeHelper _dateTimeHelper = new DateTimeHelper();

         public static DateTimeHelper PobierzInstancje
        {
            get { return _dateTimeHelper; }
        }

        public DateTime Nastepny(DayOfWeek dayOfWeek)
        {
            int start = (int)BiezacaData().DayOfWeek;
            int target = (int)dayOfWeek;
            if (target <= start)
                target += 7;
            return BiezacaData().AddDays(target - start);
        }
        public DateTime WyliczDate(DataOdKiedyLiczyc typ, string wartosc)
        {
            DateTime? wynik;
            switch (typ)
            {
                case DataOdKiedyLiczyc.WybranejDaty:
                    wynik = TextHelper.PobierzInstancje.ParsujDate(wartosc) ?? DateTime.MinValue;
                    break;
                case DataOdKiedyLiczyc.PoczatekRoku:
                    wynik = new DateTime(BiezacaData().Year, 1, 1);
                    break;
                case  DataOdKiedyLiczyc.BiezacyDzien:
                    wynik = BiezacaData().Date;
                    break;
                case DataOdKiedyLiczyc.OstatniDzienRoku:
                    wynik = new DateTime(BiezacaData().Year, 12, 31);
                    break;
                case DataOdKiedyLiczyc.NieskonczonoscMinus:
                    wynik = DateTime.MinValue;
                    break;
                case DataOdKiedyLiczyc.NieskonczonoscPlus:
                    wynik = DateTime.MaxValue;
                    break;
                default:
                    throw new Exception("Nie znana opcja");

            }
            if (wynik ==null)
            {
                throw new Exception("Nie sparsowano daty");
            }
            return wynik.Value;
        }

        //potrzebne żeby do testów dało sie wrzucić fake'ową datę
        public virtual DateTime BiezacaData()
        {
            return DateTime.Now;
        }
    }
}
