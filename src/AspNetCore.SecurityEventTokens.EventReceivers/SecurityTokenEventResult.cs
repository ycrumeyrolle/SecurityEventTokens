using System;

namespace AspNetCore.SecurityEventTokens
{
    public class SecurityTokenEventResult
    {
        public SecurityTokenEventStatus Status { get; private set; }

        public string ChallengeResponse { get; private set; }

        public static SecurityTokenEventResult OK() => new SecurityTokenEventResult { Status = SecurityTokenEventStatus.OK };

        public static SecurityTokenEventResult Verified(string challengeResponse) => new SecurityTokenEventResult { Status = SecurityTokenEventStatus.Verified, ChallengeResponse = challengeResponse };

        public static SecurityTokenEventResult Duplicate() => new SecurityTokenEventResult { Status = SecurityTokenEventStatus.DuplicateSecurityEventToken };

        public static SecurityTokenEventResult DataError() => new SecurityTokenEventResult { Status = SecurityTokenEventStatus.SecurityEventTokenDataError };

        public static SecurityTokenEventResult ParseError() => new SecurityTokenEventResult { Status = SecurityTokenEventStatus.SecurityEventTokenParseError };

        public static SecurityTokenEventResult TypeError() => new SecurityTokenEventResult { Status = SecurityTokenEventStatus.SecurityEventTokenTypeError };

        public static SecurityTokenEventResult IssuerError() => new SecurityTokenEventResult { Status = SecurityTokenEventStatus.SecurityEventTokenIssuerError };
    }
}