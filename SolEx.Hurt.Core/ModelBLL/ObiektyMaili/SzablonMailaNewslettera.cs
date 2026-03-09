using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class SzablonMailaNewslettera : SzablonMailaBaza
    {
        public SzablonMailaNewslettera() : base(null)
        {
        }
        public SzablonMailaNewslettera(Model.NewsletterKampania newsletterKampania, IKlient klient) : base(klient)
        {
            NewsletterKampania = newsletterKampania;
            DoKogoWysylany = TypyPowiadomienia.Klient;
        }

        public override string NazwaFormatu()
        {
            return "SzablonMailaNewslettera";
        }        

        public override string NazwaSzablonu()
        {
            return string.Format(PlikiDostep.PobierzInstancje.KatalogSzablonowNewsletterow + "newsletter{0}.cshtml", this.NewsletterKampania.Id);
        }

        public override TypyPowiadomienia[] ObslugiwaneRodzajePowiadomien
        {
            get
            {
                return new[] { TypyPowiadomienia.Klient };
            }
        }
        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get
            {
                return new[] { TypyPowiadomienia.Klient };
            }
        }
        public override string OpisFormatu()
        {
            //"SzablonMailaNewslettera " +
            //        "Powiadomienia są wysyłane przez moduł synchronizacji 'WysylanieMailingow' lub " +
            //        "przez API: Api/powiadomienia.mailing.wyslij.ashx. ";

            return null;
        }
        public override string OpisDlaKlienta()
        {
            return "Newsletter";
        }
        public Model.NewsletterKampania NewsletterKampania { get; set; }

        public List<IProduktKlienta> Produkty
        {
            get
            {
                List<IProduktKlienta> produkty = new List<IProduktKlienta>();
                if (NewsletterKampania.WybraneProdukty != null && NewsletterKampania.WybraneProdukty.Any())
                {
                    var produktyKlienta = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktKlienta>(Klient.JezykId, Klient, x => NewsletterKampania.WybraneProdukty.Contains(x.Id)).ToList();
                    produkty.AddRange(produktyKlienta);
                }
                return produkty;

            }
        }

        public string SzablonListyProduktow
        {
            get { return this.NewsletterKampania.SzablonListyProduktow; }
        }
    }
}
