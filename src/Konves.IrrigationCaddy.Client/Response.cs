using System;

namespace Konves.IrrigationCaddy.Client
{
    public class Response
    {
        public Response(bool success, string error, TimeSpan duration)
        {
            m_success = success;
            m_error = error;
            m_duration = duration;
        }

        public bool Success { get { return m_success; } }

        public string Error { get { return m_error; } }

        public TimeSpan Duration { get { return m_duration; } }

        readonly bool m_success;
        readonly string m_error;
        readonly TimeSpan m_duration;
    }

    public class Response<T> : Response
    {
        public Response(bool success, string error, TimeSpan duration, T value)
            : base(success, error, duration)
        {
            m_value = value;
        }

        public T Value { get { return m_value; } }

        readonly T m_value;
    }
}
