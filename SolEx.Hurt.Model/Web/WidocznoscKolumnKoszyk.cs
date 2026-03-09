using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Web
{
    public class WidocznoscKolumnKoszyk
    {
        public bool PokazywacDateDodania { get; set; }
        public bool PokazywacMetkeRodzinowa { get; set; }
        public bool PokazywacZdjecieProduktu { get; set; }
        public bool PokazywacNazweProduktu { get; set; }
        public bool PokazywacSymbolProduktu { get; set; }
        public bool PokazywacKodKreskowy { get; set; }
        public JakieCenyPokazywac CenaJednostowa { get; set; }
        public bool PokazywacVat { get; set; }
        public bool UkrywacJednoskeMiaryIIlosc { get; set; }
        public JakieCenyPokazywac WartoscPozycji { get; set; }
        public bool PokazywacWartoscVat { get; set; }
        public JakieCenyPokazywac CenaHurtowa { get; set; }
        public bool CenaWPunktach { get; set; }
        public bool WartoscPozycjiWPunktach { get; set; }

        public bool PokazywacWage { get; set; }
        public string FormatPokazywanejWagi { get; set; }

        public TypPozycjiKoszyka TypPozycjiKoszyka { get; set; }

        public string RozmiarZdjecia { get; set; }

        public WidocznoscKolumnKoszyk()
        {
        }
    }
}
