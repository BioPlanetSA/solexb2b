using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Sync.App.Modules_.DAO;

namespace SolEx.Hurt.Sync.App.Modules_.Helpers
{
    public class Subiekt_DoPoprawki
    {
        public static string GetFieldName(string fieldName, SubiektDataContext db, int type)
        {
            //type -14 produkt
            //type -2 dokument
            pw_Pole field = db.pw_Poles.FirstOrDefault(p => p.pwp_Nazwa == fieldName && p.pwp_TypObiektu == type);

            if (field == null)
            {
                return "";
            }

            return field.pwp_Pole;
        }
        public static string GetFieldValue(string name, pw_Dane field)
        {
            string fieldName = string.Empty;

            if (field == null)
                return fieldName;

            switch (name)
            {
                case "pwd_Tekst01":
                    fieldName = field.pwd_Tekst01;
                    break;
                case "pwd_Tekst02":
                    fieldName = field.pwd_Tekst02;
                    break;
                case "pwd_Tekst03":
                    fieldName = field.pwd_Tekst03;
                    break;
                case "pwd_Tekst04":
                    fieldName = field.pwd_Tekst04;
                    break;
                case "pwd_Tekst05":
                    fieldName = field.pwd_Tekst05;
                    break;
                case "pwd_Tekst06":
                    fieldName = field.pwd_Tekst06;
                    break;
                case "pwd_Tekst07":
                    fieldName = field.pwd_Tekst07;
                    break;
                case "pwd_Tekst08":
                    fieldName = field.pwd_Tekst08;
                    break;
                case "pwd_Tekst09":
                    fieldName = field.pwd_Tekst09;
                    break;
                case "pwd_Tekst10":
                    fieldName = field.pwd_Tekst10;
                    break;
                case "pwd_Fk01":
                    fieldName = (field.pwd_Fk01.HasValue ? field.pwd_Fk01.Value.ToString() : "");
                    break;
                case "pwd_Fk02":
                    fieldName = (field.pwd_Fk02.HasValue ? field.pwd_Fk02.Value.ToString() : "");
                    break;
                case "pwd_Fk03":
                    fieldName = (field.pwd_Fk03.HasValue ? field.pwd_Fk03.Value.ToString() : "");
                    break;
                case "pwd_Fk04":
                    fieldName = (field.pwd_Fk04.HasValue ? field.pwd_Fk04.Value.ToString() : "");
                    break;
                case "pwd_Fk05":
                    fieldName = (field.pwd_Fk05.HasValue ? field.pwd_Fk05.Value.ToString() : "");
                    break;
                case "pwd_Fk06":
                    fieldName = (field.pwd_Fk06.HasValue ? field.pwd_Fk06.Value.ToString() : "");
                    break;
                case "pwd_Fk07":
                    fieldName = (field.pwd_Fk07.HasValue ? field.pwd_Fk07.Value.ToString() : "");
                    break;
                case "pwd_Fk08":
                    fieldName = (field.pwd_Fk08.HasValue ? field.pwd_Fk08.Value.ToString() : "");
                    break;
                case "pwd_Fk09":
                    fieldName = (field.pwd_Fk09.HasValue ? field.pwd_Fk09.Value.ToString() : "");
                    break;
                case "pwd_Fk10":
                    fieldName = (field.pwd_Fk10.HasValue ? field.pwd_Fk10.Value.ToString() : "");
                    break;
                case "pwd_Liczba01":
                    fieldName = field.pwd_Liczba01.HasValue ? field.pwd_Liczba01.Value.ToString() : "";
                    break;
                case "pwd_Liczba02":
                    fieldName = field.pwd_Liczba02.HasValue ? field.pwd_Liczba02.Value.ToString() : "";
                    break;
                case "pwd_Liczba03":
                    fieldName = field.pwd_Liczba03.HasValue ? field.pwd_Liczba03.Value.ToString() : "";
                    break;
                case "pwd_Liczba04":
                    fieldName = field.pwd_Liczba04.HasValue ? field.pwd_Liczba04.Value.ToString() : "";
                    break;
                case "pwd_Liczba05":
                    fieldName = field.pwd_Liczba05.HasValue ? field.pwd_Liczba05.Value.ToString() : "";
                    break;
                case "pwd_Liczba06":
                    fieldName = field.pwd_Liczba06.HasValue ? field.pwd_Liczba06.Value.ToString() : "";
                    break;
                case "pwd_Liczba07":
                    fieldName = field.pwd_Liczba07.HasValue ? field.pwd_Liczba07.Value.ToString() : "";
                    break;
                case "pwd_Liczba08":
                    fieldName = field.pwd_Liczba08.HasValue ? field.pwd_Liczba08.Value.ToString() : "";
                    break;
                case "pwd_Liczba09":
                    fieldName = field.pwd_Liczba09.HasValue ? field.pwd_Liczba09.Value.ToString() : "";
                    break;
                case "pwd_Liczba10":
                    fieldName = field.pwd_Liczba10.HasValue ? field.pwd_Liczba10.Value.ToString() : "";
                    break;
                //default: fieldName = subiektProduct.tw_Nazwa; break;
            }

            if (string.IsNullOrEmpty(fieldName))
                fieldName = string.Empty;

            return fieldName;
        }
    }
}
