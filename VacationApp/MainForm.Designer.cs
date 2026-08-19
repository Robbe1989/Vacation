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

            this.panelMonthHeader = new System.Windows.Forms.Panel();
            this.dgvCalendar = new System.Windows.Forms.DataGridView();

            ((System.ComponentModel.ISupportInitialize)(this.nudYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCalendar)).BeginInit();
            this.SuspendLayout();

            // menuStrip1 (menu is added in MainForm.AddMenu at runtime)
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

            // lblYear
            this.lblYear.Text = "Jahr:";
            this.lblYear.AutoSize = true;
            this.lblYear.Location = new System.Drawing.Point(12, 10);

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

            // panelMonthHeader (draws month banner and day row)
            this.panelMonthHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMonthHeader.Height = 92; // bigger to fit month banner + day numbers
            this.panelMonthHeader.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelMonthHeader.Name = "panelMonthHeader";

            // dgvCalendar
            this.dgvCalendar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCalendar.Location = new System.Drawing.Point(0, 136);
            this.dgvCalendar.Name = "dgvCalendar";
            this.dgvCalendar.ReadOnly = true;
            this.dgvCalendar.RowHeadersVisible = false;
            this.dgvCalendar.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            // Important for day view: do NOT auto-size columns; allow horizontal scroll
            this.dgvCalendar.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.dgvCalendar.AllowUserToAddRows = false;
            this.dgvCalendar.AllowUserToDeleteRows = false;
            this.dgvCalendar.AllowUserToOrderColumns = false;
            this.dgvCalendar.AllowUserToResizeColumns = true;
            this.dgvCalendar.AllowUserToResizeRows = false;
           this.dgvCalendar.ColumnHeadersVisible = false;
this.dgvCalendar.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
this.dgvCalendar.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTristate.False;
this.dgvCalendar.RowHeadersWidth = 30;
            this.dgvCalendar.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // MainForm
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.dgvCalendar);
            this.Controls.Add(this.panelMonthHeader);
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

        private System.Windows.Forms.Panel panelMonthHeader;
        private System.Windows.Forms.DataGridView dgvCalendar;
    }
}