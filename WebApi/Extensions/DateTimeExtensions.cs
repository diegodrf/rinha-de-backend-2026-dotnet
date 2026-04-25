namespace WebApi.Extensions;

public static class DateTimeExtensions
{
    extension(DateTime d) {
        public int DayOfWeekMonToSun => d.DayOfWeek switch
        {
            DayOfWeek.Monday => 0,
            DayOfWeek.Tuesday => 1,
            DayOfWeek.Wednesday => 2,
            DayOfWeek.Thursday => 3,
            DayOfWeek.Friday => 4,
            DayOfWeek.Saturday => 5,
            DayOfWeek.Sunday => 6,
            _ => throw new InvalidCastException()
        };
    }
}