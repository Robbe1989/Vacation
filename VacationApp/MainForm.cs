using System;
using System.Windows.Forms;
using VacationApp.Forms;
using System.Drawing;
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

            // Optionen Menü
            var menuOptions = new ToolStripMenuItem("Optionen");
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
            this.Controls.Add(menu);
            menu.BringToFront();
        }
    }
}