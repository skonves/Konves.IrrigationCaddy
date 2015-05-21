using System;
using System.Collections.ObjectModel;

namespace Konves.IrrigationCaddy.Client
{
    public sealed class Program
    {
        internal Program()
        {
        }

        public int Number { get; internal set; }
        public bool IsEnabled { get; set; }
        public DayOfWeek RunDays { get; set; }
        public ReadOnlyCollection<StartTime> StartTimes { get; internal set; }
        public ReadOnlyCollection<ZoneRunTime> ZoneRunTimes { get; internal set; }
        public TimeSpan MaxZoneRunTime { get; internal set; }
        public ScheduleType ScheduleType { get; set; }
        public int Interval
        {
            get { return m_interval; }
            set
            {
                if (m_interval != 0)
                    ScheduleType = IrrigationCaddy.Client.ScheduleType.EveryNDays;

                m_interval = value;
            }
        }

        int m_interval = 0;
    }

    public class StartTime
    {
        internal StartTime()
        {
        }
        public bool IsEnabled { get; set; }
        public TimeSpan TimeOfDay { get; set; }
    }

    public class ZoneRunTime
    {
        internal ZoneRunTime(TimeSpan maxDuration)
        {
            m_maxDuration = maxDuration;
        }
        public string ZoneName { get; internal set; }
        public TimeSpan Duration
        {
            get { return m_duration; }
            set
            {
                if (value > m_maxDuration)
                    throw new ArgumentOutOfRangeException("value", "'value' is greater than the max duration.");

                m_duration = value;
            }
        }
        readonly TimeSpan m_maxDuration;
        TimeSpan m_duration;
    }

    public enum ScheduleType
    {
        EvenDays,
        OddDays,
        EveryNDays
    }
}
