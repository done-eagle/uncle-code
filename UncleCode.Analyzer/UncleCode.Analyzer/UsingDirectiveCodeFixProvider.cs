using System.Linq;
using System.Threading;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UncleCode.Analyzer
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UsingDirectiveCodeFixProvider)), Shared]
	internal sealed class UsingDirectiveCodeFixProvider : CodeFixProvider
	{
		public override System.Collections.Immutable.ImmutableArray<string> FixableDiagnosticIds =>
			System.Collections.Immutable.ImmutableArray.Create("UDA001");

		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			var diagnostic = context.Diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;

			if (root != null && root.FindNode(diagnosticSpan) is not UsingDirectiveSyntax) 
				return;

			context.RegisterCodeFix(
				CodeAction.Create(
					title: "Sort using directives",
					createChangedDocument: c => SortUsingsAsync(context.Document, root!, c),
					equivalenceKey: "SortUsings"),
				diagnostic);
		}

		private static Task<Document> SortUsingsAsync(Document document, SyntaxNode root, CancellationToken cancellationToken)
		{
			var compilationUnit = (CompilationUnitSyntax)root;
			var sortedUsings = compilationUnit.Usings
				.OrderBy(UsingDirectivePriorityManager.GetUsingDirectivePriority)
				.ToList();

			var newRoot = compilationUnit.WithUsings(SyntaxFactory.List(sortedUsings));
			return Task.FromResult(document.WithSyntaxRoot(newRoot));
		}
	}
}