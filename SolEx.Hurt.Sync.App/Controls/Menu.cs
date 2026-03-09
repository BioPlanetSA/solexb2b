using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SolEx.Hurt.Sync.App
{
    public delegate void FormChangedDelegate(FormType type);

    public partial class Menu : UserControl
    {
        public FormChangedDelegate FormChangedCallback;

        public Menu()
        {
            InitializeComponent();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            FormChangedCallback(FormType.Eksport);
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            FormChangedCallback(FormType.Import);
        }
        private void btnSettings_Click(object sender, EventArgs e)
        {
            FormChangedCallback(FormType.Ustawienia);
        }

    }
}
