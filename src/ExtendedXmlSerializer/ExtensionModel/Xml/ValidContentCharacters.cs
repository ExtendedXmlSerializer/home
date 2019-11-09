using System.Text.RegularExpressions;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	// ATTRIBUTION: https://stackoverflow.com/a/961504/3602057
	public sealed class ValidContentCharacters : IAlteration<string>
	{
		public static ValidContentCharacters Default { get; } = new ValidContentCharacters();

		ValidContentCharacters()
			: this(new
				       Regex(@"(?<![\uD800-\uDBFF])[\uDC00-\uDFFF]|[\uD800-\uDBFF](?![\uDC00-\uDFFF])|[\x00-\x08\x0B\x0C\x0E-\x1F\x7F-\x9F\uFEFF\uFFFE\uFFFF]",
				             RegexOptions.Compiled)) {}

		readonly Regex _expression;

		public ValidContentCharacters(Regex expression) => _expression = expression;

		public string Get(string parameter) =>
			string.IsNullOrEmpty(parameter) ? string.Empty : _expression.Replace(parameter, string.Empty);
	}
}