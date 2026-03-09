using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class Formularz : SzablonMailaBaza
    {
        public Formularz(): base(null)
        {
            
        }

        public Formularz(FormularzZapytanieModel model, IKlient klient) : base(klient)
        {
            FormularzModel = model;
        }
        public override string NazwaFormatu()
        {
            return "Formularz";
        }

        public FormularzZapytanieModel FormularzModel { get; set; }
        public override string OpisFormatu()
        {
            return "Mail wysyłany z formularza kontaktowego (kontrolka: {klienci/formularzzapytania}). Zarządzanie do kogo mail ma być wysłany odbywa się przez parametry kontrolki";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail wysyłany z formularza kontaktowego.";
        }
        public override TypyPowiadomienia[] ObslugiwaneRodzajePowiadomien
        {
            get
            {
                return new TypyPowiadomienia[0];
            }
        }
        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            
            get
            {
                return new TypyPowiadomienia[0];
            }
        }

        /// <summary>
        /// zarzadzanie do kogos wysylac maila odbywa sie w samym formularzu
        /// </summary>
        public List<ParametryWyslania> ParametryWysylania
        {
            get
            {
                List<TypyPowiadomienia> lista = new List<TypyPowiadomienia>(3);
    
                //zawsze do klienta co to wyslal
                lista.Add(TypyPowiadomienia.Klient);

                if ( this.FormularzModel.DoOpiekuna)
                {
                    lista.Add(TypyPowiadomienia.Opiekun);
                }
                if (this.FormularzModel.DoPrzedstawiciela)
                {
                    lista.Add(TypyPowiadomienia.Przedstawiciel);
                }

                return lista.Select(x => new ParametryWyslania { DoKogo = x, Aktywny = true }).ToList();
            }
        }
    }
}
