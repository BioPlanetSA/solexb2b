using System;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.ZadaniaDlaZamowienia
{
    [FriendlyName("Kopiowanie pola produkt do pola pozycji zamowienia", FriendlyOpis = "Po finalizacji koszyka kopiuje warttość z pola produktu do odpowiedniego pola produktu na zamówieniu")]
    public class KopiujPoleProduktDoPolaPozycjiZamowienia: ZadaniaPozycjiZamowienia, IZadaniePoStworzeniuZamowienia
    {
        public override bool Wykonaj(IKoszykPozycja pozycja, ZamowieniaProduktyBLL zamowieniePozycja)
        {
            var property = pozycja.Produkt.GetType().GetProperty(PoleProduktu);
            var defaultValue = property.PropertyType.IsValueType? Activator.CreateInstance(property.PropertyType):null;
            var wartosc = property.GetValue(pozycja.Produkt);
            if (wartosc == null || wartosc== defaultValue)
            {
                return false;
            }
            var polePozycji = zamowieniePozycja.GetType().GetProperty(PolePozycjiZamowienia);
            if (polePozycji == null)
            {
                SolexBllCalosc.PobierzInstancje.Log.ErrorFormat($"Brak pola na zamówieniu o nazwie: {PolePozycjiZamowienia}. Popraw konfigurację modulu o id: {this.Id}");
                return false;
            }
            try
            {
                polePozycji.SetValue(zamowieniePozycja, Convert.ChangeType(wartosc, polePozycji.PropertyType), null);
            }
            catch (Exception ex)
            {
                SolexBllCalosc.PobierzInstancje.Log.ErrorFormat($"Błąd przy ustawiania wartości dla pola: {polePozycji.Name}, wartosci: {wartosc}. Błąd: {ex.Message}");
            }
            
            return true;
        }

        [FriendlyName("Pole produktu którego wartość będzie skopiowana do pola zamówienia")]
        [WidoczneListaAdmin(false, false, true, false)]
        [PobieranieSlownika(typeof(SlownikPolProduktow))]
        public string PoleProduktu { get; set; }

        [FriendlyName("Pole pozycji zamówienia do którego będzie skopiowana wartość.")]
        [WidoczneListaAdmin(false, false, true, false)]
        [PobieranieSlownika(typeof(SlownikPolPozycjiZamowienia))]
        public string PolePozycjiZamowienia { get; set; }

        
    }
}
