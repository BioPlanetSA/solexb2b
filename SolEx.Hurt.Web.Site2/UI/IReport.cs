using System;

namespace SolEx.Hurt.Web.Site2.UI
{
    public interface IReport
    {
        DateTime? From { get; set; }
        DateTime? To { get; set; }
        int Account { get; set; }
        string Title { get; }

        bool ShowBooked { get; set; }

        bool ShowDelay { get; set; }

        bool ShowNoRealized { get; set; }

        bool ShowOverdue { get; set; }

        int SelectedAccount { get; set; }
        string SortColumn { get; set; }
        string SortOrder { get; set; }
        string AdditionalPars { get; set; }

        string Filters { get; set; }

        string AccountGroups { get; set; }
    }
}
