using ServiceStack.DataAnnotations;
using System;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model wiadomoœci mailowej
    /// </summary>
    [TworzDynamicznieTabele]
    [NieSprawdzajCzyIsnieje]
    [Alias("HistoriaWiadomosci")]
    [FriendlyName("Historia e-maili")]
    public class WiadomoscEmail : IHasLongId
    {
        [AutoIncrement]
        [PrimaryKey]
        [WidoczneListaAdmin(true, false, false, false)]
        public long Id { get; set; }
        
        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Tytu³")]
        public string Tytul { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Odbiorca")]
        public string DoKogo { get; set; }

        /// <summary>
        /// Jesli mail byl przechwyconu to mamy tu informacje do kogo by³ pierwotnie adresowany
        /// </summary>
        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Odbiorca pierwotny")]
        public string DoKogoPierwotnieJesliPrzechwycony { get; set; }

        [WidoczneListaAdmin(true, false, false, false)]
        public string OdKogo { get; set; }

        [WidoczneListaAdmin(true, false, false, false)]
        public string KopiaBCC { get; set; }
        
        public bool WyslijJakoHTML { get; set; }
        
        //todo: do wywalenia?
        public int PowiazaneZdarzenie { get; set;  }

        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Data wys³ania")]
        public DateTime? DataStworzenia { get; set; }

        [FriendlyName("B³¹d wysy³ki")]
        [WidoczneListaAdmin(true, true, false, false)]
        public bool BylBlad { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Komunikat b³êdu")]
        public string BladKomunikat { get; set; }

        [WidoczneListaAdmin(true, false, false, false)]   
        public int? KampaniaId { get; set; }

        [FriendlyName("Treœæ wiadomoœci w HTML")]
        [WidoczneListaAdmin(true, true, false, false, StyleCss = "max-width: 900px")]
        [WymuszonyTypEdytora(TypEdytora.PoleHtml)]
        public string TrescWiadomosci { get; set; }

        public WiadomoscEmail() {
            WyslijJakoHTML = true;
            DataStworzenia = DateTime.Now;
        }

        public WiadomoscEmail(string doKogo,string odKogo,string kopiaBCC) {
            WyslijJakoHTML = true;
            DodajDoKogo(doKogo);
            OdKogo = odKogo;
            DodajBCC(kopiaBCC);
        }

        /// <summary>
        /// Dodaje bcc do maila
        /// </summary>
        /// <param name="bcc">Dodatkowy mail do bcc </param>
        /// <returns></returns>
        public void DodajBCC(string bcc)
        {
            if (string.IsNullOrEmpty(bcc))
                return;
            if (string.IsNullOrEmpty(KopiaBCC))
            {
                KopiaBCC = bcc;
            }
            else
            {
                if (!KopiaBCC.Contains(bcc))
                {
                    KopiaBCC += ";" + bcc;
                }
            }
        }

       /// <summary>
       /// Dodaje odbiorce do maila
       /// </summary>
       /// <param name="to">Dodatkowy mail do to</param>
       /// <returns></returns>
        public void DodajDoKogo(string to)
        {
            if (string.IsNullOrEmpty(to))
                return;
            if (string.IsNullOrEmpty(DoKogo))
            {
                DoKogo = to;
            }
            else
            {
                if (!DoKogo.Contains(to))
                {
                    DoKogo += ";" + to;
                }
            }
        }

      
        [Ignore]
        public string ReplayTo { get; set; }

    }
}
