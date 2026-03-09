using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class PrzekroczoneStanyBaza : ZadanieCalegoKoszyka, ITestowalna
    {
        [Niewymagane]
        [FriendlyName("Magazyny z których braæ stany", FriendlyOpis = "Magazyny, które maj¹ byæ brane pod uwagê przez modu³. Jeœli nic nie wybrane - brany jest stan z magazynu realizuj¹cego zamówienie " +
                                                                      "(np. domyœlny magazyn klienta, lub wybrany przez klienta w module koszyka WyborMagazynu)")]
        [PobieranieSlownika(typeof(SlownikMagazynow))]
        [WidoczneListaAdmin(false, false, true, false)]
        public List<int> IdMagazynow { get; set; }

        [FriendlyName("Jeœli klient samodzielnie wybierze magazyn realizuj¹cy, to pobieraæ stany tylko z tego magazynu", 
            FriendlyOpis = "Klient mo¿e samodzielnie wybieraæ magazyn realizuj¹cy w module koszykowym WyborMagazynuRealizujacego. " +
                           "Jeœli to ustawienie jest wy³¹czone to znaczy ¿e magazyn wybrany przez klienta nie jest brany pod uwagê w przeliczeniu przekrocznych stanów")]
        [Niewymagane]
        [WidoczneListaAdmin(false, false, true, false)]
        public bool PobieracStanyTylkoZMagazynuPodstawowegoKoszyka { get; set; }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();
            if (IdMagazynow != null)
            {
                foreach (var magazyn in IdMagazynow)
                {
                    var mag = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Magazyn>(magazyn);
                    if (mag == null)
                    {
                        listaBledow.Add(string.Format("Brak magazynu o id: {0}", magazyn));
                    }
                }
            }
            return listaBledow;
        }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            Dictionary<IKoszykPozycja, decimal> braki = null;
            return Wykonaj(koszyk, ref braki);
        }

        protected bool Wykonaj(IKoszykiBLL koszyk, ref Dictionary<IKoszykPozycja, decimal> braki)
        {
            if (IdMagazynow == null || this.IdMagazynow.IsEmpty())
            {
                IdMagazynow = null;
            }
            
            HashSet<int> idMagazynowDlaKtorychPobracStan = null;
            //czy bierzemy magazyn z koszyka podstawowy
            if (this.PobieracStanyTylkoZMagazynuPodstawowegoKoszyka && !string.IsNullOrEmpty(koszyk.MagazynRealizujacy))
            {
                idMagazynowDlaKtorychPobracStan = new HashSet<int> { Calosc.Konfiguracja.SlownikMagazynowPoSymbolu[koszyk.MagazynRealizujacy].Id};
            }
            else
            {
                if (this.IdMagazynow != null && this.IdMagazynow.Any())
                {
                    idMagazynowDlaKtorychPobracStan = new HashSet<int>( this.IdMagazynow );
                }
            }

            braki = new Dictionary<IKoszykPozycja, decimal>();

            foreach (IKoszykPozycja p in koszyk.PobierzPozycje.Where(x => x.TypPozycji != TypPozycjiKoszyka.ZaPunkty))
            {
                p.StanKoszyk = StanKoszyk.Ok;

                decimal stan = 0;

                //brak podanych maGAZYNOW - bierzemy wszystko - iloscLaczna pobiera tylko dostpene magazyny dla klienta w srodku dlatego tego juz tu nie robimy
                if (idMagazynowDlaKtorychPobracStan == null)
                {
                    stan = p.Produkt.IloscLaczna;
                }
                else
                {
                    stan = p.Produkt.PobierzStan(idMagazynowDlaKtorychPobracStan); // Calosc.ProduktyStanBll.PobierzStanyDlaProduktu(idMagazynowDlaKtorychPobracStan, p.ProduktId);
                }

                if (stan < p.IloscWJednostcePodstawowej)
                {
                    p.StanKoszyk = StanKoszyk.Przekroczony;
                    braki.Add(p, (p.IloscWJednostcePodstawowej - stan)/p.Jednostka().Przelicznik);
                }

                if (stan == 0)
                {//toco wyzej ale dodaotkowo
                    p.StanKoszyk = StanKoszyk.Niedostepy;
                }
            }
            
            return false;
        }
    }
}