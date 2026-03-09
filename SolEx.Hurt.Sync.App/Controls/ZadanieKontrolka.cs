using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Controls
{
    public partial class ZadanieKontrolka : UserControl
    {
      
        private Zadanie _zad = null;
        public bool Zaznaczone
        {
            get
            {
                return chbox.Checked;
            }
            set { chbox.Checked = value; }
        }
        public Zadanie Zadanie
        {
            get { return _zad; }
            set
            {
                _zad = value; 
                nazwa.Text = _zad.NazwaZadania.ToString();
             //   nazwa. = _zad.ToString();
               // czas.Text = _zad.ostatniCzasTrwaniaMinuty == -1 ? "" : _zad.ostatniCzasTrwaniaMinuty.ToString();
            }
        }

        public ZadanieKontrolka(Zadanie z)
        {
            InitializeComponent();

            Zadanie = z;
        }


        public delegate void onClick(object sender, Zadanie zadanieDoUruchomienia);

        public new event onClick OnClick;

        private void btn_Click(object sender, EventArgs e)
        {
            OnClick(this, Zadanie);
        }

        private void nazwa_Click(object sender, EventArgs e)
        {
            Zaznaczone = !Zaznaczone;
        }

        private void opis_MouseClick(object sender, MouseEventArgs e)
        {
            Klikniecie(e);
        }

        private void Klikniecie(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                OnClick(this, Zadanie);
            }
        }

        private void chbox_MouseClick(object sender, MouseEventArgs e)
        {
            Klikniecie(e);
        }

        private void nazwa_MouseClick(object sender, MouseEventArgs e)
        {
            Klikniecie(e);
        }
        private void chbox_MouseDown(object sender, MouseEventArgs e)
        {
            Klikniecie(e);
        } 
    }

}
