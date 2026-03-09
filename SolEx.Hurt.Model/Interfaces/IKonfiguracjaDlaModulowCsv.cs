using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Interfaces
{
    public interface IKonfiguracjaDlaModulowCsv
    {
        [FriendlyName("Kolumny z których będą importowane cechy - rozdzielone ;. (Np.: Marka;model;silnik;rocznik)")]
        [WidoczneListaAdmin(false, false, true, false)]
        string KolumnyDoImportu { get; set; }

        [FriendlyName("Po jakim polu z produktu powiązać")]
        [WidoczneListaAdmin(false, false, true, false)]
        TypyPolDoDopasowaniaZdjecia PoCzymSzukacProdukty { get; set; }

        [FriendlyName("Ścieżka do pliku CSV z którego importujemy cechy.")]
        [WidoczneListaAdmin(false, false, true, false)]
        string SciezkaDoPlikuCsv { get; set; }

    }
}
