using System;
using System.Reflection;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [FriendlyName("Dodatkowe pola w erp z pola koszyka przy zapisie zamówienia", 
        FriendlyOpis = "Ustawia dodatkowe pola w ERP z pola koszyka przy zapisie zamówienia. Moduł ten dynamicznie wpisuje wartość z pola koszuka do ERP. Uwaga! moduł nadpisuje dodatkowe pola, któe mogą być ustawiane przez inne moduły (np. definicja dokumentu ERP).")]
    public class DodatkowePolaZPropertisaKoszykaDoUstawieniaWErp : ZadanieCalegoKoszyka, IZadaniePoFinalizacji
    {
        [FriendlyName("Nazwa pola które mają być wypełnione przy zapisie zamówienia(o ile moduł księgowy to obsługuje).")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaPola { get; set; }

        [FriendlyName("Pole koszyka które ma być pobierana wartość.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(IKoszykiBLL), true)]
        public string NazwaPropertisa { get; set; }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            if (string.IsNullOrEmpty(NazwaPola) || string.IsNullOrEmpty(NazwaPropertisa)) return false;
            Type typKoszyka = typeof(IKoszykiBLL);
            PropertyInfo property = typKoszyka.GetProperty(NazwaPropertisa);
            if (property == null) return false;
            var wartosc = property.GetValue(koszyk);

            string dodatkowePola = NazwaPola + ":"+wartosc.ToString().Trim();
            if (string.IsNullOrEmpty(koszyk.DodatkowePolaErp))
            {
                koszyk.DodatkowePolaErp = dodatkowePola;
            }
            else
            {
                koszyk.DodatkowePolaErp += string.Format(";{0}", dodatkowePola);
            }
           
            return true;
        }
    }
}