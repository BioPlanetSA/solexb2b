using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class ListyPrzewozowe : SzablonMailaBaza
    {
        public ListyPrzewozowe(DokumentyBll dok, IEnumerable<HistoriaDokumentuListPrzewozowy> listy) : base(dok.DokumentOdbiorca)
        {
            KolekcjaListPrzewozowych = listy;
            Dokument = dok;
        }

        public ListyPrzewozowe() : base(null)
        {
            this.ZgodaNaZmianyPrzezKlienta = true;
        }

        public override string NazwaFormatu()
        {
            return "Nowy list przewozowy";
        }
        public IEnumerable<HistoriaDokumentuListPrzewozowy> KolekcjaListPrzewozowych { get; set; }
        public DokumentyBll Dokument  {get;set;}
        public override string OpisFormatu()
        {
            return "Mail informujący o nowym liście przewozowym. Powiadomienie wysyłane w momencie dodawania nowego listu przez moduł synchronizacji.";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail informujący o nowym liście przewozowym";
        }

        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get { return new[] {TypyPowiadomienia.Klient}; }
        }
    }
}
