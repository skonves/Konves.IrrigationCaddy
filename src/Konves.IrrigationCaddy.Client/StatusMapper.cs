using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Konves.IrrigationCaddy.Client
{
    internal static class StatusMapper
    {
        internal static Status Map(string s)
        {
            dynamic data = JsonConvert.DeserializeObject<dynamic>(s);

            JsonStatus jsonStatus = JsonConvert.DeserializeObject<JsonStatus>(s);

            return new Status(
                jsonStatus.ZoneNumber,
                jsonStatus.ProgramNumber,
                jsonStatus.AllowRun,
                jsonStatus.Running,
                jsonStatus.UseCloud,
                jsonStatus.CloudConnection,
                jsonStatus.UseSensor1,
                jsonStatus.IsRaining,
                new TimeSpan(0, 0, jsonStatus.ZoneSecondsLeft),
                new TimeSpan(0, 0, jsonStatus.ProgramSecondsLeft),
                jsonStatus.MaxZones,
                jsonStatus.Zones == null ? null : new ReadOnlyCollection<TimeSpan>(jsonStatus.Zones.Select(z => new TimeSpan(z.Hours, z.Minutes, 0)).ToList()));
        }

        class JsonStatus
        {
            [JsonProperty("zoneNumber")]
            public int ZoneNumber { get; set; }
            [JsonProperty("progNumber")]
            public int ProgramNumber { get; set; }
            [JsonProperty("allowRun")]
            public bool AllowRun { get; set; }
            [JsonProperty("running")]
            public bool Running { get; set; }
            [JsonProperty("useCloud")]
            public bool UseCloud { get; set; }
            [JsonProperty("cloudConn")]
            public string CloudConnection { get; set; }
            [JsonProperty("useSensor1")]
            public bool UseSensor1 { get; set; }
            [JsonProperty("isRaining")]
            public bool IsRaining { get; set; }
            [JsonProperty("zoneSecLeft")]
            public int ZoneSecondsLeft { get; set; }
            [JsonProperty("progSecLeft")]
            public int ProgramSecondsLeft { get; set; }
            [JsonProperty("maxZones")]
            public int MaxZones { get; set; }
            [JsonProperty("zones")]
            public List<JsonZone> Zones { get; set; }
        }

        class JsonZone
        {
            [JsonProperty("isRun")]
            public bool IsRun { get; set; }
            [JsonProperty("hr")]
            public int Hours { get; set; }
            [JsonProperty("min")]
            public int Minutes { get; set; }
        }
    }
}
