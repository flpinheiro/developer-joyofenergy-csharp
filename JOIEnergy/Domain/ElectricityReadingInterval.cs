using System;

namespace JOIEnergy.Domain
{
    public class ElectricityReadingInterval
    {
        public readonly DateTime Start;
        public readonly DateTime End;

        public ElectricityReadingInterval(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public static ElectricityReadingInterval getPreviousWeekInterval()
        {
            var today = DateTime.Now.Date;

            return getPreviousWeekInterval(today);
        }

        public static ElectricityReadingInterval getPreviousWeekInterval(DateTime today)
        {
            DateTime sunday = getSunday(today.Date);
            DateTime sundayEarly = sunday.AddDays(-7);

            return new ElectricityReadingInterval(sundayEarly, sunday);
        }

        private static DateTime getSunday(DateTime date)
        {
            while (date.DayOfWeek != DayOfWeek.Sunday)
            {
                date = date.AddDays(-1);
            }

            return date;
        }
    }
}
