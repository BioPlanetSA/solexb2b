using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [FriendlyName("Wybór magazynów dla klienta jako realizujące (gdzie zapisać dokument)", FriendlyOpis = "Magazyn realizujący to magazyn na któy zapisany jest dokument. Magazyn podstawowy jeśli jest wypełniony to wystawiana jest MM z podstawowego na realizujący.")]
    public class WyborMagazynuRealizujacegoZamowienie : ZadanieCalegoKoszyka, IModulWyboruMagazynuRealizujacego
    {
        [FriendlyName("Z jakich magazynów może klient wybierać magazyn realizujący. Gdy brak wyboru dostępnych magazynów, klient ma dostęp tylko do swoich magazynów (o ile ma jakieś zdefiniowane - zazwyczaj w ERP ustawione magazyny klienta) lub do wszystkich (brak zdefiniowanych na kliencie jego magazynów) ")]
        [PobieranieSlownika(typeof(SlownikMagazynow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        //[Niewymagane]
        public List<int> DostepneMagazyny { get; set; }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            return true;
        }

        public HashSet<string> PobierzDostepneMagazyny(IKlient klient)
        {
            var magazynyPlatforma = SolexBllCalosc.PobierzInstancje.Konfiguracja.SlownikMagazynowPoId.Values;
            if (magazynyPlatforma == null || !magazynyPlatforma.Any())
            {
                throw new Exception("Brak zdefiniowanych magazynów na platformie.");
            }

            if (DostepneMagazyny != null && DostepneMagazyny.Any())
            {
                return new HashSet<string>( magazynyPlatforma.Where(x=>DostepneMagazyny.Contains(x.Id)).Select(x=>x.Symbol) );
            }

            if (klient.DostepneMagazyny != null && klient.DostepneMagazyny.Any())
            {
                HashSet<string> magazynyKlienta =new HashSet<string>( klient.DostepneMagazyny.Select(x=>x.Trim()) );
                return new HashSet<string>( magazynyPlatforma.Where(x => magazynyKlienta.Contains(x.Symbol)).Select(x=>x.Symbol) );
            }
            return new HashSet<string>( magazynyPlatforma.Select(x=>x.Symbol) );
        }
    }
}