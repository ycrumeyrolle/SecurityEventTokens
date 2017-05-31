namespace AspNetCore.SecurityEventTokens
{
    public static class Errors
    {
        private const string JwtParseValue = "jwtParse";
        private const string JwtParseDescription = "Invalid or unparsable JWT or JSON structure.";
        public static readonly string JwtParse = FormatError(JwtParseValue, JwtParseDescription);

        private const string JwtIssuerValue = "jwtIss";
        private const string JwtIssuerDescription = "Issuer not recognized.";
        public static readonly string JwtIssuer = FormatError(JwtIssuerValue, JwtIssuerDescription);

        private const string DupValue = "dup";
        private const string DupDescription = "A duplicate SET was received and has been ignored.";
        public static readonly string Duplicate = FormatError(DupValue, DupDescription);

        public static string FormatError(string value, string description)
        {
            return $"{{\"err\":\"{value}\",\"description\":\"{description}\"}}";
        }
    }
}
