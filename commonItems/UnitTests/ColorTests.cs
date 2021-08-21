using System;
using System.Collections.Generic;
using Xunit;

namespace commonItems.UnitTests {
    public class ColorTests {
        private const int decimalPlaces = 2;

        [Fact] public void ColorDefaultToBlack() {
            var testColor = new Color();
            var rgbComponents = testColor.RgbComponents;
            Assert.Equal(0, rgbComponents[0]);
            Assert.Equal(0, rgbComponents[1]);
            Assert.Equal(0, rgbComponents[2]);

            var hsvComponents = testColor.HsvComponents;
            Assert.Equal(0, hsvComponents[0]);
            Assert.Equal(0, hsvComponents[1]);
            Assert.Equal(0, hsvComponents[2]);
        }

        [Fact] public void ColorCanBeInitializedWithRgbComponents() {
            var testColor = new Color(new [] { 64, 128, 128 });

            Assert.Equal(64, testColor.R);
            Assert.Equal(128, testColor.G);
            Assert.Equal(128, testColor.B);

            Assert.Equal(0.5, testColor.H, decimalPlaces);
            Assert.Equal(0.5, testColor.S, decimalPlaces);
            Assert.Equal(0.5, testColor.V, decimalPlaces);
        }

        [Fact]
        public void ColorCanBeInitializedWithHsvComponents() {
            var testColor = new Color(new double[] { 0.5f, 0.5f, 0.5f });

            Assert.Equal(63, testColor.R);
            Assert.Equal(127, testColor.G);
            Assert.Equal(127, testColor.B);

            Assert.Equal(0.5, testColor.H, decimalPlaces);
            Assert.Equal(0.5, testColor.S, decimalPlaces);
            Assert.Equal(0.5, testColor.V, decimalPlaces);
        }

        [Fact] public void HsvConversion_GreyHasZeroHue() {
            var testColor = new Color(new [] { 128, 128, 128 });
            Assert.Equal(0, testColor.H);
        }

        [Fact] public void HsvConversion_RedHasHueOfZero() {
            var testColor = new Color(new [] { 128, 0, 0 });
            Assert.Equal(0, testColor.H);
        }

        [Fact] public void HsvConversion_YellowHasHueOfOneSixth() {
            var testColor = new Color(new [] { 128, 128, 64 });
            Assert.Equal(0.167, testColor.H, decimalPlaces);
        }

        [Fact]
        public void HsvConversion_GreenHasHueOfOneThird() {
            var testColor = new Color(new [] { 0, 128, 0 });
            Assert.Equal(0.333, testColor.H, decimalPlaces);
        }

        [Fact]
        public void HsvConversion_CyanHasHueOfOneHalf() {
            var testColor = new Color(new [] { 64, 128, 128 });
            Assert.Equal(0.5, testColor.H, decimalPlaces);
        }

        [Fact]
        public void HsvConversion_BlueHasHueOfTwoThirds() {
            var testColor = new Color(new [] { 0, 0, 128 });
            Assert.Equal(0.667, testColor.H, decimalPlaces);
        }

        [Fact]
        public void HsvConversion_MagentaHasHueOfFiveSixths() {
            var testColor = new Color(new [] { 128, 64, 128 });
            Assert.Equal(0.833, testColor.H, decimalPlaces);
        }

        [Fact] public void HsvConversion_BlackHasZeroSaturation() {
            var testColor = new Color(new [] { 0, 0, 0 });
            Assert.Equal(0, testColor.S);
        }

        [Fact]
        public void HsvConversion_GreyHasZeroSaturation() {
            var testColor = new Color(new [] { 128, 128, 128 });
            Assert.Equal(0, testColor.S);
        }

        [Fact]
        public void HsvConversion_ColorHasSaturation() {
            var testColor = new Color(new [] { 128, 128, 64 });
            Assert.Equal(0.5, testColor.S, decimalPlaces);
        }

        [Fact]
        public void HsvConversion_BlackHasZeroValue() {
            var testColor = new Color(new [] { 0, 0, 0 });
            Assert.Equal(0, testColor.V);
        }

        [Fact]
        public void HsvConversion_ColorHasValue() {
            var testColor = new Color(new [] { 128, 64, 64 });
            Assert.Equal(0.5, testColor.V, decimalPlaces);
        }

