using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public enum MagazynyPola
    {
        [FriendlyName("Magazyn dla mm-ek")]
        MagazynDlaMM,
        [FriendlyName("Magazyn realizujący")]
        Realizujący
    }

    [FriendlyName("Ustaw wybrany magazyn jako podstawowy (do MM) lub realizujący.", FriendlyOpis = "Jeśli w zamówieniu jest ustawiony magazyn podstawowy to wystawiana jest MM na ten magazyn, dokument zamówienia jest wystawiany z magazynu realizującego.")]
    public class UstawMagazyn : ZadanieCalegoKoszyka, IZadaniePoFinalizacji, ITestowalna
    {
        public UstawMagazyn()
        {
            KtoryMagazynUstawiac = MagazynyPola.Realizujący;
        }

        [FriendlyName("Magazyn")]
        [PobieranieSlownika(typeof(SlownikMagazynow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int Magazyn { get; set; }

        [FriendlyName("Który magazyn ustawić")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public MagazynyPola KtoryMagazynUstawiac { get; set; }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();
            var mag = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Magazyn>(Magazyn);
            if (mag == null)
            {
                listaBledow.Add(string.Format("Brak magazynu o id: {0}", Magazyn));
            }
            return listaBledow;
        }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            var mag = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Magazyn>(Magazyn);

            if (KtoryMagazynUstawiac == MagazynyPola.MagazynDlaMM)
            {
                koszyk.MagazynDlaMm = mag.Symbol;
            }
            else
            {
                koszyk.MagazynRealizujacy = mag.Symbol;
            }

            return true;
        }
    }
}