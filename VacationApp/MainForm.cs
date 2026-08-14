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

            // Mitarbeiter Menü
            var menuMitarbeiter = new ToolStripMenuItem("Mitarbeiter");
            var menuOpen = new ToolStripMenuItem("Verwalten");
            menuOpen.Click += (s, e) =>
            {
                using var f = new Forms.EmployeesForm();
                f.ShowDialog(this);
            };
            menuMitarbeiter.DropDownItems.Add(menuOpen);
            menu.Items.Add(menuMitarbeiter);

            // Optionen Menü (mit Untereinträgen: Einstellungen (Kennzahlen) und Abteilungen)
            var menuOptions = new ToolStripMenuItem("Optionen");

            var menuSettings = new ToolStripMenuItem("Einstellungen");
            menuSettings.Click += (s, e) =>
            {
                using var f = new Forms.OptionsForm();
                f.ShowDialog(this);
            };
            menuOptions.DropDownItems.Add(menuSettings);

            var menuDepartments = new ToolStripMenuItem("Abteilungen");
            menuDepartments.Click += (s, e) =>
            {
                using var f = new Forms.DepartmentsForm();
                f.ShowDialog(this);
            };
            menuOptions.DropDownItems.Add(menuDepartments);

            menu.Items.Add(menuOptions);

            menu.Dock = DockStyle.Top;
            this.MainMenuStrip = menu;
            // add menu before other controls to ensure z-order
            this.Controls.Add(menu);
            menu.BringToFront();
        }
    }
}