        [Fact]
        public void RgbConversion_ZeroHueGivesRed() {
            var testColor = new Color(new double[] { 0, 1, 1 });
            Assert.InRange(testColor.R, 255 - 1, 255 + 1);
            Assert.InRange(testColor.G, 0 - 1, 0 + 1);
            Assert.InRange(testColor.B, 0 - 1, 0 + 1);
        }

        [Fact]
        public void RgbConversion_OneSixthHueGivesYellow() {
            var testColor = new Color(new double[] { 0.167, 1, 1 });
            Assert.InRange(testColor.R, 254 - 1, 254 + 1);
            Assert.InRange(testColor.G, 254 - 1, 254 + 1);
            Assert.InRange(testColor.B, 0 - 1, 0 + 1);
        }

        [Fact]
        public void RgbConversion_OneThirdHueGivesGreen() {
            var testColor = new Color(new double[] { 0.333, 1, 1 });
            Assert.InRange(testColor.R, 0 - 1, 0 + 1);
            Assert.InRange(testColor.G, 255 - 1, 255 + 1);
            Assert.InRange(testColor.B, 0 - 1, 0 + 1);
        }

        [Fact]
        public void RgbConversion_OneHalfHueGivesCyan() {
            var testColor = new Color(new double[] { 0.5, 1, 1 });
            Assert.InRange(testColor.R, 0 - 1, 0 + 1);
            Assert.InRange(testColor.G, 255 - 1, 255 + 1);
            Assert.InRange(testColor.B, 255 - 1, 255 + 1);
        }

        [Fact]
        public void RgbConversion_TwoThirdHueGivesBlue() {
            var testColor = new Color(new double[] { 0.667, 1, 1 });
            Assert.InRange(testColor.R, 0 - 1, 0 + 1);
            Assert.InRange(testColor.G, 0 - 1, 0 + 1);
            Assert.InRange(testColor.B, 255 - 1, 255 + 1);
        }

        [Fact]
        public void RgbConversion_FiveSixthsHueGivesMagenta() {
            var testColor = new Color(new double[] { 0.833, 1, 1 });
            Assert.InRange(testColor.R, 254 - 1, 254 + 1);
            Assert.InRange(testColor.G, 0-1, 0+1);
            Assert.InRange(testColor.B, 254-1, 254+1);
        }

        [Fact]
        public void RgbConversion_ZeroSaturationIsGreyscale() {
            var testColor = new Color(new double[] { 0, 0, 0.5 });
            Assert.InRange(testColor.R, 127-1, 127+1);
            Assert.InRange(testColor.G, 127 - 1, 127 + 1);
            Assert.InRange(testColor.B, 127 - 1, 127 + 1);
        }

        [Fact]
        public void RgbConversion_FullSaturationisPureColor() {
            var testColor = new Color(new double[] { 0, 1, 1 });
            Assert.InRange(testColor.R, 255-1, 255+1);
            Assert.InRange(testColor.G, 0 - 1, 0 + 1);
            Assert.InRange(testColor.B, 0-1, 0+1);
        }

        [Fact]
        public void RgbConversion_SaturationWhitensColor() {
            var testColor = new Color(new double[] { 0, 0.5, 1 });
            Assert.InRange(testColor.R, 255 - 1, 255 + 1);
            Assert.InRange(testColor.G, 127-1, 127+1);
            Assert.InRange(testColor.B, 127 - 1, 127 + 1);
        }

        [Fact]
        public void RgbConversion_ZeroValueIsBlack() {
            var testColor = new Color(new double[] { 0, 1, 0 });
            Assert.InRange(testColor.R, 0-1, 0+1);
            Assert.InRange(testColor.G, 0 - 1, 0 + 1);
            Assert.InRange(testColor.B, 0 - 1, 0 + 1);
        }

        [Fact]
        public void RgbConversion_ValueDarkensColor() {
            var testColor = new Color(new double[] { 0, 1, 0.5 });
            Assert.InRange(testColor.R, 127-1, 127+1);
            Assert.InRange(testColor.G, 0 - 1, 0 + 1);
            Assert.InRange(testColor.B, 0 - 1, 0 + 1);
        }

