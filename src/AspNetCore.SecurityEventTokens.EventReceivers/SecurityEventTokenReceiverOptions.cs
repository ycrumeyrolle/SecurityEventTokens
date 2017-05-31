using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.SecurityEventTokens
{
    public class SecurityEventTokenReceiverOptions
    {
        public PathString Path { get; set; }

        public bool EnableHub { get; set; }
    }
}
