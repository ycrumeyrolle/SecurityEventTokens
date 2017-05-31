namespace AspNetCore.SecurityEventTokens
{
    public class JwtParsingResult
    {
        private JwtParsingResult(JwtParsingState state, JsonWebToken token = null)
        {
            State = state;
            Token = token;
        }

        public JwtParsingState State { get; }

        public JsonWebToken Token { get; set; }

        public static JwtParsingResult ParseError() => new JwtParsingResult(JwtParsingState.ParseError);

        public static JwtParsingResult IssuerError() => new JwtParsingResult(JwtParsingState.IssuerError);

        public static JwtParsingResult Success(JsonWebToken token) => new JwtParsingResult(JwtParsingState.Success, token);
    }
}