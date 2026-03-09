using System.Globalization;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieProduktow
{
    [FriendlyName("Filtrowanie drzewa",FriendlyOpis = "Filtruje kategorie wg podanego drzewa.")]
    public class FiltrowanieDrzewka: SyncModul,Model.Interfaces.SyncModuly.IModulKategorieProduktow
    {
        public FiltrowanieDrzewka()
        {
            Gałąź = string.Empty;
            IDGrupy = "";
        }

  

        [FriendlyName("Gałęzie kategorii oddzielane średnikiem, z których mają być wyciągnięte kategorie.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Gałąź { get; set; }

        [FriendlyName("ID grupy, dla której będzie zastosowany filtr")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string IDGrupy { get; set; }


        public void Przetworz(ref Dictionary<long, KategoriaProduktu> listaWejsciowa, Dictionary<long, KategoriaProduktu> listaKategoriiB2B, ISyncProvider provider, List<Grupa> grupyPRoduktow)
        {
            Dictionary<long, KategoriaProduktu> przefiltrowaneKategorie = new Dictionary<long, KategoriaProduktu>(listaWejsciowa.Count);

            string[] kategorie = Gałąź.Split(new [] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string k in kategorie)
            {
                long? parentID = null;

               foreach (KeyValuePair<long, KategoriaProduktu> lw in listaWejsciowa)
                {
                    if (lw.Value.GrupaId.ToString(CultureInfo.InvariantCulture) == IDGrupy)
                    {
                        string kat = string.Empty;

                        if (lw.Value.ParentId == null)
                        {
                            kat = lw.Value.Nazwa;
                        }

                        else
                        {
                            kat = lw.Value.Nazwa;
                            long? parent = lw.Value.ParentId;

                            while (true)
                            {
                                try
                                {
                                    var p = listaWejsciowa.First(a => a.Key == parent.Value);
                                    parent = p.Value.ParentId;
                                    kat = p.Value.Nazwa + "\\" + kat;
                                }
                                catch { break; }
                            }
                        }
                        Log.Debug(kat);
                        if (kat.StartsWith(Gałąź) && kat != k)
                        {
                            przefiltrowaneKategorie.Add(lw.Key, lw.Value);
                        }
                        else if (kat == Gałąź)
                            parentID = lw.Key;
                    }
                    //jeśli kategoria jest w innej grupie to nie filtrujemy jej tylko z automatu dodajemy
                    else przefiltrowaneKategorie.Add(lw.Key, lw.Value);
                }

               foreach (KeyValuePair<long, KategoriaProduktu> lw in przefiltrowaneKategorie)
                {
                    if (lw.Value.ParentId == parentID)
                        lw.Value.ParentId = null;                
                }
            }
            listaWejsciowa = przefiltrowaneKategorie;
        }


    }

}
