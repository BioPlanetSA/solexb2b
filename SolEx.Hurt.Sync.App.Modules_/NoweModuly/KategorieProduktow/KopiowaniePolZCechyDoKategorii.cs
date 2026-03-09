using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Common.Extensions;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieProduktow
{
    public class KopiowaniePolZCechyDoKategorii:SyncModul, Model.Interfaces.SyncModuly.IModulKategorieProduktow
    {
        [FriendlyName("Lista atrybutów dla których kopiujemy wybrane pola")]
        [WidoczneListaAdmin(false, false, true, false)]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        public List<string> Atrybuty  { get; set; }

        [FriendlyName("Pola które mają być kopiowane z cechy do kategorii")]
        [WidoczneListaAdmin(false, false, true, false)]
        public Pola ListaPol { get; set; }

        public List<int> IdAtrybutow
        {
            get { return Atrybuty.Select(int.Parse).ToList(); }
        } 

        public override string uwagi
        {
            get { return "Przepisz z pola cech do kategorii utworzonych na podstawie tej cechy"; }
        }

        public void Przetworz(ref Dictionary<long, Model.KategoriaProduktu> listaWejsciowa, Dictionary<long, Model.KategoriaProduktu> listaKategoriiB2B, Model.Interfaces.Sync.ISyncProvider provider, List<Model.Grupa> grupyPRoduktow)
        {
            var cechyProduktow = ApiWywolanie.PobierzCechy();
            List<long> idKategoriiZOpisem = new List<long>();
            var cechyZAtrybutem = cechyProduktow.Where(x => x.Value.AtrybutId.HasValue && IdAtrybutow.Contains(x.Value.AtrybutId.Value));
            foreach (var c in cechyZAtrybutem)
            {
              //  Log.InfoFormat("nazwa cechy: {0}", c.Value.Nazwa);
                
                var wartosc = listaWejsciowa.Values.FirstOrDefault(x => x.Nazwa == c.Value.Nazwa);
                if (wartosc == null) { continue;}
                long idKategorii = wartosc.Id;
                PropertyInfo prop = c.Value.GetType().GetProperty(ListaPol.ToString());
                var wartoscPolaCechy = prop.GetValue(c.Value,null);
                if (wartoscPolaCechy == null)
                {
                    continue;
                }
                PropertyInfo prop2 = wartosc.GetType().GetProperty(ListaPol.ToString());
                prop2.SetValue(listaWejsciowa[idKategorii], wartoscPolaCechy);
                idKategoriiZOpisem.Add(idKategorii);
            }
            List<long> idDoNadpisania = listaKategoriiB2B.Keys.Where(x => !idKategoriiZOpisem.Contains(x)).ToList();
            foreach (var i in idDoNadpisania)
            {
                PropertyInfo prop = listaKategoriiB2B[i].GetType().GetProperty(ListaPol.ToString());
                var w1 = prop.GetValue(listaKategoriiB2B[i], null);

                if (listaWejsciowa.ContainsKey(i))
                {
                    prop.SetValue(listaWejsciowa[i], w1);
                }
            }
        }


        public enum Pola
        {
            ObrazekId, Opis, OpisNaProdukcie
        }

    }
}
