using System;

namespace AspNetCore.SecurityEventTokens
{
    public class SecurityEventTokenException : Exception
    {
        public SecurityEventTokenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public SecurityEventTokenException(string description, string value)
            : base(description)
        {
            ErrorValue = value;
        }

        public SecurityEventTokenException(string description)
            : base(description)
        {
        }

        public string ErrorValue { get; }
    }
}
