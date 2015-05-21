using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Konves.IrrigationCaddy.Client
{
    public sealed class Status
    {
        internal Status(int zoneNumber, int programNumber, bool allowRun, bool isRunning, bool useCloud, string cloudStatus, bool useRainSensor, bool isRaining, TimeSpan zoneTimeLeft, TimeSpan programTimeLeft, int maxZones, IEnumerable<TimeSpan> currentZoneRunTimes)
        {
            m_zoneNumber = zoneNumber;
            m_programNumber = programNumber;
            m_allowRun = allowRun;
            m_isRunning = isRunning;
            m_useCloud = useCloud;
            m_cloudStatus = cloudStatus;
            m_useRainSensor = useRainSensor;
            m_isRaining = isRaining;
            m_zoneTimeLeft = zoneTimeLeft;
            m_programTimeLeft = programTimeLeft;
            m_maxZones = maxZones;
            m_currentZoneRunTimes = currentZoneRunTimes == null ? null : new ReadOnlyCollection<TimeSpan>(currentZoneRunTimes.ToList());
        }

        public int CurrentZoneNumber { get { return m_zoneNumber; } }
        public int CurrentProgramNumber { get { return m_programNumber; } }
        public bool IsSystemOn { get { return m_allowRun; } }
        public bool IsSystemRunning { get { return m_isRunning; } }
        public bool UseCloud { get { return m_useCloud; } }
        public string CloudStatus { get { return m_cloudStatus; } }
        public bool UseRainSensor { get { return m_useRainSensor; } }
        public bool IsRaining { get { return m_isRaining; } }
        public TimeSpan CurrentZoneTimeLeft { get { return m_zoneTimeLeft; } }
        public TimeSpan CurrentProgramTimeLeft { get { return m_programTimeLeft; } }
        public int ZoneCount { get { return m_maxZones; } }
        public IReadOnlyCollection<TimeSpan> CurrentProgramZoneRunTimes { get { return m_currentZoneRunTimes; } }

        readonly int m_zoneNumber;
        readonly int m_programNumber;
        readonly bool m_allowRun;
        readonly bool m_isRunning;
        readonly bool m_useCloud;
        readonly string m_cloudStatus;
        readonly bool m_useRainSensor;
        readonly bool m_isRaining;
        readonly TimeSpan m_zoneTimeLeft;
        readonly TimeSpan m_programTimeLeft;
        readonly int m_maxZones;
        readonly IReadOnlyCollection<TimeSpan> m_currentZoneRunTimes;
    }
}
