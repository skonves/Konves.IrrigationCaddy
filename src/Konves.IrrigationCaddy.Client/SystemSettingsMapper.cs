using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace Konves.IrrigationCaddy.Client
{
    public static class SystemSettingsMapper
    {
        public static SystemSettings Map(string s)
        {
            JsonSystemSettings jsonSettings = JsonConvert.DeserializeObject<JsonSystemSettings>(s.Substring(s.IndexOf('{')));

            TimeSpan dstOffset = new TimeSpan(0, (int)(jsonSettings.Timezone * 60), 0);
            List<TimeZoneInfo> timezones = TimeZoneInfo.GetSystemTimeZones().Where(z => z.SupportsDaylightSavingTime == jsonSettings.IsDst && z.BaseUtcOffset == dstOffset).ToList();

            return new SystemSettings
            {
                FirmwareVersion = jsonSettings.ICVersion,
                NtpServer = jsonSettings.NtpServer,
                Hostname = jsonSettings.Hostname,
                IpAddressAssignment = jsonSettings.IpAssign == "Static" ? IpAddressAssignment.Static : IpAddressAssignment.DHCP,
                HttpPort = jsonSettings.HttpPort,
                IpAddress = new IPAddress(jsonSettings.IpAddressPart.ToArray()),
                SubnetMask = new IPAddress(jsonSettings.NetMaskPart.ToArray()),
                GatewayAddress = new IPAddress(jsonSettings.GatewayPart.ToArray()),
                ZoneCount = jsonSettings.MaxZones,
                ZoneNames = new ReadOnlyCollection<string>(jsonSettings.ZoneNames),
                MaxRunTime = new TimeSpan(0, jsonSettings.MaxRunTime, 0),
                UseNtpServer = jsonSettings.IsNtp,
                Timezones = new ReadOnlyCollection<TimeZoneInfo>(timezones),
                UseRainSensor = jsonSettings.UseSensor1,
                IsRainSensorNormallyOpen = jsonSettings.IsSensor1Open,
                UseZone10AsMaster = jsonSettings.UseMaster
            };
        }

        class JsonSystemSettings
        {
            [JsonProperty("icVersion")]
            public string ICVersion { get; set; }
            [JsonProperty("ntpServer")]
            public string NtpServer { get; set; }
            [JsonProperty("hostname")]
            public string Hostname { get; set; }
            [JsonProperty("ipAssign")]
            public string IpAssign { get; set; }
            [JsonProperty("httpPort")]
            public int HttpPort { get; set; }
            [JsonProperty("ipAddress")]
            public string IpAddress { get; set; }
            [JsonProperty("mask")]
            public string Mask { get; set; }
            [JsonProperty("gateway")]
            public string Gateway { get; set; }
            [JsonProperty("ipAddressPart")]
            public List<byte> IpAddressPart { get; set; }
            [JsonProperty("netMaskPart")]
            public List<byte> NetMaskPart { get; set; }
            [JsonProperty("gatewayPart")]
            public List<byte> GatewayPart { get; set; }
            [JsonProperty("maxZones")]
            public int MaxZones { get; set; }
            [JsonProperty("zNames")]
            public List<string> ZoneNames { get; set; }
            [JsonProperty("maxZRunTime")]
            public int MaxRunTime { get; set; }
            [JsonProperty("isNTP")]
            public bool IsNtp { get; set; }
            [JsonProperty("isDST")]
            public bool IsDst { get; set; }
            [JsonProperty("timezone")]
            public decimal Timezone { get; set; }
            [JsonProperty("useCloud")]
            public bool UseCloud { get; set; }
            [JsonProperty("useSensor1")]
            public bool UseSensor1 { get; set; }
            [JsonProperty("controllerId")]
            public int ControllerId { get; set; }
            [JsonProperty("isSensor1Open")]
            public bool IsSensor1Open { get; set; }
            [JsonProperty("useMaster")]
            public bool UseMaster { get; set; }
            [JsonProperty("isAuth")]
            public bool IsAuth { get; set; }
        }
    }
}
