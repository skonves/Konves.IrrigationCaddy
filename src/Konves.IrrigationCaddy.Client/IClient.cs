using System;
namespace Konves.IrrigationCaddy.Client
{
    public interface IClient
    {
        Response<Program> GetProgram(int number);
        Response<SystemSettings> GetSystemSettings();
        Response<Status> GetSystemStatus();
        Response<DateTime> GetSystemTime();
        Response Restart();
        Response RunNow(TimeSpan zone1 = new TimeSpan(), TimeSpan zone2 = new TimeSpan(), TimeSpan zone3 = new TimeSpan(), TimeSpan zone4 = new TimeSpan(), TimeSpan zone5 = new TimeSpan(), TimeSpan zone6 = new TimeSpan(), TimeSpan zone7 = new TimeSpan(), TimeSpan zone8 = new TimeSpan(), TimeSpan zone9 = new TimeSpan());
        Response RunProgram(int number);
        Response SetMiscSettings(bool useRainSensor, bool isRainSensorNormallyOpen, bool useZone10AsMaster, TimeSpan maxZoneTime);
        Response SetNetworkSettings(string hostname, int httpPort, IpAddressAssignment ipAddressAssignment, System.Net.IPAddress ipAddress, System.Net.IPAddress subnetMask, System.Net.IPAddress gatewayAddress);
        Response SetNtpSettings(bool useNtpServer, string ntpServer, TimeZoneInfo timeZone);
        Response SetSystemTime(DateTime dateTime);
        Response SetZoneNames(string zone1, string zone2, string zone3, string zone4, string zone5, string zone6, string zone7, string zone8, string zone9);
        Response StopActiveZone();
        Response TurnSystemOff();
        Response TurnSystemOn();
        Response UpdateProgram(Program program);
    }
}
