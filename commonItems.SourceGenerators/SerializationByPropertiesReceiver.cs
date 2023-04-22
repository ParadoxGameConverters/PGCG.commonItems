using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace commonItems.SourceGenerators;

internal class SerializationByPropertiesReceiver : ISyntaxReceiver {
	private const string AttributeName = "SerializationByProperties";
	private readonly List<ClassDeclarationSyntax> candidates = new();

	public IEnumerable<ClassDeclarationSyntax> Candidates => candidates;

	public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
		if (syntaxNode is ClassDeclarationSyntax classSyntax && ClassHasAttribute(classSyntax)) {
			candidates.Add(classSyntax);
		}
	}

	private bool ClassHasAttribute(ClassDeclarationSyntax classDeclaration)
		=> classDeclaration
			.AttributeLists
			.SelectMany(l => l.Attributes.Where(a => (a.Name as IdentifierNameSyntax)?.Identifier.Text == AttributeName))
			.Any();
}