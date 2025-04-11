using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UncleCode.Analyzer;

internal static class UsingDirectivePriorityManager
{
	public static int GetUsingDirectivePriority(UsingDirectiveSyntax usingDirective)
	{
		var name = usingDirective.Name.ToString();

		if (name.StartsWith("System")) return 0;

		if (name.StartsWith("Microsoft.")) return 1;

		if (name.StartsWith("Gems.") || name.StartsWith("Grad.")) return 3;

		return 2;
	}
}