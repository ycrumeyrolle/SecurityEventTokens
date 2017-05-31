namespace AspNetCore.SecurityEventTokens
{
    public enum SecurityTokenEventStatus
    {
        OK,
        Verified,
        DuplicateSecurityEventToken,
        SecurityEventTokenTypeError,
        SecurityEventTokenParseError,
        SecurityEventTokenDataError,
        SecurityEventTokenIssuerError
    }
}