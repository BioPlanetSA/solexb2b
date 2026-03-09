using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Importy.Eksporty
{
    public class SmallBussinesExp: GenerowanieDokumentu
    {
        public override bool MoznaGenerowac(DokumentyBll dokument)
        {
            return true;
        }

        public override Licencje? WymaganaLicencja
        {
            get
            {
                return Licencje.DokumentySmallBussinesExp;
            }
        }

        public override Encoding Kodowanie
        {
            get { return Encoding.GetEncoding(28592); }
        }

        public override string Nazwa
        {
            get { return "SmallBussines EXP"; }
        }

        public override string PobierzNazwePliku(DokumentyBll dokument)
        {
            return NazwaPliku(dokument) + "-SmallBussines.exp";
        }

        protected override byte[] PobierzDane(DokumentyBll dokument, IKlient klient)
        {
            var pozycje = dokument.PobierzPozycjeDokumentu().ToList();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[Naglowek]");

            sb.AppendLine("Dost=" );
            sb.AppendLine("NIP=" + klient.Nip);
            sb.AppendLine("Nr="+  dokument.NazwaDokumentu);
            sb.AppendLine("DataWyst="+ dokument.DataUtworzenia);
            sb.AppendLine("DataSprzed="+ dokument.DataDodania);
            sb.AppendLine("Platnosc="+ dokument.NazwaPlatnosci);
            sb.AppendLine("Termin="+ dokument.TerminPlatnosci);
            
            for (int i = 0; i < pozycje.Count(); i++)
            {
                sb.AppendLine();
                sb.AppendLine(string.Format("[Poz{0}]", i + 1));
                sb.AppendLine("Nazwa=" + pozycje[i].NazwaProduktu);
                sb.AppendLine("Symbol=" + pozycje[i]?.ProduktBazowy?.KodKreskowy);
                sb.AppendLine("Jm=" + pozycje[i].Jednostka);
                sb.AppendLine("PKWIU=" + pozycje[i]?.ProduktBazowy?.PKWiU);
                sb.AppendLine("Ilosc=" + pozycje[i].Ilosc);
                sb.AppendLine("CenaNetto=" + pozycje[i].CenaNettoPoRabacie);
                sb.AppendLine(string.Format("Vat={0}%",pozycje[i].Vat));
            }

            return Kodowanie.GetBytes(sb.ToString());
        }
    }
}
