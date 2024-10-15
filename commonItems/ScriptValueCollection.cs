using commonItems.Collections;
using commonItems.Mods;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace commonItems; 

public class ScriptValueCollection : IReadOnlyDictionary<string, double> {
	private readonly Dictionary<string, double> dict = [];
	
	/// <summary>
	/// Loads script values from common/script_values
	/// </summary>
	/// <param name="modFilesystem">mod file system to load script values from</param>
	public void LoadScriptValues(ModFilesystem modFilesystem) {
		Logger.Info("Reading script values...");
		
		// Some values can be used in other values before they are defined, for example:
		//		scheme_agent_general_bonuses_contribution_score_bonus_max_value = agent_max_skill_value
		//		agent_max_skill_value = 10
		// To handle this, we read the script values multiple times until no new values are added.
		OrderedSet<string> unresolvedScriptValues = [];
		int addedValuesCount;
		do {
			addedValuesCount = 0;
			
			var parser = new Parser();
			parser.RegisterRegex(CommonRegexes.String, (reader, name) => {
				var value = ParseValue(reader, unresolvedScriptValues);
				if (value is not null) {
					if (!dict.ContainsKey(name)) {
						++addedValuesCount;
					}
					dict[name] = (double)value;
				}
			});
			parser.RegisterRegex(CommonRegexes.Catchall, ParserHelpers.IgnoreAndLogItem);
			parser.ParseGameFolder("common/script_values", modFilesystem, "txt", recursive: true);
		} while (addedValuesCount > 0);
		
		if (unresolvedScriptValues.Count > 0) {
			// Log unresolvable values (excluding complex ones).
			Logger.Warn($"The following script values were not loaded: {unresolvedScriptValues}");
		}
	}

	private double? ParseValue(BufferedReader reader, OrderedSet<string> unresolvedScriptValues) {
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
		if (value is null) {
			unresolvedScriptValues.Add(valueStr);
		} else {
			unresolvedScriptValues.Remove(valueStr);
		}
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

		return null;
	}
}