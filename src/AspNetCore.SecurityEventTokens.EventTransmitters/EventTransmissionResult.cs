using System;

namespace AspNetCore.SecurityEventTokens
{
    public class EventTransmitterResult
    {
        internal static EventTransmitterResult Off()
        {
            throw new NotImplementedException();
        }

        internal static EventTransmitterResult Fail()
        {
            throw new NotImplementedException();
        }

        internal static EventTransmitterResult Paused()
        {
            throw new NotImplementedException();
        }

        internal static EventTransmitterResult Success()
        {
            return new EventTransmitterResult();
        }

        internal static EventTransmitterResult Error()
        {
            throw new NotImplementedException();
        }

        internal static EventTransmitterResult Error(string v1, string v2)
        {
            throw new NotImplementedException();
        }
    }
}