        [Fact]
        public void RgbConversion_ExcessiveHueIsDiscarded() {
            var testColor1 = new Color(new double[] { 1, 1, 1 });
            var testColor2 = new Color(new [] { 1.1, 1.0, 1.0 });
            Assert.InRange(testColor1.R, 255 - 1, 255 + 1);
            Assert.InRange(testColor1.G, 0 - 1, 0 + 1);
            Assert.InRange(testColor1.B, 0 - 1, 0 + 1);
            Assert.InRange(testColor2.R, 255 - 1, 255 + 1);
            Assert.InRange(testColor2.G, 0 - 1, 0 + 1);
            Assert.InRange(testColor2.B, 0-1, 0+1);
        }

        [Fact] public void ColorCanBeInitializedFromStream() {
            var reader = new BufferedReader("= { 64 128 128 }");
            var testColor = new ColorFactory().GetColor(reader);

            Assert.Equal(64, testColor.R);
            Assert.Equal(128, testColor.G);
            Assert.Equal(128, testColor.B);

            Assert.Equal(0.5, testColor.H, decimalPlaces);
            Assert.Equal(0.5, testColor.S, decimalPlaces);
            Assert.Equal(0.5, testColor.V, decimalPlaces);
        }

        [Fact] public void ColorRGBDoublesCanBeInitializedFromStream() { // Yes, this is a thing.
            var reader = new BufferedReader("= { 0.5 0.9 0.1 }");
            var color = new ColorFactory().GetColor(reader);
            Assert.Equal(128, color.R);
            Assert.Equal(230, color.G);
            Assert.Equal(26, color.B);

            Assert.Equal(0.25, color.H, decimalPlaces);
            Assert.Equal(0.89, color.S, decimalPlaces);
            Assert.Equal(0.9, color.V, decimalPlaces);
        }

        [Fact] public void ColorCanBeInitializedFromStreamWithQuotes() {
            var reader = new BufferedReader("= { \"64\" \"128\" \"128\" }");
            var color = new ColorFactory().GetColor(reader);
            Assert.Equal(64, color.R);
            Assert.Equal(128, color.G);
            Assert.Equal(128, color.B);

            Assert.Equal(0.5, color.H, decimalPlaces);
            Assert.Equal(0.5, color.S, decimalPlaces);
            Assert.Equal(0.5, color.V, decimalPlaces);
        }

        [Fact] public void ColorInitializationRequiresThreeComponentsWhenUnspecified() {
            var reader = new BufferedReader("= { 64 128 }");
            Assert.Throws<FormatException>(()=>new ColorFactory().GetColor(reader));
        }

        [Fact]
        public void ColorCanBeInitializedFromStreamInRgb() {
            var reader = new BufferedReader("= rgb { 64 128 128 }");
            var color = new ColorFactory().GetColor(reader);
            Assert.Equal(64, color.R);
            Assert.Equal(128, color.G);
            Assert.Equal(128, color.B);

            Assert.Equal(0.5, color.H, decimalPlaces);
            Assert.Equal(0.5, color.S, decimalPlaces);
            Assert.Equal(0.5, color.V, decimalPlaces);
        }

        [Fact]
        public void ColorInitializationRequiresThreeComponentsWhenRgb() {
            var reader = new BufferedReader("= rgb { 64 128 }");
            Assert.Throws<FormatException>(() => new ColorFactory().GetColor(reader));
        }

        [Fact]
        public void ColorCanBeInitializedFromStreamInHex() {
            var reader = new BufferedReader("= hex { 408080 }");
            var color = new ColorFactory().GetColor(reader);
            Assert.Equal(64, color.R);
            Assert.Equal(128, color.G);
            Assert.Equal(128, color.B);

            Assert.Equal(0.5, color.H, decimalPlaces);
            Assert.Equal(0.5, color.S, decimalPlaces);
            Assert.Equal(0.5, color.V, decimalPlaces);
        }

        [Fact]
        public void ColorInitializationRequiresSixDigitsWhenHex() {
            var reader = new BufferedReader("= hex { 12345 }");
            Assert.Throws<FormatException>(() => new ColorFactory().GetColor(reader));
        }

        [Fact]
        public void ColorCanBeInitializedFromStreamInHsv() {
            var reader = new BufferedReader("= hsv { 0.5 0.5 0.5 }");
            var color = new ColorFactory().GetColor(reader);
            Assert.Equal(63, color.R);
            Assert.Equal(127, color.G);
            Assert.Equal(127, color.B);

            Assert.Equal(0.5, color.H, decimalPlaces);
            Assert.Equal(0.5, color.S, decimalPlaces);
            Assert.Equal(0.5, color.V, decimalPlaces);
        }

