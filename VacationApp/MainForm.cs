using System;
using System.Drawing;
using System.Windows.Forms;
using VacationApp.Forms;

namespace VacationApp
{
    public partial class MainForm : Form
    {
        private Button btnEmployeesVisible;

        public MainForm()
        {
            InitializeComponent();
            AddMenu();
            AddEmployeesButton();
        }

        private void AddMenu()
        {
            var menu = new MenuStrip();
            var menuMitarbeiter = new ToolStripMenuItem("Mitarbeiter");
            var menuOpen = new ToolStripMenuItem("Verwalten");
            menuOpen.Click += (s, e) =>
            {
                using var f = new EmployeesForm();
                f.ShowDialog(this);
            };
            menuMitarbeiter.DropDownItems.Add(menuOpen);
            menu.Items.Add(menuMitarbeiter);
            this.MainMenuStrip = menu;
            this.Controls.Add(menu);
            menu.Dock = DockStyle.Top;
        }

        private void AddEmployeesButton()
        {
            btnEmployeesVisible = new Button
            {
                Text = "Mitarbeiter verwalten",
                Size = new Size(220, 44),
                Anchor = AnchorStyles.None
            };
            btnEmployeesVisible.Click += (s, e) =>
            {
                using var f = new EmployeesForm();
                f.ShowDialog(this);
            };
            this.Controls.Add(btnEmployeesVisible);

            // Positionieren und bei Resize mittig halten
            this.Load += (s, e) => CenterEmployeesButton();
            this.Resize += (s, e) => CenterEmployeesButton();
        }

        private void CenterEmployeesButton()
        {
            if (btnEmployeesVisible == null) return;
            var x = Math.Max(12, (this.ClientSize.Width - btnEmployeesVisible.Width) / 2);
            var y = Math.Max(40, (this.ClientSize.Height - btnEmployeesVisible.Height) / 2); // 40 so it's below menu/title
            btnEmployeesVisible.Location = new Point(x, y);
        }
    }
}