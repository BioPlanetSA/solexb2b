using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class ZmianaStatusuDokumentu : SzablonMailaBaza
    {
        public ZmianaStatusuDokumentu(DokumentyBll dok) : base(dok.DokumentOdbiorca)
        {
            Dokument = dok;
        }
        public ZmianaStatusuDokumentu() : base(null)
        {
            this.ZgodaNaZmianyPrzezKlienta = true;
        }
        public DokumentyBll Dokument { get; set; }

        public override string NazwaFormatu()
        {
            return "Zmiana statusu dokumentu";
        }

        public override string OpisFormatu()
        {
            return "Mail informujący o zmianie statusu dokumentu - wysyłany tylko jeśli nowy status ma ustawioną opcje 'Wysyłać mail o zmianie statusu' na TAK ";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail informujący o zmianie statusu dokumentu.";
        }

        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne => new[] { TypyPowiadomienia.Klient };
    }
}