        [Fact]
        public void ColorInitializationRequiresThreeComponentsWhenHsv() {
            var reader = new BufferedReader("= hsv { 0.333 0.5 }");
            Assert.Throws<FormatException>(() => new ColorFactory().GetColor(reader));
        }

        [Fact]
        public void ColorCanBeInitializedFromStreamInHsv360() {
            var reader = new BufferedReader("= hsv360 { 180 50 50 }");
            var color = new ColorFactory().GetColor(reader);
            Assert.Equal(63, color.R);
            Assert.Equal(127, color.G);
            Assert.Equal(127, color.B);

            Assert.Equal(0.5, color.H, decimalPlaces);
            Assert.Equal(0.5, color.S, decimalPlaces);
            Assert.Equal(0.5, color.V, decimalPlaces);
        }

        [Fact]
        public void ColorInitializationRequiresThreeComponentsWhenHsv360() {
            var reader = new BufferedReader("= hsv360 { 120 50 }");
            Assert.Throws<FormatException>(() => new ColorFactory().GetColor(reader));
        }

        [Fact]
        public void ColorCanBeInitializedFromStreamWithName() {
            var colorFactory = new ColorFactory();
            colorFactory.AddNamedColor("dark_moderate_cyan", new Color(new [] { 64, 128, 128 }));

            var reader = new BufferedReader("= dark_moderate_cyan");
            var color = colorFactory.GetColor(reader);

            Assert.Equal(64, color.R);
            Assert.Equal(128, color.G);
            Assert.Equal(128, color.B);

            Assert.Equal(0.5, color.H, decimalPlaces);
            Assert.Equal(0.5, color.S, decimalPlaces);
            Assert.Equal(0.5, color.V, decimalPlaces);
        }

        [Fact]
        public void ColorCanBeInitializedFromQuotedStreamWithName() {
            var colorFactory = new ColorFactory();
            colorFactory.AddNamedColor("dark_moderate_cyan", new Color(new int[] { 64, 128, 128 }));

            var reader = new BufferedReader("= \"dark_moderate_cyan\"");
            var color = colorFactory.GetColor(reader);

            Assert.Equal(64, color.R);
            Assert.Equal(128, color.G);
            Assert.Equal(128, color.B);

            Assert.Equal(0.5, color.H, decimalPlaces);
            Assert.Equal(0.5, color.S, decimalPlaces);
            Assert.Equal(0.5, color.V, decimalPlaces);
        }

        [Fact] public void ColorInitializingRequiresCachedColorWhenUsingName() {
            var colorFactory = new ColorFactory();
            var reader = new BufferedReader("= dark_moderate_cyan");
            Assert.Throws<ArgumentException>(() => colorFactory.GetColor(reader));
        }

        [Fact]
        public void ColorCanBeCachedFromStream() {
            var colorFactory = new ColorFactory();
            var cacheReader = new BufferedReader("= rgb { 64 128 128 }");
            colorFactory.AddNamedColor("dark_moderate_cyan", cacheReader);

            var reader = new BufferedReader("= dark_moderate_cyan");
            var color = colorFactory.GetColor(reader);

            Assert.Equal(64, color.R);
            Assert.Equal(128, color.G);
            Assert.Equal(128, color.B);

            Assert.Equal(0.5, color.H, decimalPlaces);
            Assert.Equal(0.5, color.S, decimalPlaces);
            Assert.Equal(0.5, color.V, decimalPlaces);
        }

        [Fact]
        public void ColorCanBeInitializedWithName() {
            var colorFactory = new ColorFactory();
            colorFactory.AddNamedColor("dark_moderate_cyan", new Color(new int[] { 64, 128, 128} ));
            
            var color = colorFactory.GetColor("dark_moderate_cyan");

            Assert.Equal(64, color.R);
            Assert.Equal(128, color.G);
            Assert.Equal(128, color.B);

            Assert.Equal(0.5, color.H, decimalPlaces);
            Assert.Equal(0.5, color.S, decimalPlaces);
            Assert.Equal(0.5, color.V, decimalPlaces);
        }

