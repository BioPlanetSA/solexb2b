using System;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model
{
    [FriendlyName("Logi")]
    public class LogWpis: IHasIntId
    {
        [PrimaryKey]

        [WidoczneListaAdmin(true, false, false, false)]
        public int Id { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        public DateTime Data { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        public string User { get; set; }

        [WidoczneListaAdmin(true, false, false, false)]
        public string Poziom { get; set; }

        [WymuszonyTypEdytora(Enums.TypEdytora.PoleHtml)]
        [WidoczneListaAdmin(true, true, false, false)]
        public string Wiadomosc { get; set; }

        [WymuszonyTypEdytora(Enums.TypEdytora.PoleHtml)]
        [WidoczneListaAdmin(true, true, false, false)]
        public string Url { get; set; }

        [WidoczneListaAdmin(true, false, false, false)]
        public string Modul { get; set; }

        [WymuszonyTypEdytora(Enums.TypEdytora.PoleHtml)]

        [WidoczneListaAdmin(true, false, false, false)]
        public string Opis { get; set; }

        [WymuszonyTypEdytora(Enums.TypEdytora.PoleHtml)]
        [WidoczneListaAdmin(true, false, false, false)]
        public string Ex { get; set; }

        [WymuszonyTypEdytora(Enums.TypEdytora.PoleHtml)]
        [WidoczneListaAdmin(true, false, false, false)]
        public string StackTrace { get; set; }
    }
}
