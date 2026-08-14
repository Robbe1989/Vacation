using System;
using System.Windows.Forms;

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
                using var f = new Forms.EmployeesForm();
                f.ShowDialog(this);
            };
            menuMitarbeiter.DropDownItems.Add(menuOpen);
            menu.Items.Add(menuMitarbeiter);

            var menuDepartments = new ToolStripMenuItem("Abteilungen");
            menuDepartments.Click += (s, e) =>
            {
                using var f = new Forms.DepartmentsForm();
                f.ShowDialog(this);
            };
            menu.Items.Add(menuDepartments);

            menu.Dock = DockStyle.Top;
            this.MainMenuStrip = menu;
            this.Controls.Add(menu);
            menu.BringToFront();
        }
    }
}