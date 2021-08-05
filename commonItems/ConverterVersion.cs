using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace commonItems {
    class ConverterVersion {
        public ConverterVersion() { }

        public string Name { get; private set; } = "";
        public string Version { get; private set; } = "";
        public string Source { get; private set; } = "";
        public string Target { get; private set; } = "";
        public GameVersion MinSource { get; private set; } = new GameVersion();
        public GameVersion MaxSource { get; private set; } = new GameVersion();
        public GameVersion MinTarget { get; private set; } = new GameVersion();
        public GameVersion MaxTarget { get; private set; } = new GameVersion();

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
            parser.RegisterKeyword("name", (sr) => {
                Name = new SingleString(sr).String;
            });
            parser.RegisterKeyword("version", (sr) => {
                Version = new SingleString(sr).String;
            });
            parser.RegisterKeyword("source", (sr) => {
                Source = new SingleString(sr).String;
            });
            parser.RegisterKeyword("target", (sr) => {
                Target = new SingleString(sr).String;
            });
            parser.RegisterKeyword("minSource", (sr) => {
                var str = new SingleString(sr).String;
                MinSource = new GameVersion(str);
            });
            parser.RegisterKeyword("maxSource", (sr) => {
                var str = new SingleString(sr).String;
                MaxSource = new GameVersion(str);
            });
            parser.RegisterKeyword("minTarget", (sr) => {
                var str = new SingleString(sr).String;
                MinTarget = new GameVersion(str);
            });
            parser.RegisterKeyword("maxTarget", (sr) => {
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



        #region Gets the build date and time (by reading the COFF header)

        // http://msdn.microsoft.com/en-us/library/ms680313

        struct _IMAGE_FILE_HEADER {
            public ushort Machine;
            public ushort NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public ushort SizeOfOptionalHeader;
            public ushort Characteristics;
        };

        static DateTime GetBuildDateTime(Assembly assembly) {
            var path = assembly.GetName().CodeBase;
            if (File.Exists(path)) {
                var buffer = new byte[Math.Max(Marshal.SizeOf(typeof(_IMAGE_FILE_HEADER)), 4)];
                using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                    fileStream.Position = 0x3C;
                    fileStream.Read(buffer, 0, 4);
                    fileStream.Position = BitConverter.ToUInt32(buffer, 0); // COFF header offset
                    fileStream.Read(buffer, 0, 4); // "PE\0\0"
                    fileStream.Read(buffer, 0, buffer.Length);
                }
                var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try {
                    var coffHeader = (_IMAGE_FILE_HEADER)Marshal.PtrToStructure(pinnedBuffer.AddrOfPinnedObject(), typeof(_IMAGE_FILE_HEADER));

                    return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1) + new TimeSpan(coffHeader.TimeDateStamp * TimeSpan.TicksPerSecond));
                } finally {
                    pinnedBuffer.Free();
                }
            }
            return new DateTime();
        }

        #endregion
    }
}
