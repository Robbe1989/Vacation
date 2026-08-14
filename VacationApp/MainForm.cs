using System;
using System.Windows.Forms;
using VacationApp.Forms;
using System.Drawing;

namespace VacationApp
{
    public partial class MainForm : Form
    {
	private Button btnEmployeesVisible;
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
        using var f = new VacationApp.Forms.EmployeesForm();
        f.ShowDialog(this);
    };
    menuMitarbeiter.DropDownItems.Add(menuOpen);
    menu.Items.Add(menuMitarbeiter);

    var menuSettings = new ToolStripMenuItem("Einstellungen");
    var menuDepartments = new ToolStripMenuItem("Abteilungen");
    menuDepartments.Click += (s, e) =>
    {
        using var f = new VacationApp.Forms.DepartmentsForm();
        f.ShowDialog(this);
    };
    menuSettings.DropDownItems.Add(menuDepartments);
    menu.Items.Add(menuSettings);

    menu.Dock = DockStyle.Top;
    this.MainMenuStrip = menu;
    this.Controls.Add(menu);
    menu.BringToFront();
}

private void CenterEmployeesButton()
{
    if (btnEmployeesVisible == null) return;

    int topOffset = (this.MainMenuStrip != null && this.MainMenuStrip.Visible) ? this.MainMenuStrip.Height + 8 : 40;

    var x = Math.Max(12, (this.ClientSize.Width - btnEmployeesVisible.Width) / 2);
    var y = Math.Max(topOffset, (this.ClientSize.Height - btnEmployeesVisible.Height) / 2);
    btnEmployeesVisible.Location = new Point(x, y);
}
    }
}
