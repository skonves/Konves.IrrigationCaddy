using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Konves.IrrigationCaddy.Client
{
    public sealed class SystemSettings
    {
        public string FirmwareVersion { get; internal set; }
        public string NtpServer { get; internal set; }
        public string Hostname { get; internal set; }
        public IpAddressAssignment IpAddressAssignment { get; internal set; }
        public int HttpPort { get; internal set; }
        public IPAddress IpAddress { get; internal set; }
        public IPAddress SubnetMask { get; internal set; }
        public IPAddress GatewayAddress { get; internal set; }
        public int ZoneCount { get; internal set; }
        public IReadOnlyCollection<string> ZoneNames { get; internal set; }
        public TimeSpan MaxRunTime { get; set; }
        public bool UseNtpServer { get; internal set; }
        public IReadOnlyCollection<TimeZoneInfo> Timezones { get; internal set; }
        public string TimezoneDisplayString { get { return string.Join(", ", Timezones.Skip(1).Select(z => z.DisplayName.Substring(z.DisplayName.IndexOf(")") + 2)).Prepend(Timezones.First().DisplayName)); } }
        public bool UseRainSensor { get; internal set; }
        public bool IsRainSensorNormallyOpen { get; internal set; }
        public bool UseZone10AsMaster { get; internal set; }
    }
}
