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

    // initialisiere btnEmployeesVisible, falls noch nicht existent
    btnEmployeesVisible = new System.Windows.Forms.Button();
    btnEmployeesVisible.Text = "Mitarbeiter";
    btnEmployeesVisible.Size = new System.Drawing.Size(140, 36);
    btnEmployeesVisible.Click += (s, e) =>
    {
        using var f = new VacationApp.Forms.EmployeesForm();
        f.ShowDialog(this);
    };
    this.Controls.Add(btnEmployeesVisible);
    // positioniere zentriert (nutze deine bestehende Methode, falls vorhanden)
    CenterEmployeesButton();

    AddMenu(); // falls du das Menü dort erzeugst
}

        private void AddMenu()
{
    // MainForm.cs — im AddMenu() oder direkt im Konstruktor nach InitializeComponent()
var menu = new MenuStrip();
// ... erstelle Menü-Items wie gewohnt ...
menu.Dock = DockStyle.Top;
this.MainMenuStrip = menu;

// Controls.Add(menu) möglichst VOR anderen Controls, damit Menu oben in Z bleibt
this.Controls.Add(menu);
// bring menu to front in case other controls overlap
menu.BringToFront();

// Falls es ein grosses Titellabel gibt, verschiebe es unter das Menü:
foreach (Control c in this.Controls)
{
    if (c is Label lbl)
    {
        var txt = (lbl.Text ?? "").ToLowerInvariant();
        if (txt.Contains("vacation planner") || txt.Contains("urlaub") || txt.Contains("platzhalter"))
        {
            // Abstand = menühöhe + 6px
            lbl.Top = menu.Height + 6;
            lbl.BringToFront(); // falls du möchtest dass der Title sichtbar bleibt
            break;
        }
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
