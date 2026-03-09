using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    /// <summary>
    /// Klasa dla maila informujacego o pobraniu faktury
    /// </summary>
    public class PobranieFaktury : SzablonMailaBaza
    {
        public PobranieFaktury() : base(null) { }

        public PobranieFaktury(DokumentyBll dokument, IKlient klient) : base(klient)
        {
            Dokument = dokument;
        }
        public override string NazwaFormatu()
        {
            return "Pobranie faktury przez klienta z platformy";
        }
        public DokumentyBll Dokument { get; set; }
        public override string OpisFormatu()
        {
            return "Mail informujacy o pobraniu faktury przez klienta - mail jest potwierdzeniem otrzymania faktury np. korekty. Dobrą praktyką jest założenie oddzielnej skrzynki mailowej i wysyłanie tam kopii tych wiadomości";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail informujacy o pobraniu faktury przez klienta.";
        }
        public string AdresIP
        {
            get { return SesjaHelper.PobierzInstancje.IpKlienta; }
        }

        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get { return new[] {TypyPowiadomienia.Opiekun}; }
        }
    }
}
