using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UncleCode.Analyzer;

internal static class UsingDirectiveManager
{
	private static string[] _corporatePrefixes = [];

	public static void SetCorporatePrefixes(string[] prefixes) =>
		_corporatePrefixes = prefixes;

	public static int GetUsingDirectivePriority(UsingDirectiveSyntax usingDirective)
	{
		var name = usingDirective.Name.ToString();

		if (name.StartsWith("System")) return 0;

		if (name.StartsWith("Microsoft.")) return 1;

		if (_corporatePrefixes.Any(p => name.StartsWith(p))) return 3;

		return 2;
	}
}