using System;

namespace Konves.IrrigationCaddy.Client
{
    [Flags]
    public enum DayOfWeek
    {
        Sunday = 0x01,
        Monday = 0x02,
        Tuesday = 0x04,
        Wednesday = 0x08,
        Thursday = 0x10,
        Friday = 0x20,
        Saturday = 0x40,
        None = 0x00,
        Weekend = Sunday|Saturday,
        Weekday = Monday|Tuesday|Wednesday|Thursday|Friday,
        All = Weekday|Weekend
    }
}
