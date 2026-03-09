using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class ZapisDoNewslettera : SzablonMailaBaza
    {
        public ZapisDoNewslettera() : base(null) { }

        public ZapisDoNewslettera(NewsletterZapisani zapis)
            : base(null)
        {
            Zapisany = zapis;
            Klient = new Klient();
            Klient.Email = zapis.Email;
        }
        public override string NazwaFormatu()
        {
            return "Zapisanie do Newslettera";
        }
        public NewsletterZapisani Zapisany { get; set; }
        public override string OpisFormatu()
        {
            return "Mail o zapisaniu się do Newslettera - wysyłany po wypełnieniu formularza Newslettera (formularza dla klientów niezalogowanych na stronie firmowej otwartej). Mail nie jest wysyłany dla klientów zalogowanych";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail o zapisaniu się do Newslettera.";
        }
        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get { return new[] {TypyPowiadomienia.Klient}; }
        }
    }
}
