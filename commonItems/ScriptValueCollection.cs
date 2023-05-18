using commonItems.Mods;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace commonItems; 

public class ScriptValueCollection : IReadOnlyDictionary<string, double> {
	private readonly Dictionary<string, double> dict = new();
	
	/// <summary>
	/// Loads script values from common/script_values
	/// </summary>
	/// <param name="modFilesystem">mod file system to load script values from</param>
	public void LoadScriptValues(ModFilesystem modFilesystem) {
		Logger.Info("Reading script values...");

		var parser = new Parser();
		parser.RegisterRegex(CommonRegexes.String, (reader, name) => {
			var value = ParseValue(reader);
			if (value is not null) {
				dict[name] = (double)value;
			}
		});
		parser.RegisterRegex(CommonRegexes.Catchall, ParserHelpers.IgnoreAndLogItem);
		parser.ParseGameFolder("common/script_values", modFilesystem, "txt", recursive: true);
	}

	private double? ParseValue(BufferedReader reader) {
		var valueStringOfItem = reader.GetStringOfItem();
		if (valueStringOfItem.IsArrayOrObject()) {
			return null;
		}

		var valueStr = valueStringOfItem.ToString();
		if (CommonRegexes.Variable.IsMatch(valueStr)) {
			var variableValue = reader.ResolveVariable(valueStr);
			if (Information.IsNumeric(variableValue)) {
				return Convert.ToDouble(variableValue);
			}
		}
		
		if (CommonRegexes.InterpolatedExpression.IsMatch(valueStr)) {
			var expressionValue = reader.EvaluateExpression(valueStr);
			if (Information.IsNumeric(expressionValue)) {
				return Convert.ToDouble(expressionValue);
			}
		}

		var value = GetValueForString(valueStr);
		return value;
	}

	public IEnumerator<KeyValuePair<string, double>> GetEnumerator() {
		return dict.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}

	public int Count => dict.Count;
	public bool ContainsKey(string key) => dict.ContainsKey(key);

	public bool TryGetValue(string key, out double value) => dict.TryGetValue(key, out value);

	public double this[string key] => dict[key];

	public IEnumerable<string> Keys => dict.Keys;
	public IEnumerable<double> Values => dict.Values;
	
	public double? GetValueForString(string valueStr) {
		if (double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue)) {
			return parsedValue;
		}
		if (TryGetValue(valueStr, out double definedValue)) {
			return definedValue;
		}
		
		// Value might be a bool - convert it to double.
		var tempReader = new BufferedReader(valueStr);
		try {
			var boolValue = tempReader.GetBool();
			return Convert.ToDouble(boolValue);
		} catch {
			// ignored
		}

		Logger.Warn($"No script value found for \"{valueStr}\"!");
		return null;
	}
}