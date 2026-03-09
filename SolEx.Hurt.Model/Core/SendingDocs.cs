using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SolEx.Hurt.Model.Core
{

    public interface SendingDocs
    {

        void SetDocCreateTime(int id, DateTime date);
        DateTime? GetDocCreateTime(int id);
        DateTime? GraceTime { get; }
        void SaveData();
    }
}
