using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static commonItems.SourceGenerators.SerializationSourceGenerator;

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

		// Credits: https://andrewlock.net/creating-a-source-generator-part-5-finding-a-type-declarations-namespace-and-type-hierarchy/
		/// <summary>
		/// Determine the namespace the class/enum/struct is declared in, if any.
		/// </summary>
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

		internal class ParentClass {
			public ParentClass(string keyword, string name, string constraints, ParentClass child) {
				Keyword = keyword;
				Name = name;
				Constraints = constraints;
				Child = child;
			}

			public ParentClass Child { get; }
			public string Keyword { get; }
			public string Name { get; }
			public string Constraints { get; }
		}

		// Credits: https://andrewlock.net/creating-a-source-generator-part-5-finding-a-type-declarations-namespace-and-type-hierarchy/
		static ParentClass GetParentClasses(BaseTypeDeclarationSyntax typeSyntax) {
			// Try and get the parent syntax. If it isn't a type like class/struct, this will be null
			var parentSyntax = typeSyntax.Parent as TypeDeclarationSyntax;
			ParentClass parentClassInfo = null;

			// We can only be nested in class/struct/record
			static bool IsAllowedKind(SyntaxKind kind) =>
				kind == SyntaxKind.ClassDeclaration ||
				kind == SyntaxKind.StructDeclaration ||
				kind == SyntaxKind.RecordDeclaration;

			// Keep looping while we're in a supported nested type
			while (parentSyntax != null && IsAllowedKind(parentSyntax.Kind())) {
				// Record the parent type keyword (class/struct etc), name, and constraints
				parentClassInfo = new ParentClass(
					keyword: parentSyntax.Keyword.ValueText,
					name: parentSyntax.Identifier.ToString() + parentSyntax.TypeParameterList,
					constraints: parentSyntax.ConstraintClauses.ToString(),
					child: parentClassInfo); // set the child link (null initially)

				// Move to the next outer type
				parentSyntax = (parentSyntax.Parent as TypeDeclarationSyntax);
			}

			// return a link to the outermost parent type
			return parentClassInfo;

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

		private static string GetPropertyNameForSerialization(IPropertySymbol propertySymbol) {
			var serializedNameAttr = propertySymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "SerializedName");
			if (serializedNameAttr != null) {
				return serializedNameAttr.ConstructorArguments[0].Value.ToString();
			}
			return propertySymbol.Name;
		}

		private struct SerializablePropertyModel {
			public string Name;
			public string SerializedName;
			public bool CanBeNull;
			public bool SerializeOnlyValue;
		}

		/// <summary>
		/// Get all serializable properties of class.
		/// </summary>
		private static SerializablePropertyModel[] GetSerializableProperties(ITypeSymbol symbol) =>
			GetSerializablePropertySymbols(symbol)
				.Select(prop => new SerializablePropertyModel {
					Name = prop.Name,
					SerializedName = GetPropertyNameForSerialization(prop),
					CanBeNull = prop.Type.NullableAnnotation == NullableAnnotation.Annotated,
					SerializeOnlyValue = prop.GetAttributes().Any(a => a.AttributeClass?.Name == "SerializeOnlyValue")
				})
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
				.Where(p => !p.GetAttributes().Any(a => a.AttributeClass?.Name == "NonSerialized"))
				.ToArray();
		}

		private string GenerateMethodImplementationForClass(ClassDeclarationSyntax syntax, Compilation compilation) {
			var className = syntax.Identifier.Text;
			var classModifier = syntax.Modifiers.ToFullString().Trim();

			string classNamespace = GetNamespace(syntax);

			SemanticModel classSemanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
			if (classSemanticModel.GetDeclaredSymbol(syntax) is INamedTypeSymbol classSymbol) {
				var serializableClassProperties = GetSerializableProperties(classSymbol);

				var codeBuilder = new StringBuilder();
				codeBuilder.AppendLine("using System.Text;");
				codeBuilder.AppendLine("using commonItems.Serialization;");
				codeBuilder.AppendLine($"namespace {classNamespace};");

				var parentClass = GetParentClasses(syntax);
				int parentsCount = 0;
				// Loop through the full parent type hiearchy, starting with the outermost
				while (parentClass != null) {
					codeBuilder
						.Append(" partial ")
						.Append(parentClass.Keyword) // e.g. class/struct/record
						.Append(' ')
						.Append(parentClass.Name) // e.g. Outer/Generic<T>
						.Append(' ')
						.Append(parentClass.Constraints) // e.g. where T: new()
						.Append(" {");
					++parentsCount; // keep track of how many layers deep we are
					parentClass = parentClass.Child; // repeat with the next child
				}

				codeBuilder.AppendLine($"{classModifier} class {className} {{");

				codeBuilder.AppendLine(@"
					public string SerializeProperties(string indent) {
						var sb = new StringBuilder();
				");
				foreach (var propertyModel in serializableClassProperties) {
					string lineVariableName = $"{propertyModel.Name}_lineRepresentation";
					codeBuilder.AppendLine($"string {lineVariableName};");
					if (propertyModel.CanBeNull) {
						codeBuilder.AppendLine($@"
							if ({propertyModel.Name} is null) {{
								{lineVariableName} = string.Empty;
							}} else {{
						");
					}
					if (propertyModel.SerializeOnlyValue) {
						codeBuilder.AppendLine($@"
							{lineVariableName} = PDXSerializer.Serialize({propertyModel.Name}, indent, false);
						");
					} else {
						codeBuilder.AppendLine($@"
							{lineVariableName} = $""{propertyModel.SerializedName}={{PDXSerializer.Serialize({propertyModel.Name}, indent)}}"";
						");
					}
					if (propertyModel.CanBeNull) {
						// Close else block.
						codeBuilder.AppendLine(@"
							}
						");
					}
					codeBuilder.AppendLine($@"
						if (!string.IsNullOrWhiteSpace({lineVariableName})) {{
							sb.Append(indent).AppendLine({lineVariableName});
						}}
					");
				}
				codeBuilder.AppendLine(@"
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

				// We need to "close" each of the parent types, so write
				// the required number of '}'
				for (int i = parentsCount; i < parentsCount; ++i) {
					codeBuilder.AppendLine("}");
				}

				return FormatCode(codeBuilder.ToString());
			}
			throw new Exception($"Cannot get class symbol for class: {className} in namespace {classNamespace}");
		}

		private static string FormatCode(string generatedCode) {
			var tree = CSharpSyntaxTree.ParseText(generatedCode);
			var root = (CSharpSyntaxNode)tree.GetRoot();
			generatedCode = root.NormalizeWhitespace().ToFullString();

			return generatedCode;
		}

		public void Initialize(GeneratorInitializationContext context) {
			context.RegisterForSyntaxNotifications(() => new SerializationByPropertiesReceiver());
		}
	}
}