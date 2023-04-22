using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace commonItems.SourceGenerators; 

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
				context.AddSource($"{className}.cs", SourceText.From(code, Encoding.UTF8));
			}
		}
	}
	
	/// <summary>
	/// Get all types in the inheritance hierarchy.
	/// </summary>
	private static IEnumerable<ITypeSymbol> GetTypes(ITypeSymbol symbol) {
		ITypeSymbol? current = symbol;
		while (current is not null) {
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
		
		CompilationUnitSyntax? root = syntax.Ancestors().OfType<CompilationUnitSyntax>().FirstOrDefault();
		var classNamespace = root?.ChildNodes()
			.OfType<NamespaceDeclarationSyntax>()
			.FirstOrDefault()?
			.Name
			.ToString();
		
		SemanticModel classSemanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
		if (classSemanticModel.GetDeclaredSymbol(syntax) is not INamedTypeSymbol classSymbol) {
			throw new System.Exception($"Cannot get class symbol for class: {className} in namespace {classNamespace}");
		}
		var serializableClassProperties = GetSerializablePropertyNames(classSymbol);
		//throw new Exception("Serializable properties: " + string.Join(", ", serializableClassProperties)); // TODO: REMOVE THIS
		
		var codeBuilder = new StringBuilder();
		codeBuilder.AppendLine($"namespace {classNamespace};");
		codeBuilder.AppendLine($"{classModifier} class {className} {{");
		
		codeBuilder.AppendLine("""
			public string SerializeProperties(string indent) {
				var properties = this.GetProperties().Values;

				throw new AggregateException($"LOLOLO {properties.First()}"); // TODO: REMOVE
				
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
						lineRepresentation = $"{property.Name}={PDXSerializer.Serialize(propertyValue, indent)}";
					}
					if (!string.IsNullOrWhiteSpace(lineRepresentation)) {
						sb.Append(indent).AppendLine(lineRepresentation);
					}
				}

				return sb.ToString();
			}
		""");
		codeBuilder.AppendLine("""
			public string Serialize(string indent, bool withBraces) {
				// Default implementation: serialize properties.
				var sb = new StringBuilder();
				if (withBraces) {
					sb.AppendLine("{");
				}
				sb.Append(SerializeProperties(withBraces ? indent + '\t' : indent));
				if (withBraces) {
					sb.Append(indent).Append('}');
				}
				return sb.ToString();
			}
		""");
		codeBuilder.AppendLine("}");

		return codeBuilder.ToString();
	}

	public void Initialize(GeneratorInitializationContext context) {
		context.RegisterForSyntaxNotifications(() => new SerializationByPropertiesReceiver());
	}
}