        private class Foo : Parser {
            public Foo(BufferedReader reader) {
                RegisterKeyword("color", (BufferedReader reader) => {
                    color = new ColorFactory().GetColor(reader);
                });
                ParseStream(reader);
            }
            public Color color = new();
        }
        [Fact] public void ColorCanBeInitializedFromLongerStream() {
            var reader = new BufferedReader("= { color = { 64 128 128 } } more text");
            var color = new Foo(reader).color;

            Assert.Equal(64, color.R);
            Assert.Equal(128, color.G);
            Assert.Equal(128, color.B);

            Assert.Equal(0.5, color.H, decimalPlaces);
            Assert.Equal(0.5, color.S, decimalPlaces);
            Assert.Equal(0.5, color.V, decimalPlaces);

            Assert.Equal(" more text", reader.ReadLine());
        }

        [Fact]
        public void ColorCanBeInitializedInRgbFromLongerStream() {
            var reader = new BufferedReader("= { color = rgb { 64 128 128 } } more text");
            var color = new Foo(reader).color;

            Assert.Equal(64, color.R);
            Assert.Equal(128, color.G);
            Assert.Equal(128, color.B);

            Assert.Equal(0.5, color.H, decimalPlaces);
            Assert.Equal(0.5, color.S, decimalPlaces);
            Assert.Equal(0.5, color.V, decimalPlaces);

            Assert.Equal(" more text", reader.ReadLine());
        }

        [Fact]
        public void ColorCanBeInitializedInHsvFromLongerStream() {
            var reader = new BufferedReader("= { color = hsv { 0.5 0.5 0.5 } } more text");
            var color = new Foo(reader).color;

            Assert.Equal(63, color.R);
            Assert.Equal(127, color.G);
            Assert.Equal(127, color.B);

            Assert.Equal(0.5, color.H, decimalPlaces);
            Assert.Equal(0.5, color.S, decimalPlaces);
            Assert.Equal(0.5, color.V, decimalPlaces);

            Assert.Equal(" more text", reader.ReadLine());
        }

        [Fact] public void ColorCanBeOutputInUnspecifiedColorSpace() {
            var color = new Color(new [] { 64, 128, 128 });
            Assert.Equal("= { 64 128 128 }", color.Output());
        }

        [Fact]
        public void ColorCanBeOutputInRgbColorSpace() {
            var color = new Color(new [] { 64, 128, 128 });
            Assert.Equal("= rgb { 64 128 128 }", color.OutputRgb());
        }

        [Fact]
        public void ColorCanBeOutputInHexColorSpace() {
            var color = new Color(new [] { 64, 128, 128 });
            Assert.Equal("= hex { 408080 }", color.OutputHex());
        }

        [Fact]
        public void ColorCanBeOutputInHexColorSpacePreservingZeroes() {
            var color = new Color(new [] { 0, 0, 0 });
            Assert.Equal("= hex { 000000 }", color.OutputHex());
        }

        [Fact]
        public void ColorCanBeOutputInHsvColorSpace() {
            var color = new Color(new [] { 64, 128, 128 });
            Assert.Equal("= hsv { 0.5 0.5 0.5 }", color.OutputHsv());
        }

        [Fact]
        public void ColorCanBeOutputInHsv360ColorSpace() {
            var color = new Color(new [] { 64, 128, 128 });
            Assert.Equal("= hsv360 { 180 50 50 }", color.OutputHsv360());
        }

        [Fact]
        public void UnequalFromDifferentRgb() {
            var color1 = new Color(new [] { 2, 4, 8 });
            var color2 = new Color(new [] { 3, 4, 8 });
            Assert.NotEqual(color1, color2);
        }

        [Fact] public void UnequalFromDifferentHsv() {
            var color1 = new Color(new [] { 0.333, 0.50, 0.50 });
            var color2 = new Color(new [] { 0.333, 0.75, 0.75 });
            Assert.NotEqual(color1, color2);
        }

        [Fact]
        public void Equality() {
            var color1 = new Color(new [] { 2, 4, 8 });
            var color2 = new Color(new [] { 2, 4, 8 });
            Assert.NotEqual(color1, color2);
        }

