using System;
using System.IO;
using System.Net;
using StackExchange.Redis;

namespace AspNetCore.SecurityEventTokens.Redis
{
    public class RedisOptions
    {
        public ConfigurationOptions Options { get; set; } = new ConfigurationOptions();

        public Func<TextWriter, ConnectionMultiplexer> Factory { get; set; }
        
        internal ConnectionMultiplexer Connect(TextWriter log)
        {
            if (Factory == null)
            {
                // REVIEW: Should we do this?
                if (Options.EndPoints.Count == 0)
                {
                    Options.EndPoints.Add(IPAddress.Loopback, 0);
                    Options.SetDefaultPorts();
                }

                return ConnectionMultiplexer.Connect(Options, log);
            }

            return Factory(log);
        }
    }
}