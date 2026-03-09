using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class ZmienDane : KontrolkaTresciBaza
    {
        public ZmienDane()
        {
            Opis = "Tu zebraliśmy informacje o Twoim koncie w naszej firmie. W celu wykonania zmiany swoich danych wyślij do nas poniższy formularz. " +
                   "Wniosek zostanie wysłany do opiekuna, który skontakuje się z Tobą i potwierdzi wykonanie zmiany. ";
        }

        public override string Grupa => "Klienci";

        public override string Nazwa => "Moje dane";

        public override string Kontroler => "MojeDane";

        public override string Akcja => "ZmienDane";

        [FriendlyName("Opis nad formularzem")]
        [WidoczneListaAdmin(true, true,true, true)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        public new string Opis { get; set; }


        [FriendlyName("Adres mailowy na jaki wysłać formularz - gdy klient NIE MA OPIEKUNA. Wniosek domyślnie jest wysyłany na maila opiekuna klienta")]
        [WalidatorDanych("SolEx.Hurt.Core.BLL.WalidatoryDanych.Klienci.WalidatorMaila,SolEx.Hurt.Core")]
        [WidoczneListaAdmin(true,true,true,true)]
        [Niewymagane]
        public string EmailNaJakiOdeslacOdpowiedz { get; set; }
    }
}