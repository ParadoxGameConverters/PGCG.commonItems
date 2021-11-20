using System;
using System.Globalization;
using System.Text;

namespace commonItems {
	public class ConverterVersion {
		public string? Name { get; private set; }
		public string? Version { get; private set; }
		public string? Source { get; private set; }
		public string? Target { get; private set; }
		public GameVersion MinSource { get; private set; } = new();
		public GameVersion MaxSource { get; private set; } = new();
		public GameVersion MinTarget { get; private set; } = new();
		public GameVersion MaxTarget { get; private set; } = new();

		public void LoadVersion(string fileName) {
			var parser = new Parser();
			RegisterKeys(ref parser);
			parser.ParseFile(fileName);
			parser.ClearRegisteredRules();
		}
		public void LoadVersion(BufferedReader reader) {
			var parser = new Parser();
			RegisterKeys(ref parser);
			parser.ParseStream(reader);
			parser.ClearRegisteredRules();
		}

		private void RegisterKeys(ref Parser parser) {
			parser.RegisterKeyword("name", reader => Name = ParserHelpers.GetString(reader));
			parser.RegisterKeyword("version", reader => Version = ParserHelpers.GetString(reader));
			parser.RegisterKeyword("source", reader => Source = ParserHelpers.GetString(reader));
			parser.RegisterKeyword("target", reader => Target = ParserHelpers.GetString(reader));
			parser.RegisterKeyword("minSource", reader =>
				MinSource = new GameVersion(ParserHelpers.GetString(reader))
			);
			parser.RegisterKeyword("maxSource", reader =>
				MaxSource = new GameVersion(ParserHelpers.GetString(reader))
			);
			parser.RegisterKeyword("minTarget", reader =>
				MinTarget = new GameVersion(ParserHelpers.GetString(reader))
			);
			parser.RegisterKeyword("maxTarget", reader =>
				MaxTarget = new GameVersion(ParserHelpers.GetString(reader))
			);
			parser.RegisterRegex(CommonRegexes.Catchall, ParserHelpers.IgnoreItem);
		}

		public string GetDescription() {
			var sb = new StringBuilder();
			sb.Append("Compatible with ");
			sb.Append(Source);
			sb.Append(" [v");
			sb.Append(MinSource.ToShortString());
			if (!MaxSource.Equals(MinSource)) {
				sb.Append("-v");
				sb.Append(MaxSource.ToShortString());
			}
			sb.Append("] and ");
			sb.Append(Target);
			sb.Append(" [v");
			sb.Append(MinTarget.ToShortString());
			if (!MaxTarget.Equals(MinTarget)) {
				sb.Append("-v");
				sb.Append(MaxTarget.ToShortString());
			}
			sb.Append(']');
			return sb.ToString();
		}

		public override string ToString() {
			var sb = new StringBuilder();
			sb.Append("\n\n");
			sb.Append("************ -= The Paradox Game Converters Group =- *****************\n");

			if (!string.IsNullOrEmpty(Version) && !string.IsNullOrEmpty(Name)) {
				sb.Append("* Converter version ");
				sb.Append(Version);
				sb.Append(" \"");
				sb.Append(Name);
				sb.Append("\"\n");
			}

			sb.Append("* ");
			sb.Append(GetDescription());
			sb.Append('\n');
			sb.Append("* Built on ");
			var compileTime = new DateTime(Builtin.CompileTime, DateTimeKind.Utc);
			sb.Append(compileTime.ToString("u", CultureInfo.InvariantCulture));
			sb.Append('\n');

			var footerTitleBuilder = new StringBuilder();
			footerTitleBuilder.Append(" + ");
			footerTitleBuilder.Append(Source);
			footerTitleBuilder.Append(" To ");
			footerTitleBuilder.Append(Target);
			footerTitleBuilder.Append(" + ");
			if (footerTitleBuilder.Length >= 68) {
				footerTitleBuilder.Insert(0, '*');
				footerTitleBuilder.Append("*\n");
			} else {
				var target = (70 - footerTitleBuilder.Length) / 2;
				for (var counter = 0; counter < target; ++counter) {
					footerTitleBuilder.Insert(0, '*');
				}
				target = 70 - footerTitleBuilder.Length;
				for (var counter = 0; counter < target; ++counter) {
					footerTitleBuilder.Append('*');
				}
				footerTitleBuilder.Append('\n');
			}

			sb.Append(footerTitleBuilder);
			return sb.ToString();
		}
	}
}
