using System;
using System.Text;

namespace commonItems {
	public class ConverterVersion {
		public string Name { get; private set; } = "";
		public string Version { get; private set; } = "";
		public string Source { get; private set; } = "";
		public string Target { get; private set; } = "";
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
			parser.RegisterKeyword("name", sr => Name = new SingleString(sr).String);
			parser.RegisterKeyword("version", sr => Version = new SingleString(sr).String);
			parser.RegisterKeyword("source", sr => Source = new SingleString(sr).String);
			parser.RegisterKeyword("target", sr => Target = new SingleString(sr).String);
			parser.RegisterKeyword("minSource", sr => {
				var str = new SingleString(sr).String;
				MinSource = new GameVersion(str);
			});
			parser.RegisterKeyword("maxSource", sr => {
				var str = new SingleString(sr).String;
				MaxSource = new GameVersion(str);
			});
			parser.RegisterKeyword("minTarget", sr => {
				var str = new SingleString(sr).String;
				MinTarget = new GameVersion(str);
			});
			parser.RegisterKeyword("maxTarget", sr => {
				var str = new SingleString(sr).String;
				MaxTarget = new GameVersion(str);
			});
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
			sb.Append(compileTime.ToLongDateString());
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
