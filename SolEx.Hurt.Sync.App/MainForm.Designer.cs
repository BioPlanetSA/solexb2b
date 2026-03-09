namespace SolEx.Hurt.Sync.App
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            ApiWywolanie.WylogujKlienta();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.logText = new System.Windows.Forms.RichTextBox();
            this.panelZadan = new System.Windows.Forms.FlowLayoutPanel();
            this.tylkoPlanowe = new System.Windows.Forms.CheckBox();
            this.btnUruchom = new System.Windows.Forms.Button();
            this.chOdwrocZaznaczenie = new System.Windows.Forms.CheckBox();
            this.LabelTimera = new System.Windows.Forms.Label();
            this.CzasTimera = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblWersja = new System.Windows.Forms.Label();
            this.labelLicencja = new System.Windows.Forms.Label();
            this.TimerWylaczajacy = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 243F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 72F));
            this.tableLayoutPanel1.Controls.Add(this.logText, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.panelZadan, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tylkoPlanowe, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnUruchom, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.chOdwrocZaznaczenie, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LabelTimera, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.CzasTimera, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 91.05368F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.946322F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(944, 554);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // logText
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.logText, 2);
            this.logText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logText.Location = new System.Drawing.Point(286, 54);
            this.logText.Name = "logText";
            this.logText.Size = new System.Drawing.Size(655, 452);
            this.logText.TabIndex = 1;
            this.logText.Text = "";
            this.logText.MouseEnter += new System.EventHandler(this.logText_MouseEnter);
            // 
            // panelZadan
            // 
            this.panelZadan.AutoScroll = true;
            this.panelZadan.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.panelZadan, 2);
            this.panelZadan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelZadan.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.panelZadan.Location = new System.Drawing.Point(3, 54);
            this.panelZadan.Name = "panelZadan";
            this.panelZadan.Size = new System.Drawing.Size(277, 452);
            this.panelZadan.TabIndex = 2;
            this.panelZadan.WrapContents = false;
            this.panelZadan.MouseEnter += new System.EventHandler(this.panelZadan_MouseEnter);
            // 
            // tylkoPlanowe
            // 
            this.tylkoPlanowe.AutoSize = true;
            this.tylkoPlanowe.Checked = true;
            this.tylkoPlanowe.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tylkoPlanowe.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tylkoPlanowe.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tylkoPlanowe.Location = new System.Drawing.Point(43, 3);
            this.tylkoPlanowe.Name = "tylkoPlanowe";
            this.tylkoPlanowe.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.tylkoPlanowe.Size = new System.Drawing.Size(237, 45);
            this.tylkoPlanowe.TabIndex = 4;
            this.tylkoPlanowe.Text = "Pokaż tylko te zadania, które planowo można w tej chwili uruchomić";
            this.tylkoPlanowe.UseVisualStyleBackColor = true;
            this.tylkoPlanowe.CheckedChanged += new System.EventHandler(this.tylkoPlanowe_CheckedChanged);
            // 
            // btnUruchom
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.btnUruchom, 2);
            this.btnUruchom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnUruchom.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnUruchom.Location = new System.Drawing.Point(3, 512);
            this.btnUruchom.Name = "btnUruchom";
            this.btnUruchom.Size = new System.Drawing.Size(277, 39);
            this.btnUruchom.TabIndex = 5;
            this.btnUruchom.Text = "Uruchom wybrane zadania";
            this.btnUruchom.UseVisualStyleBackColor = true;
            this.btnUruchom.Click += new System.EventHandler(this.btnUruchom_Click);
            // 
            // chOdwrocZaznaczenie
            // 
            this.chOdwrocZaznaczenie.AutoSize = true;
            this.chOdwrocZaznaczenie.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chOdwrocZaznaczenie.Checked = true;
            this.chOdwrocZaznaczenie.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chOdwrocZaznaczenie.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chOdwrocZaznaczenie.Location = new System.Drawing.Point(3, 3);
            this.chOdwrocZaznaczenie.Name = "chOdwrocZaznaczenie";
            this.chOdwrocZaznaczenie.Size = new System.Drawing.Size(34, 45);
            this.chOdwrocZaznaczenie.TabIndex = 6;
            this.chOdwrocZaznaczenie.UseVisualStyleBackColor = true;
            this.chOdwrocZaznaczenie.CheckedChanged += new System.EventHandler(this.chOdwrocZaznaczenie_CheckedChanged);
            // 
            // LabelTimera
            // 
            this.LabelTimera.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelTimera.AutoSize = true;
            this.LabelTimera.Location = new System.Drawing.Point(739, 531);
            this.LabelTimera.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.LabelTimera.Name = "LabelTimera";
            this.LabelTimera.Size = new System.Drawing.Size(130, 13);
            this.LabelTimera.TabIndex = 7;
            this.LabelTimera.Text = "Czas do wyłączenia (min):";
            this.LabelTimera.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.LabelTimera.Visible = false;
            // 
            // CzasTimera
            // 
            this.CzasTimera.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CzasTimera.AutoSize = true;
            this.CzasTimera.Location = new System.Drawing.Point(941, 531);
            this.CzasTimera.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.CzasTimera.Name = "CzasTimera";
            this.CzasTimera.Size = new System.Drawing.Size(0, 13);
            this.CzasTimera.TabIndex = 8;
            this.CzasTimera.Visible = false;
            // 
            // panel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 2);
            this.panel1.Controls.Add(this.lblWersja);
            this.panel1.Controls.Add(this.labelLicencja);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(286, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(655, 45);
            this.panel1.TabIndex = 12;
            // 
            // lblWersja
            // 
            this.lblWersja.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWersja.AutoSize = true;
            this.lblWersja.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblWersja.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblWersja.Location = new System.Drawing.Point(3, 23);
            this.lblWersja.Name = "lblWersja";
            this.lblWersja.Size = new System.Drawing.Size(51, 20);
            this.lblWersja.TabIndex = 11;
            this.lblWersja.Text = "label1";
            // 
            // labelLicencja
            // 
            this.labelLicencja.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelLicencja.AutoSize = true;
            this.labelLicencja.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelLicencja.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.labelLicencja.Location = new System.Drawing.Point(3, 0);
            this.labelLicencja.Name = "labelLicencja";
            this.labelLicencja.Size = new System.Drawing.Size(51, 20);
            this.labelLicencja.TabIndex = 9;
            this.labelLicencja.Text = "label1";
            // 
            // TimerWylaczajacy
            // 
            this.TimerWylaczajacy.Interval = 1000;
            this.TimerWylaczajacy.Tick += new System.EventHandler(this.TimerWylaczajacy_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 554);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MainForm";
            this.Text = "SolEx B2B - Moduł synchronizacji";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RichTextBox logText;
        private System.Windows.Forms.FlowLayoutPanel panelZadan;
        private System.Windows.Forms.CheckBox tylkoPlanowe;
        private System.Windows.Forms.Button btnUruchom;
        private System.Windows.Forms.CheckBox chOdwrocZaznaczenie;
        private System.Windows.Forms.Label LabelTimera;
        private System.Windows.Forms.Timer TimerWylaczajacy;
        private System.Windows.Forms.Label CzasTimera;
        private System.Windows.Forms.Label labelLicencja;
        private System.Windows.Forms.Label lblWersja;
        private System.Windows.Forms.Panel panel1;


    }
}

