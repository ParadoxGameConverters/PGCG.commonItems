using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace commonItems.UnitTests {
    [Collection("Sequential")]
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class DateTests {
        private const int decimalPlaces = 4;

        [Fact]
        public void DefaultDateIsNotSet() {
            var date = new Date();
            Assert.False(date.IsSet());
        }
        [Fact]
        public void DefaultDateEqualsOneJanuaryFirst() {
            var date = new Date();
            Assert.Equal("1.1.1", date.ToString());
        }
        [Fact]
        public void DateCanBeInput() {
            var date = new Date(2020, 4, 25);
            Assert.Equal("2020.4.25", date.ToString());
        }
        [Fact]
        public void DateCanBeInputFromString() {
            var date = new Date("2020.4.25");
            Assert.Equal("2020.4.25", date.ToString());
        }

        [Fact]
        public void DateLogsBadInitialization() {
            var output = new StringWriter();
            Console.SetOut(output);
            _ = new Date("2020.4");
            Assert.StartsWith("[WARN] Problem inputting date: System.ArgumentOutOfRangeException", output.ToString());
        }

        [Fact]
        public void DateIsNotSetOnBadInitialization() {
            var date = new Date("2020.4");
            Assert.False(date.IsSet());
        }

        [Fact]
        public void DateIsOneJanuaryFirstOnBadInitialization() {
            var date = new Date("2020.4");
            Assert.Equal("1.1.1", date.ToString());
        }

        [Fact]
        public void DatesCanBeEqual() {
            var testDate = new Date(2020, 4, 25);
            var testDateTwo = new Date(2020, 4, 25);
            Assert.Equal(testDate, testDateTwo);
        }

        [Fact]
        public void DatesCanBeUnequalFromDay() {
            var testDate = new Date(2020, 4, 25);
            var testDateTwo = new Date(2020, 4, 24);
            Assert.NotEqual(testDate, testDateTwo);
        }

        [Fact]
        public void DatesCanBeUnequalFromMonth() {
            var testDate = new Date(2020, 4, 25);
            var testDateTwo = new Date(2020, 3, 25);
            Assert.NotEqual(testDate, testDateTwo);
        }

        [Fact]
        public void DatesCanBeUnequalFromYear() {
            var testDate = new Date(2019, 4, 25);
            var testDateTwo = new Date(2020, 4, 25);
            Assert.NotEqual(testDate, testDateTwo);
        }

        [Fact]
        public void DatesCanBeLessThanFromDay() {
            var testDate = new Date(2019, 4, 24);
            var testDateTwo = new Date(2020, 4, 25);
            Assert.True(testDate < testDateTwo);
        }

        [Fact]
        public void DatesCanBeLessThanFromMonth() {
            var testDate = new Date(2020, 3, 26);
            var testDateTwo = new Date(2020, 4, 25);
            Assert.True(testDate < testDateTwo);
        }

        [Fact]
        public void DatesCanBeLessThanFromYear() {
            var testDate = new Date(2019, 5, 26);
            var testDateTwo = new Date(2020, 4, 25);
            Assert.True(testDate < testDateTwo);
        }

        [Fact]
        public void DatesCanBeGreaterThanFromDay() {
            var testDate = new Date(2020, 4, 26);
            var testDateTwo = new Date(2020, 4, 25);
            Assert.True(testDate > testDateTwo);
        }

        [Fact]
        public void DatesCanBeGreaterThanFromMonth() {
            var testDate = new Date(2020, 5, 24);
            var testDateTwo = new Date(2020, 4, 25);
            Assert.True(testDate > testDateTwo);
        }

        [Fact]
        public void DatesCanBeGreaterThanFromYear() {
            var testDate = new Date(2021, 3, 26);
            var testDateTwo = new Date(2020, 4, 25);
            Assert.True(testDate > testDateTwo);
        }

        [Fact]
        public void DatesCanBeLessThanOrEqualsFromLessThan() {
            var testDate = new Date(2020, 4, 24);
            var testDateTwo = new Date(2020, 4, 25);
            Assert.True(testDate <= testDateTwo);
        }

        [Fact]
        public void DatesCanBeLessThanOrEqualsFromEqualsThan() {
            var testDate = new Date(2020, 4, 25);
            var testDateTwo = new Date(2020, 4, 25);
            Assert.True(testDate <= testDateTwo);
        }

        [Fact]
        public void DatesCanBeGreaterThanOrEqualsFromGreaterThan() {
            var testDate = new Date(2020, 4, 26);
            var testDateTwo = new Date(2020, 4, 25);
            Assert.True(testDate >= testDateTwo);
        }

        [Fact]
        public void DatesCanBeGreaterThanOrEqualsFromEquals() {
            var testDate = new Date(2020, 4, 25);
            var testDateTwo = new Date(2020, 4, 25);
            Assert.True(testDate >= testDateTwo);
        }

        [Fact]
        public void DiffInYearsHandlesExactYears() {
            var date1 = new Date(2020, 4, 25);
            var date2 = new Date(2019, 4, 25);
            Assert.Equal(1.0d, date1.DiffInYears(date2));
        }

        [Fact]
        public void DiffInYearsHandlesPartialYears() {
            var date1 = new Date(2020, 4, 25);
            var date2 = new Date(2020, 1, 25);
            Assert.Equal(0.246575, date1.DiffInYears(date2), decimalPlaces);
        }

        [Fact]
        public void DiffInYearsHandlesWrapAround() {
            var date1 = new Date(2020, 4, 25);
            var date2 = new Date(2019, 8, 25);
            Assert.Equal(0.665753, date1.DiffInYears(date2), decimalPlaces);
        }

        [Fact]
        public void DiffInYearsHandlesNegative() {
            var date1 = new Date(2020, 1, 25);
            var date2 = new Date(2020, 4, 25);
            Assert.Equal(-0.246575, date1.DiffInYears(date2), decimalPlaces);
        }

        [Fact]
        public void MonthsCanBeIncreased() {
            var date = new Date(2020, 4, 25);
            date.IncreaseByMonths(4);
            Assert.Equal("2020.8.25", date.ToString());
        }
        [Fact]
        public void MonthsCanBeIncreasedAndWrapAround() {
            var date = new Date(2020, 4, 25);
            date.IncreaseByMonths(9);
            Assert.Equal("2021.1.25", date.ToString());
        }
        [Fact]
        public void YearsCanBeIncreased() {
            var date = new Date(2020, 4, 25);
            date.AddYears(4);
            Assert.Equal("2024.4.25", date.ToString());
        }
        [Fact]
        public void YearsCanBeDecreased() {
            var date = new Date(2020, 4, 25);
            date.SubtractYears(4);
            Assert.Equal("2016.4.25", date.ToString());
        }
        [Fact]
        public void AUCcanBeConvertedToAD() {
            var testDate = new Date(450, 10, 1, true);
            var testDate2 = new Date(1306, 3, 1, true);
            var testDate3 = new Date("450.10.1", true);
            var testDate4 = new Date("1306.3.1", true);

            Assert.Equal("-304.10.1", testDate.ToString());
            Assert.Equal("553.3.1", testDate2.ToString());
            Assert.Equal("-304.10.1", testDate3.ToString());
            Assert.Equal("553.3.1", testDate4.ToString());
        }
        [Fact]
        public void SeparateComponentsCanBeGotten() {
            var testDate = new Date("450.10.7");

            Assert.Equal(450, testDate.Year);
            Assert.Equal(10, testDate.Month);
            Assert.Equal(7, testDate.Day);
        }
        [Fact]
        public void NegativeYearComponentsCanBeGotten() {
            var testDate = new Date("-450.10.7");

            Assert.Equal(-450, testDate.Year);
            Assert.Equal(10, testDate.Month);
            Assert.Equal(7, testDate.Day);
        }

        class DescendingComparer<T> : IComparer<T> where T : IComparable<T> {
            public int Compare(T x, T y) {
                return y.CompareTo(x);
            }
        }
        [Fact] public void DateCanBeUsedByIComparer() {
            var dates = new SortedSet<Date>(new DescendingComparer<Date>()) { // should keep dates in descending order
                new Date(1000, 1, 1),
                new Date(1000, 2, 3),
                new Date(1000, 2, 1),
                new Date(900, 1, 1),
                new Date(5000, 1, 1),
                new Date(4, 1, 1)
            };
            Assert.Collection(dates,
                item => Assert.Equal(new Date(5000, 1, 1), item),
                item => Assert.Equal(new Date(1000, 2, 3), item),
                item => Assert.Equal(new Date(1000, 2, 1), item),
                item => Assert.Equal(new Date(1000, 1, 1), item),
                item => Assert.Equal(new Date(900, 1, 1), item),
                item => Assert.Equal(new Date(4, 1, 1), item)
            );
        }
    }
}
