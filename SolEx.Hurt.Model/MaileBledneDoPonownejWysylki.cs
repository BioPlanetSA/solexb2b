using System;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.Enums;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model
{
    public class MaileBledneDoPonownejWysylki :IHasIntId ,IEquatable<MaileBledneDoPonownejWysylki>
    {
        public MaileBledneDoPonownejWysylki() { }

        public MaileBledneDoPonownejWysylki(string tytul, string tresc, string doKogo, string bcc, TypyUstawieniaSkrzynek typSkrzynki)
        {
            Tytul = tytul;
            Tresc = tresc;
            DoKogo = doKogo;
            Bcc = bcc;
            RodzajSkrzynki = typSkrzynki;
        }
        public MaileBledneDoPonownejWysylki(WiadomoscEmail mail,TypyUstawieniaSkrzynek typSkrzynki)
        {
            Tytul = mail.Tytul;
            Tresc = mail.TrescWiadomosci;
            DoKogo = mail.DoKogo;
            Bcc = mail.KopiaBCC;
            RodzajSkrzynki = typSkrzynki;
        }

        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string Tytul { get; set; }
        public string Tresc { get; set; }
        public string DoKogo { get; set; }
        public string Bcc { get; set; }
        public TypyUstawieniaSkrzynek RodzajSkrzynki { get; set; }
        public int IloscBledow { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Odbiorca pierwotny")]
        public string DoKogoPierwotnieJesliPrzechwycony { get; set; }

        /// <summary>
        /// Porównuje dwa maile, uwzględnia wszystkie pola oprócz id i ilości błędów
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MaileBledneDoPonownejWysylki other)
        {
            return Tytul == other.Tytul && Tresc == other.Tresc && DoKogo == other.DoKogo && Bcc == other.Bcc && RodzajSkrzynki == other.RodzajSkrzynki;
        }
    }
}
