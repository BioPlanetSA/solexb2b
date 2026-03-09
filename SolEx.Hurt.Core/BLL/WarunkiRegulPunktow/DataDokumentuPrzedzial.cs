using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.BLL.WarunkiRegulPunktow
{
    public class DataDokumentuPrzedzial : WarunekRegulyPunktowej, IWarunekRegulyCalegoDokumentu, IWarunekRegulyPozycjiDokumentu
    {
        [FriendlyName("Data początkowa")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public DateTime? PoczątekPrzedzialu { get; set; }

        [FriendlyName("Data końcowa")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public DateTime? KoniecPrzedzialu { get; set; }

        public bool SpelniaWarunek(DokumentuPozycjaBazowa pozycja, DokumentyBll dokument)
        {
            return SpelniaWarunek(dokument);
        }

        public bool SpelniaWarunek(DokumentyBll dokument)
        {
            return (!PoczątekPrzedzialu.HasValue || PoczątekPrzedzialu.Value <= dokument.DataUtworzenia.Date) && (!KoniecPrzedzialu.HasValue || KoniecPrzedzialu.Value >= dokument.DataUtworzenia.Date);
        }
    }
}