using System.Text.Json;

namespace FoodReserve.AdminPortal.Helpers
{
    public static class OperatingHoursHelper
    {
        // List of time options for dropdowns
        public static IEnumerable<string> TimeOptions => Enumerable.Range(0, 24)
            .SelectMany(hour => new[] { $"{hour:00}:00", $"{hour:00}:30" })
            .ToList();

        // Convert from JSON string array to human-readable format
        public static string FormatOperatingHours(string? operatingHoursJson)
        {
            if (string.IsNullOrEmpty(operatingHoursJson))
                return "Not specified";

            try
            {
                var hours = JsonSerializer.Deserialize<string[]>(operatingHoursJson);
                
                if (hours == null || hours.Length < 2)
                    return "Invalid hours format";

                var startTime = FormatTimeDisplay(hours[0]);
                var endTime = FormatTimeDisplay(hours[1]);

                return $"{startTime} - {endTime}";
            }
            catch
            {
                return "Invalid hours format";
            }
        }

        // Convert from two time strings to JSON array string
        public static string CreateOperatingHoursJson(string startTime, string endTime)
        {
            var hours = new[] { startTime, endTime };
            return JsonSerializer.Serialize(hours);
        }

        // Format time in 12-hour format for display
        private static string FormatTimeDisplay(string time)
        {
            if (string.IsNullOrEmpty(time) || !time.Contains(':'))
                return time;

            string[] parts = time.Split(':');
            if (parts.Length != 2 || !int.TryParse(parts[0], out int hour))
                return time;

            string period = hour >= 12 ? "PM" : "AM";
            int displayHour = hour > 12 ? hour - 12 : (hour == 0 ? 12 : hour);
            
            return $"{displayHour}:{parts[1]} {period}";
        }
    }
}
