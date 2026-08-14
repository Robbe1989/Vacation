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
        using var f = new EmployeesForm();
        f.ShowDialog(this);
    };
    menuMitarbeiter.DropDownItems.Add(menuOpen);
    menu.Items.Add(menuMitarbeiter);

    // Dock oben und als Hauptmenü setzen
    menu.Dock = DockStyle.Top;
    this.MainMenuStrip = menu;

    // Controls.Add hinterher einfügen und sicher stellen, dass das Menu im Vordergrund ist
    this.Controls.Add(menu);
    this.Controls.SetChildIndex(menu, 0); // Menu zuerst in Controls-Zugreihenfolge
    menu.BringToFront();

    // Falls es ein Title-Label gibt, verschiebe es unter das Menu (robuste Erkennung)
    var title = this.Controls.OfType<System.Windows.Forms.Label>()
                  .FirstOrDefault(l => (l.Text ?? "").StartsWith("Vacation Planner", StringComparison.OrdinalIgnoreCase));
    if (title != null)
    {
        title.Top = menu.Height + 6;
        title.BringToFront(); // falls du willst, dass Titel sichtbar bleibt
    }
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
