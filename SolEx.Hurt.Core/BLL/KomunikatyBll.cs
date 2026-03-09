using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.BLL
{
    public class KomunikatyBll: LogikaBiznesBaza
    {
        public virtual HashSet<Komunikaty> PobierzKomunikatyKlienta(IKlient klient)
        {
            //todo:pobiermay komunikaty, ale nie pokazujemy jak jest przedstawiciel zalgoowany - mozna by to jakos uproscic. Swoja droga ten kod nizej tez mozna sprowadzic do jednego SQL a nie kilku zapytan
            HashSet<Komunikaty> listaKomunikatow = new HashSet<Komunikaty>();
            var komunikatyAktywne = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Komunikaty>(klient,x => x.OdKiedy!=null && x.OdKiedy <= DateTime.Now);
            if (komunikatyAktywne == null || !komunikatyAktywne.Any())
            {
                return null;
            }
            DateTime minData = komunikatyAktywne.Min(x => x.OdKiedy.Value);
            List<DzialaniaUzytkownikow> dzialaniaUzytkownikow= new List<DzialaniaUzytkownikow>();
            dzialaniaUzytkownikow = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<DzialaniaUzytkownikow>(null, x => x.Data >= minData && x.EmailKlienta == klient.Email && x.ZdarzenieGlowne == ZdarzenieGlowne.WyswietlenieKomunikatu).ToList();
            foreach (var komunikatyBll in komunikatyAktywne)
            {
                if (!klient.DataOstatniegoLogowania.HasValue || (klient.DataOstatniegoLogowania.Value < komunikatyBll.OdKiedy) || !dzialaniaUzytkownikow.Any())
                {
                    listaKomunikatow.Add(komunikatyBll);
                    continue;
                }
                var dzialanie = dzialaniaUzytkownikow.Where( x => x.Parametry.ContainsKey("idKomunikatu") && x.Parametry["idKomunikatu"] == komunikatyBll.Id.ToString()).OrderByDescending(x=>x.Data);
                if (!dzialanie.Any())
                {
                    listaKomunikatow.Add(komunikatyBll);
                }
                else
                {
                    if (komunikatyBll.CyklPokazywania == CyklKomunikatu.ZawszePoZalogowaniu && dzialanie.First().Data < klient.DataOstatniegoLogowania.Value)
                    {
                        listaKomunikatow.Add(komunikatyBll);
                    }
                }
            }
            return listaKomunikatow;
        }




        public KomunikatyBll(ISolexBllCalosc calosc) : base(calosc)
        {
        }
    }
}
