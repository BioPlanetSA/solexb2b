namespace SolEx.Hurt.Sync.App.Controls
{
    partial class ZadanieKontrolka
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.chbox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.nazwa = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chbox
            // 
            this.chbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.chbox.Location = new System.Drawing.Point(3, 3);
            this.chbox.Name = "chbox";
            this.chbox.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.chbox.Size = new System.Drawing.Size(24, 24);
            this.chbox.TabIndex = 7;
            this.chbox.UseVisualStyleBackColor = true;
            this.chbox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.chbox_MouseClick);
            this.chbox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chbox_MouseDown);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.chbox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.nazwa, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(300, 30);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // nazwa
            // 
            this.nazwa.AutoSize = true;
            this.nazwa.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nazwa.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.nazwa.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.nazwa.Location = new System.Drawing.Point(33, 0);
            this.nazwa.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.nazwa.Name = "nazwa";
            this.nazwa.Size = new System.Drawing.Size(267, 30);
            this.nazwa.TabIndex = 8;
            this.nazwa.Text = "label1";
            this.nazwa.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.nazwa.Click += new System.EventHandler(this.nazwa_Click);
            this.nazwa.MouseClick += new System.Windows.Forms.MouseEventHandler(this.nazwa_MouseClick);
            // 
            // ZadanieKontrolka
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(200, 30);
            this.Name = "ZadanieKontrolka";
            this.Size = new System.Drawing.Size(300, 30);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chbox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label nazwa;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
