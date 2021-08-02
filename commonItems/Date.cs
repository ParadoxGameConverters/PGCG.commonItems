using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace commonItems {
    public class Date {
        private int year = 1;
        private int month = 1;
        private int day = 1;

        public Date() {}

        public Date(int year, int month, int day, bool AUC) {
            this.year = AUC ? ConvertAUCtoAD(year) : year;
            this.month = month;
            this.day = day;
        }
        public Date(int year, int month, int day) : this(year, month, day, false) {}
        public Date(string init) : this(init, false) {}
        public Date(string init, bool AUC) {
            if (init.Length < 1) {
                return;
            }
            init = StringUtils.RemQuotes(init);

            var firstDot = init.IndexOf('.');
            var lastDot = init.LastIndexOf('.');
            try {
                year = int.Parse(init.Substring(0, firstDot));
                if (AUC) {
                    year = ConvertAUCtoAD(year);
                }
                month = int.Parse(init.Substring(firstDot + 1, lastDot - firstDot));
                day = int.Parse(init.Substring(lastDot + 1, 2));
            } catch (Exception e) {
                Logger.Log(LogLevel.Warning, "Problem inputting date: " + e);
                year = 1;
                month = 1;
                day = 1;
            }
        }

        public void IncreaseByMonths(int months) {
            year += months / 12;
            month += months % 12;
            if (month > 12) {
                ++year;
                month -= 12;
            }
        }

        public void SubtractYears(int years) {
            year -= years;
        }

        private int ConvertAUCtoAD(int yearAUC) {
            var yearAD = yearAUC - 753;
            if (yearAD <= 0) {
                --yearAD;
            }
            return yearAD;
        }

        public double DiffInYears(Date rhs) {
            double years = year - rhs.year;
            years += CalculateDayInYear() - rhs.CalculateDayInYear() / 365;

            return years;
        }

        public bool IsSet() {
            return !this.Equals(new Date());
        }

        public override string ToString() {
            var sb = new StringBuilder(year);
            sb.Append('.');
            sb.Append(month);
            sb.Append('.');
            sb.Append(day);
            return sb.ToString();
        }

        private int[] DaysByMonth = new[] {
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
            if (month >= 1 && month <= 12) {
                return day + DaysByMonth[month - 1];
            } else {
                return day;
            }
        }
    }
}
