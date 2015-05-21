using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Konves.IrrigationCaddy.Client
{
    /// <summary>
    /// Provides access to Irrigation Caddy functionality.
    /// </summary>
    public sealed class Client : IClient
    {
        /// <summary>
        /// Creates an client instance for interacting with your Irrigation Caddy.
        /// </summary>
        /// <param name="host">The host name or IP address of your Irrigation Caddy</param>
        /// <param name="username">The user name.</param>
        /// <param name="password">The system password.</param>
        /// <param name="userAgent">The user agent used when making requests.</param>
        public Client(string host, string username, string password, string userAgent)
        {
            m_host = host;
            m_authHeader = string.Format("Basic {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password))));
            m_userAgent = userAgent;
        }

        /// <summary>
        /// Set system to the "On" state allowing any programs to run.
        /// </summary>
        public Response TurnSystemOn()
        {
            return GetRequest("POST", c_runSprinklersUrlFormat)
                .WithEncoding("gzip, deflate")
                .Accepting("*/*")
                .WithContent("run=run")
                .GetResponse();
        }

        /// <summary>
        /// Set system to the "Off" state preventing any programs from running.
        /// </summary>
        public Response TurnSystemOff()
        {
            return GetRequest("POST", c_stopSprinklersUrlFormat)
                .WithEncoding("gzip, deflate")
                .Accepting("*/*")
                .WithContent("stop=off")
                .GetResponse();
        }

        /// <summary>
        /// Stops the actively running zone allowing any following zones in the program to run.
        /// </summary>
        public Response StopActiveZone()
        {
            return GetRequest("POST", c_stopSprinklersUrlFormat)
                .WithEncoding("gzip, deflate")
                .Accepting("*/*")
                .WithContent("stop=active")
                .GetResponse();
        }

        /// <summary>
        /// Runs the specified program.
        /// </summary>
        public Response RunProgram(int number)
        {
            if (number < 1 || number > 3)
                throw new ArgumentOutOfRangeException("number", "'number' must be 1, 2, or 3.");

            return GetRequest("POST", c_runSprinklersUrlFormat)
                .WithEncoding("gzip, deflate")
                .Accepting("*/*")
                .WithContent(string.Format("pgmNum={0}&doProgram=1&runNow=true&time={1}", number, DateTime.UtcNow.ToTimeStamp()))
                .GetResponse();
        }

        /// <summary>
        /// Runs the specified zones once each.
        /// </summary>
        public Response RunNow(TimeSpan zone1 = new TimeSpan(), TimeSpan zone2 = new TimeSpan(), TimeSpan zone3 = new TimeSpan(), TimeSpan zone4 = new TimeSpan(), TimeSpan zone5 = new TimeSpan(), TimeSpan zone6 = new TimeSpan(), TimeSpan zone7 = new TimeSpan(), TimeSpan zone8 = new TimeSpan(), TimeSpan zone9 = new TimeSpan())
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("doProgram", "1");
            data.Add("z1durHr", zone1.Hours.ToString());
            data.Add("z1durMin", zone1.Minutes.ToString());
            data.Add("z2durHr", zone2.Hours.ToString());
            data.Add("z2durMin", zone2.Minutes.ToString());
            data.Add("z3durHr", zone3.Hours.ToString());
            data.Add("z3durMin", zone3.Minutes.ToString());
            data.Add("z4durHr", zone4.Hours.ToString());
            data.Add("z4durMin", zone4.Minutes.ToString());
            data.Add("z5durHr", zone5.Hours.ToString());
            data.Add("z5durMin", zone5.Minutes.ToString());
            data.Add("z6durHr", zone6.Hours.ToString());
            data.Add("z6durMin", zone6.Minutes.ToString());
            data.Add("z7durHr", zone7.Hours.ToString());
            data.Add("z7durMin", zone7.Minutes.ToString());
            data.Add("z8durHr", zone8.Hours.ToString());
            data.Add("z8durMin", zone8.Minutes.ToString());
            data.Add("z9durHr", zone9.Hours.ToString());
            data.Add("z9durMin", zone9.Minutes.ToString());
            data.Add("runNow", "1");
            data.Add("pgmNum", "4");

            string content = string.Join("&", data.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

            return GetRequest("POST", c_programUrlFormat)
                .WithEncoding("gzip, deflate")
                .Accepting("*/*")
                .WithContent(content)
                .GetResponse();
        }

        /// <summary>
        /// Gets the current system status.
        /// </summary>
        public Response<Status> GetSystemStatus()
        {
            return GetRequest("GET", c_statusUrlFormat, DateTime.UtcNow.ToTimeStamp())
                .WithEncoding("gzip, deflate, sdch")
                .Accepting("application/json, text/javascript, */*; q=0.01")
                .MappedWith(StatusMapper.Map)
                .GetResponse();
        }

        /// <summary>
        /// Gets the current system status.
        /// </summary>
        public Response<DateTime> GetSystemTime()
        {
            return GetRequest("GET", c_dateTimeUrlFormat, DateTime.UtcNow.ToTimeStamp())
                .WithEncoding("gzip, deflate, sdch")
                .Accepting("application/json, text/javascript, */*; q=0.01")
                .MappedWith(DateTimeMapper.Map)
                .GetResponse();
        }

        /// <summary>
        /// Sets system time.
        /// </summary>
        public Response SetSystemTime(DateTime dateTime)
        {
            string content = string.Format("day={0}&date={1}&month={2}&year={3}&hr={4}&min={5}&sec={6}",
                (int)dateTime.DayOfWeek + 1,
                dateTime.Day,
                dateTime.Month,
                dateTime.Year.ToString().Substring(dateTime.Year.ToString().Length - 2, 2),
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second);

            return GetRequest("POST", c_setClockUrlFormat)
                .WithEncoding("gzip, deflate")
                .Accepting("*/*")
                .WithContent(content)
                .GetResponse();
        }

        /// <summary>
        /// Gets the program specified by number.
        /// </summary>
        public Response<Program> GetProgram(int number)
        {
            if (number < 1 || number > 3)
                throw new ArgumentOutOfRangeException("number", "'number' must be 1, 2, or 3.");

            return GetRequest("GET", c_getProgramVariablesUrlFormat, number, DateTime.UtcNow.ToTimeStamp())
                .WithEncoding("gzip, deflate, sdch")
                .Accepting("text/javascript, application/javascript, application/ecmascript, application/x-ecmascript, */*; q=0.01")
                .MappedWith(ProgramMapper.Map)
                .GetResponse();
        }

        /// <summary>
        /// Updates the specified program
        /// </summary>
        public Response UpdateProgram(Program program)
        {
            return GetRequest("POST", c_programUrlFormat)
                .WithEncoding("gzip, deflate")
                .Accepting("*/*")
                .WithContent(ProgramMapper.Map(program))
                .GetResponse();
        }

        /// <summary>
        /// Gets the current system settings.
        /// </summary>
        public Response<SystemSettings> GetSystemSettings()
        {
            return GetRequest("GET", c_settingsUrlformat, DateTime.UtcNow.ToTimeStamp())
                .WithEncoding("gzip, deflate, sdch")
                .Accepting("text/javascript, application/javascript, application/ecmascript, application/x-ecmascript, */*; q=0.01")
                .MappedWith(SystemSettingsMapper.Map)
                .GetResponse();
        }

        /// <summary>
        /// Sets the system time server settings
        /// </summary>
        public Response SetNtpSettings(bool useNtpServer, string ntpServer, TimeZoneInfo timeZone)
        {
            Dictionary<string,string> data = new Dictionary<string,string>();

            if (useNtpServer)
            {
                data.Add("isNTP", "1");

                if (timeZone.SupportsDaylightSavingTime)
                    data.Add("isDST", "1");

                data.Add("ntpServer", ntpServer);
                data.Add("timezone", string.Format("{0:0.0}", timeZone.BaseUtcOffset.TotalMinutes / 60));
            }

            data.Add("saveNTPButton", string.Empty);

            string content = string.Join("&", data.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

            return GetRequest("POST", c_saveNtpUrlFormat)
                .WithEncoding("gzip, deflate")
                .Accepting("*/*")
                .WithContent(content)
                .GetResponse();
        }

        /// <summary>
        /// Sets the system network settings
        /// WARNING! Changes may cause system to become inaccessible!  Use caution when changing network settings!
        /// </summary>
        public Response SetNetworkSettings(string hostname, int httpPort, IpAddressAssignment ipAddressAssignment, IPAddress ipAddress, IPAddress subnetMask, IPAddress gatewayAddress)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            if(ipAddressAssignment == IpAddressAssignment.DHCP)
            {
                data.Add("hostname", hostname);
                data.Add("iptp", "1");
                data.Add("port", httpPort.ToString());
            }
            else
            {
                data.Add("hostname", hostname);
                data.Add("iptp", "0");
                data.Add("port", httpPort.ToString());

                data.Add("iplb", ipAddress.GetAddressBytes()[0].ToString());
                data.Add("iphb", ipAddress.GetAddressBytes()[1].ToString());
                data.Add("ipub", ipAddress.GetAddressBytes()[2].ToString());
                data.Add("ipmb", ipAddress.GetAddressBytes()[3].ToString());

                data.Add("masklb", subnetMask.GetAddressBytes()[0].ToString());
                data.Add("maskhb", subnetMask.GetAddressBytes()[1].ToString());
                data.Add("maskub", subnetMask.GetAddressBytes()[2].ToString());
                data.Add("maskmb", subnetMask.GetAddressBytes()[3].ToString());

                data.Add("gatelb", gatewayAddress.GetAddressBytes()[0].ToString());
                data.Add("gatehb", gatewayAddress.GetAddressBytes()[1].ToString());
                data.Add("gateub", gatewayAddress.GetAddressBytes()[2].ToString());
                data.Add("gatemb", gatewayAddress.GetAddressBytes()[3].ToString());
            }

            data.Add("saveConfigButton", string.Empty);

            string content = string.Join("&", data.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

            return GetRequest("POST", c_saveConfigUrlFormat)
                .WithEncoding("gzip, deflate")
                .Accepting("*/*")
                .WithContent(content)
                .GetResponse();
        }
        
        /// <summary>
        /// Sets miscellaneous system settings.
        /// </summary>
        public Response SetMiscSettings(bool useRainSensor, bool isRainSensorNormallyOpen, bool useZone10AsMaster, TimeSpan maxZoneTime)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            if (useRainSensor)
                data.Add("useSensor1", "1");

            if (isRainSensorNormallyOpen)
                data.Add("isS1Open", "1");

            if (useZone10AsMaster)
                data.Add("useMaster", "1");

            data.Add("maxZRunTime", maxZoneTime.TotalMinutes.ToString());
            data.Add("saveOtherSettingsButton", string.Empty);

            string content = string.Join("&", data.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

            return GetRequest("POST", c_saveOtherSettingsUrlFormat)
                .WithEncoding("gzip, deflate")
                .Accepting("*/*")
                .WithContent(content)
                .GetResponse();
        }
        
        /// <summary>
        /// Sets zone names.
        /// </summary>
        public Response SetZoneNames(string zone1, string zone2, string zone3, string zone4, string zone5, string zone6, string zone7, string zone8, string zone9)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("zName1", zone1);
            data.Add("zName2", zone2);
            data.Add("zName3", zone3);
            data.Add("zName4", zone4);
            data.Add("zName5", zone5);
            data.Add("zName6", zone6);
            data.Add("zName7", zone7);
            data.Add("zName8", zone8);
            data.Add("zName9", zone9);
            data.Add("saveNamesButton", string.Empty);

            string content = string.Join("&", data.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
            
            return GetRequest("POST", c_saveZoneNamesUrlFormat)
                .WithEncoding("gzip, deflate")
                .Accepting("*/*")
                .WithContent(content)
                .GetResponse();
        }

        /// <summary>
        /// Reboots the system.
        /// </summary>
        public Response Restart()
        {
            return GetRequest("GET", c_rebootUrlFormat)
                .WithEncoding("gzip, deflate")
                .Accepting("*/*")
                .GetResponse();
        }

        private Request GetRequest(string verb, string urlFormat, params object[] parameters)
        {
            return new Request(m_host, m_authHeader, m_userAgent, verb, urlFormat, parameters);
        }
        
        readonly string m_host;
        readonly string m_authHeader;
        readonly string m_userAgent;

        // Routes
        const string c_runSprinklersUrlFormat = "http://{0}/runSprinklers.htm";
        const string c_stopSprinklersUrlFormat = "http://{0}/stopSprinklers.htm";
        const string c_statusUrlFormat = "http://{0}/status.json?time={1}";
        const string c_dateTimeUrlFormat = "http://{0}/dateTime.json?time={1}";
        const string c_setClockUrlFormat = "http://{0}/setClock.htm";
        const string c_getProgramVariablesUrlFormat = "http://{0}/js/indexVarsDyn.js?program={1}&_={2}";
        const string c_programUrlFormat = "http://{0}/program.htm";
        const string c_settingsUrlformat = "http://{0}/js/settingsVarsDyn.js?time={1}&_={1}";
        const string c_saveNtpUrlFormat = "http://{0}/saveNTP.htm";
        const string c_saveConfigUrlFormat = "http://{0}/saveConfig.htm";
        const string c_saveZoneNamesUrlFormat = "http://{0}/saveZoneNames.htm";
        const string c_saveOtherSettingsUrlFormat = "http://{0}/saveOSettings.htm";
        const string c_rebootUrlFormat = "http://{0}/saveOSettings.htm";
    }
}
