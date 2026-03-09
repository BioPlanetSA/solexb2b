using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class NoweZamowienie : PowiadomienieZZamowieniem
    {

        public NoweZamowienie() { }
        public NoweZamowienie(ZamowieniaBLL zamowienia, IKlient klient)
            : base(zamowienia, klient)
        {
 
    }

        public override string NazwaFormatu()
        {
            return "Nowe zamówienie";
        }

        public override string OpisFormatu()
        {
            return "Powiadomienie dotyczące zamówień złożonych przez platformę. Powiadomienie wysyłane w momenci finalizacji koszyka lub dopiero po imporcie zamówienia do systemu ERP (wg. ustawienia: KiedyWysylacMailaOZamowieniu). " +
                   "Powiadomienie jest również wysyłane w przypadku zamówień subkont do zatwierdzenia/odrzucenia, oraz w przypadku błędu importu zamówienia do systemu ERP. " +
                   "Jeśli mail jest wysyłany po imporcie do ERP, zawiera już dokładny numer zamówienia nadany przez ERP.";
        }
        public override string OpisDlaKlienta()
        {
            return "Powiadomienie dotyczące zamówień złożonych przez platformę.";
        }

    }
}
