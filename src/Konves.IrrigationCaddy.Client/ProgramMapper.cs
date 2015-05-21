using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Konves.IrrigationCaddy.Client
{
    internal static class ProgramMapper
    {
        internal static Program Map(string s)
        {
            JsonProgram jsonProgram = JsonConvert.DeserializeObject<JsonProgram>(s.Substring(s.IndexOf('{')));

            // Start times
            var hoursEnum = jsonProgram.StartTimeHours.GetEnumerator();
            var minutesEnum = jsonProgram.StartTimeMinutes.GetEnumerator();
            var isAmEnum = jsonProgram.IsAM.GetEnumerator();
            var statusEnum = jsonProgram.StartTimeStatus.Prepend(true).GetEnumerator();
            List<StartTime> startTimes = new List<StartTime>();
            while(hoursEnum.MoveNext() && minutesEnum.MoveNext() && isAmEnum.MoveNext() && statusEnum.MoveNext())
            {
                startTimes.Add(new StartTime
                {
                    IsEnabled = statusEnum.Current,
                    TimeOfDay = new TimeSpan(GetHour(hoursEnum.Current, isAmEnum.Current), minutesEnum.Current, 0)
                });
            }

            // Zones
            var zoneNamesEnum = jsonProgram.ZoneNames.GetEnumerator();
            var zoneDurEnum = jsonProgram.ZoneDurations.GetEnumerator();
            List<ZoneRunTime> zoneRunTimes = new List<ZoneRunTime>();
            while (zoneNamesEnum.MoveNext() && zoneDurEnum.MoveNext())
            {
                zoneRunTimes.Add(new ZoneRunTime(new TimeSpan(0, jsonProgram.MaxZoneRunTime, 0))
                {
                    ZoneName = zoneNamesEnum.Current,
                    Duration = new TimeSpan(zoneDurEnum.Current.Hours, zoneDurEnum.Current.Minutes, 0)
                });
            }

            // Run days
            DayOfWeek runDays = DayOfWeek.None;
            if (jsonProgram.Days[0])
                runDays |= DayOfWeek.Monday;
            if (jsonProgram.Days[1])
                runDays |= DayOfWeek.Tuesday;
            if (jsonProgram.Days[2])
                runDays |= DayOfWeek.Wednesday;
            if (jsonProgram.Days[3])
                runDays |= DayOfWeek.Thursday;
            if (jsonProgram.Days[4])
                runDays |= DayOfWeek.Friday;
            if (jsonProgram.Days[5])
                runDays |= DayOfWeek.Saturday;
            if (jsonProgram.Days[6])
                runDays |= DayOfWeek.Sunday;

            return new Program
            {
                Number = int.Parse(jsonProgram.Number),
                IsEnabled = jsonProgram.AllowRun,
                RunDays = runDays,
                StartTimes = new System.Collections.ObjectModel.ReadOnlyCollection<StartTime>(startTimes),
                ZoneRunTimes = new System.Collections.ObjectModel.ReadOnlyCollection<ZoneRunTime>(zoneRunTimes),
                MaxZoneRunTime = new TimeSpan(0, jsonProgram.MaxZoneRunTime, 0),
                ScheduleType = jsonProgram.EvenOdd == 1 ? ScheduleType.EvenDays : jsonProgram.EvenOdd == 2 ? ScheduleType.OddDays : ScheduleType.EveryNDays,
                Interval = jsonProgram.EveryNDays
            };
        }

        public static string Map(Program program)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("doProgram", "1");
            data.Add("allowRun", program.IsEnabled ? "yes" : "no");

            if (program.RunDays.HasFlag(DayOfWeek.Monday))
                data.Add("day_Mon", "1");
            if (program.RunDays.HasFlag(DayOfWeek.Tuesday))
                data.Add("day_Tue", "1");
            if (program.RunDays.HasFlag(DayOfWeek.Wednesday))
                data.Add("day_Wed", "1");
            if (program.RunDays.HasFlag(DayOfWeek.Thursday))
                data.Add("day_Thu", "1");
            if (program.RunDays.HasFlag(DayOfWeek.Friday))
                data.Add("day_Fri", "1");
            if (program.RunDays.HasFlag(DayOfWeek.Saturday))
                data.Add("day_Sat", "1");
            if (program.RunDays.HasFlag(DayOfWeek.Sunday))
                data.Add("day_Sun", "1");

            data.Add("evenodd", program.ScheduleType == ScheduleType.EvenDays ? "1" : program.ScheduleType == ScheduleType.OddDays ? "2" : "0");
            data.Add("everyNDays", program.Interval.ToString());

            for (int i = 0; i < program.StartTimes.Count; i++)
            {
                if (i > 0 && program.StartTimes[i].IsEnabled)
                    data.Add("stStat" + i, "1");

                if (i == 0)
                {
                    data.Add("stHr" + i, GetHour(program.StartTimes[i].TimeOfDay));
                    data.Add("startTime" + i, GetTimeString(program.StartTimes[i].TimeOfDay));
                }
                else
                {
                    data.Add("startTime" + i, GetTimeString(program.StartTimes[i].TimeOfDay));
                    data.Add("stHr" + i, GetHour(program.StartTimes[i].TimeOfDay));
                }

                data.Add("stMin" + i, GetMinute(program.StartTimes[i].TimeOfDay));
                data.Add("merid" + i, GetMeridian(program.StartTimes[i].TimeOfDay));
            }

            for (int i = 1; i <= program.ZoneRunTimes.Count; i++)
            {
                data.Add("z" + i + "durHr", program.ZoneRunTimes[i - 1].Duration.Hours.ToString());
                data.Add("z" + i + "durMin", program.ZoneRunTimes[i - 1].Duration.Minutes.ToString());
            }

            data.Add("pgmNum", program.Number.ToString());

            return string.Join("&", data.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
        }

        private static int GetHour(int value, bool isAm)
        {
            if(isAm)
            {
                if (value == 12)
                    return 0;

                return value;
            }
            else
            {
                if (value == 12)
                    return value;

                return value + 12;
            }
        }

        private static string GetHour(TimeSpan timeSpan)
        {
            if (timeSpan.Hours == 0)
                return "12";

            if (timeSpan.Hours > 12)
                return (timeSpan.Hours - 12).ToString();

            return timeSpan.Hours.ToString();
        }

        private static string GetMinute(TimeSpan timeSpan)
        {
            return timeSpan.Minutes.ToString();
        }

        private static string GetMeridian(TimeSpan timeSpan)
        {
            return timeSpan.Hours < 12 ? "am" : "pm";
        }

        private static string GetTimeString(TimeSpan timeSpan)
        {
            return string.Format("{0}%3A{1}+{2}", GetHour(timeSpan).PadLeft(2, '0'), GetMinute(timeSpan).PadLeft(2, '0'), GetMeridian(timeSpan));
        }

        class JsonProgram
        {
            [JsonProperty("progNumber")]
            public string Number { get; set; }
            [JsonProperty("progAllowRun")]
            public bool AllowRun { get; set; }
            [JsonProperty("days")]
            public List<bool> Days { get; set; }
            [JsonProperty("progStartTimeHr")]
            public List<int> StartTimeHours { get; set; }
            [JsonProperty("progStartTimeMin")]
            public List<int> StartTimeMinutes { get; set; }
            [JsonProperty("isAM")]
            public List<bool> IsAM { get; set; }
            [JsonProperty("zNames")]
            public List<string> ZoneNames { get; set; }
            [JsonProperty("maxZRunTime")]
            public int MaxZoneRunTime { get; set; }
            [JsonProperty("maxZones")]
            public int MaxZones { get; set; }
            [JsonProperty("zDur")]
            public List<JsonZoneDuration> ZoneDurations { get; set; }
            [JsonProperty("everyNDays")]
            public int EveryNDays { get; set; }
            [JsonProperty("evenOdd")]
            public int EvenOdd { get; set; }
            [JsonProperty("maxProgs")]
            public int MaxPrograms { get; set; }
            [JsonProperty("startTimesStatus")]
            public List<bool> StartTimeStatus { get; set; }
            [JsonProperty("hostname")]
            public string HostName { get; set; }
            [JsonProperty("ipAddress")]
            public string IpAddress { get; set; }
        }

        class JsonZoneDuration
        {
            [JsonProperty("hr")]
            public int Hours { get; set; }
            [JsonProperty("min")]
            public int Minutes { get; set; }
        }
    }
}