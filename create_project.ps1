# create_project.ps1
# In: im Verzeichnis Vacation (nach git clone)
# Führt aus: .\create_project.ps1

Set-StrictMode -Version Latest

# Helper to write files
function Write-TextFile($path, $content) {
    $dir = Split-Path $path
    if ($dir -and -not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
    $content | Out-File -FilePath $path -Encoding UTF8 -Force
}

# .gitignore
Write-TextFile ".gitignore" @'
# Build folders
/bin/
/obj/

# VS files
.vs/
*.user
*.suo

# Rider
.idea/

# NuGet
*.nupkg
'@

# README.md
Write-TextFile "README.md" @'
# Vacation - Urlaubsplaner (Initial Commit)

Dieses Repository enthält ein minimales Windows Forms (WinForms) Projekt in C# (.NET 7) als Startpunkt für den Urlaubsplaner. Eine GitHub Actions Workflow baut bei jedem Push eine Windows-Release und lädt die veröffentlichte .exe als Release-Asset hoch.

Was ist enthalten:
- VacationApp: minimaler WinForms-Prototyp (Platzhalter UI)
- .github/workflows/build-windows.yml: CI, die eine Windows Single-File .exe baut und als Release hochlädt

Wie du eine .exe bekommst:
1. Push oder Merge in den main-Branch (oder löse den Workflow manuell unter Actions aus).
2. Die Action baut die App und erstellt ein Release mit der gebauten .exe als Asset. Lade die .exe über die Release-Seite herunter.

Weiteres:
Ich baue die vollständige Anwendung (Mitarbeiterverwaltung, Feiertage, Ferien, Excel-Import/Export, Statistik) als nächsten Schritt auf dieses Grundgerüst auf, wenn du das möchtest.
'@

# VacationApp.csproj
Write-TextFile "VacationApp/VacationApp.csproj" @'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net7.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Nager.Date" Version="3.0.2" />
    <PackageReference Include="ClosedXML" Version="0.103.1" />
    <PackageReference Include="System.Data.SQLite.Core" Version="1.0.118" />
  </ItemGroup>
</Project>
'@

# Program.cs
Write-TextFile "VacationApp/Program.cs" @'
using System;
using System.Windows.Forms;

namespace VacationApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
'@

# MainForm.cs
Write-TextFile "VacationApp/MainForm.cs" @'
using System;
using System.Windows.Forms;

namespace VacationApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
    }
}
'@

# MainForm.Designer.cs
Write-TextFile "VacationApp/MainForm.Designer.cs" @'
namespace VacationApp
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(12, 9);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(343, 25);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Vacation Planner - Platzhalter UI";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.labelTitle);
            this.Name = "MainForm";
            this.Text = "Urlaubsplaner";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
    }
}
'@

# GitHub Actions workflow
Write-TextFile ".github/workflows/build-windows.yml" @'
name: Build Windows single-file .exe and create Release
on:
  push:
    branches: [ main ]
  workflow_dispatch:

jobs:
  build:
    runs-on: windows-latest

    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: "7.0.x"

      - name: Restore
        run: dotnet restore VacationApp/VacationApp.csproj

      - name: Publish single-file executable (win-x64)
        run: dotnet publish VacationApp/VacationApp.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:EnableCompressionInSingleFile=false -o ${{ github.workspace }}\publish

      - name: List publish folder
        run: dir ${{ github.workspace }}\publish

      - name: Create Release
        id: create_release
        uses: actions/create-release@v1
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        with:
          tag_name: v${{ github.run_number }}
          release_name: "Windows build ${{ github.run_number }}"
          draft: false
          prerelease: false

      - name: Upload Release Asset
        uses: actions/upload-release-asset@v1
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        with:
          upload_url: ${{ steps.create_release.outputs.upload_url }}
          asset_path: ./publish/VacationApp.exe
          asset_name: VacationApp-win-x64.exe
          asset_content_type: application/octet-stream
'@

# LICENSE
Write-TextFile "LICENSE" @'
MIT License

Copyright (c) 2026 Robbe1989

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
'@

# ROADMAP
Write-TextFile "VacationApp/ROADMAP.md" @'
# Initial placeholder for the Urlaubsplaner application.

Dies ist ein minimaler Startpunkt. Nächste Schritte (auf Wunsch):
- Implementierung Mitarbeiterverwaltung (CRUD)
- Kalender-Ansicht mit Monats-/Jahreswechsel
- Feiertage (Nager.Date) und Schulferien-Daten für Baden-Württemberg
- Farbliche Markierung und Legende
- Excel Import/Export (ClosedXML)
- Lokale Speicherung (SQLite)
'@

# Git commit and push
git add -A
git commit -m "Initial commit: minimal WinForms app + GitHub Actions build workflow" -q
git push origin main -q

Write-Host "Fertig: Dateien erstellt, committed und gepusht (falls du Push-Rechte hast). Falls ein Fehler beim Push auftritt, prüfe deine Git-Zugangsdaten und Branch-Rechte."