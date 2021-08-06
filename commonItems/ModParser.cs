using System.Collections.Generic;

namespace commonItems {
    public class ModParser : Parser {
        public string Name { get; private set; } = "";
        public string Path { get; set; } = "";

        private bool compressed = false;
        public SortedSet<string> Dependencies { get; private set; } = new();

        public ModParser() { }
        public void ParseMod(BufferedReader reader) {
            RegisterKeys();
            ParseStream(reader);
            ClearRegisteredRules();
            CheckIfCompressed();
        }
        public void ParseMod(string fileName) {
            RegisterKeys();
            ParseFile(fileName);
            ClearRegisteredRules();
            CheckIfCompressed();
        }
        private void CheckIfCompressed() {
            if (!string.IsNullOrEmpty(Path)) {
                var ending = CommonFunctions.GetExtension(Path);
                compressed = ending == "zip" || ending == "bin";
            }
        }
        public bool IsValid() {
            return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Path);
        }
        public bool IsCompressed() {
            return compressed;
        }

        private void RegisterKeys() {
            RegisterKeyword("name", (sr) => {
                Name = new SingleString(sr).String;
            });
            RegisterRegex("path|archive", (sr) => {
                Path = new SingleString(sr).String;
            });
            RegisterKeyword("dependencies", (sr) => {
                var newDependencies = new StringList(sr).Strings;
                Dependencies.UnionWith(newDependencies);
            });
            RegisterRegex(CommonRegexes.Catchall, ParserHelpers.IgnoreItem);
        }
    }
}