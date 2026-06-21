using System;
using System.Collections.Generic;
using System.Text;

namespace CommandLineSystem
{
	/// <summary>
	/// A subsystem that consumes command-line switches. It also declares its commands so the app-wide
	/// <c>/help</c> can list them and so the receiver can reject collisions.
	/// </summary>
	public interface ICliModule
	{
		/// <summary>Group label shown in /help (e.g. "log", "graphics").</summary>
		string Name { get; }

		/// <summary>The commands this module handles, for /help and conflict detection.</summary>
		IReadOnlyList<CliCommand> Commands { get; }

		/// <summary>Apply the parsed args.</summary>
		void Bind(CliArgs args);
	}

	/// <summary>
	/// The single, app-wide command receiver. Parse once at boot, then hand the parsed args to every
	/// registered module. <c>/help</c> and <c>/?</c> are built in here — they list the commands of ALL
	/// registered modules, not any one subsystem. Registering the same command from two modules (or
	/// shadowing /help) is an error.
	/// </summary>
	public static class Cli
	{
		public static CliArgs Args { get; private set; } = CliArgs.Parse(Array.Empty<string>());
		public static IReadOnlyList<ICliModule> Modules { get; private set; } = Array.Empty<ICliModule>();

		private static string _usage = "";

		public static void Initialize(params ICliModule[] modules)
			=> Initialize(Environment.GetCommandLineArgs(), 1, modules);

		public static void Initialize(string[] argv, int start, params ICliModule[] modules)
		{
			modules = modules ?? Array.Empty<ICliModule>();
			Validate(modules);                 // throws on a duplicate command or a shadowed /help
			Modules = modules;
			_usage = BuildUsage(modules);

			Args = CliArgs.Parse(argv, start);
			if (IsHelpRequested(Args)) Console.WriteLine(_usage);   // launch-time /help -> stdout

			for (int i = 0; i < modules.Length; i++)
				modules[i]?.Bind(Args);
		}

		/// <summary>True if /help or /? was passed.</summary>
		public static bool IsHelpRequested(CliArgs args) => args.Has("help") || args.Has("?");

		/// <summary>The aggregated usage for every registered module — for an in-game console's /help.</summary>
		public static string GetUsage() => _usage;

		private static void Validate(ICliModule[] modules)
		{
			var owners = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				["help"] = "(built-in)",
				["?"] = "(built-in)",
			};

			foreach (ICliModule m in modules)
			{
				if (m?.Commands == null) continue;
				foreach (CliCommand c in m.Commands)
				{
					string key = KeyOf(c.Syntax);
					if (key.Length == 0) continue;
					if (owners.TryGetValue(key, out string owner))
						throw new InvalidOperationException(
							"CommandLine: '/" + key + "' is already registered by " + owner +
							"; module '" + (m.Name ?? "(module)") + "' cannot register it again.");
					owners[key] = m.Name ?? "(module)";
				}
			}
		}

		private static string BuildUsage(ICliModule[] modules)
		{
			var sb = new StringBuilder("Available commands:\n");
			foreach (ICliModule m in modules)
			{
				if (m?.Commands == null || m.Commands.Count == 0) continue;
				sb.Append('\n').Append("  ").Append(m.Name ?? "(module)").Append('\n');
				foreach (CliCommand c in m.Commands)
					sb.Append("    ").Append(Pad(c.Syntax)).Append(c.Description).Append('\n');
			}
			sb.Append('\n').Append("    ").Append(Pad("/help, /?")).Append("show this list").Append('\n');
			return sb.ToString();
		}

		// the switch key from a syntax string: "/log.level:<level>" -> "log.level"
		private static string KeyOf(string syntax)
		{
			if (string.IsNullOrEmpty(syntax)) return "";
			string s = syntax.TrimStart('/');
			int cut = s.IndexOfAny(new[] { ':', ' ', '\t', '<' });
			return (cut >= 0 ? s.Substring(0, cut) : s).Trim();
		}

		private static string Pad(string s)
		{
			const int width = 26;
			return s.Length >= width ? s + "  " : s + new string(' ', width - s.Length);
		}
	}
}
