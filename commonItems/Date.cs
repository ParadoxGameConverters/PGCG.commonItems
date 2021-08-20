using System;
using System.Text;

namespace commonItems {
    public class Date : IComparable<Date> {
        public int Year { get; private set; } = 1;
        public int Month { get; private set; } = 1;
        public int Day { get; } = 1;

        public Date() { }

        public Date(int year, int month, int day, bool AUC) {
            Year = AUC ? ConvertAUCtoAD(year) : year;
            Month = month;
            Day = day;
        }
        public Date(int year, int month, int day) : this(year, month, day, false) { }
        public Date(string init) : this(init, false) { }
        public Date(string init, bool AUC) {
            if (init.Length < 1) {
                return;
            }
            init = StringUtils.RemQuotes(init);

            var firstDot = init.IndexOf('.');
            var lastDot = init.LastIndexOf('.');
            try {
                Year = int.Parse(init.Substring(0, firstDot));
                if (AUC) {
                    Year = ConvertAUCtoAD(Year);
                }
                Month = int.Parse(init.Substring(firstDot + 1, lastDot - firstDot -1));
                Day = int.Parse(init.Substring(lastDot + 1));
            } catch (Exception e) {
                Logger.Warn("Problem inputting date: " + e);
                Year = 1;
                Month = 1;
                Day = 1;
            }
        }

        public void IncreaseByMonths(int months) {
            Year += months / 12;
            Month += months % 12;
            if (Month > 12) {
                ++Year;
                Month -= 12;
            }
        }

        public void AddYears(int years) {
            Year += years;
        }

        public void SubtractYears(int years) {
            Year -= years;
        }

        private static int ConvertAUCtoAD(int yearAUC) {
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

        private readonly int[] DaysByMonth = new[] {
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
    }
}
