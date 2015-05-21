using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace Konves.IrrigationCaddy.Client
{
    internal class Request
    {
        public Request(string host, string authHeader, string userAgent, string verb, string urlFormat, params object[] parameters)
        {
            m_request = GetHttpWebRequest(host, authHeader, userAgent, verb, urlFormat, parameters);
        }

        internal static HttpWebRequest GetHttpWebRequest(string host, string authHeader, string userAgent, string verb, string urlFormat, params object[] parameters)
        {
            parameters = parameters == null ? new object[] { host } : parameters.Prepend(host).ToArray();

            string uri = string.Format(urlFormat, parameters);

            HttpWebRequest request = WebRequest.CreateHttp(uri);

            // Client
            request.Method = verb.ToUpper();
            request.Headers.Add("Accept-Language", "en-US,en;q=0.8");
            request.Headers.Add("X-Requested-With", "XmlHttpRequest");
            request.UserAgent = userAgent;

            // Login
            request.Headers.Add("Authorization", authHeader);

            // Misc
            request.Referer = string.Format("http://{0}", host);

            //Transport
            request.KeepAlive = true;
            request.Host = host;

            return request;
        }

        internal Request(HttpWebRequest request)
        {
            m_request = request;
        }

        public Request WithEncoding(string encoding)
        {
            m_request.Headers.Set("Accept-Encoding", encoding);
            return this;
        }

        public Request Accepting(string accepts)
        {
            m_request.Accept = accepts;
            return this;
        }

        public Request WithContent(string content)
        {
            m_request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

            using(Stream stream = m_request.GetRequestStream())
            using(StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(content);
            }
            return this;
        }

        public Request<TResponse> MappedWith<TResponse>(Func<string, TResponse> mapping)
        {
            return new Request<TResponse>(mapping, m_request);
        }

        public virtual Response GetResponse()
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            try
            {
                HttpWebResponse response = (HttpWebResponse)m_request.GetResponse();
                return new Response(true, null, new TimeSpan(s.ElapsedTicks));
            }
            catch (WebException ex)
            {
                return new Response(false, ex.Message, new TimeSpan(s.ElapsedTicks));
            }
        }

        readonly HttpWebRequest m_request;
    }

    internal class Request<TResponse>
    {
        internal Request(Func<string, TResponse> mapping, string host, string authHeader, string userAgent, string verb, string urlFormat, params object[] parameters)
        {
            m_request = Request.GetHttpWebRequest(host, authHeader, userAgent, verb, urlFormat, parameters);
            m_mapping = mapping;
        }

        internal Request(Func<string, TResponse> mapping, HttpWebRequest request)
        {
            m_request = request;
            m_mapping = mapping;
        }

        public Response<TResponse> GetResponse()
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            try
            {
                HttpWebResponse response = (HttpWebResponse)m_request.GetResponse();

                string content = null;

                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
                return new Response<TResponse>(true, null, new TimeSpan(s.ElapsedTicks), m_mapping.Invoke(content));

            }
            catch (WebException ex)
            {
                return new Response<TResponse>(false, ex.Message, new TimeSpan(s.ElapsedTicks), default(TResponse));
            } 
        }

        readonly HttpWebRequest m_request;
        readonly Func<string, TResponse> m_mapping;
    }
}
