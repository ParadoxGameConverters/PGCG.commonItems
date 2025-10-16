using commonItems.Serialization;
using System;
using System.Linq;
using System.Text;

namespace commonItems; 

public readonly struct Date : IComparable<Date>, IEquatable<Date>, IPDXSerializable {
	public int Year { get; }
	public int Month { get; }
	public int Day { get; }

	public Date() {
		Year = 1;
		Month = 1;
		Day = 1;
	}
	public Date(Date otherDate) : this(otherDate.Year, otherDate.Month, otherDate.Day) { }
	public Date(int year, int month, int day, bool AUC) : this() {
		Year = AUC ? ConvertAUCToAD(year) : year;
		Month = ClampMonth(month);
		Day = ClampDay(day);
	}
	public Date(int year, int month, int day) : this(year, month, day, false) { }
	public Date(string init) : this(init, false) { }
	public Date(string init, bool AUC) : this() {
		init = init.RemQuotes();

		var dateElements = init.Split('.').Where(x => !string.IsNullOrEmpty(x)).ToArray();
		try {
			if (dateElements.Length >= 3) {
				Year = int.Parse(dateElements[0]);
				Month = ClampMonth(int.Parse(dateElements[1]));
				Day = ClampDay(int.Parse(dateElements[2]));
			} else if (dateElements.Length == 2) {
				Year = int.Parse(dateElements[0]);
				Month = ClampMonth(int.Parse(dateElements[1]));
			} else if (dateElements.Length == 1) {
				Year = int.Parse(dateElements[0]);
			} else {
				Logger.Warn("Problem constructing date: at least a year should be provided!");
			}
		} catch (Exception e) {
			Logger.Warn($"Problem constructing date from string \"{init}\": {e.Message}!");
		}
		if (AUC) {
			Year = ConvertAUCToAD(Year);
		}
	}
	public Date(DateTimeOffset dateTimeOffset) : this(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day) {
	}

	private static int ClampMonth(int month) {
		return month switch {
			< 1 => 1,
			> 12 => 12,
			_ => month,
		};
	}
	
	private static int ClampDay(int day) {
		return day switch {
			< 1 => 1,
			> 31 => 31,
			_ => day,
		};
	}
	
	public static implicit operator Date(string dateString) => new Date(dateString);
	
	private static int DaysInMonth(int month) {
		if (month == 12) {
			return 31;
		}

		return daysByMonth[month] - daysByMonth[month - 1];
	}

	public readonly Date ChangeByDays(int days) {
		int newYear = Year;
		int newMonth = Month;
		int newDay = Day;

		if (days > 0) {
			do {
				var currentMonthIndex = newMonth - 1;
				bool doesMonthChange;
				var currentDayInYear = daysByMonth[currentMonthIndex] + newDay + days;
				if (newMonth < 12) {
					var nextMonthIndex = newMonth;
					doesMonthChange = currentDayInYear > daysByMonth[nextMonthIndex];
				} else {
					doesMonthChange = currentDayInYear > 365;
				}

				if (doesMonthChange) {
					var daysInMonth = DaysInMonth(newMonth);
					var tempDate = new Date(newYear, newMonth, newDay).ChangeByMonths(1);
					newYear = tempDate.Year;
					newMonth = tempDate.Month;
					newDay = tempDate.Day;

					var daysForward = daysInMonth - newDay + 1;
					newDay = 1;
					days -= daysForward;
				} else {
					newDay += days;
					days = 0;
				}
			}
			while (days > 0);
		} else if (days < 0) {
			do {
				var currentMonthIndex = newMonth - 1;
				bool doesMonthChange;
				var currentDayInYear = daysByMonth[currentMonthIndex] + newDay + days;
				if (newMonth > 1) {
					doesMonthChange = currentDayInYear <= daysByMonth[currentMonthIndex];
				} else {
					doesMonthChange = currentDayInYear <= 0;
				}

				if (doesMonthChange) {
					var tempDate = new Date(newYear, newMonth, newDay).ChangeByMonths(-1);
					newYear = tempDate.Year;
					newMonth = tempDate.Month;
					newDay = tempDate.Day;
					
					var daysInMonth = DaysInMonth(newMonth);
					var daysBackward = newDay;
					newDay = daysInMonth;
					days += daysBackward;
				} else {
					newDay += days;
					days = 0;
				}
			}
			while (days < 0);
		}

		return new Date(newYear, newMonth, newDay);
	}

	public readonly Date ChangeByMonths(int months) {
		int newYear = Year;
		int newMonth = Month;
		
		newYear += months / 12;
		newMonth += months % 12;
		if (newMonth > 12) {
			++newYear;
			newMonth -= 12;
		} else if (newMonth < 1) {
			--newYear;
			newMonth += 12;
		}
		
		return new Date(newYear, newMonth, Day);
	}

	public readonly Date ChangeByYears(int years) {
		return new Date(Year + years, Month, Day);
	}

	private static int ConvertAUCToAD(int yearAUC) {
		var yearAD = yearAUC - 753;
		if (yearAD <= 0) {
			--yearAD;
		}
		return yearAD;
	}

	public readonly double DiffInYears(Date rhs) {
		double years = Year - rhs.Year;
		years += (double)(CalculateDayInYear() - rhs.CalculateDayInYear()) / 365;

		return years;
	}

	public override readonly string ToString() {
		var sb = new StringBuilder();
		sb.Append(Year);
		sb.Append('.');
		sb.Append(Month);
		sb.Append('.');
		sb.Append(Day);
		return sb.ToString();
	}

	public readonly string Serialize(string indent, bool withBraces) {
		return ToString();
	}

	private static readonly int[] daysByMonth = [
		0,	// January
		31,	// February
		59,	// March
		90,	// April
		120, // May
		151, // June
		181, // July
		212, // August
		243, // September
		273, // October
		304, // November
		334, // December
	];

	private readonly int CalculateDayInYear() {
		if (Month is >= 1 and <= 12) {
			return Day + daysByMonth[Month - 1];
		}
		return Day;
	}
	
	public readonly bool Equals(Date other) {
		return Year == other.Year &&
		       Month == other.Month &&
		       Day == other.Day;
	}

	public override readonly bool Equals(object? obj) {
		return obj is Date date && Equals(date);
	}

	public override readonly int GetHashCode() {
		return HashCode.Combine(Year, Month, Day);
	}

	public static bool operator <(Date lhs, Date rhs) {
		return ((lhs.Year < rhs.Year) || ((lhs.Year == rhs.Year) && (lhs.Month < rhs.Month)) ||
		        ((lhs.Year == rhs.Year) && (lhs.Month == rhs.Month) && (lhs.Day < rhs.Day)));
	}
	public static bool operator >(Date lhs, Date rhs) {
		return ((lhs.Year > rhs.Year) || ((lhs.Year == rhs.Year) && (lhs.Month > rhs.Month)) ||
		        ((lhs.Year == rhs.Year) && (lhs.Month == rhs.Month) && (lhs.Day > rhs.Day)));
	}
	public static bool operator <=(Date lhs, Date rhs) {
		return (lhs.Equals(rhs) || (lhs < rhs));
	}
	public static bool operator >=(Date lhs, Date rhs) {
		return (lhs.Equals(rhs) || (lhs > rhs));
	}

	public readonly int CompareTo(Date other) {
		var result = Year.CompareTo(other.Year);
		if (result != 0) {
			return result;
		}
		result = Month.CompareTo(other.Month);
		if (result != 0) {
			return result;
		}
		return Day.CompareTo(other.Day);
	}
	
	public static bool operator ==(Date left, Date right) {
		return left.Equals(right);
	}
	public static bool operator !=(Date left, Date right) {
		return !left.Equals(right);
	}

	public readonly DateTimeOffset ToDateTimeOffset() {
		return new DateTimeOffset(new DateTime(Year, Month, Day), offset: TimeSpan.Zero);
	}
}
