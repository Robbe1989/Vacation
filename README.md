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

## Mitarbeiterverwaltung (MVP)

Dieses Update fügt eine einfache Mitarbeiterverwaltung mit SQLite‑Speicherung hinzu.

- Menü: Mitarbeiter → Verwalten
- SQLite‑Datenbank: vacation.db im Programmordner
- CRUD: Hinzufügen, Bearbeiten, Löschen
