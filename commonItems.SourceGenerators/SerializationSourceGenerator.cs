using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace commonItems.SourceGenerators {
	[Generator]
	public class SerializationSourceGenerator : ISourceGenerator {
		public void Execute(GeneratorExecutionContext context) {
			// Code generation goes here.

			// Generate SerializationByPropertiesAttribute.
			const string attribute = @"
				using System;
				namespace commonItems.SourceGenerators {
					[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
					public class SerializationByPropertiesAttribute : Attribute { }
				}
			";
			context.AddSource("SerializationByPropertiesAttribute.cs", SourceText.From(attribute, Encoding.UTF8));

			if (context.SyntaxReceiver is SerializationByPropertiesReceiver actorSyntaxReceiver) {
				foreach (ClassDeclarationSyntax candidate in actorSyntaxReceiver.Candidates) {
					var code = GenerateMethodImplementationForClass(candidate, context.Compilation);

					var className = candidate.Identifier.Text;
					context.AddSource($"{className}SourceGenerated.cs", SourceText.From(code, Encoding.UTF8));
				}
			}
		}
		
		/// Determine the namespace the class/enum/struct is declared in, if any.
		private static string GetNamespace(BaseTypeDeclarationSyntax syntax) {
			// If we don't have a namespace at all we'll return an empty string
			// This accounts for the "default namespace" case
			string nameSpace = string.Empty;

			// Get the containing syntax node for the type declaration
			// (could be a nested type, for example)
			SyntaxNode potentialNamespaceParent = syntax.Parent;
    
			// Keep moving "out" of nested classes etc until we get to a namespace
			// or until we run out of parents
			while (potentialNamespaceParent != null &&
			       !(potentialNamespaceParent is NamespaceDeclarationSyntax)
			       && !(potentialNamespaceParent is FileScopedNamespaceDeclarationSyntax)) {
				potentialNamespaceParent = potentialNamespaceParent.Parent;
			}

			// Build up the final namespace by looping until we no longer have a namespace declaration
			if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
			{
				// We have a namespace. Use that as the type
				nameSpace = namespaceParent.Name.ToString();
        
				// Keep moving "out" of the namespace declarations until we 
				// run out of nested namespace declarations
				while (true)
				{
					NamespaceDeclarationSyntax parent = namespaceParent.Parent as NamespaceDeclarationSyntax;
					if (parent == null) {
						break;
					}

					// Add the outer namespace as a prefix to the final namespace
					nameSpace = $"{namespaceParent.Name}.{nameSpace}";
					namespaceParent = parent;
				}
			}

			// return the final namespace
			return nameSpace;
		}


		/// <summary>
		/// Get all types in the inheritance hierarchy.
		/// </summary>
		private static IEnumerable<ITypeSymbol> GetTypes(ITypeSymbol symbol) {
			ITypeSymbol current = symbol;
			while (current != null) {
				yield return current;
				current = current.BaseType;
			}
		}

		/// <summary>
		/// Get all properties of class.
		/// </summary>
		private static string[] GetSerializablePropertyNames(ITypeSymbol symbol) =>
			GetSerializablePropertySymbols(symbol)
				.Select(par => par.Name)
				.ToArray();

		private static IEnumerable<IPropertySymbol> GetPropertySymbols(ITypeSymbol symbol) {
			var classTypes = GetTypes(symbol);

			var classMembers = classTypes.SelectMany(n => n.GetMembers());
			return classMembers
				.Where(x => x.Kind == SymbolKind.Property)
				.OfType<IPropertySymbol>()
				.ToArray();
		}

		private static IEnumerable<IPropertySymbol> GetSerializablePropertySymbols(ITypeSymbol symbol) {
			return GetPropertySymbols(symbol)
				.Where(p => !p.GetAttributes().Any(a => a.AttributeClass?.Name == "NonSerializedAttribute"))
				.ToArray();
		}

		private string GenerateMethodImplementationForClass(ClassDeclarationSyntax syntax, Compilation compilation) {
			var className = syntax.Identifier.Text;
			var classModifier = syntax.Modifiers.ToFullString().Trim();

			string classNamespace = GetNamespace(syntax);

			SemanticModel classSemanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
			if (classSemanticModel.GetDeclaredSymbol(syntax) is INamedTypeSymbol classSymbol) {
				var serializableClassProperties = GetSerializablePropertyNames(classSymbol);

				var codeBuilder = new StringBuilder();
				codeBuilder.AppendLine("using System.Text;");
				codeBuilder.AppendLine("using System.Linq;");
				codeBuilder.AppendLine("using commonItems.Serialization;");
				codeBuilder.AppendLine($"namespace {classNamespace};");
				codeBuilder.AppendLine($"{classModifier} class {className} {{");

				codeBuilder.AppendLine(@"
					public string SerializeProperties(string indent) {
						var properties = this.GetProperties().Values;
						
						var sb = new StringBuilder();
						foreach (var property in properties) {
							if (property.IsNonSerialized()) {
								continue;
							}
							if (!property.TryGetValue(this, out var propertyValue)) {
								continue;
							}
							
							string lineRepresentation;
							if (property.Attributes.Any(a => a is SerializeOnlyValue)) {
								lineRepresentation = PDXSerializer.Serialize(propertyValue, indent, false);
							} else {
								lineRepresentation = $""{property.Name}={PDXSerializer.Serialize(propertyValue, indent)}"";
							}
							if (!string.IsNullOrWhiteSpace(lineRepresentation)) {
								sb.Append(indent).AppendLine(lineRepresentation);
							}
						}
						return sb.ToString();
					}
				");
				codeBuilder.AppendLine(@"
					public string Serialize(string indent, bool withBraces) {
						// Default implementation: serialize properties.
						var sb = new StringBuilder();
						if (withBraces) {
							sb.AppendLine(""{"");
						}
						sb.Append(SerializeProperties(withBraces ? indent + '\t' : indent));
						if (withBraces) {
							sb.Append(indent).Append('}');
						}
						return sb.ToString();
					}
				");
				codeBuilder.AppendLine("}");

				return codeBuilder.ToString();
			}
			throw new Exception($"Cannot get class symbol for class: {className} in namespace {classNamespace}");
		}

		public void Initialize(GeneratorInitializationContext context) {
			context.RegisterForSyntaxNotifications(() => new SerializationByPropertiesReceiver());
		}
	}
}