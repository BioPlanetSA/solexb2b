using Xunit;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Tresci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Tresci.Tests
{
    public class DaneDoMenuOpcjaTests
    {
        [Fact()]
        public void DaneDoMenuOpcjaTest()
        {
            TreeItem<ElementMenu> menuElement = new TreeItem<ElementMenu>();

            TrescBll tresc = new TrescBll("nazwa2", "symbl1", 1);

            menuElement.Item = new ElementMenu(tresc, null);
            menuElement.Children = new List<TreeItem<ElementMenu>>(10);

            TreeItem<ElementMenu> subMenu1 = new TreeItem<ElementMenu>();
            subMenu1.Item = new ElementMenu( new TrescBll("nazwa", "symbl", 1) , null );

            menuElement.Children.Add(subMenu1);
            menuElement.Children.Add(subMenu1);
            menuElement.Children.Add(subMenu1);
            menuElement.Children.Add(subMenu1);

            DaneDoMenuOpcja testowyObiekt = new DaneDoMenuOpcja(menuElement, true, true);

            Assert.True(testowyObiekt.RenderowacSubmenu);
        }
    }
}