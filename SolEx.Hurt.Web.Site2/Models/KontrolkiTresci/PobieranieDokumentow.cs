using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class PobieranieDokumentow : KontrolkaTresciBaza
    {
        public override string Grupa
        {
            get { return "Dokumenty"; }
        }

        public override string Nazwa
        {
            get { return "Pobieranie faktur"; }
        }

        public override string Kontroler
        {
            get { return "Eksporty"; }
        }

        public override string Akcja
        {
            get { return "ListaDokumenty"; }
        }
    }
}