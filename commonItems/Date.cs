using commonItems.Serialization;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace commonItems; 

/// <summary>Slim immutable calendar date stored in 32 bits.</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct Date : IComparable<Date>, IEquatable<Date>, IPDXSerializable {
	public short Year { get; }
	public byte Month { get; }
	public byte Day { get; }

	public Date() {
		Year = 1;
		Month = 1;
		Day = 1;
	}
	public Date(Date otherDate) : this(otherDate.Year, otherDate.Month, otherDate.Day) { }
	public Date(int year, int month, int day, bool AUC) : this() {
		Year = ConvertYear(AUC ? ConvertAUCToAD(year) : year);
		Month = ClampMonth(month);
		Day = ClampDay(day);
	}
	public Date(int year, int month, int day) : this(year, month, day, false) { }
	public Date(string init) : this(init, false) { }
	public Date(string init, bool AUC) : this() {
		var dateSpan = init.RemQuotes().AsSpan();
		var parsedYear = (int)Year;
		try {
			if (TryParseDateFast(dateSpan, ref parsedYear, out var month, out var day, out var componentCount)) {
				if (componentCount >= 2) {
					Month = ClampMonth(month);
				}
				if (componentCount >= 3) {
					Day = ClampDay(day);
				}
			} else {
				var componentIndex = 0;
				var segmentStart = 0;
				for (var i = 0; i <= dateSpan.Length && componentIndex < 3; ++i) {
					if (i == dateSpan.Length || dateSpan[i] == '.') {
						var segmentLength = i - segmentStart;
						if (segmentLength > 0) {
							var segment = dateSpan.Slice(segmentStart, segmentLength);
							switch (componentIndex) {
								case 0:
									parsedYear = int.Parse(segment);
									break;
								case 1:
									Month = ClampMonth(int.Parse(segment));
									break;
								case 2:
									Day = ClampDay(int.Parse(segment));
									break;
							}
							++componentIndex;
						}
						segmentStart = i + 1;
					}
				}
				if (componentIndex == 0) {
					Logger.Warn("Problem constructing date: at least a year should be provided!");
				}
			}
		} catch (Exception e) {
			Logger.Warn($"Problem constructing date from string \"{init}\": {e.Message}!");
		}
		if (AUC) {
			parsedYear = ConvertAUCToAD(parsedYear);
		}
		Year = ConvertYear(parsedYear);
	}

	// Fast path for the common "Y.M.D" date shapes: a single forward pass with manual
	// digit accumulation. Returns false (and falls back to the int.Parse-based slow path)
	// on any anomaly, preserving the exact original semantics including partial assignments.
	private static bool TryParseDateFast(ReadOnlySpan<char> dateSpan, ref int parsedYear, out int month, out int day, out int componentCount) {
		month = 1;
		day = 1;
		componentCount = 0;
		var index = 0;
		while (index <= dateSpan.Length && componentCount < 3) {
			var segmentStart = index;
			while (index < dateSpan.Length && dateSpan[index] != '.') {
				++index;
			}
			var segmentLength = index - segmentStart;
			if (segmentLength > 0) {
				if (!TryParseIntSegment(dateSpan.Slice(segmentStart, segmentLength), out var value)) {
					return false;
				}
				switch (componentCount) {
					case 0:
						parsedYear = value;
						break;
					case 1:
						month = value;
						break;
					default:
						day = value;
						break;
				}
				++componentCount;
			}
			++index; // skip the separator dot
		}
		return componentCount > 0;
	}

	// Mirrors int.Parse(segment) semantics for sign + digit-only segments.
	private static bool TryParseIntSegment(ReadOnlySpan<char> segment, out int value) {
		value = 0;
		var index = 0;
		var isNegative = false;
		if (segment[0] is '-' or '+') {
			isNegative = segment[0] == '-';
			++index;
			if (index == segment.Length) {
				return false; // sign only
			}
		}
		long accumulated = 0;
		for (; index < segment.Length; ++index) {
			var c = segment[index];
			if (c is < '0' or > '9') {
				return false;
			}
			accumulated = (accumulated * 10) + (c - '0');
			if (accumulated > int.MaxValue) {
				return false; // let the slow path throw OverflowException as before
			}
		}
		value = isNegative ? (int)-accumulated : (int)accumulated;
		return true;
	}
	public Date(DateTimeOffset dateTimeOffset) : this(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day) {
	}

	private static byte ClampMonth(int month) {
		var clampedMonth = month switch {
			< 1 => 1,
			> 12 => 12,
			_ => month,
		};
		return (byte)clampedMonth;
	}
	
	private static byte ClampDay(int day) {
		var clampedDay = day switch {
			< 1 => 1,
			> 31 => 31,
			_ => day,
		};
		return (byte)clampedDay;
	}
	
	public static implicit operator Date(string dateString) => new Date(dateString);
	
	private static int DaysInMonth(int month) {
		if (month == 12) {
			return 31;
		}

		return daysByMonth[month] - daysByMonth[month - 1];
	}

	public Date ChangeByDays(int days) {
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

	private static short ConvertYear(int year) {
		if (year is < short.MinValue or > short.MaxValue) {
			throw new ArgumentOutOfRangeException(nameof(year), year, "Year must fit into 16 bits.");
		}
		return (short)year;
	}

	public double DiffInYears(Date rhs) {
		double years = Year - rhs.Year;
		years += (double)(CalculateDayInYear() - rhs.CalculateDayInYear()) / 365;

		return years;
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

	private int CalculateDayInYear() {
		if (Month is >= 1 and <= 12) {
			return Day + daysByMonth[Month - 1];
		}
		return Day;
	}
	
	public bool Equals(Date other) {
		return Year == other.Year &&
		       Month == other.Month &&
		       Day == other.Day;
	}

	public override bool Equals(object? obj) {
		return obj is Date date && Equals(date);
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

	public int CompareTo(Date other) {
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

	public DateTimeOffset ToDateTimeOffset() {
		return new DateTimeOffset(new DateTime(Year, Month, Day), offset: TimeSpan.Zero);
	}
}
