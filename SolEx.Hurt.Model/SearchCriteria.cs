using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model obiektu kryteriów wyszukiwania
    /// </summary>
    public  class SearchCriteria
    {
        public int Id { get; set; }
        public int Id2 { get; set; }
        public int CategoryId { get; set; }
        public int GroupId { get; set; }
        public string Sort { get; set; }

        private string _query;
        public string Query 
        {
            get
            {
                return _query != null ? _query.Replace("'", "_") : "";
               
            }
            set { _query = value; }
        }
        public string Filters { get; set; }
        public string Filters2 { get; set; }
        public bool NotEmpty { get; set; }
        public bool Grouping { get; set; }
        public string Where { get; set; }
        public List<int> HiddenColumns { get; set; }
        public bool HidePrice { get; set; }
        public int PageCurrent { get; set; }
        public int PageSize { get; set; }
        public string Changes { get; set; }
        public string Parameters { get; set; }
        public int Param1 { get; set; }
        public int ListType { get; set; }
        public bool IsBrandGroup { get; set; }
        public string SqlSelect { get; set; }
        public string SqlSelectFamily { get; set; }

        // generuje gotowy warunek na podstawie parametrów 
        public string NotEmptyWhere = " dbo.z_produkty_SprawdzDostepnosc(p.zrodlo_id, p.rodzina,s.stan,s.stan2,s.stan3,stan4,s.stan5) = 1 " ; // " ( s.stan > 0 or s.stan2 > 0 or s.stan3 > 0 ) ";
        public string NotEmptyWhereFamily = " dbo.z_produkty_SprawdzDostepnosc(p.zrodlo_id, null,s.stan,s.stan2,s.stan3,stan4,s.stan5) = 1 "; // " ( s.stan > 0 or s.stan2 > 0 or s.stan3 > 0 ) ";
        public string WhereSQL(string notEmptySetting)
        {
            string s = "";
            string and = "";

            if (NotEmpty)
            {
                s += string.IsNullOrEmpty(notEmptySetting) ? NotEmptyWhere : notEmptySetting;
                and = " and ";
            }
            if (Grouping)
            {
                s += and;
           //     s += " (1=1) ";// 
                s += " ( p.typ = 2 or isnull(p.rodzina_cecha,'')='' or isnull(p.rodzina,'') = '' ) "; 
                if (and == "")
                    and = " and ";
            }
            else
            {
                s += and;
                s += " p.typ = 1 ";
                if (and == "")
                    and = " and ";
            }
            if (!string.IsNullOrEmpty(Where.Trim()))
            {
                if(!Where.ToLower().Trim().StartsWith("and"))
                {
                s += and;
                }
                s += Where;
            }

            return s;
        }

        public SearchCriteria()
        {
        }

        public SearchCriteria(int categoryId, string sort, string query, string where, string filters, List<int> hiddenColumns, bool notEmpty, bool grouping, int page_current, int page_size)
        {
            CategoryId = categoryId;
            Sort = sort;
            Query = query;
            Filters = filters;
            NotEmpty = notEmpty;
            Grouping = grouping;
            Where = where;
            HiddenColumns = hiddenColumns == null ? new List<int>() : hiddenColumns;
            PageCurrent = page_current;
            PageSize = page_size;
        }

    }
}
