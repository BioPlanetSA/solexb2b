using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    [FriendlyName("Rozbij dane z pola - Motoroy",FriendlyOpis = "Motoroy moduł do rozbijania danych logowania, gdy są w jednym polu")]
    public class DaneLogowaniaPoleWlasne : SyncModul, IModulKlienci
    {
        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            foreach (var klienci in listaWejsciowa)
            {
                if (!klienci.Value.HasloZrodlowe.Contains("/"))
                {
                    continue;
                }//nie ma co rozbijać
                string pole = klienci.Value.HasloZrodlowe;
                string []wart = pole.Split(new []{"/"},StringSplitOptions.None);
                string login = wart[0];
                string haslo = wart[1];
                klienci.Value.Login = login;
                klienci.Value.HasloZrodlowe = haslo;
            }
        }

    }
}
