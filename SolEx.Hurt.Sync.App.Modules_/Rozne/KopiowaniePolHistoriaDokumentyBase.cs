using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;


namespace SolEx.Hurt.Sync.App.Modules_.Rozne
{
    public static class KopiowaniePolHistoriaDokumentyBase
    {
        public static void Przetworz(ref ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> listaWejsciowa, Func<List<Klient>, long,  bool> czyMoznaPrzetwarzac, string PoleZrodlowe, string PoleDocelowe, bool KopiowacNulle, List<Klient> klienciNaB2B)
        {
            PropertyInfo[] propertisy = typeof(HistoriaDokumentu).GetProperties();

            foreach (HistoriaDokumentu klient in listaWejsciowa.Keys)
            {
                if (czyMoznaPrzetwarzac(klienciNaB2B, klient.KlientId))
                {
                    var polezrodlowe = propertisy.FirstOrDefault(a => a.Name == PoleZrodlowe);
                    var poledocelowe = propertisy.FirstOrDefault(a => a.Name == PoleDocelowe);
                    if (polezrodlowe != null && poledocelowe != null)
                    {
                        try
                        {
                            object starePole = polezrodlowe.GetValue(klient, null);
                            if (starePole == null && !KopiowacNulle)
                            {
                                continue;}

                            poledocelowe.SetValue(klient, starePole, null);
                        }
                        catch (Exception ex)
                        {
                            LogiFormatki.PobierzInstancje.LogujError(new Exception("błąd przy przetwarzaniu klienta " + ex.Message, ex));;
                        }
                    }
                }
            }
        }
    }
}
