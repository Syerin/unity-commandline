# CommandLineSystem

A small command-line argument system I use across my projects. It reads the process launch args once and
hands them to whatever subsystems care — logging, graphics, cheats, whatever — through a tiny module
interface. Windows-style `/switch` and `/switch:value`. No engine reference, no dependencies, so it works
in Unity and in plain C# alike.

> Status: personal tooling, best-effort support.

## Install

Package Manager ▸ **+** ▸ *Add package from git URL*:

```
https://github.com/syerin/unity-commandline.git
```

Pin with a tag: `…/unity-commandline.git#v1.0.0`.

## Usage

Implement `ICliModule` per subsystem — declare its `Name` and `Commands` (for `/help`), and apply args in
`Bind` — then initialize once at startup:

```csharp
using CommandLineSystem;
using System.Collections.Generic;

public sealed class GraphicsCliModule : ICliModule
{
    public string Name => "graphics";
    public IReadOnlyList<CliCommand> Commands { get; } = new[]
    {
        new CliCommand("/width:<n>", "render width"),
        new CliCommand("/vsync",     "enable vsync"),
    };

    public void Bind(CliArgs args)
    {
        if (args.Get("width") is string w && int.TryParse(w, out int width)) { /* … */ }
        if (args.Has("vsync")) { /* … */ }
    }
}

// once, on boot:
Cli.Initialize(new GraphicsCliModule() /*, other modules … */);
//   App.exe /width:1920 /vsync
```

`Cli.Initialize` parses `Environment.GetCommandLineArgs()` and calls every module's `Bind`. The parser is
also reusable on its own — `CliArgs.Parse(tokens)` — for example to feed an in-game console the same
grammar as launch args.

### /help is built in

`/help` and `/?` belong to this package, not to any subsystem. They list the commands of **every**
registered module (graphics, logging, whatever you pass to `Initialize`). At launch, `/help` prints the
list to stdout; for an in-game console, call `Cli.GetUsage()` to show the same text. Registering the same
command from two modules — or trying to shadow `/help` — throws `InvalidOperationException` at
`Initialize`, so collisions surface immediately instead of silently winning.

### Grammar

```
/flag                 present/absent          -> args.Has("flag")
/key:value            single value            -> args.Get("key")
/key:a,b,c            comma list              -> args.GetValues("key")
```

Switch names are case-insensitive.

## Why separate from logging

This knows nothing about logging. Logging happens to be one consumer that ships a module for it. Keeping
it standalone means projects that only want argument parsing don't pull a logger, and vice versa.

## License

MIT — see [LICENSE](LICENSE).
