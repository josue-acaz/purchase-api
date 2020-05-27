using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Voartec.Helpers
{
    public class DateHour
    {
        // Date helpers
        public dynamic PeriodOfWeek(DateTime actual_date)
        {
            dynamic obj = new ExpandoObject();
            var day_week = actual_date.DayOfWeek;
            int first_day = 0 - (int)day_week;
            int last_day = 6 - (int)day_week;

            DateTime date1 = actual_date.AddDays(first_day);
            DateTime date2 = actual_date.AddDays(last_day);


            obj.date1 = date1.ToString("yyyy-MM-dd");
            obj.date2 = date2.ToString("yyyy-MM-dd");
            return obj;

        }

        public int DiffDates(DateTime data1, DateTime data2)
        {
            return (data1 - data2).Days;
        }

        public bool is_date(string d)
        {
            if (String.IsNullOrEmpty(d)) return false;
            DateTime resultado = DateTime.MinValue;
            if (DateTime.TryParse(d.Trim(), out resultado))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Hours Helpers
        public bool IsDateTime(string strHora)
        {
            try
            {
                Convert.ToDateTime(strHora);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string applyDivisor(string hour, int divisor)
        {
            if (divisor == 0) return "00:00";

            string[] h1 = hour.Split(":");
            int hours = Int32.Parse(h1[0]);
            int minutes = Int32.Parse(h1[1]);

            int total_minutes = ((hours * 60) + minutes) / divisor;

            int result_hours = 0;
            while (total_minutes >= 60)
            {
                result_hours++;
                total_minutes -= 60;
            }


            return result_hours.ToString("00") + ":" + total_minutes.ToString("00");

        }

        public string calcDateTime(DateTime start, DateTime end)
        {
            TimeSpan ts = end.Subtract(start);
            int days = ts.Days;
            int hours = ts.Hours + (days * 24);
            return hours.ToString("00") + ":" + ts.Minutes.ToString("00");
        }

        public string calcHour(string hour1, string hour2)
        {
            if (String.IsNullOrEmpty(hour1)) throw new System.ArgumentException("O parâmetro Hora 1 não pode ser nulo. ", "hour1");
            if (String.IsNullOrEmpty(hour2)) throw new System.ArgumentException("O parâmetro Hora 2 não pode ser nulo. ", "hour2");

            string[] h1 = hour1.Split(":");
            string[] h2 = hour2.Split(":");

            int hours = Int32.Parse(h2[0]) - Int32.Parse(h1[0]);
            if (hours < 0) hours += 24;

            int minutes = Int32.Parse(h2[1]) - Int32.Parse(h1[1]);
            if (minutes < 0)
            {
                hours--;
                minutes += 60;
            }

            return hours.ToString().PadLeft(2, '0') + ':' + minutes.ToString().PadLeft(2, '0');
        }

        public string sumHour(string hour1, string hour2)
        {
            if (String.IsNullOrEmpty(hour1)) hour1 = "00:00";
            if (String.IsNullOrEmpty(hour2)) hour2 = "00:00";

            string[] h1 = hour1.Split(":");
            string[] h2 = hour2.Split(":");

            int hours = Int32.Parse(h2[0]) + Int32.Parse(h1[0]);
            int minutes = Int32.Parse(h2[1]) + Int32.Parse(h1[1]);

            if (minutes >= 60)
            {
                hours++;
                minutes = minutes - 60;
            }

            return hours.ToString().PadLeft(2, '0') + ':' + minutes.ToString().PadLeft(2, '0');
        }

        public string subtractHour(string hour1, string hour2)
        {
            string[] h1 = hour1.Split(":");
            string[] h2 = hour2.Split(":");

            int hours = Int32.Parse(h1[0]) - Int32.Parse(h2[0]);
            int minutes = Int32.Parse(h1[1]) - Int32.Parse(h2[1]);

            if (minutes < 0)
            {
                minutes = minutes + 60;
                hours--;
            }

            return hours.ToString().PadLeft(2, '0') + ':' + minutes.ToString().PadLeft(2, '0');
        }

        public decimal convertToDecimal(string hour)
        {
            if (String.IsNullOrEmpty(hour))
            {
                return 0;
            }

            string[] hr = hour.Split(":");
            decimal h = Int32.Parse(hr[0]);
            int m = Int32.Parse(hr[1]);
            decimal mresult = 0;

            if (m > 3 && m <= 9) mresult = 0.1M;
            if (m > 9 && m <= 15) mresult = 0.2M;
            if (m > 15 && m <= 21) mresult = 0.3M;
            if (m > 21 && m <= 27) mresult = 0.4M;
            if (m > 27 && m <= 33) mresult = 0.5M;
            if (m > 33 && m <= 39) mresult = 0.6M;
            if (m > 39 && m <= 45) mresult = 0.7M;
            if (m > 45 && m <= 51) mresult = 0.8M;
            if (m > 51 && m <= 57) mresult = 0.9M;
            if (m > 57) mresult = 1.0M;

            return h + mresult;
        }

    }
}
