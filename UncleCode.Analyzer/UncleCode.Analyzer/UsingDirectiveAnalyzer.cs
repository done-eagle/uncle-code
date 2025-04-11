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
			var root = (CompilationUnitSyntax)context.Node;
			var usings = root.Usings;

			if (usings.Count == 0) return;

			var sortedUsings = usings.OrderBy(UsingDirectivePriorityManager.GetUsingDirectivePriority).ToList();

			for (var i = 0; i < usings.Count; i++)
			{
				if (!usings[i].IsEquivalentTo(sortedUsings[i]))
				{
					var diagnostic = Diagnostic.Create(Rule, usings[i].GetLocation());
					context.ReportDiagnostic(diagnostic);
				}
			}
		}
	}
}