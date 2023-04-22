using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace commonItems.SourceGenerators {
	internal class SerializationByPropertiesReceiver : ISyntaxReceiver {
		private const string AttributeName = "SerializationByProperties";
		private readonly List<ClassDeclarationSyntax> candidates = new List<ClassDeclarationSyntax>();

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
}