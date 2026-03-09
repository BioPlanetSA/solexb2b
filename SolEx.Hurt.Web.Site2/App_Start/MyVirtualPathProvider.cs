using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Web.Site2.App_Start
{
    public class MyVirtualPathProvider : VirtualPathProvider
    {
        public override bool FileExists(string virtualPath)
        {
            if (!CzyStany(virtualPath))
            {
                return base.FileExists(virtualPath);
            }
            return true;
        }
        public override VirtualFile GetFile(string virtualPath)
        {
            if (!CzyStany(virtualPath))
            {
                return base.GetFile(virtualPath);
            }
            return new WidokStanow(virtualPath);
        }


        private bool CzyStany(string virtualPath)
        {
            if (virtualPath.StartsWith("/views/stany_"))
            {
                return true;
            }
            return false;
        }

        public class WidokStanow : VirtualFile
        {
            public WidokStanow(string virtualPath) : base(virtualPath)
            {
            }

            public override System.IO.Stream Open()
            {               
                MemoryStream ms = new MemoryStream();
                if (VirtualPath != null)
                {
                    string a = SolexBllCalosc.PobierzInstancje.Cache.PobierzChwilowy<string>(VirtualPath);
                    if (string.IsNullOrEmpty(a))
                    {
                        var parametry = VirtualPath.Split('/');
                        a = string.Format("<span style=\"display: none; \">Bład pobierania stanu dla produktu {0}, pozycja: {1}</span>", parametry[1], parametry[2]);
                    }
                    byte[] data = Encoding.UTF8.GetBytes(a);
                    ms.Write(data, 0, data.Length);
                }
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
        }
    }
}