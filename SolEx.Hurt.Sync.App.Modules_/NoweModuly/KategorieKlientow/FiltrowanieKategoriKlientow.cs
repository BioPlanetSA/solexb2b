using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Common.Extensions;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieKlientow
{
    public class FiltrowanieKategoriKlientow : SyncModul, IModulKategorieKlientow
    {
        [FriendlyName("Grupy klientow oddzielone ';'. Cechy posiadające tą grupę nie zostaną pobrane")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Grupy { get; set; }

        private List<string> GrupyKlientow
        {
            get
            {
                List<string> wynik = new List<string>();
                var temp = Grupy.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var g in temp)
                {
                    var gMala = g.ToLower();
                    if (!wynik.Contains(gMala))
                    {
                        wynik.Add(gMala);
                    }
                }
                return wynik;
            }
        }


        public void Przetworz(ref List<Model.KategoriaKlienta> kategorie, ref List<Model.KlientKategoriaKlienta> laczniki)
        {
            List<string> grupyDoUsuniecia = GrupyKlientow;

            Log.Info($"Kategorii klientów przed usuwaniem: {kategorie.Count}, grup do usuniecia: {grupyDoUsuniecia.Count}, łączników: {laczniki.Count} ");

            for (int i = 0; i < kategorie.Count; i++)
            {
                string grupa = kategorie[i].Grupa;

                if(string.IsNullOrEmpty( grupa))
                {
                    continue;
                }

                if (grupyDoUsuniecia.Contains(grupa.ToLower()))
                {
                    int id = kategorie[i].Id;
                    laczniki.RemoveAll(x => x.KategoriaKlientaId == id);
                    kategorie.RemoveAt(i);
                    i--;
                }
            }
            Log.Info($"Kategorii po usuwaniu: { kategorie.Count}, łączników: {laczniki.Count}");
        }

        public override string uwagi
        {
            get { return ""; }
        }
        public override string Opis
        {
            get { return "Wyłanczanie z synchronizacji kategorie klientów z określonych  grup. Nie działa na kategorie tworzene dynamicznie za pomocą innego modułu"; }
        }
    }
}