        [Fact] public void ColorPaletteCanBeInitializedByDict() {
            var dict = new Dictionary<string, Color> {
                {"white", new Color(new[] {255, 255, 255})},
                {"gray", new Color(new[] {50, 50, 50})}
            };
            var colorFactory = new ColorFactory();
            colorFactory.AddNamedColorDict(dict);

            var reader = new BufferedReader("= white");
            var white = colorFactory.GetColor(reader);
            var reader2 = new BufferedReader("= gray");
            var gray = colorFactory.GetColor(reader2);

            Assert.Equal(255, white.R);
            Assert.Equal(255, white.G);
            Assert.Equal(255, white.B);

            Assert.Equal(0, white.H, decimalPlaces);
            Assert.Equal(0, white.S, decimalPlaces);
            Assert.Equal(1, white.V, decimalPlaces);

            Assert.Equal(50, gray.R);
            Assert.Equal(50, gray.G);
            Assert.Equal(50, gray.B);

            Assert.Equal(0, gray.H, decimalPlaces);
            Assert.Equal(0, gray.S, decimalPlaces);
            Assert.Equal(0.196, gray.V, decimalPlaces);
        }

        [Fact] public void ColorPaletteCanBeAlteredByStream() {
            var colorFactory = new ColorFactory();
            var reader = new BufferedReader("= { 255 0 0 }");
            colorFactory.AddNamedColor("gold", reader);

            var reader2 = new BufferedReader("= hex { FFD700 }");
            colorFactory.AddNamedColor("gold", reader2);

            var gold = colorFactory.GetColor("gold");

            Assert.Equal(255, gold.R);
            Assert.Equal(215, gold.G);
            Assert.Equal(0, gold.B);

            Assert.Equal(0.142, gold.H, decimalPlaces);
            Assert.Equal(1, gold.S, decimalPlaces);
            Assert.Equal(1, gold.V, decimalPlaces);
        }

        [Fact]
        public void ColorPaletteCanBeAlteredDirectly() {
            var colorFactory = new ColorFactory();
            colorFactory.AddNamedColor("gold", new Color(new [] { 255, 0, 0 }));
            colorFactory.AddNamedColor("gold", new Color(new [] { 255, 215, 0 }));

            var gold = colorFactory.GetColor("gold");

            Assert.Equal(255, gold.R);
            Assert.Equal(215, gold.G);
            Assert.Equal(0, gold.B);

            Assert.Equal(0.142, gold.H, decimalPlaces);
            Assert.Equal(1, gold.S, decimalPlaces);
            Assert.Equal(1, gold.V, decimalPlaces);
        }

        [Fact] public void ColorPaletteCanBeAlteredByDict() {
            var wrongDict = new Dictionary<string, Color> {
                {"white", new Color(new[] {0, 0, 0})},
                {"red", new Color(new[] {255, 255, 19})}
            };

            var colorFactory = new ColorFactory();
            colorFactory.AddNamedColorDict(wrongDict);

            var correctDict = new Dictionary<string, Color> {
                {"white", new Color(new[] {255, 255, 255})},
                {"red", new Color(new[] {255, 0, 0})}
            };
            colorFactory.AddNamedColorDict(correctDict);

            var reader1 = new BufferedReader("= white");
            var white = colorFactory.GetColor(reader1);
            var reader2 = new BufferedReader("= red");
            var red = colorFactory.GetColor(reader2);

            Assert.Equal(255, white.R);
            Assert.Equal(255, white.G);
            Assert.Equal(255, white.B);

            Assert.Equal(0, white.H, decimalPlaces);
            Assert.Equal(0, white.S, decimalPlaces);
            Assert.Equal(1, white.V, decimalPlaces);

            Assert.Equal(255, red.R);
            Assert.Equal(0, red.G);
            Assert.Equal(0, red.B);

            Assert.Equal(0, red.H, decimalPlaces);
            Assert.Equal(1, red.S, decimalPlaces);
            Assert.Equal(1, red.V, decimalPlaces);
        }

        [Fact]
        public void ColorPaletteCanBeCleared() {
            var colorDict = new Dictionary<string, Color> {
                {"white", new Color(new[] {0, 0, 0})},
                {"red", new Color(new[] {255, 255, 19})}
            };

            var colorFactory = new ColorFactory();
            colorFactory.AddNamedColorDict(colorDict);

            Assert.Equal(2, colorFactory.NamedColors.Count);
            colorFactory.Clear();
            Assert.Empty(colorFactory.NamedColors);
        }
    }
}
