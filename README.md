# dotpath

`dotpath` is a developer-focused command-line prompt app built for fast navigation, route shortcuts, and persistent path management. It gives developers a lightweight alternative to manually typing long folder paths, while also supporting command execution, developer-mode feedback, and saved shortcuts that remain available across sessions.

At its core, `dotpath` acts like a smart path-aware shell: you can save commonly used workspaces, jump to them quickly, and keep your workflow efficient inside a terminal-first environment. It is designed for people who work with multiple projects, tools, and directories and want a cleaner, faster way to move around the filesystem.

## Key features

- Persistent path storage in a local JSON file
- Smart path shortcuts for commonly used directories
- Command-line prompt interface for fast interactions
- Developer mode for extra visibility into command output and execution flow
- Built for Windows-based developer workflows
- Minimal, lightweight terminal experience focused on productivity

## What the app does

`dotpath` lets you store favorite paths and then access them from a command prompt environment without retyping long directory names every time. Instead of repeatedly navigating to the same repositories, tools, or project folders, you save those paths once and reuse them when needed.

The tool is especially useful for developers who work in multiple directories such as:

- source code repositories
- build or deployment folders
- project assets and tools
- environment folders
- frequently used scripts and utilities

It helps reduce friction, keeps the terminal workflow faster, and makes project switching more direct.

## Requirements

- .NET SDK 10.0 or newer
- Windows 10/11
- PowerShell or Command Prompt

## Build the app

From the project folder:

```powershell
cd "D:\AhmadAlDibo-WORKSPACE\MyTools\Aplicatios-software-tools\Console-Aplications\Short-Paths"
dotnet restore
dotnet build
```

Build output is placed here:

```text
bin\Debug\dotpath\
```

For a release build:

```powershell
dotnet build -c Release
```

Release output:

```text
bin\Release\dotpath\
```

## Path storage behavior

The application stores saved paths in a local data file inside its output runtime folder. This allows the tool to remember shortcuts across runs without needing a separate database or external service.

In practice, the path storage feature means your saved directories can persist in a structured, simple format and still be reused whenever you launch the command prompt environment.

## Where to move the .NET SDK folder

If you downloaded or extracted the .NET SDK manually, place it in a stable location such as:

```text
C:\Tools\dotnet
```

or, if you are installing it in the standard Windows location:

```text
C:\Program Files\dotnet
```

Choose a location that will not change over time, because the PATH variable points directly to it.

## Add .NET to the system environment

### Option 1: Add to the system PATH (recommended)

1. Open Environment Variables.
2. Edit the system or user `Path` variable.
3. Add one of the following paths:

```text
C:\Tools\dotnet
```

or:

```text
C:\Program Files\dotnet
```

4. Save and close.
5. Reopen the terminal.

### Option 2: Temporary session update in PowerShell

```powershell
$env:Path += ";C:\Tools\dotnet"
```

### Option 3: Persistent update for your user account

```powershell
[Environment]::SetEnvironmentVariable(
  "Path",
  $env:Path + ";C:\Tools\dotnet",
  "User"
)
```

## Verify the installation

Run:

```powershell
dotnet --info
```

If the command shows the installed SDK details, the environment is configured correctly.

## Run the app

From the project folder:

```powershell
dotnet run
```

Or run the built executable directly:

```powershell
.\bin\Debug\dotpath\dotpath.exe
```

## Why this tool is useful for developers

`dotpath` is not just a path shortcut tool; it is built to support a developer-first workflow:

- faster movement between repositories and folders
- reduced command-line friction
- persistent access to important project locations
- easier path reuse during builds, debugging, and deployments
- a lightweight custom shell experience without heavy setup

This makes it valuable for developers, engineers, and power users who spend a lot of time navigating filesystem-heavy environments.

## Notes

- The output folder is intentionally named `dotpath` instead of the default framework folder naming to keep the application identity clear.
- If the .NET SDK location changes after setup, update the PATH value to match the new folder.
- If a terminal reports that `dotnet` is not recognized, reopen it after updating the environment variables.
