using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [FriendlyName("Dodatkowe kody kreskowe",FriendlyOpis = "Pobiera dodatkowe kody kreskowe produktów")]
    class DodatkoweKodyKreskowe : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            IPobieranieKodwKreskowych kody = provider as IPobieranieKodwKreskowych;
            if (kody == null)
            {
                throw new NotImplementedException("Aktualny provider księgowy nie dziedziczy po IPobieranieKodwKreskowych");
            }
            List < ProduktyKodyDodatkowe > kodyErp = kody.PobierzAlternatywneKodyKreskowe();
            List<ProduktyKodyDodatkowe> kodyB2B = ApiWywolanie.PobierzKodyAlternatywne();

            var propertisy = typeof(ProduktyKodyDodatkowe).Properties();
            var akcesor = typeof(ProduktyKodyDodatkowe).PobierzRefleksja();

            List <ProduktyKodyDodatkowe> doAktualizacji = new List<ProduktyKodyDodatkowe>();
            for (int i = 0; i < kodyErp.Count; i++)
            {
                var kodnaplatformie = kodyB2B.FirstOrDefault(p => p.Id == kodyErp[i].Id);
                if (kodnaplatformie == null)
                {
                    doAktualizacji.Add(kodyErp[i]);
                }
                else
                {
                    kodyErp[i].Id = kodnaplatformie.Id;
                    if (!kodnaplatformie.Porownaj(kodyErp[i], propertisy, akcesor)) //SyncTools.PorownajObiekty(kodnaplatformie, kodyErp[i]))
                    {
                        doAktualizacji.Add(kodyErp[i]);
                    }
                }
            }
            ApiWywolanie.AktualizujKodyAlternatywne(doAktualizacji);
            List<int> elementyDoUsuniecia = new List<int>();
            foreach (ProduktyKodyDodatkowe c in kodyB2B)
            {
                if (kodyErp.All(p => p.Id != c.Id))
                {
                    elementyDoUsuniecia.Add(c.Id);
                }
            }
            ApiWywolanie.UsunAlternatywneKody(elementyDoUsuniecia);

        }
    }
}
