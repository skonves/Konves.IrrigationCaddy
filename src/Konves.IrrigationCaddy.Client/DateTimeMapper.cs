using Newtonsoft.Json;
using System;

namespace Konves.IrrigationCaddy.Client
{
    internal static class DateTimeMapper
    {
        internal static DateTime Map(string s)
        {
            dynamic data = JsonConvert.DeserializeObject<dynamic>(s);

            int year = data.year + 2000;
            int month = data.month;
            int day = data.date;
            int hour = data.hr;
            int minute = data.min;
            int second = data.sec;

            return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local);
        }
    }
}
