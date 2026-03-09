namespace SolEx.Hurt.Sync.App.Controls
{
    partial class ExportDataControl
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
            this.edLog = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSendPrices = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.bwMain = new System.ComponentModel.BackgroundWorker();
            this.btnCustomers = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // edLog
            // 
            this.edLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.edLog.Location = new System.Drawing.Point(3, 63);
            this.edLog.Name = "edLog";
            this.edLog.Size = new System.Drawing.Size(767, 446);
            this.edLog.TabIndex = 1;
            this.edLog.Text = "";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.edLog, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(773, 512);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCustomers);
            this.panel1.Controls.Add(this.btnSendPrices);
            this.panel1.Controls.Add(this.btnStop);
            this.panel1.Controls.Add(this.btnStart);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(55, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(718, 60);
            this.panel1.TabIndex = 0;
            // 
            // btnSendPrices
            // 
            this.btnSendPrices.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnSendPrices.Image = global::SolEx.Hurt.Sync.App.Icons.Upload_48x48_32;
            this.btnSendPrices.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSendPrices.Location = new System.Drawing.Point(207, 3);
            this.btnSendPrices.Name = "btnSendPrices";
            this.btnSendPrices.Size = new System.Drawing.Size(173, 54);
            this.btnSendPrices.TabIndex = 5;
            this.btnSendPrices.Text = "Eksportuj Ceny";
            this.btnSendPrices.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSendPrices.UseVisualStyleBackColor = true;
            this.btnSendPrices.Click += new System.EventHandler(this.btnSendPrices_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnStop.Image = global::SolEx.Hurt.Sync.App.Icons.Symbol_Delete_48x48_32;
            this.btnStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStop.Location = new System.Drawing.Point(565, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(145, 54);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "Zatrzymaj";
            this.btnStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnStart.Image = global::SolEx.Hurt.Sync.App.Icons.Upload_48x48_32;
            this.btnStart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStart.Location = new System.Drawing.Point(386, 3);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(173, 54);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Eksportuj Dane";
            this.btnStart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // bwMain
            // 
            this.bwMain.WorkerReportsProgress = true;
            this.bwMain.WorkerSupportsCancellation = true;
            this.bwMain.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwMain_DoWork);
            this.bwMain.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwMain_ProgressChanged);
            this.bwMain.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwMain_RunWorkerCompleted);
            // 
            // btnCustomers
            // 
            this.btnCustomers.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnCustomers.Image = global::SolEx.Hurt.Sync.App.Icons.Upload_48x48_32;
            this.btnCustomers.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCustomers.Location = new System.Drawing.Point(28, 3);
            this.btnCustomers.Name = "btnCustomers";
            this.btnCustomers.Size = new System.Drawing.Size(173, 54);
            this.btnCustomers.TabIndex = 6;
            this.btnCustomers.Text = "Eksportuj Kontrahentów";
            this.btnCustomers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCustomers.UseVisualStyleBackColor = true;
            this.btnCustomers.Click += new System.EventHandler(this.btnCustomers_Click);
            // 
            // ExportDataControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ExportDataControl";
            this.Size = new System.Drawing.Size(773, 512);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox edLog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.ComponentModel.BackgroundWorker bwMain;
        private System.Windows.Forms.Button btnSendPrices;
        private System.Windows.Forms.Button btnCustomers;

    }
}
