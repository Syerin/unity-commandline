# CommandLineSystem

A small command-line argument system I use across my projects. It reads the process launch args once and
hands them to whatever subsystems care — logging, graphics, cheats, whatever — through a tiny module
interface. Windows-style `/switch` and `/switch:value`. No engine reference, no dependencies, so it works
in Unity and in plain C# alike.

> Status: personal tooling, best-effort support.

## Install

Package Manager ▸ **+** ▸ *Add package from git URL*:

```
https://github.com/yourname/unity-commandline.git
```

Pin with a tag: `…/unity-commandline.git#v1.0.0`.

## Usage

Implement `ICliModule` per subsystem, then initialize once at startup:

```csharp
using CommandLineSystem;

public sealed class GraphicsCliModule : ICliModule
{
    public void Bind(CliArgs args)
    {
        if (args.Get("width")  is string w && int.TryParse(w, out int width))  { /* … */ }
        if (args.Has("vsync"))  { /* … */ }
    }
}

// once, on boot:
Cli.Initialize(new GraphicsCliModule() /*, other modules … */);
//   App.exe /width:1920 /vsync
```

`Cli.Initialize` parses `Environment.GetCommandLineArgs()` and calls every module's `Bind`. The parser is
also reusable on its own — `CliArgs.Parse(tokens)` — for example to feed an in-game console the same
grammar as launch args.

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
