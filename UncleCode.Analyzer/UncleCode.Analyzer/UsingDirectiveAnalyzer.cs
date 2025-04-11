using System.Linq;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UncleCode.Analyzer
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	internal sealed class UsingDirectiveAnalyzer : DiagnosticAnalyzer
	{
		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
			id: "UDA001",
			title: "Using directives should be sorted and grouped",
			messageFormat: "Using directives are not in the correct order",
			category: "Style",
			DiagnosticSeverity.Warning,
			isEnabledByDefault: true
		);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();
			context.RegisterSyntaxNodeAction(AnalyzeUsings, SyntaxKind.CompilationUnit);
			
		}

		private static void AnalyzeUsings(SyntaxNodeAnalysisContext context)
		{
			var syntaxTree = context.Node.SyntaxTree;

			var prefixes = LoadCorporatePrefixes(context.Options, syntaxTree);

			UsingDirectiveManager.SetCorporatePrefixes(prefixes);

			var root = (CompilationUnitSyntax)context.Node;
			var usings = root.Usings;

			if (usings.Count == 0) return;

			var sortedUsings = usings
				.OrderBy(u => (UsingDirectiveManager.GetUsingDirectivePriority(u), u.Name.ToString()))
				.ToList();

			for (var i = 0; i < usings.Count; i++)
			{
				if (!usings[i].IsEquivalentTo(sortedUsings[i]))
				{
					var diagnostic = Diagnostic.Create(Rule, usings[i].GetLocation());
					context.ReportDiagnostic(diagnostic);
				}
			}
		}

		private static string[] LoadCorporatePrefixes(AnalyzerOptions options, SyntaxTree syntaxTree)
		{
			var config = options.AnalyzerConfigOptionsProvider.GetOptions(syntaxTree);

			if (config.TryGetValue("dotnet_sorting_corporate_prefixes", out var rawPrefixes))
			{
				return rawPrefixes.Split(',').Select(p => p.Trim()).ToArray();
			}

			return [];
		}

	}
}