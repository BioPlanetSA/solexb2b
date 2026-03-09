using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Model
{
    [Alias("ZamowienieDokumentyERP")]
    public class ZamowienieDokumenty : IHasLongId
    {
        [PrimaryKey]
        public long Id
        {
            get { return (IdZamowienia + "_" + IdDokumentu).WygenerujIDObiektuSHAWersjaLong(); }
        }

        public int IdZamowienia { get; set; }
        public int IdDokumentu { get; set; }
        public string NazwaERP { get; set; }
        public ZamowienieDokumenty(int idZamowienia, int idDokumentu, string nazwaDokumentu)
        {
            IdZamowienia = idZamowienia;
            IdDokumentu = idDokumentu;
            NazwaERP = nazwaDokumentu;
        }
        public ZamowienieDokumenty() { }
    }
}
