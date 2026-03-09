using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Ajax.Utilities;
using ServiceStack.Common;
using ServiceStack.Text;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Helper;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Służy do wysyłania powiadomień o dostępności produktów. Kiedy powiadomienie zostanie wysłane do klienta z koszyka wylatuje pozycja która służyła tylko do poinformowania o dostępności.
    /// </summary>
    public class StanyPowiadomienie : ApiSessionBaseHandler
    {
        protected override object Handle()
       {
            var klienci = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(null).Where(x => x.IdInfoODostepnosci != null && x.IdInfoODostepnosci.Any() && !string.IsNullOrEmpty(x.Email)).ToList();
            List<Klient> klienciDoAktualizacji = new List<Klient>();
            HashSet<int> idMagazynow = new HashSet<int>(SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Magazyn>(null).Select(x => x.Id));

            foreach (var klient in klienci)
            {
                HashSet<IProduktKlienta> produktyDoWyslaniaDlaKlienta = new HashSet<IProduktKlienta>();
                List<long> doUsuniecia = new List<long>();

                var produktyKlienta = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktKlienta>(klient.JezykId, klient).ToDictionary(x => x.Id, x => x);

                foreach (var prod in klient.IdInfoODostepnosci)
                {
                    ProduktKlienta pk;
                    if (!produktyKlienta.TryGetValue(prod, out pk))
                    {
                        //trzeba by usuwać produkt z listy oczekujacych jesli klient już nie ma dostępu do tego produktu bo inaczej zostaje smiec do konca zycia systemu
                        doUsuniecia.Add(prod);
                        continue;
                    }
                   
                    decimal lacznie = pk.PobierzStan(idMagazynow);
                    if (lacznie <= 10)
                    {
                        continue;
                    }

                    produktyDoWyslaniaDlaKlienta.Add(pk);
                }

                if (produktyDoWyslaniaDlaKlienta.Count > 0)
                {
                    try
                    {
                        SolexBllCalosc.PobierzInstancje.Statystyki.ZdarzeniePojawienieSieProduktow(produktyDoWyslaniaDlaKlienta.ToList(), klient);
                        klient.IdInfoODostepnosci.RemoveWhere(x => produktyDoWyslaniaDlaKlienta.Select(z=> z.Id).Contains(x));
                        SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(klient);
                    }
                    catch (Exception e)
                    {
                        log.Error(  $"Błąd wysyłania powiadomienia o pokazujaniu się produktów na magazynie dla klienta id: {klient.Id}, produkty id: {produktyDoWyslaniaDlaKlienta.Select(x => x.Id).ToCsv()}" , e );
                    }
                }
                if(doUsuniecia.Count > 0)
                {
                    try
                    {
                        klient.IdInfoODostepnosci.RemoveWhere(x => doUsuniecia.Contains(x));
                        SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(klient);
                    }
                    catch (Exception e)
                    {
                        log.Error($"Błąd usuwania produktów z powiadomienia o pokazujaniu się produktów na magazynie dla klienta id: {klient.Id}, produkty id: {doUsuniecia.ToCsv()}", e);
                    }
                }

            }
            
            return null;
        }
       
    }
}
