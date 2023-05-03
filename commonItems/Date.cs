using commonItems.Serialization;
using System;
using System.Text;

namespace commonItems; 

public class Date : IComparable<Date>, IPDXSerializable {
	public int Year { get; } = 1;
	public int Month { get; } = 1;
	public int Day { get; } = 1;

	public Date() { }
	public Date(Date otherDate) {
		Year = otherDate.Year;
		Month = otherDate.Month;
		Day = otherDate.Day;
	}
	public Date(int year, int month, int day, bool AUC) {
		Year = AUC ? ConvertAUCToAD(year) : year;
		Month = month;
		Day = day;
	}
	public Date(int year, int month, int day) : this(year, month, day, false) { }
	public Date(string init) : this(init, false) { }
	public Date(string init, bool AUC) {
		if (init.Length < 1) {
			return;
		}
		init = init.RemQuotes();

		var firstDot = init.IndexOf('.');
		var lastDot = init.LastIndexOf('.');
		try {
			Year = int.Parse(init.Substring(0, firstDot));
			if (AUC) {
				Year = ConvertAUCToAD(Year);
			}
			Month = int.Parse(init.Substring(firstDot + 1, lastDot - firstDot - 1));
			Day = int.Parse(init.Substring(lastDot + 1));
		} catch (Exception e) {
			Logger.Warn("Problem inputting date: " + e);
			Year = 1;
			Month = 1;
			Day = 1;
		}
	}
	public Date(DateTimeOffset dateTimeOffset) {
		Year = dateTimeOffset.Year;
		Month = dateTimeOffset.Month;
		Day = dateTimeOffset.Day;
	}
	
	public static implicit operator Date(string dateString) => new Date(dateString);
	
	private static int DaysInMonth(int month) {
		if (month == 12) {
			return 31;
		}

		return DaysByMonth[month] - DaysByMonth[month - 1];
	}

	public Date ChangeByDays(int days) {
		int newYear = Year;
		int newMonth = Month;
		int newDay = Day;

		if (days > 0) {
			do {
				var currentMonthIndex = newMonth - 1;
				bool doesMonthChange;
				var currentDayInYear = DaysByMonth[currentMonthIndex] + newDay + days;
				if (newMonth < 12) {
					var nextMonthIndex = newMonth;
					doesMonthChange = currentDayInYear > DaysByMonth[nextMonthIndex];
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
				var currentDayInYear = DaysByMonth[currentMonthIndex] + newDay + days;
				if (newMonth > 1) {
					doesMonthChange = currentDayInYear <= DaysByMonth[currentMonthIndex];
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

	public Date ChangeByMonths(int months) {
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

	public Date ChangeByYears(int years) {
		return new Date(Year + years, Month, Day);
	}

	private static int ConvertAUCToAD(int yearAUC) {
		var yearAD = yearAUC - 753;
		if (yearAD <= 0) {
			--yearAD;
		}
		return yearAD;
	}

	public double DiffInYears(Date rhs) {
		double years = Year - rhs.Year;
		years += (double)(CalculateDayInYear() - rhs.CalculateDayInYear()) / 365;

		return years;
	}

	public bool IsSet() {
		return !Equals(new Date());
	}

	public override string ToString() {
		var sb = new StringBuilder();
		sb.Append(Year);
		sb.Append('.');
		sb.Append(Month);
		sb.Append('.');
		sb.Append(Day);
		return sb.ToString();
	}

	public string Serialize(string indent, bool withBraces) {
		return ToString();
	}

	private static readonly int[] DaysByMonth = {
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
		334	// December
	};

	private int CalculateDayInYear() {
		if (Month is >= 1 and <= 12) {
			return Day + DaysByMonth[Month - 1];
		}
		return Day;
	}
	
	public override bool Equals(object? obj) {
		return obj is Date date &&
		       Year == date.Year &&
		       Month == date.Month &&
		       Day == date.Day;
	}

	public override int GetHashCode() {
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

	public int CompareTo(Date? obj) {
		if (obj is null) {
			return 1;
		}

		var result = Year.CompareTo(obj.Year);
		if (result != 0) {
			return result;
		}
		result = Month.CompareTo(obj.Month);
		if (result != 0) {
			return result;
		}
		return Day.CompareTo(obj.Day);
	}
	
	public static bool operator ==(Date? left, Date? right) {
		if (left is null) {
			return right is null;
		}
		return left.Equals(right);
	}
	public static bool operator !=(Date? left, Date? right) {
		return !(left == right);
	}

	public DateTimeOffset ToDateTimeOffset() {
		return new DateTimeOffset(new DateTime(Year, Month, Day), offset: TimeSpan.Zero);
	}
}