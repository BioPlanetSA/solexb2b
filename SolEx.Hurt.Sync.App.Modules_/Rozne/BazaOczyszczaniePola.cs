using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FastMember;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Sync.App.Modules_.Rozne
{
    public enum JakPobieramy
    {
        Przed, Za
    }
    public enum WystapnieSeparatora
    {
        Pierwsze, Ostatnie
    }

    [FriendlyName( FriendlyOpis = "Oczyszcza wybrane pole, zostawia wartość względem określonego separatora")]
    public abstract class BazaOczyszczaniePola : SyncModul
    {
        [FriendlyName("Separator")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Separator { get; set; }

        [FriendlyName("Jak pobieramy względem separatora")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public JakPobieramy JakPobieramy { get; set; }

        [FriendlyName("Względem którego wystąpienia separatora dzielimy")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public WystapnieSeparatora WystapnieSeparatora { get; set; }



        protected void Oczysc(object o, List<PropertyInfo> propertisy,string pole, TypeAccessor akcesor)
        {

            var polezrodlowe = propertisy.FirstOrDefault(a => a.Name == pole);
                try
                {
                    object starePole = akcesor[o, polezrodlowe.Name];
                    string nowaWartosc = "";
                    if (starePole != null)
                        nowaWartosc = starePole.ToString();

                    int poz;
                    if (WystapnieSeparatora == WystapnieSeparatora.Ostatnie)
                    {
                        poz = nowaWartosc.LastIndexOf(Separator, StringComparison.Ordinal);
                    }
                    else
                    {
                        poz = nowaWartosc.IndexOf(Separator, StringComparison.Ordinal);
                    }
                    if (poz > -1)
                    {
                        if (JakPobieramy == JakPobieramy.Za)
                        {
                            nowaWartosc = nowaWartosc.Substring(poz + 1);
                        }
                        else
                        {
                            nowaWartosc = nowaWartosc.Substring(0, poz);
                        }
                    }
                    akcesor[o, polezrodlowe.Name] = nowaWartosc;
                }
                catch (Exception ex)
                {
                    Log.Error("błąd przy przetwarzaniu towaru " + ex.Message, ex);
                }
        }
    }
}
