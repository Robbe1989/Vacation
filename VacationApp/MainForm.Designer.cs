namespace VacationApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblYear = new System.Windows.Forms.Label();
            this.nudYear = new System.Windows.Forms.NumericUpDown();
            this.btnManageVacations = new System.Windows.Forms.Button();
            
            // Monats-Button Panel
            this.monthButtonPanel = new System.Windows.Forms.FlowLayoutPanel();

            this.panelMonthHeader = new System.Windows.Forms.Panel();
            this.dgvCalendar = new System.Windows.Forms.DataGridView();

            ((System.ComponentModel.ISupportInitialize)(this.nudYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCalendar)).BeginInit();
            this.SuspendLayout();

            // menuStrip1
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1200, 24);

            // panelTop
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 40;
            this.panelTop.Padding = new System.Windows.Forms.Padding(8);
            this.panelTop.Controls.Add(this.lblYear);
            this.panelTop.Controls.Add(this.nudYear);
            this.panelTop.Controls.Add(this.btnManageVacations);
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(160, 160, 160);

            // lblYear
            this.lblYear.Text = "Jahr:";
            this.lblYear.AutoSize = true;
            this.lblYear.Location = new System.Drawing.Point(12, 10);
            this.lblYear.ForeColor = System.Drawing.Color.White;

            // nudYear
            this.nudYear.Minimum = 2000;
            this.nudYear.Maximum = 2100;
            this.nudYear.Value = System.DateTime.Now.Year;
            this.nudYear.Location = new System.Drawing.Point(60, 6);
            this.nudYear.Size = new System.Drawing.Size(80, 22);
            this.nudYear.Name = "nudYear";

            // btnManageVacations
            this.btnManageVacations.Text = "Urlaube verwalten";
            this.btnManageVacations.Location = new System.Drawing.Point(160, 6);
            this.btnManageVacations.Size = new System.Drawing.Size(140, 26);
            this.btnManageVacations.Name = "btnManageVacations";
            this.btnManageVacations.BackColor = System.Drawing.Color.FromArgb(253, 253, 253);
            this.btnManageVacations.ForeColor = System.Drawing.Color.Black;
            this.btnManageVacations.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManageVacations.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(200, 200, 200);
            this.btnManageVacations.FlatAppearance.BorderSize = 1;

            // monthButtonPanel
            this.monthButtonPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.monthButtonPanel.Height = 40;
            this.monthButtonPanel.AutoScroll = true;
            this.monthButtonPanel.BackColor = System.Drawing.Color.FromArgb(160, 160, 160);
            this.monthButtonPanel.Padding = new System.Windows.Forms.Padding(5);
            this.monthButtonPanel.Name = "monthButtonPanel";

            // Monats-Buttons erstellen
            string[] monthNames = { "Januar", "Februar", "März", "April", "Mai", "Juni",
                                    "Juli", "August", "September", "Oktober", "November", "Dezember" };
            
            this.monthButtons = new System.Windows.Forms.Button[12];
            for (int i = 0; i < 12; i++)
            {
                this.monthButtons[i] = new System.Windows.Forms.Button();
                this.monthButtons[i].Text = monthNames[i];
                this.monthButtons[i].Width = 75;
                this.monthButtons[i].Height = 30;
                this.monthButtons[i].BackColor = System.Drawing.Color.FromArgb(253, 253, 253);
                this.monthButtons[i].ForeColor = System.Drawing.Color.Black;
                this.monthButtons[i].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.monthButtons[i].FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(200, 200, 200);
                this.monthButtons[i].FlatAppearance.BorderSize = 1;
                this.monthButtons[i].Margin = new System.Windows.Forms.Padding(2);
                this.monthButtons[i].Name = "btn" + monthNames[i];
                this.monthButtons[i].Tag = i + 1;
                
                int monthIndex = i;
                this.monthButtons[i].Click += (s, e) => this.ScrollToMonth(monthIndex + 1);
                
                this.monthButtonPanel.Controls.Add(this.monthButtons[i]);
            }

            // panelMonthHeader
            this.panelMonthHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMonthHeader.Height = 92;
            this.panelMonthHeader.BackColor = System.Drawing.Color.FromArgb(160, 160, 160);
            this.panelMonthHeader.Name = "panelMonthHeader";

            // dgvCalendar
            this.dgvCalendar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCalendar.Location = new System.Drawing.Point(0, 172);
            this.dgvCalendar.Name = "dgvCalendar";
            this.dgvCalendar.ReadOnly = true;
            this.dgvCalendar.RowHeadersVisible = false;
            this.dgvCalendar.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCalendar.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.dgvCalendar.AllowUserToAddRows = false;
            this.dgvCalendar.AllowUserToDeleteRows = false;
            this.dgvCalendar.AllowUserToOrderColumns = false;
            this.dgvCalendar.AllowUserToResizeColumns = true;
            this.dgvCalendar.AllowUserToResizeRows = false;
            this.dgvCalendar.ColumnHeadersVisible = false;
            this.dgvCalendar.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvCalendar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

            // MainForm
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.BackColor = System.Drawing.Color.FromArgb(160, 160, 160);
            this.Controls.Add(this.dgvCalendar);
            this.Controls.Add(this.panelMonthHeader);
            this.Controls.Add(this.monthButtonPanel);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Urlaubsplaner";

            ((System.ComponentModel.ISupportInitialize)(this.dgvCalendar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudYear)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblYear;
        private System.Windows.Forms.NumericUpDown nudYear;
        private System.Windows.Forms.Button btnManageVacations;

        private System.Windows.Forms.FlowLayoutPanel monthButtonPanel;
        private System.Windows.Forms.Button[] monthButtons;

        private System.Windows.Forms.Panel panelMonthHeader;
        private System.Windows.Forms.DataGridView dgvCalendar;
    }
}
