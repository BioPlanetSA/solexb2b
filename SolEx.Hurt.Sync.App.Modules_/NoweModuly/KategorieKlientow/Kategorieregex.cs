using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ServiceStack.Common.Extensions;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieKlientow
{
    public class KategorieRegex : Rozne.KategorieKlientow, IModulKategorieKlientow//, IModulKlienci
    {
        public override string uwagi
        {
            get { return "Tworzy kategorie klientów na podstawie pola własnego"; }
        }
        public KategorieRegex()
        {
            Regex = @"\[(.*?)\]";
        }
        
        [FriendlyName("Pole które będzie poprawione")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(KategoriaKlienta))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoleZrodlowe { get; set; }

        [FriendlyName("Nazwa tworzonej grupy")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Regex { get; set; }

        public virtual IEnumerable<Klient> WszyscyKlienci()
        {
            return ApiWywolanie.PobierzKlientow().Values;
        }

        public void Przetworz(ref List<KategoriaKlienta> kategorie, ref List<KlientKategoriaKlienta> laczniki)
        {
            Regex regex = new Regex(Regex, RegexOptions.Compiled);
            PropertyInfo[] propertisy = typeof(KategoriaKlienta).GetProperties();
            foreach (KategoriaKlienta katkl in kategorie)
            {
                var polezrodlowe = propertisy.First(a => a.Name == PoleZrodlowe);
                string starePole = (polezrodlowe.GetValue(katkl, null) ?? "").ToString();

                if (string.IsNullOrEmpty(starePole))
                {
                    continue;

                }
                var match = regex.Match(starePole);
                if (match.Groups.Count > 0)
                {
                    string kategoria = match.Groups.First().ToString();
                    kategoria = kategoria.Replace("[", "").Replace("]", "");
                    if(!string.IsNullOrEmpty(kategoria))
                        polezrodlowe.SetValue(katkl, kategoria);
                }
            }
        }
    }
}
