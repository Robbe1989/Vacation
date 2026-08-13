using System;
using System.Windows.Forms;
using VacationApp.Forms;

namespace VacationApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            AddMenu();
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
    }
}
