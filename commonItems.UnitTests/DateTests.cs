using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace commonItems.UnitTests; 

[Collection("Sequential")]
[CollectionDefinition("Sequential", DisableParallelization = true)]
public class DateTests {
	private const int DecimalPlaces = 4;

	[Fact]
	public void DefaultDateIsNotSet() {
		var date = new Date();
		Assert.False(date.IsSet());
	}
	[Fact]
	public void DateCanBeCopyConstructed() {
		var baseDate = new Date(500, 1, 1);
		var copyDate = new Date(baseDate);
		Assert.Equal("500.1.1", copyDate.ToString());
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
		Assert.Contains("[WARN] Problem inputting date: System.ArgumentOutOfRangeException", output.ToString());
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
		Assert.Equal(0.246575, date1.DiffInYears(date2), DecimalPlaces);
	}

	[Fact]
	public void DiffInYearsHandlesWrapAround() {
		var date1 = new Date(2020, 4, 25);
		var date2 = new Date(2019, 8, 25);
		Assert.Equal(0.665753, date1.DiffInYears(date2), DecimalPlaces);
	}

	[Fact]
	public void DiffInYearsHandlesNegative() {
		var date1 = new Date(2020, 1, 25);
		var date2 = new Date(2020, 4, 25);
		Assert.Equal(-0.246575, date1.DiffInYears(date2), DecimalPlaces);
	}

	[Fact]
	public void MonthsCanBeIncreased() {
		var date = new Date(2020, 4, 25).ChangeByMonths(4);
		Assert.Equal("2020.8.25", date.ToString());
	}
	[Fact]
	public void MonthsCanBeIncreasedAndWrapAround() {
		var date = new Date(2020, 4, 25).ChangeByMonths(9);
		Assert.Equal("2021.1.25", date.ToString());
	}
	[Fact]
	public void YearsCanBeIncreased() {
		var date = new Date(2020, 4, 25).ChangeByYears(4);
		Assert.Equal("2024.4.25", date.ToString());
	}
	[Fact]
	public void YearsCanBeDecreased() {
		var date = new Date(2020, 4, 25).ChangeByYears(-4);
		Assert.Equal("2016.4.25", date.ToString());
	}

	[Fact]
	public void DayCanBeIncreasedWithoutChangingMonth() {
		var date = new Date(500, 1, 5).ChangeByDays(6);
		Assert.Equal("500.1.11", date.ToString());
	}
	[Fact]
	public void DayCanBeIncreasedWithChangingMonth() {
		var date = new Date(500, 1, 30).ChangeByDays(28 + 6);
		Assert.Equal("500.3.5", date.ToString());
	}
	[Fact]
	public void DayCanBeIncreasedWithChangingYear() {
		var date = new Date(500, 12, 31).ChangeByDays(2);
		Assert.Equal("501.1.2", date.ToString());
	}

	[Fact]
	public void DayCanBeDecreasedWithoutChangingMonth() {
		var date = new Date(500, 1, 29).ChangeByDays(-9);
		Assert.Equal("500.1.20", date.ToString());
	}
	[Fact]
	public void DayCanBeDecreasedWithChangingMonth() {
		var date = new Date(500, 2, 5).ChangeByDays(-7);
		Assert.Equal("500.1.29", date.ToString());
	}
	[Fact]
	public void MonthChangesWhenDateIsDecreasedByOneDayOnTheFirstDayOfAMonth() {
		var date = new Date(500, 2, 1).ChangeByDays(-1);
		Assert.Equal("500.1.31", date.ToString());
	}
	[Fact]
	public void DayCanBeDecreasedWithChangingYear() {
		var date = new Date(501, 2, 5).ChangeByDays(-31 - 7);
		Assert.Equal("500.12.29", date.ToString());
	}

	[Theory]
	[InlineData("450.10.1", "-304.10.1")] // Imperator start date
	[InlineData("954.3.1", "201.3.1")]
	[InlineData("1306.3.1", "553.3.1")]
	public void AUCCanBeConvertedToAD(string aucDateString, string expectedADDate) {
		var date = new Date(aucDateString, true);
		Assert.Equal(expectedADDate, date.ToString());
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

	private class DescendingComparer<T> : IComparer<T> where T : IComparable<T> {
		public int Compare(T? x, T? y) {
			if (y is null) {
				return -1;
			}
			return y.CompareTo(x);
		}
	}
	[Fact]
	public void DateCanBeUsedByIComparer() {
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

	[Fact]
	public void StringCanBeImplicitlyConvertedToDate() {
		Date date = "800.4.5";
		Assert.Equal(800, date.Year);
		Assert.Equal(4, date.Month);
		Assert.Equal(5, date.Day);
	}

	[Theory]
	[InlineData(1, 1, 1, "1.1.1")]
	[InlineData(1, 12, 31, "1.12.31")]
	[InlineData(9999, 1, 1, "9999.1.1")]
	public void DateCanBeConstructedFromDateTimeOffset(int year, int month, int day, string expectedDateStr) {
		var dateTime = new DateTime(year, month, day);
		var dateTimeOffset = new DateTimeOffset(dateTime, offset: TimeSpan.Zero);
		var constructedPDXDate = new Date(dateTimeOffset);
		
		Date expectedPDXDate = expectedDateStr;
		Assert.Equal(expectedPDXDate, constructedPDXDate);
		Assert.Equal(year, constructedPDXDate.Year);
		Assert.Equal(month, constructedPDXDate.Month);
		Assert.Equal(day, constructedPDXDate.Day);
	}

	[Theory]
	[InlineData("1.1.1", 1, 1, 1)]
	[InlineData("1.12.31", 1, 12, 31)]
	[InlineData("9999.1.1", 9999, 1, 1)]
	public void DateCanBeConvertedToDateTimeOffset(string dateStr, int expectedYear, int expectedMonth, int expectedDay) {
		Date pdxDate = dateStr;
		var dateTimeOffset = pdxDate.ToDateTimeOffset();
		
		Assert.Equal(expectedYear, dateTimeOffset.Year);
		Assert.Equal(expectedMonth, dateTimeOffset.Month);
		Assert.Equal(expectedDay, dateTimeOffset.Day);
	}
}