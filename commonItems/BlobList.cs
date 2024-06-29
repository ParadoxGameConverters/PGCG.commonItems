using System.Collections.Generic;
using System.Text;

namespace commonItems;

public class BlobList {
	public List<string> Blobs { get; } = [];
	public BlobList(BufferedReader reader) {
		var next = Parser.GetNextLexeme(reader);
		if (next == "=") {
			next = Parser.GetNextLexeme(reader);
		}
		if (next != "{") {
			return;
		}

		var braceDepth = 0;
		var sb = new StringBuilder();
		while (!reader.EndOfStream) {
			char inputChar = (char)reader.Read();
			if (inputChar == '{') {
				if (braceDepth > 0) {
					sb.Append(inputChar);
				}
				++braceDepth;
			} else if (inputChar == '}') {
				--braceDepth;
				if (braceDepth > 0) {
					sb.Append(inputChar);
				} else if (braceDepth == 0) {
					Blobs.Add(sb.ToString());
					sb.Clear();
				} else if (braceDepth == -1) {
					return;
				}
			} else if (braceDepth == 0) {
				// Ignore this character. Only look for blobs.
			} else {
				sb.Append(inputChar);
			}
		}
	}
}