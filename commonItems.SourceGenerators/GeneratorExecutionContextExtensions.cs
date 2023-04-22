using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace commonItems.SourceGenerators; 

public static class GeneratorExecutionContextExtensions {
	private static readonly DiagnosticDescriptor MissingPartialModifier = new DiagnosticDescriptor(
		id: "CISG001",
		title: "Missing partial modifier",
		messageFormat: "A partial modifier is required, Serialize method implementation will not be generated",
		category: "commonItems.SourceGenerators",
		DiagnosticSeverity.Warning,
		isEnabledByDefault: true);

	public static void ReportMissingPartialModifier(
		this GeneratorExecutionContext context,
		ClassDeclarationSyntax classDeclaration)
		=> context.ReportDiagnostic(
			Diagnostic.Create(
				MissingPartialModifier,
				classDeclaration.GetLocation()));
}