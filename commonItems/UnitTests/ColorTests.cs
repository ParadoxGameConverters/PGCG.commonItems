﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace commonItems.UnitTests {
    public class ColorTests {
        readonly int decimalPlaces = 2;

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
            var testColor = new Color(new int[] { 64, 128, 128 });

            Assert.Equal(64, testColor.R);
            Assert.Equal(128, testColor.G);
            Assert.Equal(128, testColor.B);

            Assert.Equal(0.5, testColor.H, decimalPlaces);
            Assert.Equal(0.5, testColor.S, decimalPlaces);
            Assert.Equal(0.5, testColor.V, decimalPlaces);
        }

        [Fact] public void HsvConversion_GreyHasZeroHue() {
            var testColor = new Color(new int[] { 128, 128, 128 });
            Assert.Equal(0, testColor.H);
        }

        [Fact] public void HsvConversion_RedHasHueOfZero() {
            var testColor = new Color(new int[] { 128, 0, 0 });
            Assert.Equal(0, testColor.H);
        }

        [Fact] public void HsvConversion_YellowHasHueOfOneSixth() {
            var testColor = new Color(new int[] { 128, 128, 64 });
            Assert.Equal(0.167, testColor.H, decimalPlaces);
        }

        [Fact]
        public void HsvConversion_GreenHasHueOfOneThird() {
            var testColor = new Color(new int[] { 0, 128, 0 });
            Assert.Equal(0.333, testColor.H, decimalPlaces);
        }

        [Fact]
        public void HsvConversion_CyanHasHueOfOneHalf() {
            var testColor = new Color(new int[] { 64, 128, 128 });
            Assert.Equal(0.5, testColor.H, decimalPlaces);
        }

        [Fact]
        public void HsvConversion_BlueHasHueOfTwoThirds() {
            var testColor = new Color(new int[] { 0, 0, 128 });
            Assert.Equal(0.667, testColor.H, decimalPlaces);
        }

        [Fact]
        public void HsvConversion_MagentaHasHueOfFiveSixths() {
            var testColor = new Color(new int[] { 128, 64, 128 });
            Assert.Equal(0.833, testColor.H, decimalPlaces);
        }

        [Fact] public void HsvConversion_BlackHasZeroSaturation() {
            var testColor = new Color(new int[] { 0, 0, 0 });
            Assert.Equal(0, testColor.S);
        }

        [Fact]
        public void HsvConversion_GreyHasZeroSaturation() {
            var testColor = new Color(new int[] { 128, 128, 128 });
            Assert.Equal(0, testColor.S);
        }

        [Fact]
        public void HsvConversion_ColorHasSaturation() {
            var testColor = new Color(new int[] { 128, 128, 64 });
            Assert.Equal(0.5, testColor.S, decimalPlaces);
        }

        [Fact]
        public void HsvConversion_BlackHasZeroValue() {
            var testColor = new Color(new int[] { 0, 0, 0 });
            Assert.Equal(0, testColor.V);
        }

        [Fact]
        public void HsvConversion_ColorHasValue() {
            var testColor = new Color(new int[] { 128, 64, 64 });
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
            var testColor2 = new Color(new double[] { 1.1, 1.0, 1.0 });
            Assert.InRange(testColor1.R, 255 - 1, 255 + 1);
            Assert.InRange(testColor1.G, 0 - 1, 0 + 1);
            Assert.InRange(testColor1.B, 0 - 1, 0 + 1);
            Assert.InRange(testColor2.R, 255 - 1, 255 + 1);
            Assert.InRange(testColor2.G, 0 - 1, 0 + 1);
            Assert.InRange(testColor2.B, 0-1, 0+1);
        }
    }
}
