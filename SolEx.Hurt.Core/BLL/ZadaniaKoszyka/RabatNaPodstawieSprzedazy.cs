using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class RabatNaPodstawieSprzedazy : ZadaniePozycjiKoszyka, IModulStartowy, IFinalizacjaKoszyka, ITestowalna
    {
        [FriendlyName("Typ liczenia rabatu")]
        public TrybLiczeniaRabatuWKoszyku TypLiczeniaRabatu { get; set; }

        public override string Opis
        {
            get { return "Ilość podawana w jednostce podstawowej."; }
        }

        [FriendlyName("Sposób liczenia rabatu wg wzoru ilosc:rabat;, np:5:2%;10:6%;20:10%")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Reguly { get; set; }

        [FriendlyName("Z ilu lat wstecz uwzględniać dokumenty")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int IleLat { get; set; }

        [FriendlyName("Jak wyliczać okres uwzględniania dokumentów")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public DataOdKiedyLiczyc OdKiedyLiczyc { get; set; }

        [FriendlyName("Data od kiedy liczyć, uwzględnia tylko jeśli wybrano liczenie od konkretnej daty")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string DataOdKiedy { get; set; }

        [FriendlyName("Jaki typ dokumentów uwzględniać")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public HashSet<ZCzegoLiczycGradacje> RodzajDokumentu { get; set; }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = Przedzial.SpradzWartosc(IleLat, "Ilość lat");
            return listaBledow;
        }

        public override bool Wykonaj(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            DateTime odKiedy = DateTimeHelper.PobierzInstancje.WyliczDate(OdKiedyLiczyc, DataOdKiedy);
            odKiedy = odKiedy.AddYears(-IleLat);
            var zczegoLiczyc = SolexBllCalosc.PobierzInstancje.Konfiguracja.ZCzegoLiczycGradacje;
            if (zczegoLiczyc==null || !zczegoLiczyc.Any())
            {
                return true;
            }
            decimal ilosc = SolexBllCalosc.PobierzInstancje.KupowaneIlosciBLL.PobierzKupowanaIlosc(new HashSet<long>() {pozycja.ProduktId}, koszyk.Klient, RodzajDokumentu, odKiedy) + pozycja.IloscWJednostcePodstawowej;
            decimal klucz = -1;
            foreach (decimal il in SlownikRegul.Keys.OrderBy(x => x))
            {
                if (ilosc >= il)
                {
                    klucz = il;
                }
            }
            if (klucz >= 0)
            {
                string rabat = SlownikRegul[klucz];
                decimal rabatwartosc = PobierzWartosc(rabat, pozycja);
                pozycja.ZmienDodatkowyRabat(rabatwartosc, Komunikat, TypLiczeniaRabatu);
            }
            return true;
        }

        private decimal PobierzWartosc(string rabat, IKoszykPozycja pozycjaBLL)
        {
            if (rabat.Contains("%"))
            {
                decimal tmp;
                TextHelper.PobierzInstancje.SprobojSparsowac(rabat.Replace("%", ""), out tmp);
                return tmp;
            }
            return 0;
        }

        private Dictionary<decimal, string> _reguly;

        private Dictionary<decimal, string> SlownikRegul
        {
            get
            {
                if (_reguly == null)
                {
                    _reguly = new Dictionary<decimal, string>();
                    if (!string.IsNullOrEmpty(Reguly))
                    {
                        string[] wpisy = Reguly.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string s in wpisy)
                        {
                            string[] wartosc = s.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                            if (wartosc.Length > 1)
                            {
                                decimal ilosc;
                                if (TextHelper.PobierzInstancje.SprobojSparsowac(wartosc[0], out ilosc))
                                {
                                    if (_reguly.ContainsKey(ilosc)) continue;
                                    _reguly.Add(ilosc, wartosc[1]);
                                }
                            }
                        }
                    }
                }
                return _reguly;
            }
        }
    }
}