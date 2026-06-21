namespace CommandLineSystem
{
	/// <summary>
	/// One command a module declares — used to build <c>/help</c> and to detect collisions. Syntax is the
	/// display form (e.g. "/log.level:&lt;level&gt;"); the switch key ("log.level") is derived from it.
	/// </summary>
	public readonly struct CliCommand
	{
		public readonly string Syntax;
		public readonly string Description;

		public CliCommand(string syntax, string description)
		{
			Syntax = syntax;
			Description = description;
		}
	}
}
