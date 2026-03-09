using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class DaneWlasciciela : KontrolkaTresciBaza, IPrefixSofix
    {
        public DaneWlasciciela()
        {
            FormatRegionu = "woj. {0}";
        }
        public override string Opis
        {
            get { return "Wyswietla dane własciciela wraz z mikrodanymi."; }
        }

        public override string Nazwa
        {
            get { return "Dane właściciela"; }
        }

        public override string Kontroler
        {
            get { return "Wyglad"; }
        }

        public override string Akcja
        {
            get { return "DaneWlasciciela"; }
        }

        public override string Grupa
        {
            get { return "Wygląd"; }
        }
        [FriendlyName("Tekst przed")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        public string Prefix { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [FriendlyName("Test za")]
        public string Sofix { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [FriendlyName("Format pokazywania regionu")]
        public string FormatRegionu { get; set; }
    }
}