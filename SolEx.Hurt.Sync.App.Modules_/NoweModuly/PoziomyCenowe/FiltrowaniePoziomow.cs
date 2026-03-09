using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.PoziomyCenowe
{
    public class FiltrowaniePoziomowCen :SyncModul, IModulPoziomyCen
    {
        [FriendlyName("Wybrane poziomy cen oddzielone średnikiem. Wielkość znaków nie ma znaczenia.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoziomyCen { get; set; }

        [FriendlyName("Usuwanie wybranych poziomów. Jeśli na tak, to wybrane poziomy będą usunięte. Na nie zostaną usunięte wszystkie oprócz wybranych")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool UsuwanieWybranychPoziomow { get; set; }

        public FiltrowaniePoziomowCen()
        {
            PoziomyCen = string.Empty;
            UsuwanieWybranychPoziomow = false;
        }

        public void Przetworz(ref Dictionary<int, PoziomCenowy> listaPoziomowCen, ref List<CenaPoziomu> ceny, Dictionary<int, PoziomCenowy> poziomyNaB2B, Dictionary<long, CenaPoziomu> cenyPoziomyB2B)
        {
            Dictionary<int, PoziomCenowy> nowaLista = new Dictionary<int, PoziomCenowy>(listaPoziomowCen.Count);
            if (!string.IsNullOrEmpty(PoziomyCen))
            {
                PoziomyCen = PoziomyCen.ToLower();
                var poziomy = PoziomyCen.Split(';');
                foreach (PoziomCenowy poziomy_Cen in listaPoziomowCen.Values)
                {
                    if ((!poziomy.Contains(poziomy_Cen.Nazwa.ToLower()) && UsuwanieWybranychPoziomow) ||poziomy.Contains(poziomy_Cen.Nazwa.ToLower()) && !UsuwanieWybranychPoziomow )
                    {
                        nowaLista.Add(poziomy_Cen.Id, poziomy_Cen);
                    }
                }
            }
            listaPoziomowCen = nowaLista;
        }

        public override string uwagi
        {
            get { return ""; }
        }
    }
}
