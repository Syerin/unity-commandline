using System;

namespace CommandLineSystem
{
	/// <summary>A subsystem that consumes command-line switches it cares about.</summary>
	public interface ICliModule
	{
		void Bind(CliArgs args);
	}

	/// <summary>
	/// The single, app-wide command receiver. Parse once at boot, then hand the parsed args to
	/// every registered module. Logging, graphics, networking, cheats... each plug in as a module
	/// without this class ever changing (OCP). It knows nothing about any particular subsystem.
	/// </summary>
	public static class Cli
	{
		public static CliArgs Args { get; private set; } = CliArgs.Parse(Array.Empty<string>());

		/// <summary>Parse the process command line (skipping the executable path) and bind modules.</summary>
		public static void Initialize(params ICliModule[] modules)
			=> Initialize(Environment.GetCommandLineArgs(), 1, modules);

		/// <summary>Explicit argv (testing, or a custom source). <paramref name="start"/> skips leading tokens.</summary>
		public static void Initialize(string[] argv, int start, params ICliModule[] modules)
		{
			Args = CliArgs.Parse(argv, start);
			if (modules == null) return;
			for (int i = 0; i < modules.Length; i++)
				modules[i]?.Bind(Args);
		}
	}
}
