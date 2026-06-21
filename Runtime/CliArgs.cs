using System;
using System.Collections.Generic;

namespace CommandLineSystem
{
	/// <summary>
	/// Windows-style command line: <c>/switch</c>, <c>/switch:value</c>, <c>/switch:v1,v2,...</c>.
	/// Switch names are case-insensitive; the value is everything after the first ':'.
	/// Tokens that do not start with '/' (e.g. the executable path) are ignored.
	/// </summary>
	public sealed class CliArgs
	{
		private readonly Dictionary<string, string> _switches
			= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		public static CliArgs Parse(string[] argv, int start = 0)
		{
			var args = new CliArgs();
			if (argv == null) return args;

			for (int i = start; i < argv.Length; i++)
			{
				string token = argv[i];
				if (string.IsNullOrEmpty(token) || token[0] != '/') continue;

				string body = token.Substring(1);
				int colon = body.IndexOf(':');
				if (colon < 0)
					args._switches[body] = "";                                  // /flag
				else
					args._switches[body.Substring(0, colon)] = body.Substring(colon + 1); // /key:value
			}
			return args;
		}

		public bool Has(string name) => _switches.ContainsKey(name);

		public bool TryGet(string name, out string value) => _switches.TryGetValue(name, out value);

		public string Get(string name, string fallback = null)
			=> _switches.TryGetValue(name, out string v) && v.Length > 0 ? v : fallback;

		/// <summary>Comma-separated value list, e.g. <c>/log.filter:Network,UI</c>.</summary>
		public string[] GetValues(string name)
			=> _switches.TryGetValue(name, out string v) && v.Length > 0
				? v.Split(',')
				: Array.Empty<string>();
	}
}
