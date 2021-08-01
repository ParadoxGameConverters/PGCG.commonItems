using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace commonItems {
    public class GameVersionFactory : Parser {
        private int? firstPart;
        private int? secondPart;
        private int? thirdPart;
        private int? fourthPart;

        public GameVersionFactory() {
            RegisterKeyword("first", (reader) => {
                firstPart = new SingleInt(reader).Int;
            });
            RegisterKeyword("second", (reader) => {
                secondPart = new SingleInt(reader).Int;
            });
            RegisterKeyword("third", (reader) => {
                thirdPart = new SingleInt(reader).Int;
            });
            RegisterKeyword("forth", (reader) => {
                fourthPart = new SingleInt(reader).Int;
            });
            RegisterRegex(CommonRegexes.Catchall, ParserHelpers.IgnoreAndLogItem);
        }

        public GameVersion GetVersion(BufferedReader reader) {
            firstPart = null;
            secondPart = null;
            thirdPart = null;
            fourthPart = null;
            ParseStream(reader);
            return new GameVersion(firstPart, secondPart, thirdPart, fourthPart);
        }
    }
}
