using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Tresci
{
    public class DaneDoMenuOpcja
    {
        public TreeItem<ElementMenu> ElementMenuDoRenderowania { get; private set; }

        public bool RenderowacSubmenu { get; private set; }

        public DaneDoMenuOpcja(TreeItem<ElementMenu> elementMenuDoRenderowania, bool renderowacSubmenu, bool renderujReklamy = false)
        {
            ElementMenuDoRenderowania = elementMenuDoRenderowania;

            RenderowacSubmenu = Submenu_RenderujGrafikeWSubmenu = Submenu_RenderujTeksteWSubmenu =  false;

            if (renderowacSubmenu)
            {
                RenderowacSubmenu = elementMenuDoRenderowania.Children.Any();

                if (renderujReklamy && !RenderowacSubmenu)
                {
                    RenderowacSubmenu = elementMenuDoRenderowania.Item.Reklama != null;
                }

                if (RenderowacSubmenu)
                {
                    Submenu_RenderujGrafikeWSubmenu = (elementMenuDoRenderowania.Item.Reklama != null);

                    foreach (TreeItem<ElementMenu> d in elementMenuDoRenderowania.Children)
                    {
                        if (d.Item.Reklama != null || (d.Children != null && d.Children.Any()) )
                        {
                            Submenu_RenderujTeksteWSubmenu = true;
                            break;
                        }
                    }
                }
            }
        }


        public bool Submenu_RenderujGrafikeWSubmenu { get; private set; }
        public bool Submenu_RenderujTeksteWSubmenu { get; private set; }
    }
}